using Assets.Resolution.Scripts.Weapon;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;
using Assets.Resolution.Scripts.Region;
using System.Linq;
using Assets.Resolution.Scripts.Map;
using UnityEditor;
using BehaviorDesigner.Runtime;

public class RobotController : RoleController
{

    public float dis=10;
    public float angle=45;

    public FPS_PlayerController playerController;
    public Transform behaviorTreeTarget
	{
		get { return (Transform)behaviorTree.GetVariable("target").GetValue(); }
		set
		{
			behaviorTree.SetVariableValue("target", value);
		}
	}
    public Transform find_Target { get; set; }
	//TODO: traversingOffMeshLink 为jumpLink 认为在JumpLink的时候为不在地面。
	public override bool IsInGround { get { return !moveAgent.traversingOffMeshLink; } }
    public Transform targetPoint;
    public AimIK aimIK;
    public LookAtIK lookAtIK;
    public ArmIK rightHandIK;
    public LimbIK leftHandIK;
    public float aimIkLerpSpeed = 2;
    [HideInInspector]public AIRegion currenStayRegion;
    public AIRegion test_targetRegion;
    private AIRegion targetRegion;
    private Vector3 targetRegion_IntoGroundPos;
    private IntoRegionPath intoRegionPath;
    [HideInInspector]public bool isArriveRegionOrEntry=false;
    [HideInInspector]public Seeker seeker;
    [HideInInspector]public RichAI moveAgent;
    [HideInInspector]public RoadPointNode targetPointNode;
    private BehaviorTree behaviorTree;
    public override float characterRadius { get => actorSystem.GetComponent<CapsuleCollider>().radius; }
    private float bodyCenterHeight = 0.8f;//test
    [HideInInspector]public Vector3 recordTargetPos;
    public Dictionary<RoleController, List<DamageInfo>> damageStatistic=new Dictionary<RoleController, List<DamageInfo>>();
    public float onceRefreshDamageRecord = 1f;
    [HideInInspector]public DamageInfo justDamageInfo;
    public override bool isRecharging => AnimatorMachine.CheckAnimationStateByName(RuntimeInventory.currentWeapon.userAnimator,1,RuntimeInventory.currentWeapon.AnimationParameterComponent.GetAniamtionParameter(WeaponActionType.Recharge.ToString()).stateName);
    [Header("没有目标下的IK注视的辅助目标位置")]
    public Vector3 defaultAimTargetIKPose;
    [HideInInspector]public bool returnHome;

    public override bool isPullBoltting =>false;
    private Path currentPath;
    private Vector3 currentPathEndPos;
    private Vector3 previousPos;
    private BodyIKManager _BodyIKManager;
    public Transform meleeAttackCoordinateOriginPoint;
    [HideInInspector]public RecastMesh_DynamicObstacles recastMesh_DynamicObstacles;
    FiniteStateMachine<HumanState, HumanStateTransition, RobotController> stateMachine;

    #region ai相关
    public float checkViewAngle=80;
    public float checkViewDis=20;
    [HideInInspector]public Transform preOccupyPosition;

