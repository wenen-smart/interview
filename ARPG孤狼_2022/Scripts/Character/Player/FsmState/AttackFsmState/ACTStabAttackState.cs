using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ACTStabAttackState : IState<AttackType, AttackTransitionType, PlayerController>
{
    public ACTStabAttackState(AttackType stateType, FsmStateMachine<AttackType, AttackTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }
    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go);
        go.LookAtCurrentViewPort();
        Debug.Log("StabAttack");
    }

    public override void Exit(PlayerController go, params object[] args)
    {
        base.Exit(go, args);
    }

    public override void Excute(PlayerController go, params object[] args)
    {
        SkillSystem.Instance.AttackSkillHandler(go, 4);
    }

    public override void OnUpdate(PlayerController go, params object[] args)
    {
        go.StabAttack();
    }
    public override void Reason(PlayerController go, params object[] args)
    {
        //base.Reason(go);
    }
}

