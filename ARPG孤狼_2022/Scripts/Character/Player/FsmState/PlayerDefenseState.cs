using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerDefenseState : IState<PlayerStateType, PlayerTransitionType, PlayerController>
{
    public PlayerDefenseState(PlayerStateType stateType, FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);
        go.ClearAllMotionData(false);
    }

    public override void Excute(PlayerController go, params object[] args)
    {
        go.EnableCtrl(false);
        go.EnableRotate(false);
        go.StartDefense();
        go.LookAtCurrentViewPort();
    }

    public override void Exit(PlayerController go, params object[] args)
    {
        base.Exit(go, args);
        go.ExitDefense();
    }

    public override void OnUpdate(PlayerController go, params object[] args)
    {
        go.DefenseUpdate();
    }
}