	#endregion
	public override void Init()
    {
        base.Init();
        previousPos = transform.position;

    }
    #region LifeTime
    public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        playerController = FindObjectOfType<FPS_PlayerController>();
        seeker = actorSystem.GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
        moveAgent = actorSystem.GetComponent<RichAI>();
        _BodyIKManager = GetComponent<BodyIKManager>();
        _BodyIKManager?.CloseAllIK(true);
        int upperLayerIndex = animatorMachine.animator.GetLayerIndex("Upper");
        animatorMachine.animator.SetLayerWeight(upperLayerIndex, 0);
        //TODO
        behaviorTree = actorSystem.GetComponent<BehaviorTree>();
        waitTimer_whenVeryNearAnotherAgent = TimeSystem.Instance.CreateTimer();
        recastMesh_DynamicObstacles = transform.GetComponent<RecastMesh_DynamicObstacles>();
    }

    public void StateMachineInit()
	{
		if (unit == UnitCamp.A)
		{
			stateMachine = new FiniteStateMachine<HumanState, HumanStateTransition, RobotController>();
			stateMachine.Init(this, HumanState.GoToRegion);
			//HumanIdleState humanIdleState = new HumanIdleState(HumanState.Idle,stateMachine);
			HumanGoToRegionState humanGoToRegionState = new HumanGoToRegionState(HumanState.GoToRegion, stateMachine);
			HumanInRegionState humanInRegionState = new HumanInRegionState(HumanState.InRegion, stateMachine);

			stateMachine.AddState(humanGoToRegionState);
		}
        
	}
    public override void Start()
    {
        base.Start();
        RuntimeInventory.WeaponLoadInInventoryHandler += AddWeaponInRuntimeInventory;
        RuntimeInventory.Init();
        StateMachineInit();
		//暂时禁用 测试Monster

		//AIRegion aIRegion = MapManager.Instance.GetOneAIRegion();
		//if (aIRegion)
		//{
		//	Transform targetPoint = aIRegion.PreOccupy(this);
		//	if (targetPoint != null)
		//	{
		//		seeker.StartPath(transform.position, targetPoint.position);
		//	}
		//}


		//切换武器之后 执行事件
		RuntimeInventory.OnSwitchWeapnAfterHandler += () =>
        {
			if (aimIK)
			{
                aimIK.solver.transform = RuntimeInventory.currentWeapon.transform;
			}

			if (RuntimeInventory.currentWeapon.AnimationParameterComponent)
			{
            //TODO:这个组件看看后期到底需不需要。
            //组件目的就是将动画状态名字绑定在枪上，对于全身角色来说 也要根据枪上绑定的名字来执行动画
				var animationParamater = RuntimeInventory.currentWeapon.AnimationParameterComponent.GetAniamtionParameter(WeaponActionType.Idle.ToString());
				animatorMachine.animator.CrossFade(animationParamater.stateName, 0);
			}
            


            WeaponIKInfo weaponIK = RuntimeInventory.currentWeapon.GetComponent<WeaponIKInfo>();
            if (weaponIK)
            {
                if (weaponIK.rightHandTarget)
                {
                    rightHandIK.enabled = true;
                    rightHandIK.solver.arm.target = weaponIK.rightHandTarget;
                    rightHandIK.solver.SetIKPositionWeight(1);
                    rightHandIK.solver.SetRotationWeight(1);
                }
                else
                {
                    rightHandIK.enabled = false;
                    rightHandIK.solver.arm.target = null;
                    rightHandIK.solver.SetIKPositionWeight(0);
                    rightHandIK.solver.SetRotationWeight(0);
                }
                if (weaponIK.leftHandTarget)
                {
                    leftHandIK.enabled = true;
                    leftHandIK.solver.target = weaponIK.leftHandTarget;
                    leftHandIK.solver.SetIKPositionWeight(1);
                    leftHandIK.solver.SetIKRotationWeight(1);
                }
                else
                {
                    leftHandIK.enabled = false;
                    leftHandIK.solver.target = null;
                    leftHandIK.solver.SetIKPositionWeight(0);
                    leftHandIK.solver.SetIKRotationWeight(0);
                }


                aimIK.enabled = weaponIK.isAimIK;
                aimIK.solver.SetIKPositionWeight(0);
                lookAtIK.solver.SetIKPositionWeight(0);
                lookAtIK.solver.SetLookAtWeight(0);
                lookAtIK.enabled = weaponIK.isAimIK;
            }
        };


        RuntimeInventory.ExchangeWeapon(0);
        behaviorTree.EnableBehavior();
    }

    public void Update()
    {
        stateMachine?.Update();
        CalculateControllerState();
        AutoAvoidAgentInPath();

        if (test_targetRegion != null)
        {
            targetRegion = test_targetRegion;
            test_targetRegion = null;
            intoRegionPath = targetRegion.intoRegion[0];
            targetRegion.PreOccupy(this);
            seeker.StartPath(transform.position, intoRegionPath.RegionEntry.position);
            isArriveRegionOrEntry = false;
        }
        else if (targetRegion)
        {
            if (isArriveRegionOrEntry == false && moveAgent.reachedEndOfPath && Vector3.Distance(transform.position, intoRegionPath.RegionEntry.position) <= (moveAgent.endReachedDistance + 0.01f))
            {
                isArriveRegionOrEntry = true;
                seeker.StartPath(transform.position, targetRegion.GetOccupyPosition(this).position);
            }
        }

        




        if (playerController == null || actorHealth.isDeath)
        {
            return;
        }
        //if (Vector3.Distance(transform.position, playerController.transform.position) < dis)
        //{
        //    if (Vector3.Angle(transform.forward, playerController.transform.position - transform.position) < angle && playerController.actorHealth.isDeath == false)
        //    {
        //        target = playerController;
        //    }
        //    else
        //    {
        //        target = null;
        //    }
        //    if (RuntimeInventory.currentWeapon.weaponIK && RuntimeInventory.currentWeapon.weaponIK.isAimIK)
        //    {
        //        if (target != null)
        //        {
        //            targetPoint.position = target.transform.position + Vector3.up * target.movement.centerHeight * 0.5f;
        //            aimIK.solver.SetIKPositionWeight(Mathf.Lerp(aimIK.solver.GetIKPositionWeight(), 1, aimIkLerpSpeed * Time.deltaTime));
        //            if (((RuntimeInventory.currentGunWeapon != null && (RuntimeInventory.currentGunWeapon.HaveAmmo || (RuntimeInventory.currentGunWeapon.HaveAmmo == false && RuntimeInventory.currentGunWeapon.JustEmptyAmmo == true))) || RuntimeInventory.currentGunWeapon == null) && RuntimeInventory.currentWeapon.Use(false))
        //            {
        //                Debug.Log("HitPlayer");
        //            }
        //        }
        //        else
        //        {
        //            aimIK.solver.SetIKPositionWeight(0);

        //        }
        //    }

        //}
        previousPos = transform.position;
    }
    void OnEnable()
    {
        if (moveAgent != null) moveAgent.onTraverseOffMeshLink += TraverseOffMeshLink;
    }

    void OnDisable()
    {
        if (moveAgent != null) moveAgent.onTraverseOffMeshLink -= TraverseOffMeshLink;
    }
    #endregion

    public void FindClosetRoadPointAndToMove()
    {
        if (MapManager.Instance.root != null)
        {
            GoToTargetRoadPoint(MapManager.Instance.FindBestClosetPathPointInMainRoadDir(this, MapManager.Instance.GetTeamHomeInfo(GetEnemyUnitTag()).homeEnd, SearchNearbyMethod.SeekPath, MapManager.Instance.GetTeamHomeInfo(unit).homeEnd.point));
        }
    }
    public void GoToTargetRoadPoint(RoadPointNode roadPointNode)
    {
        targetPointNode = roadPointNode;
        DebugTool.DrawWireSphere(targetPointNode.point.position, 0.1f, Color.red, 10, string.Format("targetPointNode_" + name));
        seeker.StartPath(transform.position, targetPointNode.point.position);
    }
    protected virtual IEnumerator TraverseOffMeshLink(RichSpecial linkInfo)
    {
        if (!(linkInfo.nodeLink is AI_JumpLink))
        {
            yield return null;
        }

        AI_JumpLink link = linkInfo.nodeLink as AI_JumpLink;
        var saveCurrentSpeed = moveAgent.maxSpeed;
        //ai.maxSpeed = link.crossLinkSpeed; 
        float duration = link.crossLinkSpeed > 0 ? Vector3.Distance(linkInfo.second.position, linkInfo.first.position) / link.crossLinkSpeed : 1;
        float startTime = Time.time;
        //TODO
        bool isSuccess = link.successRate == 1;
        if (isSuccess == false)
        {
            float rate = Random.Range(0, 1.0f);
            if (rate <= link.successRate)
            {
                isSuccess = true;
            }
        }
        //

        while (true)
        {
            var pos = Vector3.Lerp(linkInfo.first.position, linkInfo.second.position, Mathf.InverseLerp(startTime, startTime + duration, Time.time));
            var rotation = Quaternion.LookRotation(this.ProjectOnSelfXZPlane((linkInfo.second.position-linkInfo.first.position)),Vector3.up);
            if (moveAgent.updatePosition) moveAgent.transform.position = pos;
            else moveAgent.simulatedPosition = pos;
            if (moveAgent.updateRotation) moveAgent.transform.rotation = rotation;
            else moveAgent.simulatedRotation = rotation;

            if (Time.time >= startTime + duration) break;
            yield return null;
        }
        yield return 0;
    }

    public override Vector3 GetPreScansTargetPoint(float maxDis)
    {
        return targetPoint.position;
    }

    public override bool IsInMove()
    {
        return moveAgent.velocity.magnitude>velo_ThinkIsStoped;
    }

    public override void PlayerAwakeOnRoundGameStart()
    {
        throw new System.NotImplementedException();
    }

    public void RecordThisDamage(DamageInfo damageInfo)
    {
        var attacker = damageInfo.caster;
        List<DamageInfo> damageInfos = null;
        if (damageStatistic.ContainsKey(attacker))
        {
            damageInfos = damageStatistic[attacker];
        }
        if (damageInfos == null)
        {
            damageInfos = new List<DamageInfo>();
            damageStatistic.Add(attacker, damageInfos);
        }
        damageInfos.Add(damageInfo);
        TimeSystem.Instance.AddTimeTask(onceRefreshDamageRecord, () =>
        {
            damageInfos.Remove(damageInfo);
        }, PETime.PETimeUnit.Seconds);
    }

    protected override void OnDamage(DamageInfo damageInfo)
    {
        animatorMachine.animator.CrossFade("Hit", 0);
        int layerIndex = animatorMachine.animator.GetLayerIndex("FullBodyAdditive");
        animatorMachine.animator.SetLayerWeight(layerIndex, 1);
        animatorMachine.animator.SetInteger("HitBodyPartType",(int)damageInfo.damagedBodyPart);
        animatorMachine.animator.SetInteger("HitAngle", GetHitAngle(damageInfo));
        TimeSystem.Instance.AddTimeTask(1, () => { animatorMachine.animator.SetLayerWeight(layerIndex, 0); }, PETime.PETimeUnit.Seconds);
        RecordThisDamage(damageInfo);
        justDamageInfo = damageInfo;
        //通知受击
        behaviorTree.SendEvent("OnDamage");
    }
    protected override void OnDie(DamageInfo damageInfo)
    {
		if (RuntimeGame.Instance.gameMode==GameMode.TeamFight)
		{
			animatorMachine.animator.ResetTrigger("Death");
			animatorMachine.animator.SetTrigger("Death");
			int layerIndex = animatorMachine.animator.GetLayerIndex("FullBodyOverride");
			animatorMachine.animator.SetLayerWeight(layerIndex, 1);
			int upperLayerIndex = animatorMachine.animator.GetLayerIndex("Upper");
			animatorMachine.animator.SetLayerWeight(upperLayerIndex, 0);
			animatorMachine.animator.SetInteger("HitBodyPartType", (int)damageInfo.damagedBodyPart);
			animatorMachine.animator.SetInteger("HitAngle", GetHitAngle(damageInfo));
			ThrowOutWeapon();

			//LeaveRegion(currenStayRegion);
			RuntimeGame.Instance.ActorDie(this);
			team.LeaveWaitEnemyHome(this);
		}
		else if (RuntimeGame.Instance.gameMode==GameMode.Zombie)
		{
			if (unit==UnitCamp.A)
			{
                //被感染
                //直接生成一个新的模型在这里

			}
            else
			{
				//僵尸死亡
				animatorMachine.animator.ResetTrigger("Death");
				animatorMachine.animator.SetTrigger("Death");
				int layerIndex = animatorMachine.animator.GetLayerIndex("FullBodyOverride");
				animatorMachine.animator.SetLayerWeight(layerIndex, 1);
				int upperLayerIndex = animatorMachine.animator.GetLayerIndex("Upper");
				animatorMachine.animator.SetLayerWeight(upperLayerIndex, 0);
				animatorMachine.animator.SetInteger("HitBodyPartType", (int)damageInfo.damagedBodyPart);
				animatorMachine.animator.SetInteger("HitAngle", GetHitAngle(damageInfo));

				//LeaveRegion(currenStayRegion);
				RuntimeGame.Instance.ActorDie(this);
				team.LeaveWaitEnemyHome(this);
			}
		}
        
    }
    private int GetHitAngle(DamageInfo damageInfo)
    {
        int hitAngle = 0;//0-360  front:0-90 270-360   behind 90-270
        //compare forward with castDir;
        Vector3 cross = Vector3.Cross(transform.forward, damageInfo.castDir);
        bool isLeftSide = cross.y < 0;
        hitAngle = (int)Vector3.Angle(transform.forward, damageInfo.castDir);
        if (isLeftSide)
        {
            hitAngle = 360-hitAngle;
        }
        float project = Vector3.Dot(transform.forward, damageInfo.castDir);
        if (project != 0)
        {
            bool isBehind = project > 0 ? false : true;
        }
        return hitAngle;
    }

    public override void ThrowOutWeapon()
    {
        if (RuntimeInventory.currentGunWeapon==null)
        {
            return;
        }
        ThrowOutWeapon(RuntimeInventory.currentGunWeapon);
    }

    public override void ThrowOutWeapon(BaseWeapon weapon)
    {
        if (weapon.weaponType == WeaponType.Primarily || weapon.weaponType == WeaponType.Pistol)
        {
            var gunWeapon = RuntimeInventory.currentGunWeapon;
            var clone = GameObject.Instantiate(gunWeapon.gameObject, null, true);
            GunFeed feed = clone.AddComponent<GunFeed>();
            feed.RegisterFeed(gunWeapon.GetItemDataConfig());
            feed.ammoCount = gunWeapon.AmmoCount;
            RuntimeInventory.RemoveWeaponRuntimeInventory(RuntimeInventory.currentWeapon);
            GetComponent<BodyIKManager>().CloseAllIK(true);

            Rigidbody rigidbody = clone.GetComponent<Rigidbody>();
            rigidbody.transform.position = gunWeapon.transform.position;
            rigidbody.transform.rotation = gunWeapon.transform.rotation;
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


    private void LoseTarget()
    {

    }
    public void IntoNewRegion(AIRegion region)
    {
        currenStayRegion = region;
    }
    public void LeaveRegion(AIRegion region)
    {
        currenStayRegion?.CancelOccpy(this);
        currenStayRegion = null;
    }
    public void GotoBestFallPoint()
    {
        if (currenStayRegion==null)
        {
            return;
        }
        Path path = seeker.StartMultiTargetPath(seeker.transform.position,currenStayRegion.fallPoints.Select((t)=> { return t.position; }).ToArray(),false,null);
    }
    public void GoToOpposite()
    {
        
    }
    public void StopSeeker()
    {
        seeker.CancelCurrentPathRequest();
        moveAgent.enabled = false;
        moveAgent.enabled = true;
        ResetAnimMoveParameter();
        SetMoveMode(ControllerState.Idle);
    }

    public void ResetAnimMoveParameter()
    {
        animatorMachine.animator.SetFloat(Const_Animation.Forward, 0);
        animatorMachine.animator.SetFloat(Const_Animation.Horizontal, 0);
    }

    /// <summary>
    /// Receive Target Footprint
    /// </summary>
    /// <param name="targetPoint"></param>
    public override void FindTarget(Vector3 targetPoint)
    {
        if (GetPathDistance(targetPoint,SearchNearbyMethod.SeekPath)<5)
        {
            //在通信范围内 
            behaviorTree.SetVariableValue("TargetFootprint",targetPoint);
        }
        
    }

    public void ResetDefaultAimIKPose()
    {
        targetPoint.position = defaultAimTargetIKPose;
    }

    public void FindAttacker(Transform attacker)
    {
        behaviorTree.SetVariableValue("ToMeAttacker",attacker);
    }

    public override void SetMoveMode(ControllerState controllerState)
    {
        if (controllerState.IsSelectThisEnumInMult(ControllerState.Idle))
        {
            movement_Speed = 0;
            moveAgent.maxSpeed = movement_Speed;
        }
        else if (controllerState.IsSelectThisEnumInMult(ControllerState.Walking))
        {
            movement_Speed = WalkSpeed;
            moveAgent.maxSpeed = movement_Speed;
        }
        else if (controllerState.IsSelectThisEnumInMult(ControllerState.Running))
        {
            movement_Speed = RunSpeed;
            moveAgent.maxSpeed = movement_Speed;
        }
    }
    public float GetAnimMoveParameterByState(ControllerState controllerState)
    {
        if (controllerState.IsSelectThisEnumInMult(ControllerState.Idle))
        {
            return 0;
        }
        else if (controllerState.IsSelectThisEnumInMult(ControllerState.Walking))
        {
            return 1;
        }
        else if (controllerState.IsSelectThisEnumInMult(ControllerState.Running))
        {
            return 2;
        }
        return 0;
    }
    public override void ReceiveFlashGrenade()
    {
        
    }
    public override void  ReSpawn()
    {
        base.ReSpawn();
        Init();
        actorHealth.DestoryAllEffect();
        behaviorTree.DisableBehavior();
        BehaviorManager.instance.RestartBehavior(behaviorTree);
        behaviorTree.EnableBehavior();

    }
    public void ArriveRoadPointNode(RoadPointNode roadPointNode)
    {
        if (MapManager.Instance.IsArriveEnemyHome(unit,roadPointNode))
        {
            behaviorTree.SendEvent("ArriveEnemyHome");
        }
    }
    public override void AttachWeapon(GameObject go,string attachPoint="")
    {
		if (string.IsNullOrEmpty(attachPoint))
		{
            go.transform.SetParent(transform,false);
            return;
		}
        Transform point = actorSystem.transform.DeepFind(attachPoint);
        go.transform.SetParent(point,false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
    }

    public override bool CheckItem(out Feed feed)
    {
        throw new System.NotImplementedException();
    }

	public override void OnMeleeCheck(WeaponUseInputType useInputType, MeleeCheckWay checkWay, MeleeWeapon meleeWeapon, int comboID = 0)
	{
		StartCoroutine(MeleeCheckCoroutine(useInputType, checkWay, meleeWeapon));
	}
	public IEnumerator MeleeCheckCoroutine(WeaponUseInputType useInputType, MeleeCheckWay checkWay, MeleeWeapon meleeWeapon)
	{
		Vector3 startPosInViewport;
		Vector3 startPosInWorld = Vector3.zero;
		MeleeWeaponDataConfig meleeWeaponDataConfig = meleeWeapon.data;
		List<RoleController> hitter = new List<RoleController>();
		//TODO
		Vector3 startDir = Vector3.zero;
		var cameraForward = meleeAttackCoordinateOriginPoint.forward;
		var CameraUp = meleeAttackCoordinateOriginPoint.up;
		var checkDirGlobal = meleeAttackCoordinateOriginPoint.TransformDirection(checkWay.meleeCheckWayData[0].checkDir.normalized);
		var rotateAxis = Vector3.Cross(cameraForward, checkDirGlobal);
		if (checkWay.StartPosIsByCameraCenterAutoCalculate)
		{
			startDir = Quaternion.Euler(rotateAxis * -(checkWay.meleeCheckWayData[0].checkAngle / 2)) * cameraForward;
		}
		else
		{
			startPosInWorld = meleeAttackCoordinateOriginPoint.transform.position + checkWay.startPosOffsetInCameraCoord;
			startPosInWorld += cameraForward.normalized;
			startDir = (startPosInWorld - meleeAttackCoordinateOriginPoint.transform.position).normalized;
		}
		//


		int hitScansMask = ~(1 << LayerMask.NameToLayer("FullBody") | (1 << 2));//忽略全身碰撞体 因为要检测击中的身体的部位

		if (checkWay.meleeCheckWayData != null)
		{
			foreach (var item in checkWay.meleeCheckWayData)
			{
				float rayCount = item.checkAngle / item.eachcheckIntervalAngle;
				float eachcheckIntervalTime = item.meleeCheckTime / rayCount;
				int _castNum = 0;
				if (item.rayCastTrackType == RayCastTrackType.FromPointToTwoSide)
				{
					if (checkWay.StartPosIsByCameraCenterAutoCalculate)
					{
						startDir = cameraForward;
					}
					else
					{
						startDir = (startPosInWorld - meleeAttackCoordinateOriginPoint.transform.position).normalized;
					}

					while (_castNum < rayCount)
					{
						int index = (_castNum / 2) + 1;
						var castDir = Quaternion.Euler(rotateAxis * item.eachcheckIntervalAngle * index) * startDir;
						var castDir_InverseAxis = Quaternion.Euler(-rotateAxis * item.eachcheckIntervalAngle * index) * startDir;
						var castOrigin = meleeAttackCoordinateOriginPoint.transform.position;
						RaycastHit[] hitinfos = Physics.RaycastAll(castOrigin, castDir, item.checkDis, hitScansMask);
						RaycastHit[] hitinfos_2 = Physics.RaycastAll(castOrigin, castDir_InverseAxis, item.checkDis, hitScansMask);
						hitinfos = hitinfos.Concat(hitinfos_2).ToArray<RaycastHit>();
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
						_castNum += 2;
						yield return new WaitForSeconds(eachcheckIntervalTime);
					}
				}
				else if (item.rayCastTrackType == RayCastTrackType.SingleDir)
				{
					while (_castNum < rayCount)
					{
						var castDir = Quaternion.Euler(rotateAxis * item.eachcheckIntervalAngle * _castNum) * startDir;
						var castOrigin = meleeAttackCoordinateOriginPoint.transform.position;
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

	public AIRegion FindNearbyHighArea(float limitDis, SearchNearbyMethod searchNearbyMethod)
    {
        float min = -1;
        AIRegion region = null;
        foreach (var item in MapManager.Instance.regions)
        {
            if (item.pointNodes == null || item.pointNodes.Length == 0)
            {
                continue;
            }
            float dis = 0;

            var targetPos = item.pointNodes[item.pointNodes.Length == 1 ? 0 : UnityEngine.Random.Range(0, item.pointNodes.Length)].position;
            dis = GetPathDistance(targetPos, searchNearbyMethod);

            if (dis <= limitDis)
            {
                if (dis < min || region == null)
                {
                    min = dis;
                    region = item;
                }
            }
        }
        return region;
    }

    public float GetPathDistance(Vector3 target, SearchNearbyMethod searchNearbyMethod)
    {
        var dis = 0f;
        if (searchNearbyMethod == SearchNearbyMethod.SeekPath || searchNearbyMethod == SearchNearbyMethod.BothThink)
        {
            Path pathA = seeker.GetCurrentPath();
            Path pathB = seeker.StartPath(seeker.transform.position,target);
            pathB.BlockUntilCalculated();
            dis += pathB.GetTotalLength();
            currentPath = pathA;
            if (pathA!=null&&pathA.vectorPath!=null&&pathA.vectorPath.Count>0)
            {
                seeker.StartPath(seeker.transform.position,pathA.vectorPath.Last<Vector3>());
            }
        }
        else if (searchNearbyMethod == SearchNearbyMethod.StraightLine || searchNearbyMethod == SearchNearbyMethod.BothThink)
        {
            dis += Vector3.Distance(seeker.transform.position, target);
        }
        
        return dis;
    }

    public override void AddWeaponInRuntimeInventory(BaseWeapon weapon)
    {
        weapon.RenewWeapon();
        if (weapon is Gun_Weapon)
        {
            Gun_Weapon gun_Weapon = weapon as Gun_Weapon;
            gun_Weapon.OnFireEventHandler += () =>
            {
                var animationParamater = gun_Weapon.AnimationParameterComponent.GetAniamtionParameter(WeaponActionType.Shot.ToString());
                animatorMachine.CrossFade(animationParamater.stateName, 0,0,0,true);
            };
            gun_Weapon.OnRechargeEventHandler += () =>
            {
                var animationParamater = gun_Weapon.AnimationParameterComponent.GetAniamtionParameter(WeaponActionType.Recharge.ToString());
                animatorMachine.CrossFade(animationParamater.stateName, 0,0,0,true);
                leftHandIK.solver.IKPositionWeight = 0;
                leftHandIK.solver.IKRotationWeight = 0;
                leftHandIK.enabled = false;
                rightHandIK.enabled = false;
                weapon.transform.SetParent(rightHandIK.solver.hand.transform, true);
            };
            var gunParent = weapon.transform.parent;
            var gunOffsetPos = weapon.transform.localPosition;
            var gunOffsetEuler = weapon.transform.localEulerAngles;
            gun_Weapon.OnRechargeFinishEventHandler += () =>
            {
                var animationParamater = gun_Weapon.AnimationParameterComponent.GetAniamtionParameter(WeaponActionType.Idle.ToString());
                animatorMachine.CrossFade(animationParamater.stateName, 0,0,0,true);
                weapon.transform.SetParent(gunParent, true);
                weapon.transform.localPosition = gunOffsetPos;
                weapon.transform.localEulerAngles = gunOffsetEuler;
                leftHandIK.solver.IKPositionWeight = 1;
                leftHandIK.solver.IKRotationWeight = 1;
                leftHandIK.enabled = true;
                rightHandIK.enabled = true;
            };
        }
    }

    public void StartPath(Vector3 start, Vector3 end,ControllerState controllerState)
    {
        recastMesh_DynamicObstacles.IsHandled = false;
        SetMoveMode(controllerState);
        seeker.StartPath(start, end);
    }
    public void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            currentPath = path;
            if (path.vectorPath!=null&&path.vectorPath.Count>0)
            {
                currentPathEndPos = path.vectorPath.Last();
            }
            else
            {
                currentPathEndPos = transform.position;
            }
        }
    }
    /// <summary>
    /// 自动避开在路径上的其他代理。
    /// </summary>
    public int CheckAngle_AgentInPathDir=40;
    public float CheckDis_AgentInPathDir=1.5f;
    private float MinHeight_IsInSamePlane=0.25f;
    public float velo_ThinkIsStoped=0.021f;
    public float MinWaitTime_whenVeryNearAnotherAgent=1;
    public float MaxWaitTime_whenVeryNearAnotherAgent=2;
    [HideInInspector]public MyTimer waitTimer_whenVeryNearAnotherAgent;
 
    public void AutoAvoidAgentInPath()
    {
        if (controllerState==ControllerState.Idle||moveAgent.velocity.magnitude<=velo_ThinkIsStoped||currentPath==null||currentPath.vectorPath==null||currentPath.vectorPath.Count==0)
        {
            return;
        }
        List<RoleController> camp = MapManager.Instance.FindActors(unit.ToString());
		if (camp==null)
		{
            return;
		}
        foreach (var elem in camp)
        {
            if (elem==this)
            {
                continue;
            }
            Vector3 elemPos = elem.transform.position;
            Vector3 elemToSelf = elemPos - transform.position;
            if (Mathf.Abs(elemPos.y-transform.position.y)>MinHeight_IsInSamePlane)
            {
                continue;
            }
            if (elemToSelf.magnitude > CheckDis_AgentInPathDir)
            {
                continue;
            }
            float betweenAngle = Vector3.Angle(elemToSelf,transform.forward);
            if (betweenAngle>CheckAngle_AgentInPathDir)
            {
                continue;
            }

            if (waitTimer_whenVeryNearAnotherAgent.timerState == MyTimer.TimerState.Finish)
            {
                waitTimer_whenVeryNearAnotherAgent.timerState = MyTimer.TimerState.Idle;
            }
            if (waitTimer_whenVeryNearAnotherAgent.timerState == MyTimer.TimerState.Idle)
            {
                var _controllerState = controllerState;
                waitTimer_whenVeryNearAnotherAgent.Go(Random.Range(MinWaitTime_whenVeryNearAnotherAgent,MaxWaitTime_whenVeryNearAnotherAgent));
                StopSeeker();
                
                //ERROR：两个角色相对的时候 会一直走这条逻辑。
                waitTimer_whenVeryNearAnotherAgent.OnFinish=(()=> {
                    if ((controllerState&ControllerState.Idle)==0)
                    {
                        //以最新的移动速度为准。
                        _controllerState = controllerState;
                    }
                    if (currentPath == null || currentPath.vectorPath == null || currentPath.vectorPath.Count == 0)
                    {
                        return;
                    }
                    StartPath(transform.position,currentPath.vectorPath.Last(),_controllerState);
                });
                break;
            }
        }
    }

    public bool RegionInEnemyBehind(AIRegion region,RoleController enemy)
	{
        var intoPath = region.intoRegion[0];
        return intoPath.transform.IsInEnemyBack(enemy.transform);
	}

	#region AI

	public void AIUpdate()
	{
        
	}

    

    public void SearchAIRegion()
	{
		targetRegion = MapManager.Instance.GetOneCanStandAiRegion();
		intoRegionPath = targetRegion.intoRegion[0];
		preOccupyPosition = targetRegion.PreOccupy(this);
		StartPath(transform.position, intoRegionPath.RegionEntry.position, controllerState);
		isArriveRegionOrEntry = false;
	}

    public List<RoleController> EnemyInView()
	{
		var enemyCamp = (unit == UnitCamp.A ? UnitCamp.B : UnitCamp.A);
		List<RoleController> enemys = MapManager.Instance.FindActors(enemyCamp.ToString());
		List<RoleController> targets = null;
		if (enemys != null)
		{
			for (int i = 0; i < enemys.Count; i++)
			{
				var enemy = enemys[i];
				if (enemy.actorHealth.isDeath)
				{
					continue;
				}
				var lookEnemyVec = enemy.transform.position - transform.position;
				float angle = Vector3.Angle(lookEnemyVec, transform.forward);
				if (angle <= checkViewAngle && lookEnemyVec.magnitude <= checkViewDis)
				{
					//在检测范围内
					//检测是否可见
					int hitScansMask = ~(/*1 << LayerMask.NameToLayer("FullBody") |*/ (1 << 2));
					Vector3 enemyEyePosition = enemy.transform.position;
					enemyEyePosition.y = enemy.EyePosition.y;
					Vector3 start = EyePosition;
					Vector3 end = enemyEyePosition;
					Vector3 dir = (end - start).normalized;
					//从碰撞体外发送射线，防止检测自身。不过经过研究从碰撞体内部发送射线 不会检测碰撞本身。
					//不过要注意如果发送的射线的位置有碰撞体而且碰撞体会局部位移的的话，要注意了。比如骨骼上的碰撞体。
					//start += dir * (botController.characterRadius+0.01f);
					//DebugTool.DrawLine(start, end, Color.yellow, 5);
					if (Physics.Linecast(start, end, out RaycastHit hitinfo, hitScansMask, QueryTriggerInteraction.Ignore))
					{
						//追击
						ActorComponent actorComponent = hitinfo.collider.GetComponent<ActorComponent>();
						if (actorComponent)
						{
							RoleController roleController = actorComponent.GetActorComponent<RoleController>();
							if (enemy == roleController)
							{
								//追击
								targets.Add(enemy);
								//DebugTool.DrawLine(start, target.position, Color.green, 5);
							}
						}
					}
				}
			}
		}

        //this.find_Target = target;
        ////Owner.SetVariable("target",sharedTarget);
        //if (target != null)
        //{
        //	GetTeamForEditor().FindTarget(this, target.position);
        //}
        return targets;
	}
	#endregion
}
