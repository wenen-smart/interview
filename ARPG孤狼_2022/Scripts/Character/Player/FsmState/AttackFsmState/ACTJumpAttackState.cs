using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ACTJumpAttackState : IState<AttackType, AttackTransitionType, PlayerController>
{
    public ACTJumpAttackState(AttackType stateType, FsmStateMachine<AttackType, AttackTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }

    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go, args);
       
    }
    public override void Excute(PlayerController go, params object[] args)
    {
        go.JumpAttack((TriggerArg)args[1]);
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

    }
}

