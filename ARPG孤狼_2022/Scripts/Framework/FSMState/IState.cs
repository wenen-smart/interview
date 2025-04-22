using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IState<StateTypeT, TransitionT,EntityT>  where StateTypeT : Enum where TransitionT : Enum  where EntityT:class
    {
    

    public StateTypeT stateType;
    public FsmStateMachine<StateTypeT, TransitionT, EntityT> fsmMachine;
    protected EntityT entity;
    public Dictionary<TransitionT,IStateTrigger<EntityT>> stateTriggerList = new Dictionary<TransitionT, IStateTrigger<EntityT>>();
    public int currentFrame;
    IStateTrigger<EntityT> queueTrigger = null;
    public IStateTrigger<EntityT> QueueTrigger
    {
        get => queueTrigger; set
        {

            if (value != null)
            {
#if UNITY_EDITOR
                FieldInfo memberInfo = value.machine.GetType().GetField("currentState");
                if (memberInfo != null)
                {

                    var state = memberInfo.GetValue(value.machine);
                    MethodInfo method = state.GetType().GetMethod("GetOutputTransitionType");
                    string transitionType = method.Invoke(state, new object[] { value }).ToString();
                    Debug.Log("SetQueueTrigger：" + transitionType);
                }
#endif
            }

            queueTrigger = value;
        }
    }
    public bool isBaseFsm;

    public virtual void Enter(EntityT go,params object[] args)
    {
        entity = go;
        currentFrame = 0;
        foreach (var item in stateTriggerList)
        {
           
                if (item.Value!=null)
                {
                    item.Value.Enter();
                }
        }
        ResetDefaultData();
    }
    public virtual void Exit(EntityT go, params object[] args)
    {
        entity = null;
    }

    public abstract void Excute(EntityT go, params object[] args);
    public abstract void OnUpdate(EntityT go, params object[] args);

    public virtual void Reason(EntityT go, params object[] args)
    {
        ReasonTrigger(go);
        UpdateState(go);
        currentFrame++;
    }

    public void ResetDefaultData()
    {
        isBaseFsm = false;
        QueueTrigger = null;
    }
    /// <summary>
    /// 更新触发成功的Trigger 寻找切换原因
    /// </summary>
    /// <param name="go"></param>
    public virtual void ReasonTrigger(EntityT go)
    {
        if (stateTriggerList != null)
        {
            foreach (var trigger in stateTriggerList)
            {
                if (trigger.Value.isSatisfyAreTrigger == false)
                {
                    if (trigger.Value.HandleTrigger())
                    {
                        if (SetQueueTrigger(trigger.Value))
                        {
                            isBaseFsm = true;
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
    /// <summary>
    /// 根据得到的Trigger 切换状态
    /// </summary>
    /// <param name="go"></param>
    public virtual void UpdateState(EntityT go)
    {
        if (QueueTrigger != null)
        {
            if (isBaseFsm)
            {
                fsmMachine.TranstionTo(GetOutputTransitionType(QueueTrigger), new object[] { QueueTrigger.nextStateChild, QueueTrigger.triggerArg });
                ResetDefaultData();
            }
        }
    }
    public bool SetQueueTrigger(IStateTrigger<EntityT> trigger)
    {

        if (QueueTrigger == null)
        {
            QueueTrigger = trigger;
            return true;
        }
        else
        {
            if (QueueTrigger.priority <= trigger.priority)//选取优先级高的 其次是最新的。
            {
                QueueTrigger = trigger;
                return true;
            }
        }
        return false;
    }
    public virtual StateTypeT GetOutputState(TransitionT transitionType)
    {
        if (transitionDic.ContainsKey(transitionType))
        {
            return transitionDic[transitionType];
        }
        return default(StateTypeT);
    }
    public virtual StateTypeT GetOutputState(string transitionType)
    {
        Debug.Log(transitionType);
      var  transitionTypeEnum = (TransitionT)Enum.Parse(typeof(TransitionT), transitionType);
        if (transitionDic.ContainsKey(transitionTypeEnum))
        {
            return transitionDic[transitionTypeEnum];
        }
        return default(StateTypeT);
    }

    public virtual TransitionT GetOutputTransitionType(IStateTrigger<EntityT> stateTrigger)
    {
        foreach (var item in stateTriggerList)
        {
            if (stateTrigger==item.Value)
            {
                return item.Key;
            }
        }
        return default(TransitionT);
    }

    public virtual void RemoveTransition(TransitionT transitionType)
    {
        if (transitionDic.ContainsKey(transitionType))
        {
            transitionDic.Remove(transitionType);
        }
    }

    public Dictionary<TransitionT, StateTypeT> transitionDic = new Dictionary<TransitionT, StateTypeT>();

    protected IState(StateTypeT stateType,FsmStateMachine<StateTypeT, TransitionT, EntityT> fsmStateMachine)
    {
        this.stateType = stateType;
        fsmMachine = fsmStateMachine;
    }


    public virtual void AddTransition(TransitionT transitionType, StateTypeT iStateType)
    {
        if (transitionDic.ContainsKey(transitionType))
        {
            Debug.Log("转换字典中已经存在了：" + transitionType.ToString());
            transitionDic[transitionType] = iStateType;
            return;
        }
        transitionDic.Add(transitionType, iStateType);
        Debug.Log("添加Trigger"+ transitionType.ToString());
    }

    public virtual void Init(string masterName, EntityT entity,FsmTriggerData<StateTypeT, TransitionT>[] triggerDatas)
    {
        foreach (var trigger in triggerDatas)
        {
            if (trigger.enable==false)
            {
                continue;
            }
          string triggerName=masterName+trigger.triggerName+"Trigger";

            Type type = GetType(triggerName,entity);
            //此处做强制修正(有些trigger可能mastername是不一样的，但是需要共用一个类型)
            if (type==null)
            {
                triggerName = triggerName.Replace(masterName, "");
                type = GetType(triggerName, entity);
            }
            var triggerObj= Activator.CreateInstance(type,entity,fsmMachine) as IStateTrigger<EntityT>;

            triggerObj.priority = trigger.priority;
            triggerObj.ignoreLimit = trigger.ignoreLimit;
            triggerObj.intervalEnterFrameCount = trigger.intervalEnterFrameCount;
            triggerObj.nextStateChild = trigger.nextStateChild;
            triggerObj.immNextStateFSmType = trigger.transitionFSMType;
            triggerObj.SetAreTriggerInfo(trigger.areaTriggerInfo);
            triggerObj.triggerArg = trigger.triggerArgs;
            if (stateTriggerList.ContainsKey(trigger.triggerName))
            {
                stateTriggerList[trigger.triggerName] = triggerObj;
            }
            else
            {
                stateTriggerList.Add(trigger.triggerName, triggerObj);
            }
            AddTransition(trigger.triggerName, trigger.nextState);
        }
    }
  
    public Type GetType(string className,EntityT entity)
    {
        Type type = AssemblyTool.GetCorrentType(className + "`1");
        if (type != null)
        {
            var entityType = entity.GetType();
            type = type.MakeGenericType(entityType);
        }
        else
        {
            //type = Type.GetType(triggerName,true,true);
            type = AssemblyTool.GetCorrentType(className);
        }
        return type;
    }
}

