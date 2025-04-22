using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ACTComboAttackState : IState<AttackType, AttackTransitionType, PlayerController>
{
    public ACTComboAttackState(AttackType stateType, FsmStateMachine<AttackType, AttackTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);
        Debug.Log("Enter ComboAttack");
        go.LookAtCurrentViewPort();
    }

    public override void Excute(PlayerController go, params object[] args)
    {
        go.SedAttack("ComboAttack");
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

