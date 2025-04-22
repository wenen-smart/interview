using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrollAttackState : CommonAttackState<TrollState, TrollTransitionType, TrollController>
{
    public TrollAttackState(TrollState stateType, FsmStateMachine<TrollState, TrollTransitionType, TrollController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }
    public int comboID = 0;
    public MyTimer waitSeeAroundTimer=new MyTimer();
    public bool lastHit = false;
    public Vector3 ranDir;
    public override void Enter(TrollController go, params object[] args)
    {
        base.Enter(go, args);
        go.UpdateAttackDistance(go.skillManager.PrepareSkill(comboID).skillData.attackDistance);
    }
    public override void Exit(TrollController go, params object[] args)
    {
        base.Exit(go, args);
        lastTargetPos = Vector3.zero;
    }
    public void StuntAttackDeploy()
    {
        SkillSystem.Instance.AttackSkillHandler(entity,2);
    }
    public Vector3 lastTargetPos;
    public override void OnUpdate(TrollController go, params object[] args)
    {
        if (go.stateManager.isHit)
        {
            lastHit = true;
            return;
        }
        bool isSee = go.IsSeeTarget();
        bool isFollowRange = go.TargetIsInFollowRange();
        bool isAttackRange = go.TargetIsInAttackRange();
        bool cdFinish = go.SkillCDFinish(comboID);
        if (isSee)
        {
            if (isAttackRange)
            {
                lastTargetPos = go.lockTarget.transform.position;
            }
            if (go.stateManager.CheckAnimationStateByName("Taunt"))
            {
                if (go.GetDistanceForTarget() <= go.GetSkillDistance(3)&&go.facade.characterAnim.stateInfo.normalizedTime>0.4f)
                {
                    SkillSystem.Instance.AttackSkillHandler(go, 3);
                }
            }
            else
            //攻击范围内
            if (cdFinish)//冷却结束
            {
                if (isAttackRange)
                {
                    Combo();
                    entity.LerpForwardBlend(0);
                    go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 1, Time.deltaTime*10, false);
                }
                else
                {
                    entity.LerpForwardBlend(1);
                    go.FixedCharacterLookToTarget(go.lockTarget.transform);
                    go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 0, Time.deltaTime*10, false);
                }
            }
            else
            {
                //在冷却时间内
                
                if (go.stateManager.CheckState(ActorStateFlag.isAttack.ToString())==false)
                {
                    //在下一次攻击的攻击范围内
                    if ((go.GetDistanceForTarget() < entity.attackDistance + 0.5f))
                    {
                        if (go.GetDistanceForTarget() > entity.attackDistance + 0.2f)
                        {
                            //在范围外
                            entity.LerpForwardBlend(0);
                            go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 1, Time.deltaTime*10, false);
                            if (go.SkillCDFinish(2))
                            {
                                StuntAttackDeploy();
                            }
                            else
                            {
                                if (go.stateManager.CheckAnimationStateByName("Taunt") == false)
                                {
                                    //Taunt 示威
                                    if (entity.skillManager.PrepareSkill(3).GetSkillCDState())
                                    {
                                        go.facade.characterAnim.SetInt(AnimatorParameter.Action.ToString(), 1);
                                    }
                                }
                                else
                                {
                                    SkillSystem.Instance.AttackSkillHandler(go,3);
                                }
                            }
                        }
                        else
                        {
                            //低于范围内
                            //后退   
                            entity.LerpForwardBlend(-1);
                            go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 0, Time.deltaTime * 10, false);
                        }
                    }
                    else
                    {
                        //一开始就在下一次攻击的攻击范围外
                        if (go.GetDistanceForTarget() > entity.attackDistance)
                        {
                            entity.LerpForwardBlend(1);
                            go.FixedCharacterLookToTarget(go.lockTarget.transform);
                            go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 0, Time.deltaTime*10, false);
                        }
                    }
                }
            }
            //if (isAttackRange)
            //{
                
            //}
            //else if (isFollowRange)
            //{
            //    //超出攻击范围
            //    go.FixedCharacterLookToTarget(go.lockTarget.transform);
            //     entity.LerpForwardBlend(1);
            //}
        }
        else
        {
            if (lastHit)
            {
                lastHit = false;
                go.FixedCharacterLookToTarget(go.transform.position + (-go.damageable.lastDamagedDir));
                return;
            }
            if (go.stateManager.CheckState(ActorStateFlag.isAttack.ToString()) == false)
            {
                if (lastTargetPos.sqrMagnitude > 0.1f)
                {
                    if (Vector3.Distance(go.transform.position, lastTargetPos) < 2f)
                    {
                        //后退
                        entity.LerpForwardBlend(-1);
                        MyDebug.DebugWireSphere(new Vector3[] { lastTargetPos }, 0.2f, Color.red, 1f);
                        go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 0, Time.deltaTime * 10, false);
                    }
                    else
                    {
                        entity.LerpForwardBlend(0);
                        go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 1, Time.deltaTime * 10, false);
                        //四周观察
                    }
                }
                else
                {
                    entity.LerpForwardBlend(0);
                    go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 1, Time.deltaTime * 10, false);
                }
            }
            else
            {
                entity.LerpForwardBlend(0);
                go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(), 1, Time.deltaTime * 10, false);
            }

        }
    }
    public void Combo()
    {
        SkillSystem.Instance.AttackSkillHandler(entity,comboID);
        comboID += 1;        
        comboID %= 2;
        entity.UpdateAttackDistance(entity.skillManager.PrepareSkill(comboID).skillData.attackDistance);
    }
    public override void Excute(TrollController go, params object[] args)
    {
       comboID = 0;
    }
}

