using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player_JumpState : FSMState<PlayerStateID, PlayerTransitionID, PlayerController>
{
    public Player_JumpState(PlayerStateID _stateID,FiniteStateMachine<PlayerStateID, PlayerTransitionID, PlayerController> fsm) : base(_stateID,fsm)
    {

    }

    public override void DoAfterLeaving()
    {
        
    }

    public override void DoBeforEntering()
    {
        base.DoBeforEntering();
        entity.ClearReadyClimbInfo();
        entity.JumpStateEnter();
    }

    public override void Reason()
    {
        if (entity.readyClimbType.IsSelectThisEnumInMult(ClimbType.LowerClimbOver|ClimbType.HigherClimbOver))
        {
            PerformTransition(PlayerTransitionID.Into_Climb);
        }
        else if (!entity.movement.IsInGrounded && entity.canFall)
        {
            PerformTransition(PlayerTransitionID.Happen_Fall);
        }
        else if (entity.jumpOnPhysicalStart&&entity.movement.IsInGrounded&&!entity.animatorMachine.CheckNextOrCurrentAnimationStateByName(0, "Jump") && !entity.animatorMachine.animator.IsInTransition(0))
        {
            PerformTransition(PlayerTransitionID.LandOnGround);
        }
        else if (entity.jumpAfterIsHoldPress)
        {
            if (entity.climbAbility&&entity.climbAbility.enabled&&entity.isInputingMove && entity.climbAbility.CheckContinueClimb(entity.animatorMachine.CheckAnimationStateByName(0, "Jump")))
            {
                entity.SetReadyClimbInfo(ClimbType.ContinueClimb);
                PerformTransition(PlayerTransitionID.Into_Climb);
            }
        }
    }

    public override void Tick()
    {
        
    }
}
