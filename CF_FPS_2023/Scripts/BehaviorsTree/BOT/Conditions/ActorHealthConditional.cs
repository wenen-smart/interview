using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Bot")]
public class ActorHealthConditional:Conditional
{
    public bool isOwner;
    public SharedTransform actor;
    public ValueCompareType valueCompareType;
    public int targetValue;
    private ActorHealth actorHealth;

    public override void OnAwake()
    {
        base.OnAwake();
        if (isOwner)
        {
            actorHealth = GetComponent<ActorComponent>().GetActorComponent<ActorHealth>();
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        
        if(isOwner==false)
        {
            actorHealth = actor.Value.GetComponent<ActorComponent>().GetActorComponent<ActorHealth>();
        }
    }
    public override TaskStatus OnUpdate()
    {
        switch (valueCompareType)
        {
            case ValueCompareType.LessThan:
                if (actorHealth.currentHp<targetValue)
                {
                    return TaskStatus.Success;
                }
                break;
            case ValueCompareType.GreaterThan:
                if (actorHealth.currentHp > targetValue)
                {
                    return TaskStatus.Success;
                }
                break;
            case ValueCompareType.Equal:
                if (actorHealth.currentHp == targetValue)
                {
                    return TaskStatus.Success;
                }
                break;
            default:
                break;
        }
        return TaskStatus.Failure;
    }
}
public enum ValueCompareType
{
    LessThan,
    GreaterThan,
    Equal,
}
