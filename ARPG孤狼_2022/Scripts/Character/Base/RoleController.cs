using Buff.Base;
using Buff.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IDamageable;

public enum ActorMoveMode
{
    Common = 0,
    ClosetWall = 1,
    Shimmy = 2,
    LockTarget = 3,
    ClimbLadder=4,
}

[RequireComponent(typeof(ActorSystem))]
public abstract class RoleController : ActorComponent
{
    [HideInInspector]
    public MoveComponent moveComponent;
    public float baseMoveSpeed = 10;
    [HideInInspector]
    public float targetBlend = 0;
    [Tooltip("用于数值驱动运动，插值Forward参数用")]
    public float forwardBlendSpeed = 7;
    public Vector3 targetPos;
    public float currentLerpToTargetSpeed = 1;
    public bool isStartPosLerp = false;
    [HideInInspector]
    public CapsuleCollider characterCollider;

    public float capsuleHeightWhenJump = 1;
    public float defaultCapsuleHeight = 2;

    public float moveSpeedMult { get { return moveComponent.speedMult; } set { moveComponent.speedMult = value; } }
    public RoleController lockTarget;

    public ActorMoveMode moveMode;
    public Vector3 DeltaPosition { get { return moveComponent.deltaPosition; } set { moveComponent.deltaPosition = value; } }
    public Quaternion deltaRotation;
    public Vector3 IncreaseVelo { get { return moveComponent.increaseVelo; } set { moveComponent.increaseVelo = value; } }
    private SkillManager _SkillManager;
    public SkillManager skillManager {
        get
        {
            if (_SkillManager == null)
            {
                _SkillManager = GetActorComponent<SkillManager>();
            }
            return _SkillManager;
        }
    }
    private Animator _animator;
    public Animator anim
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>(true);
            }
            return _animator;
        }
        private set
        {
            _animator = value;
        }
    }
    [HideInInspector]
    public IDamageable damageable;
    [HideInInspector]
    public GameObject centerTrans;
    [SerializeField]
    private bool _isController;
    public bool isController { get { return _isController; } set { _isController = value; } }
    public bool isCtrlRotate = false;
    private ActorStateManager _StateManager;
    public ActorStateManager stateManager {
        get
        {
            if (_StateManager == null)
            {
                _StateManager = GetActorComponent<ActorStateManager>();
            }
            return _StateManager;
        }
    }
    public Action UnBattleAction;
    public Action BattleAction;
    public float lerpMotionSpeed = 5;
    public float lerpMotionMaxTimeLimit = 20;

    public float loseCtrlMoveSpeed = 0;//record moveSpeed and save in the variable When DisableCtrl 
    private CharacterFacade _facade;
    [HideInInspector]
    public CharacterFacade facade
    {
        get
        {
            if (_facade == null)
            {
                _facade = GetActorComponent<CharacterFacade>();
            }
            return _facade;
        }
        set
        {
            _facade = value;
        }
    }
    public UnitCamp MyCampTag;//我所属的阵营
    [MultSelectTags]
    public UnitCamp opposeCampTag;//敌对阵营
    [HideInInspector]public string[] opposeCampTagStrList;
    public Dictionary<string, AudioClipCue> audioClipCurDics = new Dictionary<string, AudioClipCue>();
    protected RenderStateListener renderStateListener;
    protected float verticalVelocity;
    public float fallMultiply=10;
    public virtual Vector3 POSITION
    {
        get
        {
            if (moveComponent == null)
            {
                moveComponent = GetComponent<MoveComponent>();
            }
            return moveComponent.POSITION;
        }
        set
        {
            if (moveComponent == null)
            {
                moveComponent = GetComponent<MoveComponent>();
            }
            moveComponent.POSITION = value;
        }
    }
    public override void Init()
    {
        base.Init();
        characterCollider = GetComponent<CapsuleCollider>();
        centerTrans = new GameObject(Constants.CenterTrans);
        centerTrans.transform.SetParent(transform);
        centerTrans.transform.position = GetTrulyRigBodyCenterPoint();
        damageable = GetComponentInChildren<IDamageable>();
        anim = GetComponentInChildren<CharacterAnim>().anim;

        moveComponent = GetActorComponent<MoveComponent>();
        damageable.Init();
        moveComponent.Init();
        moveComponent.SetBaseMoveSpeed(baseMoveSpeed);
        moveComponent.SetSpeedMult(0);
        opposeCampTagStrList=BattleManager.allUNITCampTags.Where((_name) => { return (opposeCampTag).IsSelectThisEnumInMult((UnitCamp)Enum.Parse(typeof(UnitCamp), _name)); }).ToArray();
        LoadActorAudioClipCur();
        MapManager.Instance.RegisterCamp(MyCampTag.ToString(),this);
        renderStateListener = GetComponentInChildren<RenderStateListener>();
        if (renderStateListener)
        {
            renderStateListener.intoRenderer += OnRenderVisible;
            renderStateListener.exitRenderer += OnRenderInVisible;
        }
        damageable.dieAction += OnDie;
    }
    public void LoadActorAudioClipCur()
    {
        Transform audioTrans = transform.Find("Audio");
        if (audioTrans!=null)
        {
            AudioClipCue[] audioClipCues = audioTrans.GetComponentsInChildren<AudioClipCue>();
            if (audioClipCues!=null)
            {
                foreach (var item in audioClipCues)
                {
                    if (audioClipCurDics.ContainsKey(item.identity))
                    {
                        MyDebug.DebugPrint($"{item.identity}已经存在了");
                        continue;
                    }
                     audioClipCurDics.Add(item.identity,item);
                }
            }
        }
    }
    public void PlayActorAudio(string identity)
    {
        foreach (var item in audioClipCurDics)
        {
            if (item.Key == identity)
            {
                item.Value.Play();
                return;
            }
        }
    }
    public override void Awake()
    {
        base.Awake();
    }
    public virtual void DisableCtrl(bool DisRotate = true)
    {
        isController = false;
        if (DisRotate)
        {
            isCtrlRotate = false;
        }
        loseCtrlMoveSpeed = moveSpeedMult;
    }
    public virtual void EnableCtrl(bool EnableRotate = true)
    {
        isController = true;
        if (EnableRotate)
        {
            isCtrlRotate = true;
        }
    }
    public virtual void EnableRotate(bool EnableRotate = true)
    {
        isCtrlRotate = EnableRotate;
    }
    public void CleatDeltaRotation()
    {
        deltaRotation = Quaternion.identity;
    }

    public void ClearMoveSpeed()
    {
        moveComponent.ClearMoveSpeed();
    }
    /// <summary>
    /// 对 失去控制时记录的速度 乘机操作并赋值给当前速度。
    /// </summary>
    /// <param name="mult"></param>
    public void SetLastMoveSpeedMultToCurrent(float mult)
    {
        moveComponent.SetSpeedMult(loseCtrlMoveSpeed*mult);
    }
    public virtual void Start()
    {
        
    }
    public virtual void AnimatorMove(Animator anim,Vector3 velocity,Quaternion angular)
    {

    }
    public abstract void CalcuateMove();
    public void ClearIncreaseVelo()
    {
        IncreaseVelo = Vector3.zero;
    }
    public virtual void ClearAnimSpeedParameters()
    {
        anim.SetFloat(AnimatorParameter.Forward.ToString(),0);
    }

    public void LerpTarget()
    {
        if (isStartPosLerp)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * currentLerpToTargetSpeed);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                transform.position = targetPos;
                isStartPosLerp = false;
            }
        }


    }
    public void LerpToTargetOnUpdate(Vector3 target, float moveSpeed, bool immediatly = false)
    {
        targetPos = target;
        if (immediatly)
        {
            transform.position = target;
            isStartPosLerp = false;
        }
        else
        {
           
            isStartPosLerp = true;
            currentLerpToTargetSpeed = moveSpeed;
        }
     
    }
    public void SetCharacterPosition(Vector3 target, Vector3 offset, float moveSpeed, bool lerpMotion=false)
    {

        target = target + offset;
        if (Vector3.Distance(target,transform.position)<0.01f)
        {
            transform.position = target;
            return;
        }
        LerpToTargetOnUpdate(target,moveSpeed,!lerpMotion);
        //if (lerpMotion)
        //{
        //    StartLerpMotionByCor(target, moveSpeed, lerpMotionMaxTimeLimit);
        //}
        //else
        //{
        //    if (lerpMotionCorField != null)
        //    {
        //        StopCoroutine(lerpMotionCorField);
        //    }
        //    transform.position = target;
        //}
    }
    public void StartLerpMotionByCor(Vector3 target,float speed,float maxLimitDuration)
    {
        if (lerpMotionCorField!=null)
        {
            StopCoroutine(lerpMotionCorField);
        }
        lerpMotionCorField = LerpMotionToTarget(transform,target, speed, maxLimitDuration);
        StartCoroutine(lerpMotionCorField);
    }
    protected IEnumerator lerpMotionCorField;
    public IEnumerator LerpMotionToTarget(Transform trans,Vector3 target,float speed,float maxLimitDuration=0)
    {
         float timer = 0;
       
        while (true)
        {
            trans.position = Vector3.Lerp(trans.position,target,Time.deltaTime* speed);
            if (Vector3.Distance(trans.position,target)<0.005f)
            {
                trans.position = target;
                break;
            }
           
           timer += Time.deltaTime;
            if (maxLimitDuration>0)
            {
                if (timer>=maxLimitDuration)
                {
                    break;
                }
            }
            yield return 0;

        }
    }
    public void SetCharacterQuaternion(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }
    public void SetCharacterQuaternion(Vector3 forward)
    {
        transform.rotation = Quaternion.LookRotation(forward);
    }
    public  float GetCharacterRadius()
    {
        return characterCollider.radius;
    }
    public void RenewDefaultCharacterHeight()
    {
        characterCollider.height = defaultCapsuleHeight;
        SetColliderBottomAlignFoot();
    }
    public void SetCharacterHeightWhenJump()
    {
        characterCollider.height = capsuleHeightWhenJump;
        SetColliderBottomAlignFoot();
    }
    public void SetCharacterHeight(float height)
    {
        characterCollider.height = height;
    }
    public void SetColliderCenterOffset(float centerY)
    {
        characterCollider.center=characterCollider.center.SetY(centerY);
    }
    public void SetColliderBottomAlignFoot()
    {
        SetColliderCenterOffset(characterCollider.height/2);
    }
    public void ExChangeMoveMode(ActorMoveMode moveMode)
    {
        this.moveMode = moveMode;
    }
    public void SetCharacterColliderEnable(bool enable)
    {
        characterCollider.enabled = enable;
    }
    public void FixedEuler()
    {
        var euler = transform.localEulerAngles;
        euler.x = 0;
        euler.z = 0;
        transform.localEulerAngles = euler;
    }
    public Vector3 GetTrulyRigBodyCenterPoint()
    {
        return transform.TransformPoint(characterCollider.center);
    }
    public virtual void SetBlend(float tar) { anim.SetFloat(AnimatorParameter.Forward.ToString(), tar);}
    public virtual void UpdateBlend()
    {
        float current = anim.GetFloat(AnimatorParameter.Forward.ToString());
        if (current == targetBlend || targetBlend == 1)
        {
            SetBlend(targetBlend);
            return;
        }
        if (Mathf.Abs(current - targetBlend) < 0.05f)
        {
            current = targetBlend;
            SetBlend(current);
        }
        else
        {
            current = Mathf.MoveTowards(current, targetBlend, Time.deltaTime * forwardBlendSpeed);
            SetBlend(current);
        }
    }
    public virtual void ComboAttackDeployer(int skillID)
    {

    }

    public virtual void OnHit(int hitAction, float angle = 0, int defenseAction = 0)
    {
        anim.SetFloat(AnimatorParameter.HitAngle.ToString(), angle);
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), hitAction);
        anim.SetInteger(AnimatorParameter.DefenseAction.ToString(), defenseAction);
    }
    public virtual Vector3 GetAttackDir()
    {
        return facade.weaponManager?.currentWeapon?.GetAttackDir()??Vector3.zero;
    }
  
    public virtual void ClearAllMotionData(bool disableCtrl)
    {
        ClearIncreaseVelo();
        ClearMoveSpeed();
        CleatDeltaRotation();
        anim.SetFloat(AnimatorParameter.Forward.ToString(),0);
        anim.SetFloat(AnimatorParameter.VeloDir.ToString(), 0);
        if (disableCtrl)
        {
            DisableCtrl();
        }
    }
    /// <summary>
    /// 被攻击上挑
    /// </summary>
    public virtual void UpPickAttacked()
    {
        
    }
    public virtual void AirComboAttacked()
    {

    }
    public virtual void FixedCharacterLookToTarget(Transform target)
    {
        if (target)
        {
            FixedCharacterLookToTarget(target.position);
        }
    }
    public virtual void FixedCharacterLookToTarget(Vector3 targetPos)
    {
       transform.rotation =/* Quaternion.Slerp(transform.rotation,*/ Quaternion.LookRotation(Vector3.ProjectOnPlane(targetPos - transform.position, Vector3.up))/*,Time.deltaTime*15)*/;
    }
    public virtual void CharacterLerpLookToTarget(Transform target,float delta)
    {
        CharacterLerpLookToTarget(target.position,delta);
    }
    public virtual bool CharacterLerpLookToTarget(Vector3 targetPos,float delta)
    {

        Quaternion target = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetPos - transform.position, Vector3.up));
        if (Quaternion.Angle(transform.rotation,target)<0.1f)
        {
            transform.rotation = target;
            return true;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, target, delta); 
        return Quaternion.Angle(transform.rotation,target)<0.1f;
    }
    public virtual MotionModifyBuff TrackDamageableTarget(IDamageable target)
    {
        MotionModifyBuff motionBuff = GetActorComponent<BuffContainer>().DOBuff(5, actorSystem) as MotionModifyBuff;
        RoleController targetRole = target.GetActorComponent<RoleController>();
        Vector3 targetRolePosition = targetRole.moveComponent.POSITION;
        Vector3 targetToSelfDis = moveComponent.POSITION - targetRolePosition;
        Vector3 targetToSelfDir = targetToSelfDis.normalized;
        Vector3 correntDir=targetToSelfDir.SetY(0);
        motionBuff.targetPos =  targetRolePosition + correntDir*(targetRole.GetCharacterRadius()+GetCharacterRadius());
        Debug.DrawLine(transform.position, motionBuff.targetPos, Color.red, 100);
        return motionBuff;
    }
    public virtual void LerpAnimationFloatVlaue(string floatName,float endValue,float delta,bool immediately=false)
    {
        if (Mathf.Abs(anim.GetFloat(floatName)-endValue)<0.001f||immediately)
        {
            anim.SetFloat(floatName,endValue);
            return;
        }
        anim.SetFloat(floatName,Mathf.Lerp(anim.GetFloat(floatName),endValue,delta));
    }
    public virtual void AttackUpdate()
    {

    }
    public float GetDistanceForTarget()
    {
        return GetBetweenDistance(lockTarget.transform);
    }
    public float GetBetweenDistance(Transform other)
    {
        return Vector3.Distance(other.position,transform.position);
    }
    public virtual bool SkillCDFinish(int skillID)
    {
        return skillManager.PrepareSkill(skillID).GetSkillCDState();
    }
    public virtual int GetCdFinishOfComboInGroup(int groupID)
    {
        if (groupID==-1)
        {
             return -1;
        }
        for (int i = 0; i < skillManager.skillEntity.Count; i++)
        {
            SkillEntity skillEntity = skillManager.skillEntity[i];
            if (skillEntity.comboGroup==groupID&&SkillCDFinish(skillEntity.skillID))
            {
                return skillEntity.skillID;
            }
        }
        return -1;
    }
    public virtual IEnumerable<SkillEntity>  GetSkillEntitysByGroup(int groupID,bool filterCdFinish=false)
    {
        if (groupID == -1)
        {
            return null;
        }
        return skillManager.skillEntity.Where((se)=> { return se.comboGroup == groupID&&(filterCdFinish?SkillCDFinish(se.skillID):true); });
    }
    public float GetSkillDistance(int skillID)
    {
        return skillManager.PrepareSkill(skillID).skillData.attackDistance;
    }
    public virtual void  EquipWeaponHandler(IWeapon weapon)
    {

    }
    public virtual void UnEquipWeaponHandler(IWeapon weapon)
    {

    }
    public void LerpForwardBlend(float tar)
    {
        targetBlend = tar;
    }
    public abstract Vector3 GetAimPoint(Vector3 weaponPoint,Quaternion lookForward);
    public bool TargetDirCanWalk(Vector3 direction)
    {
        return moveComponent.TargetPositionIsCanArrive(POSITION + PlayerableConfig.Instance.noarriveAreasafeDistance * direction);
    }
    public virtual void OnDie(DamageData damageData)
    {

    }
    public virtual void EnableRagDoll(DamageData damageData)
    {
        RagDollSetting ragDollSetting = GetComponent<RagDollSetting>();
        if (ragDollSetting)
        {
            anim.enabled = false;
            ragDollSetting.EnableRagDoll();
            ragDollSetting.AddForceOrTorque(RagDollBone.Pelvis, damageData.attackDir.normalized * 200);
        }
    }
    /// <summary>
    /// 当物体进入摄像机渲染范围时会调用一次
    /// </summary>
    public virtual void OnRenderVisible()
    {

    }
    /// <summary>
    /// 当物体退出摄像机渲染范围时会调用一次
    /// </summary>
    public virtual void OnRenderInVisible()
    {

    }

}
