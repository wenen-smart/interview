using Assets.Resolution.Scripts.Inventory;
using Assets.Resolution.Scripts.Weapon;
using Knife.Effects;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FPS_PlayerController : RoleController
{
    public PlayerInputController InputController { get; private set; }
    public Vector2 MovementValue { get { return InputController.MovementValue; } }

    [HideInInspector] public Transform lookat_Trans;
    public float MaxVerticalAngle = 60;
    public float MinVerticalAngle = -60;

    public float verAxisSpeed =>IsOpeningScope?SettingSO.Instance.mouseSensitivityInSnipeScope:SettingSO.Instance.mouseSensitivity;
    public float horAxisSpeed=>IsOpeningScope?SettingSO.Instance.mouseSensitivityInSnipeScope:SettingSO.Instance.mouseSensitivity;
    public RoleHeightInfo standViewHeightPoint;
    public RoleHeightInfo crouchViewHeightPoint;

    private RecoilListener recoilListener;//耦合了
    private int _shotNumAfterContinueFired;
    public override float characterRadius => movement.centerDiameter/2;

    //在连射开始之后的射击数量
    private int shotNumAfterContinueFired
    {
        get
        {
            return _shotNumAfterContinueFired;
        }
        set
        {
            _shotNumAfterContinueFired = value;
            recoilListener.shot_Num = _shotNumAfterContinueFired;
        }
    }

    public float jumpSpeed=5;
    public float verticalVelocity;
    public float fallMultiply = 3;
    public float checkItemCastLen = 10;
    private float checkItemThresholdTime=0.1f;//每隔一段时间去检测是否注视掉落物
    private MyTimer checkItemThresholdTimer;
    Feed eyesOnfeed;//注视的掉落物 
    public override bool isRecharging => AnimatorMachine.CheckNextOrCurrentAnimationStateByName(RuntimeInventory.currentWeapon.userAnimator,0,WeaponActionType.Recharge.ToString());

    public GameObject snipeGo;
    public SniperscopeSetting sniperscopeSetting;
    public bool IsOpeningScope;
    //0-1-2 
    public int scopeTimes;

    public override bool isPullBoltting => false;

	public override Vector3 EyePosition
	{
		get
		{
            return lookat_Trans.position;
		}
	}

	public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        InputController = PlayerInputController.Instance;
        lookat_Trans = transform.Find("RecoilListener/LookAt");
        ResetToStandView();
        recoilListener = actorSystem.GetComponent<RecoilListener>();
        //animatorMachine = null;
        PlayerAwakeOnRoundGameStart();//Test
        checkItemThresholdTimer=TimeSystem.Instance.CreateTimer();
    }
    public void ResetToStandView()
    {
        isCrouch = false;
        lookat_Trans.position = standViewHeightPoint.viewerPoint.position;
        movement.centerHeight = standViewHeightPoint.movement_ColliderHeight;
        movement.ResetColliderConfiguration();
        if ((controllerState & ControllerState.Walking) != 0)
        {
            movement_Speed = WalkSpeed;
        }
        else
        {
            movement_Speed = RunSpeed;
        }
    }
    public void SetCrouch()
    {
        isCrouch = true;
        lookat_Trans.position = crouchViewHeightPoint.viewerPoint.position;
        movement.centerHeight = crouchViewHeightPoint.movement_ColliderHeight;
        movement.ResetColliderConfiguration();
        if ((controllerState&ControllerState.Walking)!=0)
        {
            movement_Speed = WalkSpeed;
        }
        else
        {
            movement_Speed = RunSpeed;
        }
    }
    public override void Start()
    {
        base.Start();
        GameUI.Instance.playerInfoHUD.MaxHPValue = actorHealth.MaxHp;
        GameUI.Instance.playerInfoHUD.MaxDefendValue = 0;
        actorHealth.OnHPChangeHandler += GameUI.Instance.playerInfoHUD.UpdateHPStatusUI;

        RuntimeInventory.WeaponLoadInInventoryHandler += AddWeaponInRuntimeInventory;

        RuntimeInventory.InventoryUpdatedHandler = () =>
          {
              GameUI.Instance.playerInfoHUD.UpdateWeaponHUD(RuntimeInventory.weaponList.Select<BaseWeapon, int>((bw) =>
              {
                  return bw.GetItemDataConfig().cellIndex;
              }).ToArray());
              foreach (var item in RuntimeInventory.weaponList)
              {
                  var so = item.GetItemDataConfig();
                  if (string.IsNullOrEmpty(so.iconInHUD_Name))
                  {
                      DebugTool.DebugError("没有填写武器的精灵名");
                      continue;
                  }
                  GameUI.Instance.playerInfoHUD.UpdateWeaponIcon(so.cellIndex, so.iconInHUD_Name);
              }
          };

        
        RuntimeInventory.OnSwitchWeapnBeforeHandler +=()=>{ CloseScope(); };
        RuntimeInventory.OnSwitchWeapnAfterHandler += () =>
        {

            bool isSetCrosshair = true;
            GameUI.Instance.playerInfoHUD.SwitchWeapon(RuntimeInventory.currentWeapon.GetItemDataConfig().cellIndex);
            GameUI.Instance.playerInfoHUD.SetMaxAmmo(RuntimeInventory.currentGunWeapon != null ? RuntimeInventory.currentGunWeapon.AmmoCount - RuntimeInventory.currentGunWeapon.clipRemainAmmoCount : 0);
            if (RuntimeInventory.currentGunWeapon!=null)
            {
                if (RuntimeInventory.currentGunWeapon.GunData.gunType == GunType.Snipe)
                {
                    GameUI.Instance.crosshair_HUD.CloseCrossHair();
                    isSetCrosshair = false;
                }
                GameUI.Instance.playerInfoHUD.UpdateAmmo(RuntimeInventory.currentGunWeapon.clipRemainAmmoCount,true);
            }
            else
            {
                GameUI.Instance.playerInfoHUD.HideAmmoCellUI();
                GameUI.Instance.playerInfoHUD.UpdateAmmo(1,false);
            }
            if (isSetCrosshair)
            {
                GameUI.Instance.crosshair_HUD.SetCrosshair(RuntimeInventory.currentWeapon.GetItemDataConfig().crosshairSetting);
            }

            

        };

        RuntimeInventory.Init();
        RuntimeInventory.ExchangeWeapon(0);
        
    }
    public override void AddWeaponInRuntimeInventory(BaseWeapon weapon)
    {
        weapon.RenewWeapon();
        if (weapon is Gun_Weapon)
        {
            Gun_Weapon gun = weapon as Gun_Weapon;
            gun.OnRechargeEventHandler += () => {
                if (gun.GunData.gunType==GunType.Snipe)
                {
                    CloseScope(); 
                }
                gun.transform.GetComponent<Animator>().CrossFade(WeaponActionType.Recharge.ToString(), 0);
                GameUI.Instance.playerInfoHUD.ReLoad();
            };
            gun.OnRechargeFinishEventHandler += () =>
            {
                GameUI.Instance.playerInfoHUD.SetMaxAmmo(RuntimeInventory.currentGunWeapon != null ? RuntimeInventory.currentGunWeapon.AmmoCount- RuntimeInventory.currentGunWeapon.clipRemainAmmoCount: 0);
                GameUI.Instance.playerInfoHUD.UpdateAmmo(RuntimeInventory.currentGunWeapon.clipRemainAmmoCount,true);
            };
            shotNumAfterContinueFired++;

            //gun.OnPullBoltEventHandler += () => {
            //    WeaponPullBolt(gun);
            //};
            gun.OnFireEventHandler += () => {

                //TODO:有瞄准镜的 才去执行
                if (gun.GunData.gunType==GunType.Snipe)
                {
                    CloseScopeButNextContinue();
                }
            };
            gun.FireCDFinishEventhandler += () => {
                if (scopeTimes>0)
                {
                    scopeTimes -= 1;
                    ZoomScope();
                }
            };
        }
    }
    public override void PlayerAwakeOnRoundGameStart()
    {
        RuntimeInventory.SwitchBag();
        foreach (var weapon in RuntimeInventory.weaponList)
        {
            if (weapon is Gun_Weapon)
            {
                Gun_Weapon gun = weapon as Gun_Weapon;
                //gun.OnRechargeEventHandler;

            }
        }
    }
    public void SwitchMoveMode()
    {
        if (InputController.isInputingWalk&&movement_Speed!=WalkSpeed)
        {
            SetMoveMode(ControllerState.Walking);
        }
        else if (InputController.JustReleasedWalkThisFrame)
        {
            SetMoveMode(ControllerState.Running);
        }
    }
    public void CalculateMove()
    {
        if (InputController.isInputingMove)
        {
            movement.Move(new Vector3(MovementValue.x, 0, MovementValue.y) * movement_Speed * Time.deltaTime, Space.Self);
            if (AnimatorMachine.CheckAnimationStateByName(armAnimator,0, WeaponActionType.Idle.ToString()))
            {
                armAnimator.CrossFade(WeaponActionType.Run.ToString(), 0);
            }
        }
        else
        {
            if (controllerState.IsSelectThisEnumInMult(ControllerState.Running))
            {
                if (AnimatorMachine.CheckAnimationStateByName(armAnimator,0, WeaponActionType.Walk.ToString(), WeaponActionType.Run.ToString()))
                {
                    armAnimator.CrossFade(WeaponActionType.Idle.ToString(), 0);
                }
            }
        }
        if (IsInGround == false)
        {
            if (verticalVelocity <= 0 /*|| !InputController.JustInputedJumpThisFrame*/)
            {
                verticalVelocity -= movement.gravity * fallMultiply * Time.deltaTime;
            }
            else
            {
                verticalVelocity -= movement.gravity * Time.deltaTime;
            }
        }
        if ((controllerState & ControllerState.Jumped) != 0 || (controllerState & ControllerState.InAir) != 0)
        {
            //into air force
            
            movement.deltaPosition += Vector3.up*verticalVelocity*Time.deltaTime;
        }
        
    }
   
    
    public void Update()
    {
        if (Mathf.Abs(InputController.MouseAxisX) > 0.01f)
        {
            transform.rotation *= Quaternion.AngleAxis(InputController.MouseAxisX * horAxisSpeed * Time.deltaTime, Vector3.up);
        }
        if (Mathf.Abs(InputController.MouseAxisY) > 0.01f)
        {
            lookat_Trans.localRotation *= Quaternion.AngleAxis(-InputController.MouseAxisY * verAxisSpeed * Time.deltaTime, Vector3.right);
            Vector3 lookat_TransEuler = lookat_Trans.eulerAngles;
            lookat_TransEuler.z = 0;
            lookat_Trans.eulerAngles = lookat_TransEuler;
            if ((this.FixedAngle(lookat_Trans.localEulerAngles.x)) < -MaxVerticalAngle)
            {
                var quaternion = Quaternion.Euler(lookat_Trans.localEulerAngles.SetX(-MaxVerticalAngle));
                lookat_Trans.localRotation = quaternion;
            }
            else if ((this.FixedAngle(lookat_Trans.localEulerAngles.x)) > -MinVerticalAngle)
            {
                var quaternion = Quaternion.Euler(lookat_Trans.localEulerAngles.SetX(-MinVerticalAngle));
                lookat_Trans.localRotation = quaternion;
            }
            lookat_Trans.localEulerAngles=lookat_Trans.localEulerAngles.SetY(0);
        }
        SwitchMoveMode();
        CalculateMove();
        TryExchangeWeapon();
        CalculateControllerState();

        if (InputController.isInputingFire)
        {
            switch (RuntimeInventory.currentWeapon.weaponType)
            {
                case WeaponType.Thrown:
                    if (!AnimatorMachine.CheckAnimationStateByName(RuntimeInventory.currentWeapon.userAnimator, 0, WeaponActionType.PreThrow.ToString(),WeaponActionType.Shot.ToString()))
                    {
                        RuntimeInventory.currentWeapon.userAnimator.CrossFade(WeaponActionType.PreThrow.ToString(),0);
                    }
                    break;
                case WeaponType.Knife:
                case WeaponType.Empty_Hand:
                    bool isyes = RuntimeInventory.currentWeapon.Use(!InputController.isInputedFireThisFrame);
                    if (isyes)
                    {
                        GameUI.Instance.crosshair_HUD.StartBreathe();
                    }
                    break;
                case WeaponType.Primarily:
                case WeaponType.Pistol:
                    if (RuntimeInventory.currentWeapon.Use(!InputController.isInputedFireThisFrame))
                    {
                        GameUI.Instance.crosshair_HUD.StartBreathe();
                        GameUI.Instance.playerInfoHUD.UpdateAmmoWhenReduce(RuntimeInventory.currentGunWeapon.clipRemainAmmoCount);
                        shotNumAfterContinueFired++;
                    }
                    break;
                default:
                    break;
            }
        }
        else if (InputController.JustReleasedFireThisFrame)
        {
            shotNumAfterContinueFired = 0;
            switch (RuntimeInventory.currentWeapon.weaponType)
            {
                case WeaponType.Primarily:
                    break;
                case WeaponType.Pistol:
                    break;
				case WeaponType.Knife:
				case WeaponType.Empty_Hand:
					break;
                case WeaponType.Thrown:
                    RuntimeInventory.currentWeapon.Use(false);
                    GameUI.Instance.playerInfoHUD.UpdateAmmo(0);
                    break;
                default:
                    break;
            }
        }
        if (isCrouch==false&&InputController.isInputedCrouchThisFrame)
        {
            SetCrouch();
        }
        else if (InputController.JustReleasedCrouchThisFrame)
        {
            ResetToStandView();
        }
        if (InputController.isInputingThump)
        {

            if (IsCanUseSnipe())
            {
                if (InputController.isInputedThumpThisFrame&&RuntimeInventory.isRecharging==false&&RuntimeInventory.currentGunWeapon.IsRecharging==false&&RuntimeInventory.currentGunWeapon.fireIsCDFinish)
                {
                    ZoomScope();
                }
            }
            else if (RuntimeInventory.currentWeapon.Use(!InputController.isInputedThumpThisFrame, WeaponUseInputType.Thump))
            {

            }
        }
        if (InputController.JustInputedJumpThisFrame)
        {
            if ((controllerState & ControllerState.Jumped) == 0 && (controllerState & ControllerState.InAir) == 0)
            {
                //不在jump 也不在air
                isJump = true;
                verticalVelocity = jumpSpeed;
            }
        }

        if (InputController.JustInputedRechargeThisFrame)
        {
            if (RuntimeInventory.currentGunWeapon != null)
            {
                bool isRecharging = RuntimeInventory.currentGunWeapon.IsRecharging;
                bool isExchanging = RuntimeInventory.isRecharging;
                if (!isRecharging && !isExchanging)
                {
                    RuntimeInventory.currentGunWeapon.ToRechargeClip();
                }
            }
        }
        if (checkItemThresholdTimer.timerState!=MyTimer.TimerState.Run)
        {
            if (CheckItem(out eyesOnfeed)) { }
            checkItemThresholdTimer.Go(checkItemThresholdTime);
        }
        if (eyesOnfeed)
        {
            DebugTool.DrawWireSphere(eyesOnfeed.transform.position, 0.2f, Color.yellow, 0, eyesOnfeed.VisitFeed().ITEM_Name);
            if (eyesOnfeed is GunFeed)
            {
                if ((eyesOnfeed.isDynamic&&eyesOnfeed.isInGround)||eyesOnfeed.isDynamic==false)
                {
                    var gunFeed = eyesOnfeed as GunFeed;
                    WeaponDataConfig weaponDataConfig = gunFeed.VisitFeed() as WeaponDataConfig;
                    if (RuntimeInventory.IsExistThisWeaponOfType(weaponDataConfig.weaponType) == false)
                    {
                        gunFeed.GetFeed();
                        BaseWeapon baseWeapon = RuntimeInventory.AddWeapon(weaponDataConfig);
                        Gun_Weapon gunWeapon = baseWeapon as Gun_Weapon;
                        gunWeapon.ImmediateUpdateAmmoData(gunFeed.ammoCount);
                    }
                }
            }
            eyesOnfeed=null;
        }
        if (InputController.JustInputedThrowWeaponThisFrame)
        {
            ThrowOutWeapon();
        }

    }
    public void InputSignGetFeedAndThrowWeapon()
    {
        if (InputController.JustInputedThrowWeaponThisFrame)
        {
            if (eyesOnfeed)
            {
                if (eyesOnfeed is GunFeed)
                {
                    var gunFeed = eyesOnfeed as GunFeed;
                    BaseWeapon baseWeapon = RuntimeInventory.AddWeapon(gunFeed.GetFeed() as WeaponDataConfig);
                    Gun_Weapon gunWeapon = baseWeapon as Gun_Weapon;
                    gunWeapon.ImmediateUpdateAmmoData(gunFeed.ammoCount);
                }
            }
            else
            {
                ThrowOutWeapon();
            }
        }
    }

    public void LateUpdate()
    {
        
    }
    public void FixedUpdate()
    {
        if (isJump)
        {
            var predictToAirBaseVelocity = movement.GetIntoAirMinUpVelocity()*Vector3.up;//v=s/t
            movement.deltaPosition += predictToAirBaseVelocity*Time.fixedDeltaTime;//s
            isJump = false;
        }
        movement.OnFixedUpdate();
    }
    public void TryExchangeWeapon()
    {
        float scroll = InputController.MouseScroll;
        if (scroll != 0)
        {
            if (Mathf.Abs(scroll) >= 1f)
            {
                //切换武器
                RuntimeInventory.ExchangeWeaponByScroll(scroll < 0 ? 1 : -1);

            }
        }
    }

    public override Vector3 GetPreScansTargetPoint(float maxDis)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
        int hitScansMask = ~(1 << LayerMask.NameToLayer("BodyPart") | (1 << 2));//忽略骨骼 因为骨骼会动。
        if (Physics.Raycast(ray,out RaycastHit hitinfo,maxDis,hitScansMask,QueryTriggerInteraction.Ignore))
        {
            return hitinfo.point;
        }
        return Vector3.zero;
    }

    public override bool IsInMove()
    {
        return InputController.isInputingMove;
    }

    public override void FindTarget(Vector3 targetPoint)
    {
        
    }
    public override void SetMoveMode(ControllerState controllerState)
    {
        if (controllerState.IsSelectThisEnumInMult(ControllerState.Walking))
        {
            movement_Speed = WalkSpeed; 
        }
        else if (controllerState.IsSelectThisEnumInMult(ControllerState.Running))
        {
            movement_Speed = RunSpeed; 
        }
    }

    public override void ReceiveFlashGrenade()
    {
        var blinder = CameraController.Instance.GetComponent<IBlinder>();
        blinder.Blind(1f, transform.position);
    }

    public override void AttachWeapon(GameObject go,string attachPoint="")
    {
        go.transform.SetParent(lookat_Trans.transform,false);
    }
    public override void ThrowOutWeapon()
    {
        if (RuntimeInventory.currentGunWeapon == null)
        {
            return;
        }
        ThrowOutWeapon(RuntimeInventory.currentGunWeapon);
    }
    public override void ThrowOutWeapon(BaseWeapon weapon)
    {
        if (weapon.GetItemDataConfig().isCanThrow==false)
        {
            return;
        }
        if (weapon.weaponType==WeaponType.Primarily||weapon.weaponType==WeaponType.Pistol)
        {
            var gunWeapon = weapon as Gun_Weapon;
            var clone = GameObject.Instantiate(gunWeapon.GunData._itemPrefab, null, true);
            GunFeed feed = clone.AddComponent<GunFeed>();
            feed.RegisterFeed(gunWeapon.GetItemDataConfig());
            feed.isDynamic = true;
            feed.ammoCount = gunWeapon.AmmoCount;

            RuntimeInventory.RemoveWeaponRuntimeInventory(RuntimeInventory.currentWeapon);

            Rigidbody rigidbody = clone.GetComponent<Rigidbody>();
            rigidbody.transform.position = gunWeapon.gunpoint.position;
            rigidbody.transform.rotation = Random.rotationUniform;
            Collider weaponCol = rigidbody.GetComponent<Collider>();
            weaponCol.isTrigger = false;

            rigidbody.transform.right = Vector3.up;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.WakeUp();
            rigidbody.AddForce(actorSystem.transform.forward * Random.Range(1.5f, 3f), ForceMode.Impulse);
            rigidbody.AddTorque(Vector3.up * Random.Range(5, 10f), ForceMode.Impulse);
            Destroy(clone, 10);
        }
        
    }
    public override bool CheckItem(out Feed feed)
    {
        feed = null;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        //if (Physics.SphereCast(ray,0.2f,out RaycastHit hitinfo,checkItemCastLen,itemLayerMask))
        Collider[] itemColliders =  Physics.OverlapSphere(transform.position, 2, itemLayerMask);
        if(itemColliders.Length>0)
        {
            feed = itemColliders[0].GetComponent<Feed>();
            return true;
        }
        return false;
    }

    public override void OnMeleeCheck(WeaponUseInputType useInputType, MeleeCheckWay checkWay,MeleeWeapon meleeWeapon, int comboID = 0)
    {
        StartCoroutine(MeleeCheckCoroutine(useInputType,checkWay,meleeWeapon));
    }
    public IEnumerator MeleeCheckCoroutine(WeaponUseInputType useInputType,MeleeCheckWay checkWay,MeleeWeapon meleeWeapon)
    {
        Vector3 startPosInViewport;
        Vector3 startPosInWorld = Vector3.zero;
        MeleeWeaponDataConfig meleeWeaponDataConfig = meleeWeapon.data;
        List<RoleController> hitter=new List<RoleController>();
        //TODO
        Vector3 startDir=Vector3.zero;
        var cameraForward = CameraController.Instance.GetMainCameraForward();
        var CameraUp = CameraController.Instance.GetMainCameraUp();
        var checkDirGlobal = CameraController.Instance.LocalDirToWorldDir(checkWay.meleeCheckWayData[0].checkDir);
        var rotateAxis = Vector3.Cross(cameraForward, checkDirGlobal);
        if (checkWay.StartPosIsByCameraCenterAutoCalculate)
        {
            startDir =  Quaternion.Euler(rotateAxis*-(checkWay.meleeCheckWayData[0].checkAngle/2))*cameraForward;
        }
        else
        {
            startPosInWorld = Camera.main.transform.position+checkWay.startPosOffsetInCameraCoord;
            startPosInWorld += cameraForward.normalized;
            startDir = (startPosInWorld - Camera.main.transform.position).normalized;
        }
        //
        

        int hitScansMask=~(1<<LayerMask.NameToLayer("FullBody")|(1<<2));//忽略全身碰撞体 因为要检测击中的身体的部位
        
        if (checkWay.meleeCheckWayData != null)
        {
            foreach (var item in checkWay.meleeCheckWayData)
            {
                float rayCount = item.checkAngle/item.eachcheckIntervalAngle;
                float eachcheckIntervalTime = item.meleeCheckTime / rayCount;
                int _castNum = 0;
                if (item.rayCastTrackType==RayCastTrackType.FromPointToTwoSide)
                {
                    if (checkWay.StartPosIsByCameraCenterAutoCalculate)
                    {
                        startDir =  cameraForward;
                    }
                    else
                    {
                        startDir = (startPosInWorld - Camera.main.transform.position).normalized;
                    }

                    while (_castNum < rayCount)
                    {
                        int index = (_castNum / 2)+1;
                        var castDir = Quaternion.Euler(rotateAxis * item.eachcheckIntervalAngle * index) * startDir;
                        var castDir_InverseAxis = Quaternion.Euler(-rotateAxis * item.eachcheckIntervalAngle * index) * startDir;
                        var castOrigin = CameraController.Instance.transform.position;
                        RaycastHit[] hitinfos = Physics.RaycastAll(castOrigin, castDir, item.checkDis, hitScansMask);
                        RaycastHit[] hitinfos_2 = Physics.RaycastAll(castOrigin, castDir_InverseAxis, item.checkDis, hitScansMask);
                        hitinfos=hitinfos.Concat(hitinfos_2).ToArray<RaycastHit>();
                        if (hitinfos != null && hitinfos.Length > 0)
                        {
                            foreach (var hitinfo in hitinfos)
                            {
                                AllowScansBodyPart allowScansBodyPart = hitinfo.collider.GetComponent<AllowScansBodyPart>();
                                if (allowScansBodyPart)
                                {
                                    ActorSystem Actor = allowScansBodyPart.setting.actorSystem;
                                    if (Actor && actorSystem != Actor)
                                    {
                                        RoleController roleController = Actor.GetActorComponent<RoleController>();
                                        ActorHealth health = roleController.actorHealth;
                                        if (hitter.Contains(roleController) == false && roleController.team != team && !health.isDeath)
                                        {
                                            //get damage
                                            BodyPartMainType bodyPartType = BodyPartSetting.GetBodyPartMainType(allowScansBodyPart.bodyPartType);
                                            var damage = meleeWeapon.GetDamage(castOrigin, hitinfo.point, item.checkDis, useInputType, bodyPartType);
                                            DebugTool.DebugPrint(string.Format("击中{0},造成伤害：{1}", bodyPartType.ToString(), damage));
                                            DamageInfo damageInfo = new DamageInfo(damage: damage, caster: this, damageType: DamageTypes.Knife, casterWeaponInfo: meleeWeaponDataConfig, castDir: castDir, damagedBodyPart: allowScansBodyPart.bodyPartType, hitPoint: hitinfo.point, hitNormal: hitinfo.normal, ammo_size: 1, damagedTime: Time.time);
                                            //
                                            health.Damage(damageInfo);
                                            hitter.Add(roleController);
                                        }
                                    }
                                }
                            }
                        }
                        DebugTool.DrawLine(castOrigin, castOrigin + castDir * item.checkDis, Color.green, 5);
                        DebugTool.DrawWireSphere(castOrigin + castDir * item.checkDis, 0.1f, Color.yellow, 5);
                        DebugTool.DrawLine(castOrigin, castOrigin + castDir_InverseAxis * item.checkDis, Color.green, 5);
                        DebugTool.DrawWireSphere(castOrigin + castDir_InverseAxis * item.checkDis, 0.1f, Color.yellow, 5);
                        _castNum +=2;
                        yield return new WaitForSeconds(eachcheckIntervalTime);
                    }
                }
                else if(item.rayCastTrackType==RayCastTrackType.SingleDir)
                {
                    while (_castNum < rayCount)
                    {
                        var castDir = Quaternion.Euler(rotateAxis * item.eachcheckIntervalAngle * _castNum) * startDir;
                        var castOrigin = CameraController.Instance.transform.position;
                        RaycastHit[] hitinfos = Physics.RaycastAll(castOrigin, castDir, item.checkDis, hitScansMask);
                        if (hitinfos != null && hitinfos.Length > 0)
                        {
                            foreach (var hitinfo in hitinfos)
                            {
                                AllowScansBodyPart allowScansBodyPart = hitinfo.collider.GetComponent<AllowScansBodyPart>();
                                if (allowScansBodyPart)
                                {
                                    ActorSystem Actor = allowScansBodyPart.setting.actorSystem;
                                    if (Actor && actorSystem != Actor)
                                    {
                                        RoleController roleController = Actor.GetActorComponent<RoleController>();
                                        ActorHealth health = roleController.actorHealth;
                                        if (hitter.Contains(roleController) == false && roleController.team != team && !health.isDeath)
                                        {
                                            //get damage
                                            BodyPartMainType bodyPartType = BodyPartSetting.GetBodyPartMainType(allowScansBodyPart.bodyPartType);
                                            var damage = meleeWeapon.GetDamage(castOrigin, hitinfo.point, item.checkDis, useInputType, bodyPartType);
                                            DebugTool.DebugPrint(string.Format("击中{0},造成伤害：{1}", bodyPartType.ToString(), damage));
                                            DamageInfo damageInfo = new DamageInfo(damage: damage, caster: this, damageType: DamageTypes.Knife, casterWeaponInfo: meleeWeaponDataConfig, castDir: castDir, damagedBodyPart: allowScansBodyPart.bodyPartType, hitPoint: hitinfo.point, hitNormal: hitinfo.normal, ammo_size: 1, damagedTime: Time.time);
                                            //
                                            health.Damage(damageInfo);
                                            hitter.Add(roleController);
                                        }
                                    }
                                }
                            }
                        }
                        DebugTool.DrawLine(castOrigin, castOrigin + castDir * item.checkDis, Color.green, 5);
                        DebugTool.DrawWireSphere(castOrigin + castDir * item.checkDis, 0.1f, Color.yellow, 5);
                        _castNum++;
                        yield return new WaitForSeconds(eachcheckIntervalTime);
                    }
                }
            }
        }
        yield return 0;
    }
    public bool IsUsingSnipeScope()
    {
        if (RuntimeInventory.currentGunWeapon == null)
        {
            return false;
        }
        return IsOpeningScope;
    }
    public bool IsCanUseSnipe()
    {
        if (RuntimeInventory.currentGunWeapon==null)
        {
            return false;
        }
        return RuntimeInventory.currentGunWeapon.GunData.gunType == GunType.Snipe;
    }
    public float GetFocalLength(int times)
    {
        switch (times)
        {
            case 1:
                return sniperscopeSetting.firstScopeFocalLength;
            case 2:
                return sniperscopeSetting.secondScopeFocalLength;
            default:
                return ((FPS_CameraController)FPS_CameraController.Instance).defaultFocalLength;
        }
    }
    public int ZoomScope()
    {
        IsOpeningScope = true;
        if (IsOpeningScope)
        {
            scopeTimes += 1;
            scopeTimes %= 3;
        }
        if (scopeTimes == 0)
        {
            CloseScope();
        }
        else
        {
            ((FPS_CameraController)FPS_CameraController.Instance).SetFPSCameraFOV(GetFocalLength(scopeTimes));
            RuntimeInventory.currentWeapon?.AllRender(false);
            GameUI.Instance.crosshair_HUD.SetCrosshair(RuntimeInventory.currentWeapon.GetItemDataConfig().crosshairSetting);
            snipeGo.gameObject.SetActive(true);
        }
        return scopeTimes;
    }
    /// <summary>
    /// 关闭瞄准镜。
    /// </summary>
    public void CloseScope()
    {
        IsOpeningScope = false;
        GameUI.Instance.crosshair_HUD.CloseCrossHair();
        snipeGo.gameObject.SetActive(false);
        RuntimeInventory.currentWeapon?.AllRender(true);
        scopeTimes = 0;
        ((FPS_CameraController)FPS_CameraController.Instance).SetFPSCameraFOV(GetFocalLength(0));
    }
    /// <summary>
    /// 关闭瞄准镜 但是开枪后会自动打开瞄准镜。
    /// </summary>
    public void CloseScopeButNextContinue()
    {
        IsOpeningScope = false;
        GameUI.Instance.crosshair_HUD.CloseCrossHair();
        RuntimeInventory.currentWeapon?.AllRender(true);
        ((FPS_CameraController)FPS_CameraController.Instance).SetFPSCameraFOV(GetFocalLength(0));
        snipeGo.gameObject.SetActive(false);
    }

    public void WeaponPullBolt(Gun_Weapon gun_Weapon)
    {
        gun_Weapon.transform.GetComponent<Animator>().CrossFade(WeaponActionType.PullBolt.ToString(), 0);
    }
}

