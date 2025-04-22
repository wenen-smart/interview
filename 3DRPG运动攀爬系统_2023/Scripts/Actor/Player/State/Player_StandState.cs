using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player_StandState : FSMState<PlayerStateID, PlayerTransitionID, PlayerController>
{
    public Player_StandState(PlayerStateID _stateID,FiniteStateMachine<PlayerStateID, PlayerTransitionID, PlayerController> fsm) : base(_stateID,fsm)
    {
    }

    public override void DoBeforEntering()
    {
        if (fsm.LastStateID.IsSelectThisEnumInMult(PlayerStateID.Jump|PlayerStateID.Fall))
        {
            if (!entity.animatorMachine.CheckNextAnimationStateByName(0,"FallEnd")&&!entity.animatorMachine.CheckAnimationStateByName(0,"FallEnd"))
            {
                entity.OpenMoveAndDirectionSwitch();
            }
        }
        if (true)
        {
            entity.OpenRotateSwitch();
            entity.movement.EnableKinematic(false);
            entity.movement.SetCastDirection(Vector3.down);
        }
    }

    public override void Reason()
    {
        if (entity.Trigger_Jump())
        {
            PerformTransition(PlayerTransitionID.Trigger_Jump);
        }
        else if (entity.canFall && !entity.IsInGround)
        {
            PerformTransition(PlayerTransitionID.Happen_Fall);
        }
        else if (entity.GetFixedMovementInputValueMagnitude(entity.InputController.MovementSmoothValue) == 1 && entity.InputController.isInputingWallRunTrigger && entity.wallRunAbility.CheckIsCanWallRun())
        {
            PerformTransition(PlayerTransitionID.Into_WallRun);
        }
        else if (entity.GetFixedMovementInputValueMagnitude(entity.InputController.MovementSmoothValue) == 1 && entity.jumpAfterIsHoldPress == false && !entity.animatorMachine.CheckAnimationStateByName(0, "Jump", "Fall", "Idle", "Climb Up"))
        {
            if (!entity.InputController.isInputingWallRunTrigger||entity.InputController.isInputingWallRunSlowTapTrigger)
            {
                if (entity.climbAbility && entity.climbAbility.enabled && entity.isInputingMove && entity.climbAbility.CheckContinueClimb(entity.animatorMachine.CheckAnimationStateByName(0, "Jump")))
                {
                    entity.SetReadyClimbInfo(ClimbType.ContinueClimb);
                    PerformTransition(PlayerTransitionID.Into_Climb);
                }
            }
        }
        
    }

    public override void Tick()
    {
    }
}