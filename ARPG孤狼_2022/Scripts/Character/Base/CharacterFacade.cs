using UnityEngine;
using System.Collections;
using System;
using System.Threading.Tasks;
using System.Threading;
using Buff.Core;
using System.Collections.Generic;

public abstract class CharacterFacade: ActorComponent
{
    [HideInInspector]
    public WeaponManager weaponManager;
    [HideInInspector]
    public ActorStateManager playerStateManager;
    [HideInInspector]
    public ActorPhyiscal actorPhyiscal;
    public Action attackHandler;
    [HideInInspector]
    public RoleController roleController;
    private BuffContainer _buffContainer;
    public BuffContainer buffContainer
    {
        get
        {
            if (_buffContainer == null)
            {
                _buffContainer = GetActorComponent<BuffContainer>();
            }
            return _buffContainer;
        }
    }
    [HideInInspector]
    public SkillManager skillManager;
    [HideInInspector] public CharacterAnim characterAnim;
    public override void Awake()
    {
        base.Awake();
    }
    public override void Init()
    {
        base.Init();
        roleController = GetComponent<RoleController>();

        actorPhyiscal = GetComponent<ActorPhyiscal>();

        skillManager = GetActorComponent<SkillManager>();

        weaponManager = GetActorComponent<WeaponManager>();

        playerStateManager = GetComponent<ActorStateManager>();

        characterAnim = GetActorComponent<CharacterAnim>();
        if (playerStateManager)
        {
            playerStateManager.actorPhyiscal = actorPhyiscal;
            playerStateManager.characterAnim = characterAnim;
        }
        characterAnim.Init();
        playerStateManager.Init();
        roleController.Init();
        playerStateManager.moveComponent = roleController.moveComponent;
        skillManager?.Init();
        weaponManager?.Init();
        SetBodyInfo();
    }
    public override void ActorComponentAwake()
    {
        Init();//自身初始化
    }
    public override void  Start()
    {
        
    }
    public virtual void AttackEnter()
    {
        roleController.ClearIncreaseVelo();
        roleController.ClearMoveSpeed();
        roleController.DisableCtrl();
        //Debug.Log("attackEnter");
        //attackHandler?.Invoke();
    }
    public void AttackExit()
    {
        roleController.EnableCtrl();
        playerStateManager.nextAttackVaildSign = false;
        playerStateManager.SuperBody(false);
        playerStateManager.ShakeAfter(false);
        playerStateManager.ShakeBefore(false);
    }
    public virtual void LateUpdate()
    {
        CheckGround();
    }

    public virtual void CheckGround()
    {
        characterAnim.SetBool(AnimatorParameter.isGround.ToString(), playerStateManager.IsGround||roleController.moveMode==ActorMoveMode.ClimbLadder);
        characterAnim.SetFloat(AnimatorParameter.AirValue.ToString(), playerStateManager.IsGround == true ? 0 : 10);
    }
    public virtual void RenewCollideHeight()
    {
        roleController.RenewDefaultCharacterHeight();
    }
    public virtual void SetColliderHeightWhenJump()
    {

    }
    public virtual void SetColliderHeight(float height)
    {
        roleController.SetCharacterHeight(height);
    }
    public virtual void SetColliderHeightAndAlign(float height)
    {
        SetColliderHeight(height);
        roleController.SetColliderBottomAlignFoot();
    }
    public void SetMirrorParameter(bool mirror)
    {
        roleController.anim.SetBool(AnimatorParameter.Mirror.ToString(), mirror);
    }
    public void SetAnimatorController(RuntimeAnimatorController runtimeAnimatorController)
    {
        List<AnimatorParameterDataPacker> datas = ReplaceAnimatorBeforePackData();
        characterAnim.SetAnimatorController(runtimeAnimatorController);
        ReplaceAnimatorAfterReviseData(datas);
    }
    /// <summary>
    /// 更换状态机后 去修正数据。
    /// </summary>
    public virtual List<AnimatorParameterDataPacker> ReplaceAnimatorBeforePackData() { return null; }
    /// <summary>
    /// 更换状态机后 去修正数据。
    /// </summary>
    public virtual void ReplaceAnimatorAfterReviseData(List<AnimatorParameterDataPacker> packers) { }
    /// <summary>
    /// 获取距离最近的骨头： 默认是获取Unity人形骨架，勾选Humanoid才能使用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public virtual Transform GetClosetBoneTransByHumanoid(Vector3 pos)
    {
        Animator anim = characterAnim.anim;
        Transform targetBone=null;
        float closetDis=0;
        for (int i = 1; i <= 54; i++)
        {
            Transform bone = anim.GetBoneTransform((HumanBodyBones)i);
            if (bone==null)
            {
                continue;
            }
            if (targetBone==null)
            {
                targetBone = bone;
                closetDis = (pos - bone.position).sqrMagnitude;
            }
            else
            {
                var dis = (pos - bone.position).sqrMagnitude;
                if (closetDis>dis)
                {
                    closetDis = dis;
                    targetBone = bone;
                }
            }
        }
        return targetBone;
    }
    /// <summary>
    /// 获取距离最近的骨头：使用与所有有骨骼模型 通用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public virtual Transform GetClosetBoneTransGeneric(Vector3 pos)
    {
        //暂时可能没法实现
        SkinnedMeshRenderer[] skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        
        Transform targetBone = null;
        float closetDis = 0;
        for (int i = 0; i < skinnedMeshRenderer.Length; i++)
        {
            Transform bone = skinnedMeshRenderer[i].rootBone;
            if (bone == null)
            {
                continue;
            }
            if (targetBone == null)
            {
                targetBone = bone;
                closetDis = (pos - bone.position).sqrMagnitude;
            }
            else
            {
                var dis = (pos - bone.position).sqrMagnitude;
                if (closetDis > dis)
                {
                    closetDis = dis;
                    targetBone = bone;
                }
            }
        }
        return targetBone;
    }

