using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Player_WallRunState : FSMState<PlayerStateID, PlayerTransitionID, PlayerController>
{
    private AnimatorMachine animatorMachine;
    private PlayerInputController _inputController;
    private Collider _lastWall { get;  set; }
    public Vector2 moveAxisDirection;
    private Vector2 LastMovementValue;
    public Vector2Int MovementValueInLocal=Vector2Int.zero;
    //leaveAfter  退出状态时记录
    public Vector2 movementSmoothValue_LeaveAfter;
    private Vector3 pointNormal_leaveAfter;
    public Vector3 bodyUp_leaveAfter;
    
    public Player_WallRunState(PlayerStateID _stateID, FiniteStateMachine<PlayerStateID, PlayerTransitionID, PlayerController> fsm) : base(_stateID, fsm)
    {

    }

    public override void DoAfterLeaving()
    {
        base.DoAfterLeaving();
        bodyUp_leaveAfter = entity.transform.up;
        if (entity.wallRunAbility.climbForecastHitinfo.collider!=null)
        {
            pointNormal_leaveAfter = entity.wallRunAbility.climbForecastHitinfo.normal;
        }
        else
        {
            DebugTool.DebugLogger(MyDebugType.SupperError,"WallRun状态退出时，climbForecastHitinfo数据为空，请检查");
        }
        
        entity.WallRunStateExit();
        movementSmoothValue_LeaveAfter = entity.movementSmoothValue;
    }

    public override void DoBeforEntering()
    {
        base.DoBeforEntering();
        _lastWall = entity.wallRunAbility.lastWall;
        BaseRaycastHit readyRaycastHit = entity.wallRunAbility.readyClimbForecastHitinfo;
        DebugTool.DebugLogger(MyDebugType.Print,string.Format("pointNormal_leaveAfter:{0}",pointNormal_leaveAfter));
        if (pointNormal_leaveAfter!=Vector3.zero)
        {
            Vector3 pointNormalInProject_leaveAfter = Vector3.ProjectOnPlane(pointNormal_leaveAfter, Vector3.up);
            Vector3 currentNormalInProject = Vector3.ProjectOnPlane(readyRaycastHit.normal, Vector3.up);
            float projectSize = Vector3.Dot(pointNormalInProject_leaveAfter, currentNormalInProject);
            int horInt = Mathf.RoundToInt(movementSmoothValue_LeaveAfter.x);
            DebugTool.DebugLogger(MyDebugType.Print,string.Format("movementSmoothValue_LeaveAfter:{0}",movementSmoothValue_LeaveAfter));
            DebugTool.DebugLogger(MyDebugType.Print,string.Format("projectSize:{0}",projectSize < 0?"方向相反":"方向相同"));
            if (projectSize < 0)
            {
                //方向相反
                if (_inputController.MovementValue.x>0)
                {
                     MovementValueInLocal.x = -horInt;
                }
                else if (_inputController.MovementValue.x<0)
                {
                    MovementValueInLocal.x = horInt;
                }

            }
            else if (projectSize > 0)
            {
                //方向相同
                MovementValueInLocal = Vector2Int.zero;
            }
            else
            {
                //向量垂直
            }
        }
        
        entity.WallRunStateEnter();
        animatorMachine = entity.animatorMachine;
        _inputController = entity.InputController;
        
    }

    public override void Reason()
    {
        if (_inputController.JustInputedJumpThisFrame)
        {
            PerformTransition(PlayerTransitionID.Happen_Fall);
        }
        else if (entity.currentMovementIDParameter == 0 && !entity.animatorMachine.CheckAnimationStateByTag(0, "贴墙跑") && !entity.animatorMachine.animator.IsInTransition(0) && !entity.animatorMachine.CheckAnimationStateByName(0,"Wall_Land"))
        {
            PerformTransition(PlayerTransitionID.LandOnGround);
        }
        
        //else if (_inputController.MovementSmoothValue.y<-0.9f&&fsm.LastStateID==PlayerStateID.Stand)
        //{
        //    PerformTransition(PlayerTransitionID.LandOnGround);
        //}
         
    }

    public override void Tick()
    {
        
        //TODO LOCOMOTION
        //TODO ALGIN
        //TODO environment dynameic check
        
        if (MovementValueInLocal!=Vector2Int.zero)
        {
            int horInt = Mathf.RoundToInt(_inputController.MovementSmoothValue.x);
            if (horInt==0)
            {
                MovementValueInLocal = Vector2Int.zero;
            }
        }
        if (MovementValueInLocal==Vector2Int.zero)
        {
            moveAxisDirection = Vector2.one;
        }
        else
        {
            moveAxisDirection = MovementValueInLocal;
        }
        entity.SetMoveAxisDirection(moveAxisDirection);
        animatorMachine.animator.SetFloat("Horizontal", entity.movementValue.x, 0.1f, Time.deltaTime);
        animatorMachine.animator.SetFloat("Forward", _inputController.MovementSmoothValue.y, 0.1f, Time.deltaTime);
        if (!_inputController.isInputingMove)
        {
            bool toLeft = LastMovementValue.x <= 0;
            entity.SetMirrorParameter(toLeft);
        }
        else
        {
            LastMovementValue = moveAxisDirection;
        }

        
        entity.WallRunLocomotionUpdate();
        if (entity.currentClimbProcessState == ClimbProcessState.CancelClimb && entity.IsInGround&&entity.currentMovementIDParameter==2)
        {
            entity.WallRunToStand();
            
        }
    }
    public void ClearLastLeaveData()
    {
        pointNormal_leaveAfter = Vector3.zero;
        movementSmoothValue_LeaveAfter = Vector2.zero;
    }

    
}

