using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class FSMState<T_StateID, T_TransitionID,T_Entity> where T_StateID : Enum where T_TransitionID : Enum where T_Entity:class//抽象类必须被实现
{
    protected T_StateID stateID;
    protected FiniteStateMachine<T_StateID, T_TransitionID,T_Entity> fsm;
    private List<FSMStateBehaviourEvent> fsm_StateBehaviourEvents=new List<FSMStateBehaviourEvent>();
    public T_StateID lastStateID { get; private set; }
    public FSMState<T_StateID, T_TransitionID,T_Entity> lastState { get; private set; }

    public void UpdateLastState(FSMState<T_StateID, T_TransitionID,T_Entity> lastState)
    {
        this.lastState = lastState;
        this.lastStateID = lastState.stateID;
    }
    public FSMState(T_StateID _stateID,FiniteStateMachine<T_StateID, T_TransitionID,T_Entity> fsm)
    {
        this.fsm = fsm;
        stateID = _stateID;
    }
    public T_StateID ID
    {
        get { return stateID; }
    }
    public T_Entity entity { get { return fsm.entity; } }

    protected Dictionary<T_TransitionID, T_StateID> map = new Dictionary<T_TransitionID, T_StateID>();


    public int RegisterEvent(StateBehaviourType stateBehaviourType, BehaviourOpportunityType behaviourOpportunityType, FSMStateBehaviourDelegate stateBehaviourDelegate, DelegateLifetime delegateLifetime)
    {
        int fid =fsm.GenericFID();
        FSMStateBehaviourEvent behaviourEvent = new FSMStateBehaviourEvent(fid,behaviourOpportunityType,stateBehaviourType,stateBehaviourDelegate,delegateLifetime);
        fsm.AddFID(fid);
        fsm_StateBehaviourEvents.Add(behaviourEvent);
        return fid;

    }
    public void StateBehaviourOpportunityExecute(StateBehaviourType stateBehaviourType,BehaviourOpportunityType behaviourOpportunityType)
    {
       List<FSMStateBehaviourEvent> targetEvents =  fsm_StateBehaviourEvents.Where((e)=> e.behaviourOpportunityType==behaviourOpportunityType&&e.stateBehaviourType==stateBehaviourType).ToList();
        if (targetEvents!=null)
        {
            for (int i = 0; i < targetEvents.Count; i++)
            {
                
                if (targetEvents[i].Invoke() == false)
                {
                    DeleteStateBehaviourEvent(targetEvents[i].fid);
                    targetEvents.RemoveAt(i);
                    i--;
                }
            }
        }
        
    }
    public void DeleteStateBehaviourEvent(int fid)
    {
        FSMStateBehaviourEvent removingEvent = fsm_StateBehaviourEvents.SingleOrDefault((e) => e.fid == fid);
        if (removingEvent != null)
        {
            fsm_StateBehaviourEvents.Remove(removingEvent);
        }
    }

    public bool PerformTransition(T_TransitionID transitionID, params FSMStateBehaviourEventMembers[] members)
    {
        return fsm.PerformTransition(ID,transitionID,members);
    }
    public void AddTransition(T_TransitionID trans, T_StateID id)//添加状态
    {
        //安全校验
        if (map.ContainsKey(trans))
        {
            Debug.LogError("添加转换条件时，" + trans + "已经存在于map中");
            return;
        }
        //校验通过后添加map
        map.Add(trans, id);
    }

    public void DeleteTransition(T_TransitionID trans)//删除状态
    {
        if (map.ContainsKey(trans) == false)
        {
            Debug.LogError("添加转换条件时，" + trans + "不存在于map中");
            return;
        }

        map.Remove(trans);
    }

    public T_StateID GetOutputState(T_TransitionID trans)//得到当前状态
    {
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }

        return (T_StateID)Enum.Parse(typeof(T_StateID),"None");
    }

    

    //类中的virtual不一定要实现 abstract一定要实现
    public virtual void DoBeforEntering() { }
    public virtual void DoAfterLeaving() { }
    /// <summary>
    /// 状态逻辑
    /// </summary>
    public abstract void Tick();
    /// <summary>
    /// 判断转换条件
    /// </summary>
    public abstract void Reason();

}
