using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HumanInRegionState : FSMState<HumanState, HumanStateTransition, RobotController>
{
	public HumanInRegionState(HumanState _stateID, FiniteStateMachine<HumanState, HumanStateTransition, RobotController> fsm) : base(_stateID, fsm)
	{

	}

	public override void Reason()
	{
		throw new NotImplementedException();
	}

	public override void Tick()
	{
		throw new NotImplementedException();
	}
}
