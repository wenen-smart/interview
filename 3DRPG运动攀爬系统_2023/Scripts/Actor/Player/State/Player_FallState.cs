using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player_FallState : FSMState<PlayerStateID, PlayerTransitionID, PlayerController>
{
    private Player_WallRunState _WallRunState;
    private Quaternion targetRotationInXZPlane;
    public bool isDropState;//是跌落（失落）还是可控制的落点
    
    public Player_FallState(PlayerStateID _stateID, FiniteStateMachine<PlayerStateID, PlayerTransitionID, PlayerController> fsm) : base(_stateID, fsm)
    {
    }

    public override void DoAfterLeaving()
    {
        entity.animatorMachine.animator.SetBool("Happen_Fall",false);
    }

    public override void DoBeforEntering()
    {
        isDropState = false;
        entity.animatorMachine.animator.SetBool("Happen_Fall",true);
        entity.CloseMoveAndDirectionSwitch();
        entity.CloseKinematic();
        if (fsm.LastStateID==PlayerStateID.WallRun)
        {
            entity.OpenRotateSwitch();
            _WallRunState = (Player_WallRunState)fsm.GetLastState();
            //Player_WallRunState wallStateInfo = (Player_WallRunState)fsm.GetLastState();
            //if (wallStateInfo.movementSmoothValue_LeaveAfter.magnitude>0.1f)
            //{
            //    entity.OpenRotateSwitch();
            //}
            targetRotationInXZPlane = Quaternion.LookRotation(entity.ProjectOnSelfXZPlane(-_WallRunState.bodyUp_leaveAfter),Vector3.up);
            DebugTool.DrawLine(entity.transform.position,entity.ProjectOnSelfXZPlane(-_WallRunState.bodyUp_leaveAfter).normalized,2,Color.yellow,10);
            DebugTool.DrawWireSphere(entity.transform.position+entity.ProjectOnSelfXZPlane(-_WallRunState.bodyUp_leaveAfter).normalized*2,0.1f,Color.yellow,10,"targetLook-end");
        }
    }

    public override void Reason()
    {
        if (fsm.LastStateID==PlayerStateID.WallRun)
        {
            if (entity.isInputingMove&&entity.wallRunAbility.CheckIsCanWallRun(true,5))
            {
                PerformTransition(PlayerTransitionID.Into_WallRun);
            }
            else if (entity.InputController.JustInputedJumpThisFrame&&entity.climbAbility&&entity.climbAbility.enabled&& entity.climbAbility.CheckContinueClimb(false))
            {
                //check climb
                entity.SetReadyClimbInfo(ClimbType.ContinueClimb);
                PerformTransition(PlayerTransitionID.Into_Climb);
            }
            
        }
        if (entity.IsInGround)
        {
            PerformTransition(PlayerTransitionID.LandOnGround);
        }


    }

    public override void Tick()
    {
        if (fsm.LastStateID==PlayerStateID.WallRun)
        {
            //与键盘输入不一致。有改变
            if (_WallRunState.MovementValueInLocal!=Vector2Int.zero)
            {
                DebugTool.DebugLogger(MyDebugType.Print, string.Format($"发现不一致输入：{_WallRunState.MovementValueInLocal}")) ;
                entity.SetMoveAxisDirection(_WallRunState.MovementValueInLocal);
            }
            if (entity.animatorMachine.CheckAnimationStateByName(0, Const_Animation.BackAir_Roll_End)&&!entity.animatorMachine.animator.IsInTransition(0))
            {
                
                entity.animatorMachine.animator.MatchTarget(entity.targetRootPosition, targetRotationInXZPlane, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 1), 0.08f, 0.32f,true);
                
                DebugTool.DrawWireSphere(entity.rightFootPosition,0.1f,Color.yellow,10,"rightFootPosition");
            }
        }
        entity.FallUpdate(isDropState);
    }
    public void HappenDrop()
    {
        isDropState = true;
    }
}