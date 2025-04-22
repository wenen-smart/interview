using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public delegate void FSMStateBehaviourDelegate();
public enum BehaviourOpportunityType
{
    Start,
    End
}
public enum StateBehaviourType
{
    Enter,
    Leave,
}
public enum DelegateLifetime
{
     ExecuteAfterImmDie,
     AlwaysExist,
}
public enum StateDirection
{
    Current,
    Next,
}
public struct FSMStateBehaviourEventMembers
{
    public StateDirection stateDirection;
    public StateBehaviourType stateBehaviourType;
    public BehaviourOpportunityType behaviourOpportunityType;
    public FSMStateBehaviourDelegate callback;
    public DelegateLifetime lifetime;
    public FSMStateBehaviourEventMembers(StateDirection stateDirection,StateBehaviourType stateBehaviourType,BehaviourOpportunityType behaviourOpportunityType, FSMStateBehaviourDelegate callback, DelegateLifetime lifetime)
    {
        this.stateDirection = stateDirection;
        this.stateBehaviourType= stateBehaviourType;
        this.behaviourOpportunityType= behaviourOpportunityType;
        this.callback = callback;
        this.lifetime= lifetime;
    }
}
public class FSMStateBehaviourEvent
{


    public int fid { get; protected set; }
    public StateBehaviourType stateBehaviourType { get; protected set; }
    public BehaviourOpportunityType behaviourOpportunityType { get; protected set; }
    public DelegateLifetime lifetime { get; protected set; }
    public bool isLife;
    protected FSMStateBehaviourDelegate callback;

    
    public FSMStateBehaviourEvent(int fid, BehaviourOpportunityType behaviourOpportunityType, StateBehaviourType stateBehaviourType, FSMStateBehaviourDelegate callback, DelegateLifetime lifetime)
    {
        this.fid = fid;
        this.behaviourOpportunityType = behaviourOpportunityType;
        this.stateBehaviourType = stateBehaviourType;
        this.lifetime = lifetime;
        this.callback = callback;
        isLife = true;
    }
    /// <summary>
    /// 事件执行
    /// </summary>
    /// <returns>返回活跃状态</returns>
    public bool Invoke()
    {
        callback?.Invoke();
        switch (lifetime)
        {
            case DelegateLifetime.ExecuteAfterImmDie:
                callback = null;
                isLife = false;
                break;
            default:
                break;
        }
        return isLife;
    }

}
