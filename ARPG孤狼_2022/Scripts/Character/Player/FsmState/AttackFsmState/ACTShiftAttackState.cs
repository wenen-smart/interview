using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ACTShiftAttackState : IState<AttackType, AttackTransitionType, PlayerController>
{
    public ACTShiftAttackState(AttackType stateType, FsmStateMachine<AttackType, AttackTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }
    public override void Enter(PlayerController go, params object[] args)
    {
        base.Enter(go, args);

    }
    public override void Excute(PlayerController go, params object[] args)
    {
        SkillSystem.Instance.AttackSkillHandler(go, 5);
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

