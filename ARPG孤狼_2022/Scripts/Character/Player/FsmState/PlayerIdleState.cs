using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class PlayerIdleState : IState<PlayerStateType, PlayerTransitionType, PlayerController>
{
    public PlayerIdleState(PlayerStateType stateType, FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {


    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);
        go.ClearIncreaseVelo();
        go.ClearMoveSpeed();
        go.CleatDeltaRotation();
        
        go.ClearAnimSpeedParameters();
        switch (go.moveMode)
        {
            case ActorMoveMode.Common:
            case ActorMoveMode.Shimmy:
            case ActorMoveMode.LockTarget:
                go.EnableCtrl(true);
                go.anim.SetFloat(AnimatorParameter.VeloDir.ToString(), 0);
                break;
            case ActorMoveMode.ClosetWall:
                go.EnableCtrl(false);
            break;
            default:
                break;
        }
        go.anim.SetFloat(AnimatorParameter.TurnAngle.ToString(), 0);
    }

    public override void Excute(PlayerController go, params object[] args)
    {
        if (go.stateManager.CheckState(ActorStateFlag.isDrawBow.ToString())==false)
        {
            go?.UnBattleAction?.Invoke();
        }
    }

    public override void Exit(PlayerController go, params object[] args)
    {
      base.Exit(go, args);
    }

    public override void OnUpdate(PlayerController go, params object[] args)
    {
        go.Idle();
    }
}

