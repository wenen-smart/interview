using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerMoveState : IState<PlayerStateType, PlayerTransitionType, PlayerController>
{
    public PlayerMoveState(PlayerStateType stateType, FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);
    }

    public override void Excute(PlayerController go, params object[] args)
    {
        go.BattleAction.Invoke();
    }

    public override void Exit(PlayerController go, params object[] args)
    {
        base.Exit(go, args);
        go.ClearAnimSpeedParameters();
    }

    public override void OnUpdate(PlayerController go, params object[] args)
    {
        go.Move();
    }
}

