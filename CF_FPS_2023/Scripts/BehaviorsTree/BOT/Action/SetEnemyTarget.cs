using Assets.Resolution.Scripts.Region;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

public class SetEnemyTarget : Action 
{
	public SelectUnitMethod selectUnitMethod;
	public SharedTransform target;
	RobotController bot;
	Seeker seeker;
	RichAI moveAgent;
	public TaskStatus taskStatus;
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
		RoleController targetEnemy=null;
		switch (selectUnitMethod)
		{
			case SelectUnitMethod.Random:
				targetEnemy = MapManager.Instance.RandomGetUnitInCamp(bot.GetEnemyUnitTag());
				break;
			case SelectUnitMethod.Closest:
				targetEnemy = MapManager.Instance.FilterTheClosestEnemy(bot,bot.GetEnemyUnitTag());
				break;
			default:
				break;
		}
		if (targetEnemy!=null)
		{
			target.Value = targetEnemy.transform;
		}

	}
	public override TaskStatus OnUpdate()
	{
		return taskStatus;
	}
}
public enum SelectUnitMethod
{
	Random,
	Closest,
}
