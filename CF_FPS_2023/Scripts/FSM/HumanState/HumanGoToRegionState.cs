using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HumanGoToRegionState : FSMState<HumanState, HumanStateTransition, RobotController>
{
	public HumanGoToRegionState(HumanState _stateID, FiniteStateMachine<HumanState, HumanStateTransition, RobotController> fsm) : base(_stateID, fsm)
	{
	}

	public override void DoBeforEntering()
	{
		base.DoBeforEntering();
		entity.SearchAIRegion();
	}

	public override void Reason()
	{
		if (entity.isArriveRegionOrEntry)
		{
			if (entity.moveAgent.reachedEndOfPath && entity.currenStayRegion != null)
			{
				fsm.PerformTransition(HumanState.GoToRegion,HumanStateTransition.到达高地);
			}
		}
	}

	public override void Tick()
	{
		if (entity.preOccupyPosition != null)
		{
			if (entity.isArriveRegionOrEntry == false && entity.moveAgent.reachedEndOfPath /*&& Vector3.Distance(transform.position, intoRegionPath.RegionEntry.position) <= (moveAgent.endReachedDistance + 0.01f)*/)
			{
				entity.isArriveRegionOrEntry = true;
				entity.StartPath(entity.transform.position, entity.preOccupyPosition.position, ControllerState.Running);
			}
		}
		List<RoleController> enemys = entity.EnemyInView();

		//在去高台路上---发现敌人
		if (enemys!=null&&enemys.Count>0)
		{
			//敌人个数
			RoleController closestEnemy = MapManager.Instance.FilterTheClosestEnemy(entity,enemys);
			int ememyCount = enemys.Count;
			if (ememyCount<2)
			{
				
			}
		}
	}
}
