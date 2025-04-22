using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class WolfIdleState : IState<WolfState, WolfTransitionType, WolfController>
{
    public WolfIdleState(WolfState stateType, FsmStateMachine<WolfState, WolfTransitionType, WolfController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Excute(WolfController go, params object[] args)
    {
        
    }

    public override void OnUpdate(WolfController go, params object[] args)
    {
        go.UpdateTarget();
    }
}

