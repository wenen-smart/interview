using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IStateTrigger<T>
    {

    public T entity;
    [Header("忽略限制")][Tooltip("忽略某些限制，但不忽略Trigger和优先级,具体忽略逻辑根据StateReason来看")]
    public bool ignoreLimit = false;
    public int priority;
    public object machine;
    public bool isSatisfyAreTrigger = false;//指是否是指定区间才能触发Trigger
    public TransitionFSMType immNextStateFSmType;//直接跳转的状态类型是
    public AreaTriggerInfo areaTriggerInfo;
    public TriggerArg triggerArg;

    public void SetAreTriggerInfo(AreaTriggerInfo areaTrigger)
    {
        if (areaTrigger.isEnable==false||(areaTrigger.startFrame==0&&areaTrigger.endFrame==0))
        {
            isSatisfyAreTrigger = false;
        }
        else
        {
            isSatisfyAreTrigger = true;
        }
        areaTriggerInfo = areaTrigger;
    }

    protected IStateTrigger(T entity, object machine)
    {
        this.entity = entity;
        this.machine = machine;
    }


    public void TransitionTo<StateTypeT,TransitionTypeT,EntityT>(TransitionTypeT transitionTypeT) where StateTypeT : Enum where TransitionTypeT : Enum where EntityT:class
    {
        FsmStateMachine<StateTypeT, TransitionTypeT, EntityT> fsmStateMachine = (FsmStateMachine<StateTypeT, TransitionTypeT, EntityT>)machine;
        fsmStateMachine.TranstionTo(transitionTypeT);
    }
    public abstract void Init();

    public int intervalEnterFrameCount = 0;
    public int intervalEnterFrameCounter = 0;

    public int delayTransition = 2;
    public int delayTransitionCounter = 0;
    public int nextStateChild = -1;//-1为空
    public virtual void Enter()
    {
        intervalEnterFrameCounter = 0;
    }

    public virtual bool HandleTrigger()
    {
        if (intervalEnterFrameCount<= intervalEnterFrameCounter)
        {
            return true;
        }
        intervalEnterFrameCounter++;
        return false;
    }
    /// <summary>
    /// 特殊的额外的il限制条件。只要满足这个条件与其他未知条件成或与状态且HandlerTrigger为True 时候就 TransitionTo。 配合Reason情况使用
    /// ***在攻击状态机下是跟saveInputMotion做IL关系的
    /// </summary>
    public virtual bool SpecialExtral_ilCondition()
    {
        return false;
    }
    /// <summary>
    /// 特殊的额外的AND条件。满足这个条件且与其他未知条件都满足True且HandlerTrigger为True 时候就 TransitionTo。 配合Reason情况使用
    /// </summary>
    public virtual bool SpecialExtral_AndCondition()
    {
        return true;
    }
}
[Serializable]
public class AreaTriggerInfo
{
    [Header("开始帧")][Tooltip("自切换到本状态后开始记录帧")]
    public int startFrame = 0;
    [Header("结束帧")]
    public int endFrame = 0;
    public bool isEnable = false;

}
[Serializable]
public struct TriggerArg
{
    public int argInt;
}
public enum TransitionFSMType
{
    SelfFSM,
    ChildFSM,
    ParentFSM
}
