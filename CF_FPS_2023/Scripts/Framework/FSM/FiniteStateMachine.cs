using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FiniteStateMachine<T_StateID,T_TransitionID,T_Entity> where T_StateID:Enum where T_TransitionID:Enum where T_Entity:class
{
    private Dictionary<T_StateID, FSMState<T_StateID,T_TransitionID,T_Entity>> States = new Dictionary<T_StateID, FSMState<T_StateID,T_TransitionID,T_Entity>>();
    public T_StateID CurrentStateID { get; private set; }
    public T_StateID LastStateID { get; private set; }
    private FSMState<T_StateID,T_TransitionID,T_Entity> CurrentState;
    private T_StateID defaultNullStateID;
    public T_Entity entity { get; private set; }
    private int fid;
    private List<int> fidList=new List<int>();

    private void SetNullState(T_StateID t_StateID)
    {
        defaultNullStateID = t_StateID;
        CurrentStateID = t_StateID;
    }
    public void Init(T_Entity _entity, T_StateID noneSateID)
    {
        entity = _entity;
        SetNullState(noneSateID);
    }
    public void Update()
    {
        if (CurrentState==null)
        {
            return;
        }
        CurrentState.Tick();
        CurrentState.Reason();
    }

    public void AddState(FSMState<T_StateID,T_TransitionID,T_Entity> _state)
    {
        if (_state == null)
        {
            Debug.LogError("state为空");
            return;
        }

        if (CurrentState == null)
        {
            CurrentState = _state;
            CurrentStateID = _state.ID;
        }

        if (States.ContainsKey(_state.ID))
        {
            Debug.LogError("状态" + _state.ID + "已经存在，无法重复添加");
            return;
        }

        States.Add(_state.ID, _state);
    }

    public void DeleteFSMState(T_StateID id)
    {
        if (id.Equals(defaultNullStateID))
        {
            Debug.LogError("无法删除空状态");
            return;
        }

        if (States.ContainsKey(id) == false)
        {
            Debug.LogError("无法删除不存在状态" + id);
            return;
        }

        States.Remove(id);
    }
    private FSMState<T_StateID, T_TransitionID, T_Entity> GetStateByTransitionID(FSMState<T_StateID,T_TransitionID,T_Entity> state,T_TransitionID transitionID)
    {
        FSMState<T_StateID, T_TransitionID, T_Entity> nextState=null;
        T_StateID id = CurrentState.GetOutputState(transitionID);
        if (id.Equals(defaultNullStateID))
        {
            DebugTool.DebugWarning("当前状态" + CurrentStateID + "无法根据转换条件" + transitionID + "发生转换");
            return null;
        }

        if (States.ContainsKey(id) == false)
        {
            DebugTool.DebugError("状态机内没有包含" + id + "，无法进行状态转换");
            return null;
        }
        nextState = States[id];
        return nextState;
    }
    public bool PerformTransition(T_StateID applyer,T_TransitionID transitionID, params FSMStateBehaviourEventMembers[] members)
    {
        if (!applyer.Equals(CurrentState.ID))
        {
            return false;
        }
        FSMState<T_StateID, T_TransitionID, T_Entity> state = GetStateByTransitionID(CurrentState, transitionID);
        
        if (members!=null)
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].stateDirection==StateDirection.Current)
                {
                    CurrentState.RegisterEvent(members[i].stateBehaviourType, members[i].behaviourOpportunityType, members[i].callback, DelegateLifetime.ExecuteAfterImmDie);
                }
                else if (members[i].stateDirection==StateDirection.Next)
                {
                    state.RegisterEvent(members[i].stateBehaviourType, members[i].behaviourOpportunityType, members[i].callback, DelegateLifetime.ExecuteAfterImmDie);
                }
            }
        }

        DebugTool.DebugPrint("发生状态转换！上一状态：" + CurrentStateID + " 当前状态：" + state.ID);
        LastStateID = CurrentStateID;
        CurrentState.StateBehaviourOpportunityExecute(StateBehaviourType.Leave, BehaviourOpportunityType.Start);
        CurrentState.DoAfterLeaving();
        CurrentState.StateBehaviourOpportunityExecute(StateBehaviourType.Leave, BehaviourOpportunityType.End);

        CurrentStateID = state.ID;
        CurrentState = state;
        CurrentState.UpdateLastState(GetLastState());

        CurrentState.StateBehaviourOpportunityExecute(StateBehaviourType.Enter, BehaviourOpportunityType.Start);
        CurrentState.DoBeforEntering();
        CurrentState.StateBehaviourOpportunityExecute(StateBehaviourType.Enter, BehaviourOpportunityType.End);
        return true;
    }
    public FSMState<T_StateID,T_TransitionID,T_Entity> GetLastState()
    {
         return States[LastStateID];
    }
    public FSMState<T_StateID, T_TransitionID, T_Entity> GetCurrentState()
    {
        return CurrentState;
    }
    /// <summary>
    /// 生成唯一的任务ID  
    /// </summary>
    /// <returns></returns>
    public int GenericFID()
    {
        fid++;
        //Safe Check
        while (true)
        {
            if (fid == int.MaxValue)
            {
                fid = 0;
            }
            bool used = false;
            for (int i = 0; i < fidList.Count; i++)
            {
                if (fid == fidList[i])
                {
                    used = true;
                    break;
                }
            }
            if (used == false)
            {
                break;
            }
            fid++;
        }
        return fid;
    }
    public void AddFID(int _fid)
    {
        fidList.Add(_fid);
    }
    /// <summary>
    /// 匹配过去状态栈
    /// </summary>
    /// <param name="stateid_list">请按照最近进入时间进行排序</param>
    /// <returns></returns>
    public bool IsMatchLastStateStack(params T_StateID[] stateid_list)
    {
        FSMState<T_StateID, T_TransitionID, T_Entity> state = CurrentState;
        if (stateid_list!=null)
        {
            for (int i = 0; i < stateid_list.Length; i++)
            {
                if (!state.lastStateID.Equals(stateid_list[i]))
                {
                    return false;
                }
                state = state.lastState;
            }
            return true;
        }
        return false;
    }
}
//public enum StateID
//{
//    None,
//}
//public enum TransitionID
//{
//    None,
//}

