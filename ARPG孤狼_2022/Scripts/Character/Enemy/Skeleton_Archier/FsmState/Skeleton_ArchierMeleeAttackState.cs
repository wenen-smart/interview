using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Skeleton_ArchierMeleeAttackState : CommonAttackState<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller>
{
    public Skeleton_ArchierMeleeAttackState(SkeletonState stateType, FsmStateMachine<SkeletonState, SkeletonTransitionType, Skeleton_Archier_Controller> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Excute(Skeleton_Archier_Controller go, params object[] args)
    {
        go.FixedCharacterLookToTarget(go.lockTarget.transform);
        everHit = false;
    }
    MyTimer meleeHorizontalTimer = new MyTimer();
    MyTimer singleDirTimer = new MyTimer();
    MyTimer findAroundWaitTime = new MyTimer();
    public float targetHorizontal = 0;
    public bool everHit;
    public override void OnUpdate(Skeleton_Archier_Controller go, params object[] args)
    {
        if (go.stateManager.isAttack||go.stateManager.isHit)
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
            if (go.IsInActionRange(go.lockTarget.POSITION)==false)
            {
                targetHorizontal = 0;
                go.LerpAnimationFloatVlaue(AnimatorParameter.Horizontal.ToString(),targetHorizontal, 0, true);
                return;
            }
            if (go.TargetIsInAttackRange())
            {
                bool horCondition = true;
                //在近战攻击范围
                if (go.TargetIsInRange(go.attackDistance-1))
                {
                    targetHorizontal = 0;
                    go.FixedCharacterLookToTarget(go.lockTarget.transform);
                    if (go.TargetDirCanWalk(-go.transform.forward))
                    {
                        go.LerpForwardBlend(-1);
                        horCondition = false;
                    }

                }
                if (horCondition)
                {
                    if (meleeHorizontalTimer.timerState == MyTimer.TimerState.Finish)
                    {
                        //左右移动冷却时间完成
                        if (meleeHorizontalTimer.isFinishIntervalState == MyTimer.TimerState.Idle)
                        {

                        }
                        else if (meleeHorizontalTimer.isFinishIntervalState == MyTimer.TimerState.Run)
                        {
                            //开始左右移动
                            //bool isSame=true;
                            //while (isSame==true)
                            //{
                            //    int ranIndex = UnityEngine.Random.Range(0, 3);
                            //    int targetHorizontalTemp = 0;
                            //    if (ranIndex == 0)
                            //    {
                            //        targetHorizontalTemp = -1;
                            //    }
                            //    else if (ranIndex == 2)
                            //    {
                            //        targetHorizontalTemp = 1;
                            //    }
                            //    if (targetHorizontal!=targetHorizontalTemp)
                            //    {
                            //        targetHorizontal = targetHorizontalTemp;
                            //        isSame = false;
                            //    }
                            //}
                            //开始左右移动
                            if (singleDirTimer.timerState != MyTimer.TimerState.Run)
                            {
                                //单方向移动的最短时间
                                int ranIndex = UnityEngine.Random.Range(0, 3);
                                if (ranIndex == 0)
                                {
                                    targetHorizontal = -1;
                                }
                                else if (ranIndex == 2)
                                {
                                    targetHorizontal = 1;
                                }
                                singleDirTimer.Go(UnityEngine.Random.Range(0.5f, 1f));
                            }
                            if ((targetHorizontal != 0 && go.TargetDirCanWalk(targetHorizontal * go.transform.right)) == false)
                            {
                                targetHorizontal = 0;
                            }
                        }
                        else
                        {
                            //本次移动完成，继续开始移动冷却
                            meleeHorizontalTimer.timerState = MyTimer.TimerState.Idle;
                        }
                    }
                    else
                    {
                        if (meleeHorizontalTimer.timerState == MyTimer.TimerState.Idle)
                        {
                            //继续开始移动冷却，进行攻击与闪避
                            targetHorizontal = 0;
                            meleeHorizontalTimer.Go(UnityEngine.Random.Range(2, 4), finsihInterval: 3);


                        }
                        else
                        {
                            //攻击
                            Attack();
                        }
                    }
                    if (targetHorizontal == 0)
                    {
                        go.FixedCharacterLookToTarget(go.lockTarget.transform);
                    }
                    go.LerpForwardBlend(0);
                }
            }
            else
            {
                if (go.TargetIsInRange((go.remoteAttackDistance + go.attackDistance) / 2))
                {
                    //在远程范围与进距离攻击范围 中间范围
                    if (go.TargetDirCanWalk(go.transform.forward))
                    {
                        //安全检测
                        go.LerpForwardBlend(1f);
                    }
                    else
                    {
                        go.LerpForwardBlend(0);
                    }

                }
            }
        }
        else
        {
            targetHorizontal = 0;
            if (findAroundWaitTime.timerState == MyTimer.TimerState.Idle)
            {
                findAroundWaitTime.Go(2);
            }
            if (findAroundWaitTime.timerState == MyTimer.TimerState.Finish)
            {
                RoleController opposeRole = go.CheckRangeIsHaveOppose(entity.attackDistance + 1f);
                if (opposeRole)
                {
                    go.UpdateTarget(opposeRole);
                    go.FixedCharacterLookToTarget(opposeRole.transform);
                    findAroundWaitTime.timerState = MyTimer.TimerState.Idle;
                }
                else
                {
                    go.LoseTarget();//失去目标
                }
            }
            go.LerpForwardBlend(0);
        }
        go.LerpAnimationFloatVlaue(AnimatorParameter.Horizontal.ToString(), targetHorizontal, Time.deltaTime * 2);
    }
    public void Attack()
    {
        int count = entity.skillManager.skillEntity.Count;
        bool isFan = UnityEngine.Random.Range(0,2)>0?false:true;
        for (int i = 0; i < count; i++)
        {
            int index = i;
            if (isFan)
            {
                index = count-1 - i;
            }
            if (entity.SkillCDFinish(entity.skillManager.skillEntity[index].skillID))
            {
                 SkillSystem.Instance.AttackSkillHandler(entity, entity.skillManager.skillEntity[index].skillID);
                break;
            }
           
        }
    }
    public override void Exit(Skeleton_Archier_Controller go, params object[] args)
    {
        base.Exit(go, args);
        go.StopMove();
    }
}

