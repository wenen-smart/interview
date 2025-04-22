using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Skeleton_ArchierRemoteAttackState : IState<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>
{
    public Skeleton_ArchierRemoteAttackState(SkeletonState stateType, FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Excute(Skeleton_Archier_Controller go, params object[] args)
    {
        
    }
    MyTimer inRemoteAttackHorizontalMoveTime=new MyTimer();
    MyTimer shootTimer = new MyTimer();
    float targetHorizontalValue = 0;
    MyTimer findAroundWaitTime = new MyTimer();
    public bool everHit;
    public override void OnUpdate(Skeleton_Archier_Controller go, params object[] args)
    {
        if ((go.stateManager.isAttack&&(go.stateManager.CheckState(ActorStateFlag.isDrawBow.ToString())==false)) || go.stateManager.isHit)
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
        if (go.IsSeeTarget(false))
        {
            //可视范围
            if (go.TargetIsInFollowRange())
            {
                 go.FixedCharacterLookToTarget(go.lockTarget.transform);
                //在跟随范围内
                if (go.TargetIsInRemoteAttackRange() == true)
                {
                    //在远程范围内
                    if (shootTimer.timerState==MyTimer.TimerState.Finish)
                    {
                        //射箭
                        if (shootTimer.isFinishIntervalState==MyTimer.TimerState.Idle)
                        {
                            go.FixedCharacterLookToTarget(go.lockTarget.transform);
                            go.facade.characterAnim.SetBool(AnimatorParameter.XuLi.ToString(), true);
                        }
                        else if (shootTimer.isFinishIntervalState==MyTimer.TimerState.Finish)
                        {
                            go.facade.characterAnim.SetBool(AnimatorParameter.XuLi.ToString(), false);
                            shootTimer.timerState = MyTimer.TimerState.Idle;
                        }
                        if (go.stateManager.CheckState(ActorStateFlag.isDrawBow.ToString()))
                        {
                            go.LerpForwardBlend(0);
                            targetHorizontalValue = 0;
                            go.LerpAnimationFloatVlaue(AnimatorParameter.Horizontal.ToString(), 0, Time.deltaTime * 2);
                            return;
                        }
                    }
                    else
                    {
                        if (shootTimer.timerState==MyTimer.TimerState.Idle)
                        {
                            shootTimer.Go(5,3f);
                        }
                        if (go.TargetIsInRange((go.remoteAttackDistance + go.attackDistance) / 2))
                        {
                            //在远程范围与进距离攻击范围 中间范围
                            if (go.TargetDirCanWalk(go.transform.forward))
                            {
                                //安全检测
                                go.LerpForwardBlend(-0.5f);
                            }
                            else
                            {
                                go.LerpForwardBlend(0);
                            }

                        }
                        else
                        {
                            go.LerpForwardBlend(0);
                            if (inRemoteAttackHorizontalMoveTime.timerState != MyTimer.TimerState.Run)
                            {
                                bool isHorizontal = false;
                                if (inRemoteAttackHorizontalMoveTime.timerState == MyTimer.TimerState.Finish)
                                {
                                    if (inRemoteAttackHorizontalMoveTime.isFinishIntervalState == MyTimer.TimerState.Finish)
                                    {
                                        isHorizontal = true;
                                    }
                                }
                                else
                                {
                                    isHorizontal = true;
                                }
                                if (isHorizontal)
                                {
                                    if (UnityEngine.Random.Range(0, 100) >= 95)
                                    {
                                        if (UnityEngine.Random.Range(0, 10) >= 4)
                                        {
                                            targetHorizontalValue = 0.5f;
                                        }
                                        else
                                        {
                                            targetHorizontalValue = -0.5f;
                                        }
                                        inRemoteAttackHorizontalMoveTime.Go(UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(0.4f, 1));
                                    }
                                    else
                                    {
                                        targetHorizontalValue = 0;
                                    }
                                }
                                else
                                {
                                    targetHorizontalValue = 0;
                                }
                            }
                            if (targetHorizontalValue!=0)
                            {
                                if (go.TargetDirCanWalk(go.transform.right * targetHorizontalValue)==false)
                                {
                                    //左右两侧不能到达
                                    targetHorizontalValue = 0;
                                }
                            }
                            
                        }
                    }
                }
                else
                {
                    targetHorizontalValue = 0;
                    //在远程范围外
                    if (go.TargetDirCanWalk(go.transform.forward))
                    {
                        //安全检测
                        go.LerpForwardBlend(0.5f);
                    }
                    else
                    {
                        go.LerpForwardBlend(0);
                    }
                    
                }
            }
            else
            {
                 go.LerpForwardBlend(0);
                 go.facade.characterAnim.SetBool(AnimatorParameter.XuLi.ToString(), false);
            }
        }
        else
        {
            targetHorizontalValue = 0;
            if (findAroundWaitTime.timerState == MyTimer.TimerState.Idle)
            {
                findAroundWaitTime.Go(2);
            }
            if (findAroundWaitTime.timerState == MyTimer.TimerState.Finish)
            {
                RoleController opposeRole = go.CheckRangeIsHaveOppose(entity.remoteAttackDistance + 0.1f);
                if (opposeRole)
                {
                    go.lockTarget = opposeRole;
                    go.FixedCharacterLookToTarget(opposeRole.transform);
                    findAroundWaitTime.timerState = MyTimer.TimerState.Idle;
                }
                else
                {
                    go.lockTarget = null;//失去目标
                }
            }
            go.LerpForwardBlend(0);
        }
        go.LerpAnimationFloatVlaue(AnimatorParameter.Horizontal.ToString(), targetHorizontalValue, Time.deltaTime * 10);
    }
    public override void Exit(Skeleton_Archier_Controller go, params object[] args)
    {
        base.Exit(go, args);
        go.facade.characterAnim.SetBool(AnimatorParameter.XuLi.ToString(), false);
        go.SetBlend(0);
        go.StopMove();
        go.targetBlend = 0;
    }
}

