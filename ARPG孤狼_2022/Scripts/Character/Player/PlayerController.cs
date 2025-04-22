using Buff.Base;
using Buff.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using System.Linq;
using Unity.Mathematics;

public class PlayerController : RoleController
{

    //public Animator anim;
    private Vector2 inputDir;
    private Vector3 targetDir;
    public PlayerAnim playerAnim;

    public Rigidbody rigid;

    public Vector2 lastInputDir;

    public Vector3 camRightInControlState;
    public Vector3 camForwardInControlState;

    private float currentSpeeedMult = 1;
    private ActorPhyiscal actorPhyiscal;
    



    public Vector3 standCoverAimOffset_BaseRight;
    public Vector3 bowAimOffsetBaseRight;
    public bool aimState = false;

    [HideInInspector]
    public PlayerFacade playerFacade;
    [HideInInspector]
    public InputSystem inputSystem;

    public FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmMachine;
    public FsmStateMachine<AttackType, AttackTransitionType, PlayerController> attackFsmMachine;
    [SerializeField]
    public PlayerStateType _PlayerStateType { get { return fsmMachine.currentStateType; } }
    private TeleportShadow teleportShadow;
    public bool isReceiveDefenseInput = false;
    List<DropItemObject> dropItemsList = new List<DropItemObject>();//脚下检测到的可拾取的物品
    public float lock_target_MaxAroundRotationAngle=60;//锁定状态下如果人物旋转的值大于这个值的话，就不旋转，防止相机转得太大；
    //[HideInInspector]
    //public Transform inBoxRangeEnemy = null;
    public override void Awake()
    {
        base.Awake();
        //characterCollider = GetComponent<CapsuleCollider>();
        teleportShadow = GetComponent<TeleportShadow>();
        CameraMgr.Instance.canelLockTarget = ChanelLockTarget;
    }
    public override void Init()
    {
        currentSpeeedMult = PlayerableConfig.Instance.commmonRunMult;
        actorPhyiscal = GetComponent<ActorPhyiscal>();
        playerFacade = GetComponent<PlayerFacade>();
        inputSystem = InputSystem.Instance;
        actorSystem.roleData.InventoryData.Put(ItemDataConfigManager.GetItemDataConfig(3));
        actorSystem.roleData.InventoryData.Equip(ItemDataConfigManager.GetItemDataConfig(3));
        base.Init();
    }
    public override void Start()
    {
        base.Start();
        //EndSkillTimeLineManager.Instance.targetGroup.m_Targets[0].target=centerTrans.transform;

        fsmMachine = new FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController>("Player", this, PlayerFsmStateConfig.Instance.FsmStateTypeData);
        attackFsmMachine = new FsmStateMachine<AttackType, AttackTransitionType, PlayerController>("ACT", this, PlayerAttackFSMConfig.Instance.FsmStateTypeData);
    }

   
    public void LookAtCurrentViewPort()
    {
        transform.rotation = GetPlayerForwardInNowViewPort();
    }

    #region FsmState
    #region Action
    public void HangUp(TriggerArg triggerArg)
    {

    }
    #endregion
    #region StateOnUpdate
    public void HangOnUpdate()
    {
        inputDir = inputSystem.inputDir;
        playerFacade.playerHangSystem.OnUpdate();
        if (inputSystem.pressDownMouseLBtn)
        {
                if (playerFacade.playerHangSystem.arriveHigheTaskIsExcute == false)
                {
                    playerFacade.playerHangSystem.HandlerArriveHighPoint();
                    GameRoot.Instance.ShowFloatDialog(5, 1);
                }
                CheckHangUpPlatformStrikeTarget();
        }
    }
    #endregion
    public void JumpAttack(TriggerArg triggerArg)
    {
        int intArg = triggerArg.argInt;
        //Debug.Log("triggerArg:" + intArg);
        switch (intArg)
        {
            case 1:
                SkillSystem.Instance.AttackSkillHandler(this, 6);
                break;
            case 2:
                SkillSystem.Instance.AttackSkillHandler(this,7);
                break;
            default:
                break;
        }

    }
    public void Idle()
    {
        //playerFacade.playerHangSystem.HangCheckUpdate();
        targetBlend = 0;
        inputDir = inputSystem.inputDir;
        switch (moveMode)
        {
            case ActorMoveMode.Common:
                playerFacade.playerHangSystem.HangCheckUpdate();
                bool isCheck = playerFacade.playerHangSystem.CheckHangLadder();
                if (isCheck==false)
                {
                    ListenerClosestWall();
                }
                TryExchangeWeapon();
                BowStay();

                break;
            case ActorMoveMode.ClosetWall:
                break;
            case ActorMoveMode.Shimmy:
                break;
            case ActorMoveMode.LockTarget:
                TryExchangeWeapon();
                BowStay();
                break;
            case ActorMoveMode.ClimbLadder:
                playerFacade.playerHangSystem.ClimbLadderOnUpdate();
                if (playerFacade.playerHangSystem.IsArriveLadderBestHigh())
                {
                    if (playerFacade.playerHangSystem.arriveHigheTaskIsExcute==false)
                    {
                        playerFacade.playerHangSystem.HandlerArriveHighPoint();
                        GameRoot.Instance.ShowFloatDialog(5,1);
                    }
                    if (inputSystem.pressDownMouseLBtn)
                    {
                        CheckHangUpPlatformStrikeTarget();
                    }
                }
                break;
            default:
                break;
        }
        ListenerRotateAnim();

        
    }

    public void Fall()
    {
        playerFacade.playerHangSystem.HangCheckUpdate();
    }
    public override void AttackUpdate()
    {
        //playerFacade.weaponManager.currentWeapon.Attack<PlayerController>(this);
    }
    public void SedAttack(string methodName)
    {
        if (playerFacade?.weaponManager?.currentWeapon)
        {
            playerFacade.weaponManager.currentWeapon.SendMessage(methodName);
        }
    }
    public void Move()
    {
        inputDir = inputSystem.inputDir;
        switch (moveMode)
        {
            case ActorMoveMode.Common:
                ListenerCommmonMove();
                ListenerClosestWall();
                break;
            case ActorMoveMode.ClimbLadder:
                playerFacade.playerHangSystem.ClimbLadderOnUpdate();
                break;
            case ActorMoveMode.ClosetWall:
                ListenerClosestWall();
                ListenerCloseWallMove();
                break;
            case ActorMoveMode.Shimmy:
                break;
            case ActorMoveMode.LockTarget:
                EnableRotate(false);
                ListenerLockTargetMove();
                break;
            default:
                break;
        }
    }

