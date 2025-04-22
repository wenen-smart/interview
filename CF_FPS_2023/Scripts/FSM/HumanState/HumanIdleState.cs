using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HumanIdleState : FSMState<HumanState, HumanStateTransition, RobotController>
{
	public HumanIdleState(HumanState _stateID, FiniteStateMachine<HumanState, HumanStateTransition, RobotController> fsm) : base(_stateID, fsm)
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
