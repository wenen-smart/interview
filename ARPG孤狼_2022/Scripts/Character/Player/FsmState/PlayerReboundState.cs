using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerReboundState : IState<PlayerStateType, PlayerTransitionType, PlayerController>
{
    public PlayerReboundState(PlayerStateType stateType, FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);     
        Debug.Log("格挡");
    }

    public override void Excute(PlayerController go, params object[] args)
    {
        go.StartRebound();
    }

    public override void Exit(PlayerController go, params object[] args)
    {
        base.Exit(go, args);
       go.playerAnim.SetLayerWeight(1, 0, true);
    }

    public override void OnUpdate(PlayerController go, params object[] args)
    {
       
    }
}

