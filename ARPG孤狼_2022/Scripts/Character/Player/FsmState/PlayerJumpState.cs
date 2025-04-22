using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerJumpState : IState<PlayerStateType, PlayerTransitionType, PlayerController>
{
    public PlayerJumpState(PlayerStateType stateType, FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);
        go.JumpExcute();
    }

    public override void Excute(PlayerController go, params object[] args)
    {
       
    }

    public override void Exit(PlayerController go, params object[] args)
    {
        base.Exit(go, args);
        go.JumpExit();
    }

    public override void OnUpdate(PlayerController go, params object[] args)
    {
        go.Jump();
    }
}

