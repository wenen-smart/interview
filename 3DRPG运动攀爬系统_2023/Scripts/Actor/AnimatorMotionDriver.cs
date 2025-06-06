﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;


public class AnimatorMotionDriver : ActorComponent
{
    private List<RootMotionConfig> rootMotionConfigs;
    private RoleController roleController;

    public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        roleController = GetActorComponent<RoleController>();
    }
    public override void Start()
    {
        base.Start();
        InitRootMotionConfigs();
    }
    public  void InitRootMotionConfigs()
    {
        rootMotionConfigs = RootMotionConfigManager.GetRootMotionConfigsByAnimator(roleController.animatorMachine.animator);
    }
    public void AnimatorMove(Animator animator, Vector3 velocity, Quaternion angular)
    {
        if (rootMotionConfigs==null)
        {
            return;
        }
        RootMotionConfig matchConfig = null;
        if (/*EndSkillTimeLineManager.Instance.EndKillTLPlayed*/false)
        {
            //AnimationClip[] animationClips = EndSkillTimeLineManager.Instance.GetAnimatorClipInTimeLinePlay(animator);
            //if (animationClips!=null&&animationClips.Length>0)
            //{
            //    //暂不考虑融合片段 不算权重，先直接用
            //    string clipName = animationClips[0].name;
            //    for (int i = 0; i < rootMotionConfigs.Count; i++)
            //    {
            //        RootMotionConfig config = rootMotionConfigs[i];
            //        if (config.judgeWay.HasFlag(AnimClipJudgeWay.ClipName))
            //        {
            //            if (clipName.Equals(config.clipName))
            //            {
            //                matchConfig = config;
            //                break;
            //            }
            //        }
            //    }
            //}
        }
        else
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            for (int i = 0; i < rootMotionConfigs.Count; i++)
            {
                RootMotionConfig config = rootMotionConfigs[i];
                if (config.judgeWay.HasFlag(AnimClipJudgeWay.ClipName))
                {
                    if (stateInfo.IsName(config.clipName))
                    {
                        matchConfig = config;
                        break;
                    }
                }
                if (config.judgeWay.HasFlag(AnimClipJudgeWay.Tag))
                {
                    if (stateInfo.IsTag(config.clipTag))
                    {
                        matchConfig = config;
                        break;
                    }
                }
            }
        }
        UpdateRootMotion(matchConfig,velocity,angular);
    }
    private void UpdateRootMotion(RootMotionConfig matchConfig, Vector3 velocity, Quaternion angular)
    {
        if (matchConfig != null)
        {
            if (matchConfig.rootAnyPriority.HasFlag(RootMotionHandlerAim.Position))
            {
                CalcuatePositionMotion(matchConfig, velocity);
            }
            if (matchConfig.rootAnyPriority.HasFlag(RootMotionHandlerAim.Rotation))
            {
                CalcuateRotationMotion(matchConfig, angular);
            }
        }
    }
    void CalcuatePositionMotion(RootMotionConfig matchConfig, Vector3 velocity)
    {
        var localDelta = roleController.transform.InverseTransformDirection(velocity);//DeltaPosition 是世界空间下
        if (matchConfig.posClearPriorityInProcess.HasFlag(RootMotionProcessClear.ClearX))
        {
            localDelta.x=(0);
        }
        if (matchConfig.posClearPriorityInProcess.HasFlag(RootMotionProcessClear.ClearY))
        {
            localDelta.y=(0);
        }
        if (matchConfig.posClearPriorityInProcess.HasFlag(RootMotionProcessClear.ClearZ))
        {
            localDelta.z=(0);
        }
        velocity=roleController.transform.TransformDirection(localDelta);
        switch (matchConfig.posCalcuateModal)
        {
            case CalcuateRootMotionModal.CommonAdd:
                roleController.deltaPosition += velocity * matchConfig.motionMult;
                break;
            case CalcuateRootMotionModal.AddThenAverage:
                roleController.deltaPosition = (roleController.deltaPosition + velocity * matchConfig.motionMult) / 2;
                break;
            case CalcuateRootMotionModal.AverageIncreaseThenAdd:
                roleController.deltaPosition += velocity * matchConfig.motionMult / 2;
                break;
            default:
                break;
        }
    }
    void CalcuateRotationMotion(RootMotionConfig matchConfig, Quaternion angular)
    {
        switch (matchConfig.rotationCalcuateModal)
        {
            case CalcuateRootMotionModal.CommonAdd:
                roleController.transform.rotation *= angular;
                break;
        }
    }
    
}


public enum AnimClipJudgeWay
{
    ClipName = 1,
    Tag = 1 << 1,
    TagAndName = ClipName | Tag,
}
public enum RootMotionHandlerAim
{
    Rotation = 1,
    Position = 1 << 1,
    All = Rotation | Position,
}
public enum CalcuateRootMotionModal
{
    CommonAdd,//delta=delta+increase*mult  rotate*=increase
    AddThenAverage,//delta=(delta+increase*mult)/2
    AverageIncreaseThenAdd,//delta=delta+(increase*mult)/2
}
public enum RootMotionProcessClear
{
    None = 0,
    ClearX = 1,
    ClearY = 1 << 1,
    ClearZ = 1 << 2,
    ClearW = 1 << 3,
    ClearXAndY = ClearX | ClearY,
    ClearXAndZ = ClearX | ClearZ,
    ClearXAndW = ClearX | ClearW,
    ClearYAndZ = ClearY | ClearZ,
    ClearYAndW = ClearY | ClearW,
    ClearZAndW = ClearZ | ClearW,
    ClearAll = ClearX | ClearY | ClearZ | ClearW,
}