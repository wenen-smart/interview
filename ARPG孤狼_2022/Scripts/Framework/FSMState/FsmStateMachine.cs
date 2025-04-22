using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

[Serializable]
public class FsmStateMachine<StateTypeT,TransitionT,EntityT> where StateTypeT:Enum where TransitionT:Enum where EntityT:class
    {
    public StateTypeT currentStateType;
    public EntityT entity;

    public Dictionary<StateTypeT, IState<StateTypeT, TransitionT, EntityT>> stateDic=new Dictionary<StateTypeT,IState<StateTypeT, TransitionT, EntityT>>();
    public IState<StateTypeT, TransitionT, EntityT> currentState;
    protected IState<StateTypeT, TransitionT, EntityT> m_defaultState;
    public IState<StateTypeT, TransitionT, EntityT> lastState;
    public object parentStateMacine;
    public bool isEnterFrame;//是否是刚进入的。没有触发过Update的
  

    public FsmStateMachine(string masterName,EntityT entity, FsmStateData<StateTypeT, TransitionT>[] fsmStaterDatas)
    {
       this.entity = entity;
        Init(masterName, fsmStaterDatas);
    }

    public FsmStateMachine()
    {
    }

    public void SetDefaultState(IState<StateTypeT, TransitionT, EntityT> state)
    {
        m_defaultState = state;
        currentStateType=m_defaultState.stateType;
        ResetState();
        currentState.Enter(entity);
        currentState.Excute(entity);
    }
    public void AddState(IState<StateTypeT, TransitionT, EntityT> state)
    {
        if (stateDic.Count<=0)
        {
            currentStateType = state.stateType;
            currentState = state;
            Debug.Log("Default state "+ state.stateType);
            currentState.Enter(entity);
            currentState.Excute(entity);
        }
        if (stateDic.ContainsKey(state.stateType) ==false)
        {
            stateDic.Add(state.stateType, state);
            Debug.Log("add state" + state.stateType);
        }
        else
        {
            Debug.Log("StateDic中已经存在："+ state.stateType);
        }
    }
    
    public void TranstionTo(TransitionT transitionType, params object[] args)
    {
        if (stateDic==null)
        {
            Debug.Log("stateDic  null");
            return;
        }
        IState<StateTypeT, TransitionT, EntityT> nextState;
      var nextStateType =currentState.GetOutputState(transitionType.ToString());
        Debug.Log(nextStateType);
        //currentStateType = (StateTypeT)Enum.ToObject(typeof(StateTypeT), nextStateType); 
        if (stateDic.TryGetValue(nextStateType, out nextState))
        {
            lastState = currentState;
            currentState?.Exit(entity, args);
            currentState = nextState;
            currentStateType = nextStateType;
            isEnterFrame = true;
            currentState.Enter(entity, args);
            currentState.Excute(entity, args);
            Tick();//Immediately Excute

        }
        else
        {
            Debug.Log("字典里找不到"+ nextStateType + "状态");
        }
    }
    public void TranstionTo(int stateTypeTIndex, params object[] args)
    {
        if (stateDic == null)
        {
            Debug.Log("stateDic  null");
            return;
        }
        IState<StateTypeT, TransitionT, EntityT> nextState;
        StateTypeT stateTypeT = (StateTypeT)Enum.ToObject(typeof(StateTypeT), stateTypeTIndex);
        Debug.Log(stateTypeT);
        if (stateDic.TryGetValue(stateTypeT, out nextState))
        {
            lastState = currentState;
            currentState?.Exit(entity, args);
            currentState = nextState;
            currentStateType = stateTypeT;
            isEnterFrame = true;
            currentState.Enter(entity, args);
            currentState.Excute(entity, args);
            Tick();//Immediately Excute
        }
        else
        {
            Debug.Log("字典里找不到" + stateTypeT + "状态");
        }
    }
    public void ResetState()
    {
        currentState = m_defaultState;
    }
    public void Tick()
    {
        if (currentStateType.ToString()!="None")
        {
            currentState?.OnUpdate(entity);
            currentState?.Reason(entity);
            if (isEnterFrame)
            {
                isEnterFrame = false;
            }
        }
        
    }
    
    protected void Init(string masterName,FsmStateData<StateTypeT,TransitionT>[] fsmStaterDatas)
    {
        foreach (var stateData in fsmStaterDatas)
        {
            string className = masterName+stateData.stateType.ToString() + "State";

            Type type = AssemblyTool.GetCorrentType(className + "`1");;
            if (type != null)
            {
                var entityType = entity.GetType();
                type = type.MakeGenericType(entityType);
            }
            else
            {
                //type = Type.GetType(className,true,true);
                type = AssemblyTool.GetCorrentType(className);
            }
            var state=Activator.CreateInstance(type,stateData.stateType,this) as IState<StateTypeT, TransitionT, EntityT>;
            
            state.Init(masterName, entity, stateData.triggerData);
            AddState(state);
        }
    }

    public void SetNoneState()
    {
        IState<StateTypeT, TransitionT, EntityT> itemState = null;
        StateTypeT stateType=default(StateTypeT);
        foreach (var item in stateDic)
        {
            if (item.Key.ToString()=="None")
            {
                itemState = item.Value;
                stateType = item.Key;
                break;
            }
        }
        currentState = itemState;
        currentStateType = stateType;
        currentState?.Exit(entity);
        if (currentState!=null)
        {
            currentState.Enter(entity);
            currentState.Excute(entity);
        }
    }

    
    }


[Serializable]
public class FsmStateData<StateTypeT1, TranstionTypeT> where StateTypeT1 : Enum where TranstionTypeT : Enum
{
    public string masterType;
    public StateTypeT1 stateType;
    public FsmTriggerData<StateTypeT1, TranstionTypeT>[] triggerData;

}
[Serializable]
public class FsmTriggerData<StateTypeT1, TranstionTypeT> where StateTypeT1 : Enum where TranstionTypeT : Enum
{
    public TranstionTypeT triggerName;

    public StateTypeT1 nextState;

    public TransitionFSMType transitionFSMType = TransitionFSMType.SelfFSM;
    public AreaTriggerInfo areaTriggerInfo;
    public int nextStateChild = -1;//-1为空
    public TriggerArg triggerArgs;
    [Range(0, 120)]
    public int intervalEnterFrameCount;
    public bool ignoreLimit = false;
    [Range(-100, 100)]
    public int priority;
    public bool enable = true;
}
