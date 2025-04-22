using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEditor;
using Pathfinding;

[TaskCategory("Bot")]

public class SoliderBotInRegion : Conditional
{
	RobotController bot;
	public override void OnAwake()
	{
		base.OnAwake();
		bot = Owner.GetComponent<ActorComponent>().GetActorComponent<RobotController>();
	}

	public override TaskStatus OnUpdate()
	{
		if (bot.currenStayRegion)
		{
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}
}
