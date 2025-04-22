using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_ArchierIdleState : IState<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>
{
    public Skeleton_ArchierIdleState(SkeletonState stateType, FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }
    public bool everHit;
    public override void Excute(Skeleton_Archier_Controller go, params object[] args)
    {
        entity.lockTarget = null;
    } 

    public override void OnUpdate(Skeleton_Archier_Controller go, params object[] args)
    {
        go.LerpForwardBlend(0);
        if (go.stateManager.isHit)
        {
            everHit = true;
        }
        if (everHit)
        {
            go.TryLockAttackTarget();
            everHit = false;
        }
         go.UpdateTarget(0,false);

    }
}
