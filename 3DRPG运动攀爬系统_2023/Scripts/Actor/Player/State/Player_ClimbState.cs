using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player_ClimbState : FSMState<PlayerStateID, PlayerTransitionID, PlayerController>
{
    public Player_ClimbState(PlayerStateID _stateID,FiniteStateMachine<PlayerStateID, PlayerTransitionID, PlayerController> fsm) : base(_stateID,fsm)
    {
    }

    public override void DoAfterLeaving()
    {
        entity.ClimbStateExit();
        entity.ClearReadyClimbInfo();
    }

    public override void DoBeforEntering()
    {
        entity.ClimbStateEnter();
        if (entity.readyClimbType==ClimbType.ContinueClimb)
        {
            entity.ContinueClimbStateEnter();
        }
        else if (entity.readyClimbType==ClimbType.LowerClimbOver)
        {
            DebugTool.DrawWireSphere(entity.climbAbility.ledge_Hit.point, 0.1f, Color.green, 2);
            entity.animatorMachine.animator.SetInteger("攀爬方式", 1);
            entity.animatorMachine.animator.CrossFadeInFixedTime("Lower_ClimbOver",0,0,0);
            entity.LowClimbOverStateEnter();
        }
        else if(entity.readyClimbType==ClimbType.HigherClimbOver)
        {
            DebugTool.DrawWireSphere(entity.climbAbility.ledge_Hit.point, 0.1f, Color.green, 2);
            entity.animatorMachine.animator.CrossFadeInFixedTime("Higher_ClimbOver",0,0,0.1f);
            entity.animatorMachine.animator.SetInteger("攀爬方式", 2);
            entity.HighClimbOverStateEnter();
        }
        
    }

    public override void Reason( )
    {
        if (entity.InputController.JustInputedJumpThisFrame)
        {
            entity.ClimbToFall();
            PerformTransition(PlayerTransitionID.Happen_Fall);
        }
        else if (entity.currentMovementIDParameter==0&&!entity.animatorMachine.CheckAnimationStateByTag(0,"攀爬")&&!entity.animatorMachine.animator.IsInTransition(0))
        {
            if (entity.readyClimbType==ClimbType.None)
            {
                PerformTransition(PlayerTransitionID.LandOnGround);
            }
        }
    }

    public override void Tick( )
    {
        ClimbType currentClimbType = ClimbType.None;
        if (entity.animatorMachine.CheckAnimationStateByName(0, "Lower_ClimbOver"))
        {
            currentClimbType = ClimbType.LowerClimbOver;
        }
        else if (entity.animatorMachine.CheckAnimationStateByName(0, "Higher_ClimbOver"))
        {
            currentClimbType = ClimbType.HigherClimbOver;
        }

        if (currentClimbType.IsSelectThisEnumInMult(ClimbType.LowerClimbOver | ClimbType.HigherClimbOver))
        {
            entity.animatorMachine.animator.SetInteger("攀爬方式", 0);
            entity.ClearReadyClimbInfo();
            
            if (currentClimbType == ClimbType.LowerClimbOver)
            {
                DebugTool.DrawWireSphere(entity.leftHandPosition, 0.1f, Color.green, 2);
                if (!entity.animatorMachine.animator.IsInTransition(0))
                {
                    entity.animatorMachine.animator.MatchTarget(entity.leftHandPosition, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.1f);
                    entity.animatorMachine.animator.MatchTarget(entity.leftHandPosition + Vector3.up * (0.2f + 0.054f), Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.up, 0f), 0.1f, 0.3f);
                }
            }
            else if (currentClimbType == ClimbType.HigherClimbOver)
            {
                 DebugTool.DrawWireSphere(entity.rightFootPosition, 0.1f, Color.green, 2,"highClimb_foot");
                 DebugTool.DrawWireSphere(entity.rightHandPosition, 0.1f, Color.green, 2);
                
                if (!entity.animatorMachine.animator.IsInTransition(0))
                {
                    entity.animatorMachine.animator.MatchTarget(entity.rightFootPosition, Quaternion.identity, AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0f), 0.1f,  0.33f);
                    entity.animatorMachine.animator.MatchTarget(entity.rightHandPosition, Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0f), 0.33f, 0.45f);
                    
                }
            }
        }
        else if (entity.currentClimbProcessState == ClimbProcessState.ClimbWholeFinish && !entity.animatorMachine.CheckNextAnimationStateByTag(0, "攀爬") && entity.animatorMachine.animator.IsInTransition(0))
        {
            entity.ClearReadyClimbInfo();
        }
        entity.ClimbUpdate();
        if (entity.currentClimbProcessState == ClimbProcessState.CancelClimb && entity.currentMovementIDParameter == 1)
        {
            entity.ClimbToIdle();
            entity.ClearReadyClimbInfo();
        }
        else if (entity.currentClimbProcessState == ClimbProcessState.ClimbWholeFinish && entity.currentMovementIDParameter == 1)
        {
            entity.ClimbUpToTop();
        }
    }
}
