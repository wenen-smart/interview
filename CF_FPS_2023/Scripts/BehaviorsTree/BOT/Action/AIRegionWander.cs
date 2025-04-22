using Assets.Resolution.Scripts.Region;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskCategory("Bot")]
public class AIRegionWander:Action
{
	RobotController bot;
	Seeker seeker;
	RichAI moveAgent;
	bool canOccupy;
	public ControllerState controllerState;
	public override void OnAwake()
	{
		base.OnAwake();
		bot = Owner.GetComponent<ActorComponent>().GetActorComponent<RobotController>();
		seeker = bot.seeker;
		moveAgent = bot.moveAgent;
	}
	public override void OnStart()
	{
		base.OnStart();
		
		Transform targetNode = bot.currenStayRegion.GetUnoccpiedPoint();
		if (targetNode)
		{
			bot.currenStayRegion.CancelOccpy(bot);//取消之前那个
			canOccupy =  bot.currenStayRegion.PreOccupy(bot,targetNode);
			if (canOccupy)
			{
				bot.StartPath(transform.position,targetNode.position,controllerState);
			}
		}
	}

	public override TaskStatus OnUpdate()
	{
		if (canOccupy==false)
		{
			return TaskStatus.Failure;
		}
		if (moveAgent.reachedEndOfPath)
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
