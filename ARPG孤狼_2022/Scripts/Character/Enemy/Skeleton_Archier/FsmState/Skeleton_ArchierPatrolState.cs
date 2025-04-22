using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Skeleton_ArchierPatrolState : IState<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>
{
    public Skeleton_ArchierPatrolState(SkeletonState stateType, FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }
    public override void Enter(Skeleton_Archier_Controller go, params object[] args)
    {
        base.Enter(go, args);
        go.StartPatrol();
        everHit = false;
    }
    public MyTimer patrolWaitTime=new MyTimer();
    public int currentPatrolPointIndex = 0;
    public bool everHit;
    public override void Excute(Skeleton_Archier_Controller go, params object[] args)
    {
        patrolWaitTime.timerState = MyTimer.TimerState.Idle;
        if (go.NavPointGroup == null)
        {
            return;
        }
        currentPatrolPointIndex %= go.NavPointGroup.pointGroup.Count;
        go.SetNavMeshTarget(go.NavPointGroup.pointGroup[currentPatrolPointIndex].position);
        if (go.moveComponent.Agent!=null)
        {
            go.moveComponent.Agent.speed = 1;
        }
    }

    public override void OnUpdate(Skeleton_Archier_Controller go, params object[] args)
    {
        if (go.stateManager.isFall)
        {
            go.StopMove();
            return;
        }
        if (go.stateManager.isAttack || go.stateManager.isHit)
        {
            go.StopMove();
            if (go.stateManager.isHit)
            {
                everHit = true;
            }
            return;
        }
        go.UpdateTarget();
        if (everHit)
        {
            go.TryLockAttackTarget();
            everHit = false;
        }
        if (go.NavPointGroup==null)
        {
            return;
        }
        float remainDis = go.moveComponent.GetNavRemainingDistance();
        
        if (remainDis>0.1f)
        {
            go.LerpForwardBlend(0.5f);
        }
        else
        {
            go.LerpForwardBlend(0);
            if (patrolWaitTime.timerState == MyTimer.TimerState.Idle)
            {
                patrolWaitTime.Go(UnityEngine.Random.Range(2, 4));
            }
        }
        if (patrolWaitTime.timerState==MyTimer.TimerState.Finish)
        {
            patrolWaitTime.timerState = MyTimer.TimerState.Idle;
            currentPatrolPointIndex += 1;
            currentPatrolPointIndex %= go.NavPointGroup.pointGroup.Count;
            go.SetNavMeshTarget(go.NavPointGroup.pointGroup[currentPatrolPointIndex].position);
            go.moveComponent.Agent.speed = 1;
        }
    }
    public override void Exit(Skeleton_Archier_Controller go, params object[] args)
    {
        base.Exit(go, args);
        go.StopPatrol();
    }
}

