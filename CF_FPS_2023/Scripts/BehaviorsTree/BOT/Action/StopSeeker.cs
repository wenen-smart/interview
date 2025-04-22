

using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;

[TaskCategory("Bot")]
public class StopSeeker:Action
{
    private RobotController bot;
    public override void OnAwake()
    {
        base.OnAwake();
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }
    public override void OnStart()
    {
        base.OnStart();
        bot.StopSeeker();
    }
}

