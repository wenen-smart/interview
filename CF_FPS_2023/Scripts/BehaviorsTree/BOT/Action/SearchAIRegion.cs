using Assets.Resolution.Scripts.Region;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;


[TaskCategory("Bot")]

public class SearchAIRegion : Action
{
	RobotController bot;
	Seeker seeker;
	RichAI moveAgent;
	bool isArriveRegionOrEntry = false;
	IntoRegionPath intoRegionPath;
	AIRegion targetRegion;
	Transform occupyPosition;
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
		if (targetRegion!=null)
		{
			targetRegion.CancelOccpy(bot);
		}
		targetRegion = MapManager.Instance.GetOneCanStandAiRegion();
		intoRegionPath = targetRegion.intoRegion[0];
		occupyPosition = targetRegion.PreOccupy(bot);
		bot.StartPath(transform.position, intoRegionPath.RegionEntry.position,controllerState);
		//path.BlockUntilCalculated();
		Debug.Log("IntoPathPoint:"+intoRegionPath.name+intoRegionPath.RegionEntry.name);
		
		isArriveRegionOrEntry = false;
	}
	public override TaskStatus OnUpdate()
	{
		if (occupyPosition==null)
		{
			return TaskStatus.Failure;
		}
		if (isArriveRegionOrEntry == false && moveAgent.reachedEndOfPath /*&& Vector3.Distance(transform.position, intoRegionPath.RegionEntry.position) <= (moveAgent.endReachedDistance + 0.01f)*/)
		{
			isArriveRegionOrEntry = true;
			bot.StartPath(transform.position, occupyPosition.position,controllerState);
			Debug.Log("occupyPosition:"+occupyPosition.name);
		}
		else if (isArriveRegionOrEntry)
		{
			if (moveAgent.reachedEndOfPath&&bot.currenStayRegion!=null)
			{
				return TaskStatus.Success;
			}
		}
		return TaskStatus.Running;
	}

	public override void OnConditionalAbort()
	{
		bot.StopSeeker();
	}
}