    public virtual Transform GetClosetBoneTrans(Vector3 pos)
    {
        Transform boneTrans=null;
        if (characterAnim.anim.isHuman)
        {
            boneTrans=GetClosetBoneTransByHumanoid(pos);
        }
        else
        {
            boneTrans = GetClosetBoneTransGeneric(pos);
        }
        return boneTrans;
    }
    public void SetBodyInfo()
    {
        if (characterAnim.anim.isHuman==false||characterAnim.anim.avatar.isHuman==false)
        {

            return;
        }
        Transform headTrans = characterAnim.GetBodyTransform(HumanBodyBones.Head);
        if (headTrans)
        {
            if (headTrans.childCount>0)
            {
                Transform headTop = headTrans.GetChild(0);
                actorPhyiscal.AddBodyPoint(CheckPointType.HeadTop, headTop);
            }
        }
        
        
        actorPhyiscal.AddBodyPoint(CheckPointType.Head, headTrans);
        //
        Transform chestTrans = characterAnim.GetBodyTransform(HumanBodyBones.Chest);
        actorPhyiscal.AddBodyPoint(CheckPointType.Chest, chestTrans);
        //
        Transform feetTrans = transform;
        actorPhyiscal.AddBodyPoint(CheckPointType.Feet, feetTrans);
        //
        Transform waistTrans = characterAnim.GetBodyTransform(HumanBodyBones.Hips);
        actorPhyiscal.AddBodyPoint(CheckPointType.Waist, waistTrans);
        //

        Transform leftLeg = characterAnim.GetBodyTransform(HumanBodyBones.LeftLowerLeg);
        var legPos = leftLeg.position;
        legPos.x = transform.position.x;
        legPos.z = transform.position.z;
        GameObject legGo = new GameObject("LegPoint");
        Transform legTrans = legGo.transform;
        legTrans.position = legPos;
        legTrans.SetParent(transform, true);

        actorPhyiscal.AddBodyPoint(CheckPointType.Leg, legTrans);
        //
        var offsetMult = 0.3f;
        Transform leftOffestPoint = new GameObject("LeftOffsetPoint").transform;
        leftOffestPoint.SetParent(transform, true);

        leftOffestPoint.position = waistTrans.position - transform.right * offsetMult;
        actorPhyiscal.AddBodyPoint(CheckPointType.LeftOffsetPoint, leftOffestPoint);
        Transform rightOffsetPoint = new GameObject("RightOffsetPoint").transform;

        rightOffsetPoint.SetParent(transform, true);

        rightOffsetPoint.position = waistTrans.position + transform.right * offsetMult;
        actorPhyiscal.AddBodyPoint(CheckPointType.RightOffsetPoint, rightOffsetPoint);

    }
    public virtual void OpenRightArrowPoseRig()
    {
        
    }
    public virtual void CloseRightArrowPoseRig()
    {
        
    }
    public virtual HpBarEntity ApplyHpBarEntity()
    {
        return null;
    }
}
/// <summary>
/// 仅用作值传递
/// </summary>
public struct AnimatorParameterDataPacker
{
    public string parameterName;
    public AnimatorControllerParameterType argType;
    public int intValue;// int
    public float floatValue;//float
    public bool boolValue;//boolean trigger

    public AnimatorParameterDataPacker(string parameterName, AnimatorControllerParameterType argType,Animator anim)
    {
        this.parameterName = parameterName;
        this.argType = argType;
        intValue = 0;
        floatValue = 0;
        boolValue = false;
        switch (argType)
        {
            case AnimatorControllerParameterType.Float:
                floatValue = anim.GetFloat(parameterName);
                break;
            case AnimatorControllerParameterType.Int:
                intValue = anim.GetInteger(parameterName);
                break;
            case AnimatorControllerParameterType.Bool:
                boolValue = anim.GetBool(parameterName);
                break;
            case AnimatorControllerParameterType.Trigger:
                boolValue = anim.GetBool(parameterName);
                break;
            default:
                break;
        }
    }
    
}
