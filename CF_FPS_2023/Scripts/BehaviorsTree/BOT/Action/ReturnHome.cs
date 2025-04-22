using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEditor;

[TaskCategory("Bot")]
public class ReturnHome : Action
{
    RobotController bot;
    private RichAI moveAgent;
    private Seeker seeker;
    public ControllerState controllerState;
    public override void OnAwake()
    {
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
        moveAgent = bot.moveAgent;
        seeker = bot.seeker;
        base.OnAwake();
    }
    public override void OnStart()
    {
        bot.returnHome = true;
        bot.SetMoveMode(controllerState);
        bot.GoToTargetRoadPoint(MapManager.Instance.GetTeamHomeInfo(bot.unit).homeEnd);
    }
    public override TaskStatus OnUpdate()
    {
        if (Vector3.Distance(transform.position, bot.targetPointNode.point.position) <= (moveAgent.endReachedDistance + 0.1f))
        {
            DebugTool.DebugPrint("到达家里");
            return TaskStatus.Success;
        }
            return TaskStatus.Running;
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
        bot.returnHome = false;
    }
}

