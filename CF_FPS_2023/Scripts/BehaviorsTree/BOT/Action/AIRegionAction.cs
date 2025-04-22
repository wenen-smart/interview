using Assets.Resolution.Scripts.Region;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

[TaskCategory("Bot")]

public class AIRegionAction : Action
{
	RobotController bot;
	Seeker seeker;
	RichAI moveAgent;
	public float wanderMinWaitTime;
	public float wanderMaxWaitTime;
	MyTimer wanderWaitTimer;
	bool canOccupy;
	public ControllerState controllerState;
	public override void OnAwake()
	{
		base.OnAwake();
		bot = Owner.GetComponent<ActorComponent>().GetActorComponent<RobotController>();
		seeker = bot.seeker;
		moveAgent = bot.moveAgent;
		wanderWaitTimer = TimeSystem.Instance.CreateTimer();
	}
	public override void OnStart()
	{
		base.OnStart();

		
	}

	//public override TaskStatus OnUpdate()
	//{
	//	//find target
	//	//fire
	//}

	public override TaskStatus OnUpdate()
	{
		return base.OnUpdate();
	}

	public void Wander()
	{
		Transform targetNode = bot.currenStayRegion.GetUnoccpiedPoint();
		if (targetNode)
		{
			bot.currenStayRegion.CancelOccpy(bot);//取消之前那个
			canOccupy = bot.currenStayRegion.PreOccupy(bot, targetNode);
			if (canOccupy)
			{
				bot.StartPath(transform.position, targetNode.position, controllerState);
			}
		}
	}
}
