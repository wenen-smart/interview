using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_ArchierMoveState : IState<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>
{
    public Skeleton_ArchierMoveState(SkeletonState stateType, FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }

    private MyTimer navFindPathWait = new MyTimer();
    private float noForwawrdSumTimer = 0;
    public bool everHit;
    
    public override void Enter(Skeleton_Archier_Controller go, params object[] args)
    {
        base.Enter(go, args);
        everHit = false;
    }
    public override void Excute(Skeleton_Archier_Controller go, params object[] args)
    {
        noForwawrdSumTimer = 0;
        if (go.moveComponent.Agent != null)
        {
            go.moveComponent.Agent.speed = 2;
        }
    }

    public override void OnUpdate(Skeleton_Archier_Controller go, params object[] args)
    {
        if (go.stateManager.isAttack || go.stateManager.isHit)
        {
            go.StopMove();
            if (go.stateManager.isHit)
            {
                everHit = true;
            }
            return;
        }
        if (go.stateManager.isFall)
        {
            go.StopMove();
            return;
        }
        if (everHit)
        {
            go.TryLockAttackTarget();
            everHit = false;
        }
        if (go.IsSeeTarget())
        {

            if (go.moveComponent.Agent != null)
            {
                float remainDis = go.moveComponent.GetNavRemainingDistance();

                if (remainDis > 0.1f)
                {
                    go.LerpForwardBlend(1f);
                }
                else
                {
                    go.LerpForwardBlend(0);
                }
            }


            //可视范围
            if (go.TargetIsInFollowRange())
            {
                //在跟随范围内
                if (go.TargetIsInRemoteAttackRange() == false)
                {
                    //在远程范围外
                    if (go.TargetDirCanWalk(go.transform.forward))
                    {
                        if (go.moveComponent.Agent != null)
                        {
                            if (Vector3.Distance(go.moveComponent.Agent.destination, go.lockTarget.transform.position) >= 0.2f)
                            {
                                go.SetNavMeshTarget(go.lockTarget.transform.position);
                            }
                        }
                        else
                        {

                        }
                        go.LerpForwardBlend(1f);
                        go.FixedCharacterLookToTarget(go.lockTarget.transform);
                    }
                    else
                    {
                        noForwawrdSumTimer += Time.deltaTime;
                        if (noForwawrdSumTimer > 1)
                        {
                            if (navFindPathWait.timerState == MyTimer.TimerState.Idle)
                            {
                                navFindPathWait.Go(1);
                                noForwawrdSumTimer = 0;
                            }
                        }
                        if (navFindPathWait.timerState == MyTimer.TimerState.Finish)
                        {
                            navFindPathWait.timerState = MyTimer.TimerState.Idle;
                            go.SetNavMeshTarget(go.lockTarget.transform.position);
                        }
                    }
                }
            }
        }
    }
    public override void Exit(Skeleton_Archier_Controller go, params object[] args)
    {
        base.Exit(go, args);
        go.StopMove();
    }
}

