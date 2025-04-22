using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class WolfAttackState : IState<WolfState, WolfTransitionType, WolfController>
{
    public WolfAttackState(WolfState stateType, FsmStateMachine<WolfState, WolfTransitionType, WolfController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }
    private bool nextJumpAttack;
    private MyTimer jumpWaitTimer=new MyTimer();
    private MyTimer findAroundWaitTime = new MyTimer();
    private MyTimer attackWaitTime = new MyTimer();
    public override void Excute(WolfController go, params object[] args)
    {
        
    }

    public override void OnUpdate(WolfController go, params object[] args)
    {
        if (go.stateManager.isAttack||go.stateManager.isHit||go.stateManager.isDie)
        {
            return;
        }
        if (go.IsSeeTarget())
        {
            if (nextJumpAttack==false&&go.GetBetweenDistance(go.lockTarget.transform)>(go.maxFollowDis-2))
            {
                nextJumpAttack = true;
            }
            if (go.TargetIsInAttackRange() == false)
            {
                NavMeshPath navMeshPath = go.moveComponent.CalculatePath(go.lockTarget.transform.position);
                bool continueMove=true;
                if (go.TargetIsInRange(3)&&jumpWaitTimer.timerState!=MyTimer.TimerState.Run)
                {
                    if (nextJumpAttack)
                    {
                        if (navMeshPath!=null)
                        {
                            if (navMeshPath.corners!=null&&navMeshPath.corners.Length<=2)
                            {
                                go.StopMove();
                                go.FixedCharacterLookToTarget(go.lockTarget.transform);
                                SkillSystem.Instance.AttackSkillHandler(go, 4);
                                continueMove = false;
                                nextJumpAttack = false;
                                jumpWaitTimer.Go(3);
                                
                            }
                        }
                        //Jump
                    }
                }
                if (continueMove)
                {
                    Vector3 targetPos = go.lockTarget.POSITION;
                    go.SetNavMeshTarget(targetPos);
                    go.LerpForwardBlend(3f);
                }
            }
            else
            {
                go.StopMove();
                nextJumpAttack = false;
                if (attackWaitTime.timerState!=MyTimer.TimerState.Run)
                {
                    ComboAttack();
                    attackWaitTime.Go(2);

                }
                
                //InSafeDis
            }
        }
        else
        {
            if (findAroundWaitTime.timerState == MyTimer.TimerState.Idle)
            {
                findAroundWaitTime.Go(1);
            }
            if (findAroundWaitTime.timerState == MyTimer.TimerState.Finish)
            {
                RoleController opposeRole = go.CheckRangeIsHaveOppose(entity.maxFollowDis + 1f);
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
    }
    public void ComboAttack()
    {
        IEnumerable<SkillEntity> skillEntities = entity.GetSkillEntitysByGroup(1,true);
        if (skillEntities==null)
        {
            return;
        }
        List<SkillEntity> skillEntityList = skillEntities.ToList();
        int count = skillEntityList.Count;
        if (count>0)
        {
            SkillSystem.Instance.AttackSkillHandler(entity, skillEntityList[UnityEngine.Random.Range(0, count)].skillID);
        }
        
    }
}