    public void JumpExcute()
    {
        if ((int)fsmMachine.lastState.stateType==(int)PlayerStateType.ShiftTeleport)
        {
            playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 3);
        }
        else
        {
            playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 1);
        }
        var predictToAirBaseVelocity = moveComponent.GetIntoAirMinUpVelocity() * Vector3.up;//v=s/t
        moveComponent.deltaPosition += predictToAirBaseVelocity * Time.fixedDeltaTime;//s
    }
    public void Jump()
    {
        RaycastHit hitInfo;
        if (actorPhyiscal.SpherecastFromBodySphere(false, false, transform.forward, PlayerableConfig.Instance.check_InWallForward_MaxDis, PlayerableConfig.Instance.allowUpWallLayerMask, out hitInfo, 0.2f))
        {
            playerFacade.playerStateManager.IsGround = false;
            if (playerAnim.stateInfo.IsName("Jump1") && playerAnim.stateInfo.normalizedTime > 0.25f && (inputSystem.jumpValue > 0.1f&&inputSystem.jumpValue<0.3f))
            {

                if (actorPhyiscal.IsOwnCollider(hitInfo) == false)
                {
                    Debug.Log("forward wall and press jump");
                    if (inputDir.magnitude != 0)
                    {
                        Debug.Log("--------Jump02--------");
                        playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 2);
                    }
                }
            }
            else
            {
                //if (playerAnim.stateInfo.IsName("Falling Idle"))
                //{
                //    playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 1);
                //}
            }
        }
        playerFacade.playerHangSystem.HangCheckUpdate();
    }

    public void StabAttack()
    {
        //if (transform.IsInEnemyBack(actorPhyiscal.CheckLockTargetIsExist()) == false)
        //{
        //    playerAnim.SetInt(AnimatorParameter.AttackAction.ToString(), 4);
        //    playerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
        //    playerAnim.SetTrigger(AnimatorParameter.Attack.ToString());
        //}
    }
    public void StartDefense()
    {

        ClearIncreaseVelo();
        EnableCtrl(false);
        EnableRotate(false);
        isReceiveDefenseInput = false;
        ClearMoveSpeed();
        targetBlend = 0;
        anim.SetFloat(AnimatorParameter.VeloDir.ToString(), 0);
        anim.SetFloat(AnimatorParameter.TurnAngle.ToString(), 0);
        stateManager.isDenfense = true;
        playerAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 1);
        playerAnim.SetLayerWeight(1, 1, true);
        playerFacade.weaponManager.StartDefense();
    }
    public void DefenseUpdate()
    {
        inputDir = inputSystem.inputDir;
        if (isReceiveDefenseInput == false&&inputSystem.CurrentPressMoveKey())
        {
            return;
        }
        isReceiveDefenseInput = true;
        if (isController)
        {
            if (inputDir.magnitude > 0)
            {
                lastInputDir = inputDir;
                var hor = inputDir.x * 1;
                var vert = inputDir.y * 1;
                playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), hor);
                //playerAnim.SetFloat(AnimatorParameter.Forward.ToString(), vert);
                targetBlend = vert;
                
            }
            else
            {
                if (lastInputDir != inputDir)
                {
                    targetBlend = 0;
                    playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), 0);
                    lastInputDir = Vector2.zero;
                }
                else
                {
                    playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), 0);
                }
            }
        }
        switch (moveMode)
        {
            case ActorMoveMode.Common:
                if (inputDir.magnitude > 0)
                {
                    var forward = camForwardInControlState;

                    forward = Vector3.ProjectOnPlane(forward, Vector3.up);
                    forward.y = 0;
                    if (forward != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * rotateSpeed);
                        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(forward)) < 0.1f)
                        {

                            transform.rotation = Quaternion.LookRotation(forward);

                            //SetTurn(0, false);
                        }
                    }
                }
                break;
            case ActorMoveMode.ClosetWall:
                break;
            case ActorMoveMode.Shimmy:
                break;
            case ActorMoveMode.LockTarget:
                ListenerLockTargetMove();
                break;
            default:
                break;
        }
    }
    public void ExitDefense()
    {
        isReceiveDefenseInput = false;
        stateManager.isDenfense = false;
        playerAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 0);
        playerAnim.SetLayerWeight(1, 0, true);
        this.EnableCtrl(true);
        playerFacade.weaponManager.CanelDefense();
    }
    public void StartRebound()
    {
        stateManager.isReBound = true;
        playerAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 2);
        playerAnim.SetLayerWeight(1, 1, true);
        playerFacade.weaponManager.StartDefense();

    }

    public void ShiftTeleportExcute()
    {
        targetDir = camRightInControlState * inputDir.x + camForwardInControlState * inputDir.y;
        targetDir.y = 0;
        transform.rotation = Quaternion.LookRotation(targetDir);
        teleportShadow.enabled = true;
        if (stateManager.isBattle == false)
        {
            anim.SetInteger(AnimatorParameter.JumpAction.ToString(),3);
            GameRoot.Instance.AddTask(800, () => { teleportShadow.enabled = false; });
        }
        else
        {
            IncreaseVelo = transform.forward * 200;
            PlayActorAudio("QuickRush");
            GameRoot.Instance.AddTask(1000, () => { teleportShadow.enabled = false; });
        }
        
       
    }
    public void JumpExit()
    {
        playerAnim.anim.SetInteger(AnimatorParameter.JumpAction.ToString(), 0);
    }

    #endregion

    #region Trigger
    public bool HangFallTrigger()
    {
        return inputSystem.pressDownJumpBtn;
    }
    public bool HangFinishTrigger()
    {
        return stateManager.isShimmy == false;
    }
    public bool IsHangUpTrigger()
    {
        return stateManager.isShimmy&&stateManager.isFall==false;
    }
    public bool InputMoveTrigger()
    {
        return inputSystem.inputDir.magnitude > 0;
    }
    public bool StopMoveTrigger()
    {
        return !InputMoveTrigger();
    }
    public bool CanelDefenseTrigger()
    {
        return (inputSystem.pressUpMouseRBtn) || inputSystem.mouseRightIsPressed == false;
    }
    public bool IntoDefenseTrigger()
    {
        return (inputSystem.mouseRightIsPressed) &&
           (inputSystem.pressUpMouseRExtend == false) &&
                (stateManager.isReBound == false && stateManager.isDenfense == false && stateManager.allowDenfense) && (moveMode == ActorMoveMode.Common || moveMode == ActorMoveMode.LockTarget);
    }
    public bool IsStabAttackTrigger()
    {

        return inputSystem.pressShiftBtn && (inputSystem.pressDownMouseLBtn||inputSystem.pressMouseLBtn);

    }
    public bool IsAttackFinishTrigger()
    {
        return stateManager.isAttack == false&&stateManager.nextAnimIsAttackAnim==false;
    }
    public bool ComboAttackTrigger()
    {
        return stateManager.allowAttack && inputSystem.pressDownMouseLBtn && (moveMode == ActorMoveMode.Common || moveMode == ActorMoveMode.LockTarget)&&facade.weaponManager.GetWeaponType()==WeaponType.Sword;
    }
    public bool AirComboAttackTrigger()
    {
         return ((stateManager.allowAttack||stateManager.waitNextAttackInput) && inputSystem.pressDownMouseLBtn && (moveMode == ActorMoveMode.Common || moveMode == ActorMoveMode.LockTarget));
    }
    public bool IntoReboundTrigger()
    {
        if (inputSystem.mouseRightIsPressed)
        {
            if (inputSystem.pressUpMouseRExtend)
            {
                if (stateManager.isReBound == false && (moveMode == ActorMoveMode.Common || moveMode == ActorMoveMode.LockTarget))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool JumpTrigger()
    {
        return (inputSystem.pressDownJumpBtn /*|| (inputSystem.pressJumpBtn&& inputSystem.jumpValue < 0.15f)*/) && (isController && stateManager.IsGround);
    }

    public bool NoGroundTrigger()
    {
        return stateManager.IsGround == false;
    }
    public bool InGroundTrigger()
    {
       return NoGroundTrigger()==false&&playerAnim.stateInfo.IsTag("LockStateJump") == false;
    }
    public bool JumpExitTrigger()
    {
        return stateManager.isJump == false && NoGroundTrigger()&&moveMode!=ActorMoveMode.ClimbLadder;
    }

    public bool ShiftTeleportTrigger()
    {
        if (inputSystem.pressShiftBtnExtend&&inputSystem.pressDownShiftBtn)
        {
            return true;
        }
        return false;
    }

    public bool JumpAttackTrigger()
    {
        if ((inputSystem.pressDownJumpBtn || inputSystem.pressJumpBtn))
        {

        }
        return (inputSystem.pressDownJumpBtn || inputSystem.pressJumpBtn) && (stateManager.IsGround) && inputSystem.pressDownMouseLBtn;
    }
     
    #endregion

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MotionModifyBuff motionBuff = GetActorComponent<BuffContainer>().DOBuff(2,actorSystem) as MotionModifyBuff;
            motionBuff.targetPos = transform.position+transform.forward * 10; 
            Debug.DrawLine(transform.position,motionBuff.targetPos,Color.red,100);
        }
        if (TimeLineManager.Instance.TimeLineIsPlay)
        {
            return;
        } 
        if (isController)
        {
            Transform cam = CameraMgr.Instance.transform;
            camRightInControlState = cam.right;
            camForwardInControlState = cam.forward;
        }
        //inBoxRangeEnemy = actorPhyiscal.CheckLockTargetIsExist();
        fsmMachine.Tick();
        //var h = Input.GetAxis("Horizontal");
        //var v = Input.GetAxis("Vertical");

        //inputDir = new Vector2(h, v);
       
        if (isCtrlRotate)
        {
            CalcuateRotate();
        }
        if (deltaRotation != Quaternion.identity)
        {
            transform.rotation *= deltaRotation;
        }
        CleatDeltaRotation();
        //ListenerJump();
        if (isController==false||moveMode==ActorMoveMode.ClosetWall)
        {
            LerpTarget();
        }
        ListenIsLockTargetMode();
        //ListenerAttack();
        //DenfenseMono();
        UpdateBlend();
        CheckingPickItem();
    }
    public void FixedUpdate()
    {
        CalcuateMove();
    }
    public float rotateSpeed = 45;
    public void SetTurn(float angle, bool turn = true)
    {
        if (isCtrlRotate)
        {
            if (turn)
            {
                playerAnim.SetTrigger("Turn", true);
            }
           
            playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), angle);
        }
    }
    public void CalcuateRotate()
    {
        if (stateManager.CheckState(ActorStateFlag.isDrawBow.ToString()))
        {
            return;
        }
        targetDir = camRightInControlState * inputDir.x + camForwardInControlState * inputDir.y;

        targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up);
        targetDir.y = 0;
        if (inputSystem.CurrentPressMoveKey() && inputDir.magnitude > 0.2f)
        {
            if (targetDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * rotateSpeed);
                if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(targetDir)) < 0.1f)
                {

                    transform.rotation = Quaternion.LookRotation(targetDir);
                    targetDir = Vector3.zero;
                    //SetTurn(0, false);
                }
            }

        }

        FixedEuler();
    }

    public void ListenerCommmonMove()
    {
        if (isController)
        {
            Transform cam = CameraMgr.Instance.transform;
            camRightInControlState = cam.right;
            camForwardInControlState = cam.forward;
            if (inputSystem.CurrentPressMoveKey() && inputDir.magnitude > 0.2f)
            {
                lastInputDir = inputDir;

                targetDir = camRightInControlState * inputDir.x + camForwardInControlState * inputDir.y;

                targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up);
                targetDir.y = 0;
                var magnitude = Mathf.Clamp01(inputDir.magnitude) * currentSpeeedMult;
                targetBlend = magnitude;
                if (playerAnim.stateInfo.IsTag("Locomotion"))
                {
                    moveSpeedMult = magnitude;
                }

                if (targetDir != Vector3.zero)
                {
                    //int angle = Mathf.RoundToInt(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(targetDir)));
                    //if (angle > 145)
                    //{
                    //    SetTurn(this.FixedAngle(angle), true);
                    //}
                }

                playerAnim.SetFloat(AnimatorParameter.VeloDir.ToString(), 1);
                if (stateManager.CheckState(ActorStateFlag.isDrawBow.ToString()))
                {
                    //瞄准状态下 人物旋转跟随镜头旋转。
                     playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), inputDir.x);//需不需要单位化
                     targetBlend = inputDir.y;
                }
            }
            else
            {
                if (moveSpeedMult!=0)
                {
                    ClearMoveSpeed();//
                }
                if (lastInputDir != inputDir)
                {
                    targetBlend = 0;
                    if ((lastInputDir.magnitude - inputDir.magnitude) > 0.1f)
                    {
                        playerAnim.SetFloat(AnimatorParameter.VeloDir.ToString(), -1);
                        lastInputDir = Vector2.zero;
                    }
                }
                else
                {
                    playerAnim.SetFloat(AnimatorParameter.VeloDir.ToString(), 0);
                }

                ListenerRotateAnim();
            }


            ListenerQuickSpeed();

        }
    }

    public virtual void ListenerCloseWallMove()
    {
        if (isController)
        {
            var dir = -transform.forward;


            int right = CalcuateInputDirInCam();
            if (right != 0)
            {
                RaycastHit hitInfo;
                dir = -transform.forward + right * transform.right * PlayerableConfig.Instance.check_closetWall_HorDirMult;
                Debug.DrawRay(actorPhyiscal.GetCheckPoint(CheckPointType.Chest).position, dir, Color.red, 2);
                Transform sliderOffsetPoint = right == 1 ? actorPhyiscal.GetCheckPoint(CheckPointType.RightOffsetPoint) : actorPhyiscal.GetCheckPoint(CheckPointType.LeftOffsetPoint);
                var sliderOrigin = sliderOffsetPoint.position;
                var slidercastDir = -transform.forward;
                playerFacade.SetMirrorParameter(right != 1);
                if (Physics.Raycast(sliderOrigin, slidercastDir, PlayerableConfig.Instance.boxCast_closestWall_maxDis, PlayerableConfig.Instance.allowClosetWallBehaviourLayerMask, QueryTriggerInteraction.Ignore))
                {
                    if (aimState)
                    {
                        aimState = false;
                        CameraMgr.Instance.CloseVirtualAim();
                    }
                    actorPhyiscal.SpherecastFromBodySphere(false, false, dir, PlayerableConfig.Instance.boxCast_closestWall_maxDis, PlayerableConfig.Instance.allowClosetWallBehaviourLayerMask, out hitInfo, 0.1f);
                    var offsetDir = Vector3.ProjectOnPlane(hitInfo.normal, Vector3.up);
                    float offsetDis = GetCharacterRadius();
                    CheckPointDownIsCanStand(hitInfo.point, Vector3.down, offsetDir, out hitInfo, offsetDis, (h) =>
                    {
                        Debug.DrawRay(h.point, Vector3.down, Color.yellow, 1);
                        if (Vector3.Distance(h.point,transform.position)>0.02f)
                        {
                            SetCharacterPosition(h.point, Vector3.up * 0.02f, lerpMotionSpeed, true);
                            SetCharacterQuaternion(offsetDir);
                        }
                    });
                }
                else
                {
                    if (aimState == false)
                    {
                        var aimPos = playerAnim.GetBodyTransform(HumanBodyBones.Neck).position;
                        var aimOffset = standCoverAimOffset_BaseRight;
                        aimOffset.x *= right; 
                        aimPos += transform.TransformVector(aimOffset);
                        CameraMgr.Instance.SetVirtualAim(aimPos, -transform.forward);
                        aimState = true;
                    }
                    targetBlend = 0;
                }
            }
            else
            {
                InputKeyUpWhenMove();
            }

        }
    }

    public void ListenerLockTargetMove()
    {
        if (isController)
        {
            if (inputDir.magnitude > 0)
            {
                lastInputDir = inputDir;
                var hor = inputDir.x * 1;
                var vert = inputDir.y * 1;



                playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), hor);
                //playerAnim.SetFloat(AnimatorParameter.Forward.ToString(), vert);
                targetBlend = vert;
            }
            else
            {
                if (lastInputDir != inputDir)
                {
                    targetBlend = 0;
                    playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), 0);
                    lastInputDir = Vector2.zero;
                }
            }
            bool pressDownDodge = inputSystem.pressDownAltLBtn;
            if (pressDownDodge)
            {
                playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(),10);
            }
        }
    }
    public void InputKeyUpWhenMove()
    {
        if (lastInputDir != inputDir)
        {

            lastInputDir = Vector2.zero;

        }
        targetBlend = 0;


    }
    public int CalcuateInputDirInCam()
    {
        Transform cam = CameraMgr.Instance.transform;
        camRightInControlState = cam.right;
        camForwardInControlState = cam.forward;
        int right = 0;
        if (inputDir.magnitude > 0)
        {
            lastInputDir = inputDir;
            targetDir = camRightInControlState * inputDir.x + camForwardInControlState * inputDir.y;

            var inOwnPlaneVec = Vector3.ProjectOnPlane(targetDir, transform.up);
            if (Vector3.Angle(transform.right, inOwnPlaneVec) <= 45)
            {
                targetBlend = 1;
                right = 1;

                playerAnim.SetFloat(AnimatorParameter.VeloDir.ToString(), 1);
            }
            else if (Vector3.Angle(-transform.right, inOwnPlaneVec) <= 45)
            {
                targetBlend = -1;
                right = -1;

                playerAnim.SetFloat(AnimatorParameter.VeloDir.ToString(), -1);
            }

        }
        return right;
    }

    public Quaternion GetPlayerForwardInNowViewPort()
    {
        Transform cam = CameraMgr.Instance.transform;
        var inOwnPlaneVec = Vector3.ProjectOnPlane(cam.forward, transform.up);
        inOwnPlaneVec = inOwnPlaneVec.normalized;
        if (inOwnPlaneVec == Vector3.zero)
        {
            inOwnPlaneVec = transform.forward;
        }
        var forward=Quaternion.LookRotation(inOwnPlaneVec,Vector3.up);
        return forward;
    }
    public void ListenerRotateAnim()
    {
        if (isCtrlRotate == false)
        {
            return;
        }
        if ((inputSystem.CurrentPressMoveKey()||inputSystem.CurrentPressDownMoveKey())&&inputDir.magnitude<=0.2f)
        {

            Transform cam = CameraMgr.Instance.transform;
            if (inputDir.y != 0)
            {
                inputDir.y = Mathf.Abs(inputDir.y) / inputDir.y;
            }
            if (inputDir.x != 0)
            {
                inputDir.x = Mathf.Abs(inputDir.x) / inputDir.x;
            }
            var currentTargetDir = cam.right * inputDir.x + cam.forward * inputDir.y;
            currentTargetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }

            //int angle = Mathf.RoundToInt(Quaternion.Angle(Quaternion.LookRotation(transform.forward), Quaternion.LookRotation(currentTargetDir)));
            Vector3 cross=Vector3.Cross(currentTargetDir,transform.forward);
            int mark = cross.y > 0 ?1:-1;
            float angle = Vector3.Angle(currentTargetDir, transform.forward);
            if (Mathf.Abs(angle)>10)
            {
                SetTurn(angle * mark);
            }
            
            //Vector3 localInput = transform.TransformDirection(new Vector3(inputDir.x, 0, inputDir.y));
            //float angle = (int)Vector2.SignedAngle(inputDir, Vector2.up);
            //angle /= 90 * 1.0f;
            //angle = Mathf.RoundToInt(angle);
            //angle *= 90;
            //float right = Vector2.Dot(Vector2.up, inputDir);
            //Vector3 cross = Vector3.Cross(Vector2.up, inputDir);
            //if (cross.z != 0)
            //{
            //    right = cross.z / Mathf.Abs(cross.z);
            //}

            //targetDir = currentTargetDir;
            //angle = (int)this.FixedAngle(angle);

            //if (angle == 0)
            //{
            //    angle = -180;
            //}
            //SetTurn(Mathf.Abs(angle) * right);
        }
    }
    public override void CalcuateMove()
    {
        moveComponent.CalcuateMove();
        if (moveComponent.IsInGrounded == false)
        {
            if (verticalVelocity <= 0 )
            {
                verticalVelocity -= moveComponent.GetGravityABS() * fallMultiply * Time.deltaTime;
            }
            else
            {
                verticalVelocity -= moveComponent.GetGravityABS() * Time.deltaTime;
            }
        }
        if (stateManager.isFall||stateManager.isJump)
        {
            moveComponent.deltaPosition += Vector3.up*verticalVelocity*Time.deltaTime;
        }
    }

    public override void AnimatorMove(Animator animator, Vector3 velocity, Quaternion angular)
    {
        //if (EndSkillTimeLineManager.Instance.EndKillTLPlayed)
        //{
        //    return;
        //}
        
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        

        //if (stateInfo.IsName("FastRush"))
        //{

        //    DeltaPosition += velocity * PlayerAnimatorConfig.Instance.runRushMult;
        //}
        //if (stateInfo.IsName("Run To Stop"))
        //{

        //    DeltaPosition += velocity * PlayerAnimatorConfig.Instance.runToStopMult;
        //}
        //if (stateInfo.IsName("Jump1"))
        //{
        //    DeltaPosition = (DeltaPosition + Vector3.up * velocity.y * PlayerAnimatorConfig.Instance.jump1Mult) / 2;

        //}
        //if (stateInfo.IsTag("Turn"))
        //{
        //    DeltaPosition += velocity * PlayerAnimatorConfig.Instance.turnVeloMult;
        //    deltaRotation *= angular;
        //    Debug.Log(Quaternion.ToEulerAngles(angular));
        //}
        //if (stateInfo.IsName("Land2"))
        //{
        //    DeltaPosition += velocity * PlayerAnimatorConfig.Instance.land2Mult;
        //}
        //if (stateInfo.IsName("cover to stand"))
        //{
        //    var mult = PlayerAnimatorConfig.Instance.coverToStandMult;
        //    DeltaPosition += velocity * mult;
        //    deltaRotation *= angular;
        //}
        //if (stateInfo.IsName("HangToClimb"))
        //{
        //    var mult = PlayerAnimatorConfig.Instance.hangToClimbMult;
        //    DeltaPosition += velocity * mult;
        //}
        //if (stateInfo.IsName("Drop To Freehang"))
        //{
        //    deltaRotation *= angular;
        //}
        //if (stateInfo.IsTag("LockLocomotion"))
        //{

        //    DeltaPosition += velocity * PlayerAnimatorConfig.Instance.lockLocomotionMult;
        //    DeltaPosition=DeltaPosition.SetY(0);
        //    deltaRotation *= angular;
        //}
        //if (stateInfo.IsTag("Attack"))
        //{
        //    var mult = PlayerAnimatorConfig.Instance.comboAttackMotionMult;
        //    DeltaPosition += velocity * mult;
        //    deltaRotation *= angular;
        //}
        //if (stateInfo.IsTag("Hit"))
        //{
        //    var mult = PlayerAnimatorConfig.Instance.test;
        //    DeltaPosition += velocity * mult;
        //    deltaRotation *= angular;
        //}
        //if (stateInfo.IsTag("LockStateJump"))
        //{
        //    var mult = PlayerAnimatorConfig.Instance.lockStateJumpMult;
        //    DeltaPosition += velocity * mult;
        //    deltaRotation *= angular;
        //}
        //if (stateInfo.IsTag("DefenseLocomotion"))
        //{

        //    DeltaPosition += velocity * PlayerAnimatorConfig.Instance.lockLocomotionMult;
        //    DeltaPosition=DeltaPosition.SetY(0);
        //    deltaRotation *= angular;
        //}

    }

    public bool IsCurrentPlayingClip(int layer, string name)
    {
        bool have = false;
        AnimatorClipInfo[] infos = playerAnim.anim.GetCurrentAnimatorClipInfo(layer);
        foreach (var item in infos)
        {
            if (item.clip.name == name)
            {
                have = true;
                break;
            }
        }
        return have;
    }


    public override void DisableCtrl(bool DisRotate = true)
    {
        isController = false;
        if (DisRotate)
        {
            isCtrlRotate = false;
        }
        loseCtrlMoveSpeed = moveSpeedMult;
    }
    public override void EnableCtrl(bool EnableRotate = true)
    {
        isController = true;
        if (EnableRotate)
        {
            isCtrlRotate = true;
        }
    }

    /// <summary>
    /// 直接对MoveSpeed进行操作
    /// </summary>
    /// <param name="mult"></param>

    public void RenewCommmondRunSpeed()
    {
        currentSpeeedMult = PlayerableConfig.Instance.commmonRunMult;
    }
    public void ListenerQuickSpeed()
    {
        if (inputSystem.pressDownShiftBtn && playerAnim.stateInfo.IsTag("Locomotion") && IsCurrentPlayingClip(0, "Running"))
        {
            playerAnim.SetTrigger("Rush");
        }
        if (inputSystem.pressShiftBtn)
        {
            currentSpeeedMult = PlayerableConfig.Instance.quickRunMult;

        }
        else
        {
            RenewCommmondRunSpeed();
        }
    }

    public void CheckingQuickTurn()
    {
        if (playerAnim.anim.GetInteger(AnimatorParameter.CoverAction.ToString()) != 0)
        {
            SetTurn(0, false);
            return;
        }
        Transform cam = CameraMgr.Instance.transform;
        targetDir = camRightInControlState * inputDir.x + camForwardInControlState * inputDir.y;
        targetDir.y = 0;
        if (targetDir != Vector3.zero)
        {
            int angle = Mathf.RoundToInt(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(targetDir)));
            if (angle > 145)
            {
                SetTurn(this.FixedAngle(angle), true);
                Debug.Log("Turn");
            }
        }

    }

    //public void ListenerJump()
    //{//
    //    if (inputSystem.pressDownJumpBtn || inputSystem.pressJumpBtn)
    //    {
    //        if ((isController && stateManager.isGround))
    //        {

    //            if (inputSystem.pressDownJumpBtn)
    //            {
    //                playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 1);
    //            }

    //        }//
    //        else
    //        {
    //            RaycastHit hitInfo;
    //            if (actorPhyiscal.SpherecastFromBodySphere(false, false, transform.forward, PlayerableConfig.Instance.check_InWallForward_MaxDis, PlayerableConfig.Instance.allowUpWallLayerMask, out hitInfo, 0.2f))
    //            {
    //                playerFacade.playerStateManager.isGround = false;
    //                if (playerAnim.stateInfo.IsName("Jump1") && playerAnim.stateInfo.normalizedTime > 0.1f && inputSystem.jumpValue < 0.2f)
    //                {

    //                    if (actorPhyiscal.IsOwnCollider(hitInfo) == false)
    //                    {
    //                        Debug.Log("forward wall and press jump");
    //                        if (inputDir.magnitude != 0)
    //                        {
    //                            Debug.Log("--------Jump02--------");
    //                            playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 2);
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    //if (playerAnim.stateInfo.IsName("Falling Idle"))
    //                    //{
    //                    //    playerAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 1);
    //                    //}
    //                }
    //            }
    //        }
    //    }
    //}


    public Collider lastWall;
    public bool isCanCanelClosetWall = false;
    public void ListenerClosestWall()
    {
        if (playerAnim.stateInfo.IsTag("Locomotion") && IsCurrentPlayingClip(0, "FastRun"))
        {
            //当加速跑 此刻不做检测，节省性能
            return;
        }

        if (moveMode == ActorMoveMode.ClosetWall)
        {
            var origin = actorPhyiscal.GetCheckPoint(CheckPointType.Chest).position;
            var offsetDir = transform.forward;

            float offsetDis = GetCharacterRadius() + 0.1f;
            RaycastHit hitInfo2 = new RaycastHit();
            if (CheckPointDownIsCanStand(origin, Vector3.down, offsetDir, out hitInfo2, offsetDis, null))
            {
                if (isCanCanelClosetWall == false)
                {
                    GameRoot.Instance.SignHandlerMainEvent(new EventInfo { eventTips = "Canel Closet Wall", eventCode = EventCode.CanelClosetWall,keyBehaviourType=KeyBehaviourType.RoleBehaviour }, () =>
                    {
                        playerAnim.SetInt(AnimatorParameter.CoverAction.ToString(), -1);
                        SetCharacterPosition(transform.position, Vector3.up * 0.02f, 1);
                        isCanCanelClosetWall = false;
                        lastWall = null;
                        moveMode = ActorMoveMode.Common;
                        return true;
                    });
                }
                isCanCanelClosetWall = true;

                ////SetCharacterQuaternion(offsetDir);
            }
            else
            {
                isCanCanelClosetWall = false;
                GameRoot.Instance.LeaveHandlerMainEvent(EventCode.CanelClosetWall);
            }
        }
        else
        {
            if (moveMode==ActorMoveMode.ClimbLadder)
            {
                return;
            }
            if (playerAnim.anim.GetInteger(AnimatorParameter.CoverAction.ToString()) == 0 && playerAnim.stateInfo.IsTag("Locomotion"))
            {
                RaycastHit hitInfo;
                var boxCastHalf = transform.TransformDirection(PlayerableConfig.Instance.boxCast_closestWall_halfExtent);
                boxCastHalf = boxCastHalf.Abs();
                if (actorPhyiscal.BoxCast(CheckPointType.Chest, boxCastHalf, transform.forward, Quaternion.LookRotation(transform.forward, Vector3.up), PlayerableConfig.Instance.allowClosetWallBehaviourLayerMask, out hitInfo, PlayerableConfig.Instance.boxCast_closestWall_maxDis, QueryTriggerInteraction.Ignore))
                {
                }
                if (lastWall != hitInfo.collider && hitInfo.collider != null)
                {
                    var origin = hitInfo.point;
                    var offsetDir = Vector3.ProjectOnPlane(hitInfo.normal, Vector3.up);
                    float offsetDis = GetCharacterRadius() + 0.02f;
                    Func<bool> closetWallAction = () =>
                    {
                        RaycastHit hitInfo2 = new RaycastHit();
                        bool isVaildExcute = false;
                        Debug.DrawRay(origin, offsetDir, Color.red, 5, true);
                        Debug.DrawRay(origin, hitInfo.normal, Color.green, 5, true);
                        CheckPointDownIsCanStand(origin, Vector3.down, offsetDir, out hitInfo2, offsetDis, (h) =>
                            {
                                if (playerAnim.anim.GetInteger(AnimatorParameter.CoverAction.ToString()) == 0 && playerAnim.stateInfo.IsTag("Locomotion") && playerAnim.anim.IsInTransition(0) == false)
                                {
                                    playerAnim.SetInt(AnimatorParameter.CoverAction.ToString(), 1);

                                    StartCoroutine(GameRoot.Instance.StayExcuteOnMuchFrame(3, () =>
                             {
                                 SetCharacterPosition(h.point, Vector3.up * 0.02f, 1);
                                 SetCharacterQuaternion(offsetDir);
                             }));
                                    isVaildExcute = true;
                                }
                            });
                        Debug.DrawLine(origin, hitInfo2.point, Color.blue, 2);
                        return isVaildExcute;
                    };
                    GameRoot.Instance.SignHandlerMainEvent(new EventInfo { eventTips = "Check Wall", eventCode = EventCode.ClosetWall,keyBehaviourType=KeyBehaviourType.RoleBehaviour }, closetWallAction);
                }
                else if (lastWall != hitInfo.collider)
                {
                    GameRoot.Instance.LeaveHandlerMainEvent(EventCode.ClosetWall);
                }
                lastWall = hitInfo.collider;
            }
        }
    }
    public bool CheckPointDownIsCanStand(Vector3 origin, Vector3 castDir, Vector3 offsetDir, out RaycastHit hitInfo2, float offsetDis, Action<RaycastHit> canAction)
    {
        origin += offsetDir * offsetDis;
        if (Physics.SphereCast(origin, 0.1f, castDir, out hitInfo2, Mathf.Infinity, actorPhyiscal.walkableLayer, QueryTriggerInteraction.Ignore))
        {
            canAction?.Invoke(hitInfo2);
            return true;
        }
        return false;
    }




    public void CtrlEnableGravity(bool isGravity)
    {
        moveComponent.EnableGravity(isGravity);
    }

    
    public void ChanelCharacterLookToTarget()
    {

    }

    public void ListenerAttack()
    {
        //if (inputSystem.pressShiftBtnExtend && inputSystem.pressDownMouseLBtn)
        //{
        //    //
        //    if (transform.IsInEnemyBack(actorPhyiscal.CheckLockTargetIsExist()) == false)
        //    {
        //        playerAnim.SetInt(AnimatorParameter.AttackAction.ToString(), 4);
        //        playerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
        //        playerAnim.SetTrigger(AnimatorParameter.Attack.ToString());
        //    }
        //    else
        //    {
        //        SkillSystem.Instance.AttackSkillHandler(this, 4);
        //    }

        //}
        //else
        //{
        //    //CommonAttack();
        //}
    }


    public void ListenIsLockTargetMode()
    {
        if (moveMode == ActorMoveMode.LockTarget)
        {
            if (lockTarget)
            {
                Vector3 dis = lockTarget.transform.position - transform.position;
                
                if (dis.magnitude>1f)
                {
                    transform.rotation =/* Quaternion.Slerp(transform.rotation,*/ Quaternion.LookRotation(Vector3.ProjectOnPlane(dis, Vector3.up))/*,Time.deltaTime*15)*/;
                }
                if (lockTarget.damageable.IsDie)
                {
                    EnableCtrl();
                    CameraMgr.Instance.CanelLockTarget();
                }
            }

        }

        if (inputSystem.key_MouseMiddle_Event.OnPressed)
        {
            if (CameraMgr.Instance.isLockTarget)
            {
                EnableCtrl();
                CameraMgr.Instance.CanelLockTarget();
            }
            else
            {
                if (CameraMgr.Instance.CurrentFreeLookIsLive())
                {
                    Transform _lockTarget = actorPhyiscal.CheckLockTargetIsExist();
                    if (_lockTarget)
                    {
                        Debug.Log(" have target");
                        EnemyController enemy = _lockTarget.GetComponent<EnemyController>();
                        if (enemy)
                        {
                            CameraMgr.Instance.SetLockTarget(enemy);
                            ExChangeMoveMode(ActorMoveMode.LockTarget);
                            SetLockTarget(enemy);
                            //playerAnim.SetInt(AnimatorParameter.State.ToString(), (int)ActorMoveMode.LockTarget);
                            playerAnim.SetBool(AnimatorParameter.LockTarget.ToString(), true);
                            isCtrlRotate = false;
                            //Lock Target
                        }
                        ClearAllMotionData(false);
                    }
                    else
                    {
                        CameraMgr.Instance.SetLockTarget(null);
                    }
                }
                else
                {
                    //Novalid
                }

            }
        }
    }

    public void SetLockTarget(RoleController _lockTarget)
    {
        lockTarget = _lockTarget;
        FixedCharacterLookToTarget(lockTarget?.transform);
    }


    public List<ComoboEffectSetting> weaponEffect;
    public void CommonAttack()
    {
        if (!stateManager.allowAttack)
        {
            return;
        }
        if (inputSystem.key_MouseLeft_Event.OnPressed)
        {

            //bool attack = false;
            //if (stateManager.nextAttackVaildSign)
            //{
            //    //if (playerFacade.weaponManager.attackedTarget)
            //    //{
            //    //    EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(this, playerFacade.weaponManager.attackedTarget.GetComponentInParent<RoleController>(),EndKillTimeLineConfig.Instance.specialTimeLineConfigs[0].playerableAssets[0]);
            //    //    stateManager.nextAttackVaildSign = false;
            //    //    return;
            //    //}
            //    //playerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
            //    //playerAnim.SetTrigger(AnimatorParameter.Attack.ToString(), true);
            //    SkillSystem.Instance.AttackSkillHandler(this, commonAttackID + 1);
            //    attack = true;
            //}
            //else
            //{
            //    if (playerAnim.stateInfo.IsTag("Attack") == false)
            //    {
            //        commonAttackID = 0;
            //        playerAnim.SetInt(AnimatorParameter.AttackAction.ToString(), commonAttackID);
            //        playerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
            //        SkillSystem.Instance.AttackSkillHandler(this, commonAttackID);

            //        //playerAnim.SetTrigger(AnimatorParameter.Attack.ToString(), true);
            //        attack = true;
            //    }
            //}
        }
    }
    public void PlayWeaponEffect(int id)
    {
        //if (id >= weaponEffect.Count)
        //{
        //    return;
        //}
        //GameObject effect = Instantiate(weaponEffect[id].effect);
        //effect.transform.position = weaponEffect[id].target.position;
        //effect.transform.rotation = weaponEffect[id].target.rotation;
        //effect.SetActive(false);
        //effect.SetActive(true);
        //StartCoroutine(GameRoot.Instance.AddTask((int)(weaponEffect[id].disableTime * 1000), () => { Destroy(effect); }));
    }

    public void DenfenseMono()
    {
        if (inputSystem.mouseRightIsPressed)
        {
            if (inputSystem.pressUpMouseRExtend)
            {
                if (stateManager.isReBound == false)
                {
                    stateManager.isReBound = true;
                    Debug.Log("格挡");
                    playerAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 2);
                    playerAnim.SetLayerWeight(1, 1, true);
                    //StartCoroutine(GameRoot.Instance.AddTask(1000, () => { stateManager.isReBound = false; }));
                }
            }
            else
                if (stateManager.isReBound == false && stateManager.isDenfense == false && stateManager.allowDenfense)
            {
                stateManager.isDenfense = true;
                playerAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 1);
                playerAnim.SetLayerWeight(1, 1, true);
                playerFacade.weaponManager.StartDefense();
            }
        }
        else
        {

            if (inputSystem.pressUpMouseRBtn)
            {
                stateManager.isDenfense = false;
                playerAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 0);
                playerAnim.SetLayerWeight(1, 0, true);
                playerFacade.weaponManager.CanelDefense();
            }
        }

    }
    public override void OnHit(int hitAction, float angle = 0, int defenseAction = 0)
    {
        anim.SetFloat(AnimatorParameter.HitAngle.ToString(), angle);
        playerFacade.weaponManager.CanelDefense(false);
        Debug.Log(" defenseAction" + defenseAction);
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), hitAction);
        anim.SetInteger(AnimatorParameter.DefenseAction.ToString(), defenseAction);
        anim.speed = 0.1f;
        GameRoot.Instance.AddTask(100, () => { anim.speed = 1; });
    }
    public void ChanelLockTarget()
    {
        playerAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), 0);
        playerAnim.SetFloat(AnimatorParameter.Forward.ToString(), 0);
        SetLockTarget(null);
        ExChangeMoveMode(ActorMoveMode.Common);
        CameraMgr.Instance.SetLockTarget(null);
        //Canel Lock Target
        playerAnim.SetBool(AnimatorParameter.LockTarget.ToString(), false);
    }

    public void TryExchangeWeapon()
    {
        if (inputSystem.scrollWheel!=0)
        {
            float scrollAbs = Mathf.Abs(inputSystem.scrollWheel);
            if (scrollAbs>0.1f)
            {
                //切换武器
                facade.weaponManager.ExchangeWeaponByScroll(scrollAbs>0?1:-1);
            }
        }
    }

    public void BowStay()
    {
        if (facade.weaponManager.GetWeaponType()==WeaponType.Sword||stateManager.CheckState(ActorStateFlag.AllowBowStay.ToString())==false)
        {
            return;
        }
        if (inputSystem.pressMouseLBtn)
        {
            if (stateManager.CheckState(ActorStateFlag.isDrawBow.ToString())==false)
            {
                playerAnim.SetLayerWeight(3,1,false);
                playerAnim.SetBool(AnimatorParameter.XuLi.ToString(),true);
                if (aimState == false)
                {
                    BattleAction?.Invoke();
                    //var aimPos = playerAnim.GetBodyTransform(HumanBodyBones.Neck).position;
                    var aimOffset = bowAimOffsetBaseRight;
                    //aimPos += transform.TransformVector(aimOffset);
                    CameraMgr.Instance.SetCanControlVirtualAim(aimOffset, transform.forward);
                    aimState = true;

                }
            }
        }
        else if (inputSystem.pressUpMouseLBtn)
        {
            playerAnim.SetBool(AnimatorParameter.XuLi.ToString(), false);
        }
        if (aimState&&CameraMgr.Instance.isControlVirtualCam)
        {
            CharacterLerpLookToTarget(transform.position+CameraMgr.Instance.transform.forward,Time.deltaTime*10);
        }
        if (playerAnim.anim.GetLayerWeight(3)>=1)
        {
            AnimatorStateInfo animatorStateInfo = playerAnim.anim.GetCurrentAnimatorStateInfo(3);
            if (animatorStateInfo.IsName("Idle")&&playerAnim.anim.GetBool(AnimatorParameter.XuLi.ToString())==false)
            {
                playerAnim.SetLayerWeight(3, 0, true);
                if (aimState)
                {
                    aimState = false;
                    CameraMgr.Instance.CloseVirtualAim();
                }
            }
        }
    }
    public override void EquipWeaponHandler(IWeapon weapon)
    {
        base.EquipWeaponHandler(weapon);
        switch (weapon.weaponData.GetWeaponType())
        {
            case WeaponType.Sword:
                weapon.weaponEquipAndTake = () => { EquipSword(weapon); };
                weapon.weaponUnEquipAndUnTake = () => { UnEquipSowrd(weapon); };
                break;
            case WeaponType.Bow:
                weapon.weaponEquipAndTake = () => { EquipBow(weapon);  };
                weapon.weaponUnEquipAndUnTake = () => { UnEquipBow(weapon); };
                break;
            default:
                break;
        }
    }
    public void EquipSword(IWeapon weapon)
    {
        facade.weaponManager.weaponRigList[0].weight = 1; weapon.gameObject.SetActive(true); weapon.EquipWeaponInRendererView(); 
    }
    public void UnEquipSowrd(IWeapon weapon)
    {
        facade.weaponManager.weaponRigList[0].weight = 0; weapon.gameObject.SetActive(false); weapon.EquipWeaponInRendererView();
    }
    public void EquipBow(IWeapon weapon)
    {
        facade.weaponManager.weaponRigList[1].weight = 1; weapon.gameObject.SetActive(true); weapon.EquipWeaponInRendererView();
        if (NGUI_GameMainPopup.Instance)
        {
            NGUI_GameMainPopup.Instance.OpenCrosshairUI();
        }
    }
    public void UnEquipBow(IWeapon weapon)
    {
         facade.weaponManager.weaponRigList[1].weight = 0;weapon.gameObject.SetActive(false); weapon.UnEquipWeaponInRendererView();
        if (NGUI_GameMainPopup.Instance)
        {
            NGUI_GameMainPopup.Instance.CloseCrosshairUI();
        }
    }
    public override void UnEquipWeaponHandler(IWeapon weapon)
    {
        if (weapon==null)
        {
            return;
        }
        base.UnEquipWeaponHandler(weapon);
        EquipWeaponHandler(weapon);
    }
    public void CheckHangUpPlatformStrikeTarget()
    {
        //Check  have  Target in head top and forward Direction
        var origin = actorPhyiscal.GetCheckPoint(CheckPointType.HeadTop).position + transform.TransformVector(PlayerableConfig.Instance.castFromHeadTopOffset_hangUpStrick);
        Collider[] colliders;
        var boxCastHalf = transform.TransformDirection(PlayerableConfig.Instance.boxCast_hangUpStrick_halfExtent);
        boxCastHalf=boxCastHalf.Abs();
        colliders = Physics.OverlapBox(origin, boxCastHalf, Quaternion.identity, PlayerableConfig.Instance.enemyLayerMask.value,QueryTriggerInteraction.Collide);
        if (colliders!=null&&colliders.Length>0)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject != gameObject)
                {
                    IDamageable damageable = collider.GetComponent<IDamageable>();
                    if (damageable)
                    {
                        RoleController enemy_roleController = damageable.characterFacade.roleController;
                        Vector3 raycastOrigin = new Vector3(0, origin.y, 0);
                        if (Physics.Linecast(raycastOrigin, enemy_roleController.centerTrans.transform.position, PlayerableConfig.Instance.enemyLayerMask))
                        {
                            EndSkillType endSkillType = EndSkillType.FrontStrike;
                            if (transform.IsInEnemyBack(damageable.transform))
                            {
                                endSkillType = EndSkillType.BackStrike;
                            }
                            if (enemy_roleController.stateManager.CheckState(ActorStateFlag.isSleep.ToString()))
                            {
                                endSkillType|=EndSkillType.SleepStrike;
                            }
                            PlayableAsset asset = EndKillTimeLineConfig.Instance.GetPlayerableAsset(actorSystem.roleDefinition, damageable.actorSystem.roleDefinition, endSkillType);
                            if (asset==null)
                            {
                                MyDebug.DebugSupperError("突袭TimeLine获取失败，请检查");
                                return;
                            }
                            MyDebug.DebugPrint("HangUpStrick");
                            playerFacade.playerHangSystem.hangUpPlatfromFinish = () => { EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(this, enemy_roleController, asset);};
                            switch (moveMode)
                            {
                                case ActorMoveMode.Common:
                                    break;
                                case ActorMoveMode.ClosetWall:
                                    break;
                                case ActorMoveMode.Shimmy:
                                    playerFacade.playerHangSystem.HangToPlatform();
                                    break;
                                case ActorMoveMode.LockTarget:
                                    break;
                                case ActorMoveMode.ClimbLadder:
                                    playerFacade.playerHangSystem.ClimbLadderToPlaform();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
    public void CheckingPickItem()
    {
        //可以设置间隔多少帧进行检测 节省性能
        Collider[] colliders = Physics.OverlapBox(actorPhyiscal.GetCheckPoint(CheckPointType.Feet).position, transform.TransformDirection(PlayerableConfig.Instance.boxCast_CheckPicker_HalfExtent).Abs(), Quaternion.identity, PlayerableConfig.Instance.pickItemLayerMask.value, QueryTriggerInteraction.Collide);
        if (colliders!=null&&colliders.Length>0)
        {
            List<DropItemObject> itemObjectList = null;
            foreach (var collider in colliders)
            {
                DropItemObject itemObject = collider.GetComponent<DropItemObject>();
                if (itemObject==null)
                {
                    continue;
                }
                if (itemObjectList==null)
                {
                    itemObjectList = new List<DropItemObject>();
                }
                else
                {
                    if (itemObjectList.Contains(itemObject))
                    {
                        //可能存在一个物体上有多个碰撞体，所以进行判断Item是否之前已经存入列表了
                        continue;
                    }
                }
                itemObjectList.Add(itemObject);
            }
            if (itemObjectList!=null)
            {
                ShowItemInPickerPopup(itemObjectList);
            }
        }
        else if (dropItemsList.Count>0)
        {
            MyDebug.DebugPrint("关闭拾取列表");
            UIManager.Instance.TryPopPanel(UIPanelIdentity.LootItemPanel);
            dropItemsList.Clear();
        }
    }
    public void ShowItemInPickerPopup(List<DropItemObject> itemObjects)
    {
        bool isUpdate = false;
        //数量是否一致
        if (itemObjects.Count!=dropItemsList.Count)
        {
            isUpdate = true;
        }
        else
        {
            bool isSame = true;
            foreach (var currentItem in itemObjects)
            {
                if (dropItemsList.Contains(currentItem)==false)
                {
                    isSame = false;
                    break;
                }
            }
            isUpdate = !isSame;
        }
        if (isUpdate)
        {
            MyDebug.DebugPrint("更新拾取列表");
            UnityEngine.Profiling.Profiler.BeginSample("UIManagerPush");
            UIManager.Instance.PushPanel(new LootObjectsPopupDefinition(itemObjects));
            UnityEngine.Profiling.Profiler.EndSample();
            dropItemsList = itemObjects;
        }
    }
    public override Vector3 GetAimPoint(Vector3 weaponPoint,Quaternion lookForward)
    {
        Vector3 point = Vector3.zero;

        RaycastHit raycastHit = CameraMgr.Instance.CamCenterRaycast(-1, PlayerableConfig.Instance.bowArrowMaxRayDis, QueryTriggerInteraction.Ignore);
        if (raycastHit.collider != null)
        {
            point = raycastHit.point;
        }
        else
        {
            point = CameraMgr.Instance.transform.position + CameraMgr.Instance.transform.forward * PlayerableConfig.Instance.bowArrowMaxRayDis;
        }
        return point;
    }


    public override void OnDie(IDamageable.DamageData damageData)
    {
        base.OnDie(damageData);
        facade.characterAnim.SetInt(AnimatorParameter.HitAction.ToString(),-1);
        isController = false;
        isCtrlRotate = false;
        UIManager.Instance.PushPanel(new PopupDefinition(UIPanelIdentity.DiePopup,UILayerType.TopPopup,true));
    }
    public void OnDrawGizmos()
    {
        switch (moveMode)
        {
            case ActorMoveMode.Common:
                Gizmos.color = Color.white;
                
                var pickCheckRange = transform.TransformDirection(PlayerableConfig.Instance.boxCast_CheckPicker_HalfExtent).Abs()*2;
                
                Gizmos.DrawWireCube(transform.position,pickCheckRange);
                break;
            case ActorMoveMode.ClosetWall:
                break;
            case ActorMoveMode.Shimmy:
                if (Application.isPlaying)
                {
                    var origin = actorPhyiscal.GetCheckPoint(CheckPointType.HeadTop).position + transform.TransformVector(PlayerableConfig.Instance.castFromHeadTopOffset_hangUpStrick);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(origin,0.1f);
                    var boxCastSize = transform.TransformDirection(PlayerableConfig.Instance.boxCast_hangUpStrick_halfExtent) * 2;
                    boxCastSize=boxCastSize.Abs();
                    Gizmos.DrawWireCube(origin,boxCastSize);
                }
                
                break;
            case ActorMoveMode.LockTarget:
                break;
            default:
                break;
        }
    }

}
[Serializable]
public class ComoboEffectSetting
{
    public Transform target;
    public GameObject effect;
    public float disableTime = 4;
}