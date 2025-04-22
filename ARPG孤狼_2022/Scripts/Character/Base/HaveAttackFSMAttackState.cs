using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
public abstract class HaveAttackFSMAttackState<StateT, TransitionT, EntityT, AttackStateT, AttackTransitionT> : IState<StateT, TransitionT, EntityT> where StateT : Enum where TransitionT : Enum where EntityT : RoleController where AttackStateT : Enum where AttackTransitionT : Enum
{
    public HaveAttackFSMAttackState(StateT stateType, FsmStateMachine<StateT, TransitionT, EntityT> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }

    public override void Enter(EntityT go, params object[] args)
    {
        base.Enter(go);
        if (attackFsmMachine == null)
        {
            attackFsmMachine = GetAttackFsmMachine();
            if (attackFsmMachine != null)
            {
                attackFsmMachine.parentStateMacine = fsmMachine;
            }
        }
        int nextState = (int)args[0];
        //根据Trigger 直接转到另一个状态
        if (nextState != -1)
        {
            attackFsmMachine?.TranstionTo(nextState, args);
        }
        attackFsmMachine?.currentState?.Enter(go);
        ResetDefaultData();
        go.BattleAction?.Invoke();
        Debug.Log("AttackEnter");

    }
    public abstract FsmStateMachine<AttackStateT, AttackTransitionT, EntityT> GetAttackFsmMachine();

    public override void Excute(EntityT go, params object[] args)
    {

    }

    public override void Exit(EntityT go, params object[] args)
    {
        base.Exit(go, args);
        attackFsmMachine?.SetNoneState();
        QueueTrigger = null;
        Debug.Log("Attack Exit");
    }

    public override void OnUpdate(EntityT go, params object[] args)
    {
        go.AttackUpdate();
        attackFsmMachine?.Tick();
    }
    public List<IStateTrigger<EntityT>> iStateAllTrigger = new List<IStateTrigger<EntityT>>();
    //public List<IStateTrigger<EntityT>> queueTrigger = new List<IStateTrigger<EntityT>>();

    IStateTrigger<EntityT> ignoreLimitTrigger = null;

    public FsmStateMachine<AttackStateT, AttackTransitionT, EntityT> attackFsmMachine;

    public override void ReasonTrigger(EntityT go)
    {
        if (attackFsmMachine.currentState != null)
        {
            foreach (var trigger in attackFsmMachine.currentState.stateTriggerList.Values)
            {
                if (trigger.isSatisfyAreTrigger == false)
                {
                    if (trigger.HandleTrigger())
                    {


                        if (trigger.ignoreLimit)
                        {
                            if (SetQueueTrigger(trigger))
                            {
                                isBaseFsm = false;
                            }

                        }
                        else
                        {
                            if ((go.stateManager.canSaveInputInMotion || trigger.SpecialExtral_ilCondition()) && trigger.SpecialExtral_AndCondition())
                            {
                                if (SetQueueTrigger(trigger))
                                {
                                    isBaseFsm = false;
                                }

                            }
                        }
                    }
                }
                else
                {
                    AreaTriggerInfo areaTriggerInfo = trigger.areaTriggerInfo;
                    if (currentFrame >= areaTriggerInfo.startFrame && currentFrame <= areaTriggerInfo.endFrame)
                    {
                        if (trigger.HandleTrigger())
                        {
                            if (SetQueueTrigger(trigger))
                            {
                                isBaseFsm = false;
                            }
                        }
                    }
                }
            }
        }
        if (stateTriggerList != null)
        {
            foreach (var trigger in stateTriggerList)
            {
                if (trigger.Value.isSatisfyAreTrigger == false)
                {
                    if (trigger.Value.HandleTrigger())
                    {


                        if (trigger.Value.ignoreLimit)
                        {
                            if (SetQueueTrigger(trigger.Value))
                            {
                                Debug.Log("AttackFinish trigger");
                                isBaseFsm = true;
                            }
                        }
                        else if ((go.stateManager.canSaveInputInMotion || trigger.Value.SpecialExtral_ilCondition()) && trigger.Value.SpecialExtral_AndCondition())
                        {
                            if (SetQueueTrigger(trigger.Value))
                            {
                                isBaseFsm = true;
                            }
                        }
                    }
                }
                else
                {
                    AreaTriggerInfo areaTriggerInfo = trigger.Value.areaTriggerInfo;
                    if (currentFrame >= areaTriggerInfo.startFrame && currentFrame <= areaTriggerInfo.endFrame)
                    {
                        if (trigger.Value.HandleTrigger())
                        {
                            if (SetQueueTrigger(trigger.Value))
                            {
                                isBaseFsm = true;
                            }
                        }
                    }
                }
            }
        }
    }
    public override void UpdateState(EntityT go)
    {
        if (QueueTrigger != null)
        {
            if (isBaseFsm)
            {
                if (QueueTrigger.ignoreLimit || QueueTrigger.isSatisfyAreTrigger)
                {
                    fsmMachine.TranstionTo(GetOutputTransitionType(QueueTrigger), new object[] { QueueTrigger.nextStateChild, QueueTrigger.triggerArg });
                    ResetDefaultData();
                }
                else if (go.stateManager.shakeAfter||(go.stateManager.shakeAfter==false&&go.stateManager.canSaveInputInMotion==false&&go.stateManager.clipthatPreInputEnable))
                {
                    fsmMachine.TranstionTo(GetOutputTransitionType(QueueTrigger), new object[] { QueueTrigger.nextStateChild, QueueTrigger.triggerArg });
                    ResetDefaultData();
                    go.stateManager.clipthatPreInputEnable = null;
                }
            }
            else
            {
                if (QueueTrigger.ignoreLimit)
                {
                    attackFsmMachine?.TranstionTo((attackFsmMachine.currentState.GetOutputTransitionType(QueueTrigger)), new object[] { QueueTrigger.nextStateChild, QueueTrigger.triggerArg });
                    ResetDefaultData();
                }
                else if (go.stateManager.shakeAfter||(go.stateManager.shakeAfter==false&&go.stateManager.canSaveInputInMotion==false&&go.stateManager.clipthatPreInputEnable))
                {
                    attackFsmMachine?.TranstionTo((attackFsmMachine.currentState.GetOutputTransitionType(QueueTrigger)), new object[] { QueueTrigger.nextStateChild, QueueTrigger.triggerArg });
                    ResetDefaultData();
                    go.stateManager.clipthatPreInputEnable = null;
                }
            }
        }
    }
    public override void Init(string masterName, EntityT entity, FsmTriggerData<StateT, TransitionT>[] triggerDatas)
    {
        base.Init(masterName, entity, triggerDatas);
        //attackFsmMachine = entity.attackFsmMachine;
        //Debug.Log(attackFsmMachine.currentState + "33");
        //foreach (var item in stateTriggerList.Values)
        //{
        //    if (iStateAllTrigger.Contains(item) == false)
        //    {
        //        iStateAllTrigger.Add(item);
        //    }

        //}
        //foreach (var item in attackFsmMachine.stateDic.Values)
        //{
        //    if (item?.stateTriggerList != null)
        //    {
        //        foreach (var attackItem in item.stateTriggerList.Values)
        //        {
        //            if (iStateAllTrigger.Contains(attackItem) == false)
        //            {
        //                iStateAllTrigger.Add(attackItem);
        //            }

        //        }
        //    }
        //}
    }
}

