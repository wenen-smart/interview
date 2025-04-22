using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ACTNoneState : IState<AttackType, AttackTransitionType, PlayerController>
{
    public ACTNoneState(AttackType stateType, FsmStateMachine<AttackType, AttackTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);
        Debug.Log("Attack None");
        if (args!=null&&args.Length>0)
        {
            int nextState = (int)args[0];
            //根据Trigger 直接转到另一个状态
            if (nextState != -1)
            {
                fsmMachine.TranstionTo(nextState, args);
            }
        }
        
    }

    public override void Excute(PlayerController go, params object[] args)
    {
        
    }

    public override void Exit(PlayerController go, params object[] args)
    {
      base.Exit(go, args);
    }

    public override void OnUpdate(PlayerController go, params object[] args)
    {
        
    }
    public override void Reason(PlayerController go, params object[] args)
    {
        //base.Reason(go);
    }
}

