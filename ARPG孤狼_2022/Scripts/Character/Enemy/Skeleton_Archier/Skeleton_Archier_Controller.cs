using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_Archier_Controller : EnemyController
{
    public FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller> fsmStateMachine = new FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>();
    public Skeleton_ArchierFSMConfig fsmStateConfig;

    public override void Init()
    {
        base.Init();
        fsmStateMachine = new FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>("Skeleton_Archier",this,fsmStateConfig?.FsmStateTypeData??Skeleton_ArchierFSMConfig.Instance.FsmStateTypeData);
    }
    protected override void Update()
    {
        base.Update();
        if (damageable.IsDie)
        {
            return;
        }
        fsmStateMachine.Tick();
    }
    
}
