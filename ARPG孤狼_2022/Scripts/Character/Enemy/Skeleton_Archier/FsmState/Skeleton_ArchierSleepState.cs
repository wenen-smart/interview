using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Skeleton_ArchierSleepState : IState<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>
{
    public Skeleton_ArchierSleepState(SkeletonState stateType, FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }
    public Vector3 sleepPos = Vector3.zero;
    public Vector3 sleepForward = Vector3.zero;
    public MyTimer gotoSleepState=new MyTimer();
    public MyTimer findTargetWaitTimer=new MyTimer();
    public float sumWakeTimer = 0;
    public override void Enter(Skeleton_Archier_Controller go, params object[] args)
    {
        base.Enter(go, args);
        findTargetWaitTimer.timerState = MyTimer.TimerState.Idle;
        if (sleepPos != Vector3.zero)
        {
            gotoSleepState.Go(3,1.5f);
            
        }
        entity.lockTarget = null;
    }

    public override void Excute(Skeleton_Archier_Controller go, params object[] args)
    {
        if (sleepPos == Vector3.zero)
        {
            sleepPos = go.transform.position;
            sleepForward = go.transform.forward;
            GoToSleep();
        }
    }

    public override void OnUpdate(Skeleton_Archier_Controller go, params object[] args)
    {
        if (go.stateManager.isHit)
        {
            if (findTargetWaitTimer.timerState==MyTimer.TimerState.Idle)
            {
                findTargetWaitTimer.Go(1);
            }
        }
        //去睡觉
        if (gotoSleepState.timerState == MyTimer.TimerState.Finish)
        {
            //Sleep 
            if (gotoSleepState.isFinishIntervalState==MyTimer.TimerState.Idle)
            {
                GoToSleep();
            }
            else if (gotoSleepState.isFinishIntervalState==MyTimer.TimerState.Finish)
            {
                findTargetWaitTimer.timerState = MyTimer.TimerState.Idle;
                gotoSleepState.timerState = MyTimer.TimerState.Idle;
            }
        }
        //睡着了
        if (gotoSleepState.timerState==MyTimer.TimerState.Idle)
        {
            RoleController opposeRole = go.CheckRangeIsHaveOppose(entity.attackDistance+1f);
            if (findTargetWaitTimer.timerState == MyTimer.TimerState.Idle)
            {
                if (opposeRole)
                {
                    sumWakeTimer += Time.deltaTime;
                    if (sumWakeTimer > 2)
                    {
                        sumWakeTimer = 0;
                        findTargetWaitTimer.Go(1); 
                    }
                }
            }
        }
        if (findTargetWaitTimer.timerState == MyTimer.TimerState.Finish)
        {
            go.UpdateTarget(360);
        }
    }
    public override void Exit(Skeleton_Archier_Controller go, params object[] args)
    {
        entity.stateManager.SetState(ActorStateFlag.isSleep.ToString(),false);
        entity.facade.characterAnim.SetInt(AnimatorParameter.State.ToString(),0);
        base.Exit(go, args);
    }
    public void GoToSleep()
    {
        entity.facade.characterAnim.SetInt(AnimatorParameter.State.ToString(),4);
        entity.transform.forward = sleepForward;
        entity.stateManager.SetState(ActorStateFlag.isSleep.ToString(),true);
    }
}

