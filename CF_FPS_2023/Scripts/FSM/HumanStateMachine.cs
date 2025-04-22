using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum HumanState
{
	Idle,
	GoToFeed,
	GoToRegion,
	InRegion,
	RunAway,
	MajorState,
}

public enum HumanStateTransition
{
	弹药不足,
	去高地,
	到达高地,
}

public class HumanStateMachine:FiniteStateMachine<HumanState,HumanStateTransition,RobotController>
{

}
