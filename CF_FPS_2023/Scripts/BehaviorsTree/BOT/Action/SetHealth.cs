using BehaviorDesigner.Runtime.Tasks;

public class SetHealth : Action
{
    public int hp = 0;
    public ActorHealth actorHealth;
    public override void OnAwake()
    {
        base.OnAwake();
        actorHealth = GetComponent<ActorComponent>().GetActorComponent<ActorHealth>();
    }
    public override void OnStart()
    {
        base.OnStart();
        actorHealth.SetHp(hp);
    }
}

