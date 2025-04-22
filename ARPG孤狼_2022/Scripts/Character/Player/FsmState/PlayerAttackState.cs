using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerAttackState : HaveAttackFSMAttackState<PlayerStateType, PlayerTransitionType, PlayerController,AttackType,AttackTransitionType>
{
    public PlayerAttackState(PlayerStateType stateType, FsmStateMachine<PlayerStateType, PlayerTransitionType, PlayerController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }

    public override FsmStateMachine<AttackType, AttackTransitionType, PlayerController> GetAttackFsmMachine() 
    {
        return entity.attackFsmMachine;
    }
    public override void Reason(PlayerController go, params object[] args)
    {
       
        base.Reason(go, args);
    }
}

