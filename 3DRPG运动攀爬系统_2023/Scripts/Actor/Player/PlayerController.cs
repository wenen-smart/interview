using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum PlayerStateID
{
    None,
    Stand,
    Jump,
    Fall,
    Climb,
    WallRun,
}
public enum PlayerTransitionID
{
    None,
    Trigger_Jump,
    Happen_Fall,
    LandOnGround,
    Into_Climb,
    Into_WallRun
}
public enum LocomotionState
{
    Idle,
    Move
}

public partial class PlayerController : RoleController
{
    private FiniteStateMachine<PlayerStateID, PlayerTransitionID, PlayerController> stateMachine = new FiniteStateMachine<PlayerStateID, PlayerTransitionID, PlayerController>();
    private CinemachineFreeLook freelook;
    private PlayerInputController _inputController;
    public PlayerInputController InputController { get { return _inputController; } }
    public bool isInputingMove { get { return _inputController.isInputingMove; } }
    public float air_movement_MaxSpeed = 1;

    private CapsuleCollider characterCollider;
    public Transform HeadPoint { get { return climbAbility.HeadPoint; } }
    public bool footAndHandIK;
    public IKInfo leftHandMatchTargetInfo;
    public IKInfo rightHandMatchTargetInfo;
    public IKInfo leftFootIKInfo;
    public IKInfo rightFootIKInfo;
    [HideInInspector] public bool isMirrorParameter = false;
    public bool jumpAfterIsHoldPress;//触发跳跃之后是否一直按住跳跃键没放开。
    public bool jumpOnPhysicalStart = false;
    public Vector2 movementSmoothValue { get { return _inputController.MovementSmoothValue * inputMoveAxisDirection; } }//移动方向输入，不一定是键盘的输入值，可改变。
    public Vector2 movementValue{ get { return _inputController.MovementValue * inputMoveAxisDirection; } }
    public Vector2 inputMoveAxisDirection;

    public void  SetMoveAxisDirection(Vector2 _direction)
    {
        inputMoveAxisDirection = new Vector2(_direction.x.GetNormalizeValue(),_direction.y.GetNormalizeValue());
    }
    #region IK
    public Transform leftHandIKTargetParent;
    public Transform rightHandIKTargetParent;
    public float L_Hand_IKWeight = 0;
    public float R_Hand_IKWeight = 0;
    public bool canFall=true;
    public float fallHeight = 0.2f;
    public float verticalVelocity;
    private int commonMovementParam = 0;
    private int climbMovementParam = 1;
    private int wallRunMovementParam = 2;
    public int currentMovementIDParameter = 0;
    public float fallMultiply = 1.5f;
    public float jumpMaxHeight = 2f;
    [HideInInspector]public Vector3 leftHandPosition;
    [HideInInspector]public Vector3 rightHandPosition;
    [HideInInspector]public Vector3 leftFootPosition;
    [HideInInspector]public Vector3 rightFootPosition;
    [HideInInspector]public Vector3 targetRootPosition;

    //public bool haveforce_inAir;
    #endregion
    #region Unity_RuntimeCycle
    public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        movement = actorSystem.GetComponent<Movement>();
        animatorMachine = GetComponentInChildren<AnimatorMachine>();
        _inputController = GetComponent<PlayerInputController>();
        movement.changeGroundStateEventHandler += GroundStateChangeAfter;
        characterCollider = actorSystem.GetComponent<CapsuleCollider>();
        if (movement_baseSpeed < 0) movement_baseSpeed = 0;
        climbAbility = GetActorComponent<ClimbAbility>();
        if (climbAbility)
        {
            climbAbility.Init();
            climbAbility.SetCharacterHeight(movement.centerHeight);
        } 
        wallRunAbility = GetActorComponent<WallRunAbility>();
        if (wallRunAbility)
        {
            wallRunAbility.HeadPoint = HeadPoint;
            wallRunAbility.capsuleCollider = movement.capsuleCollider;
            wallRunAbility.Init();
        }
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        stateMachine.Init(this, PlayerStateID.None);
        //InitState
        Player_StandState standState = new Player_StandState(PlayerStateID.Stand, stateMachine);
        Player_JumpState jumpState = new Player_JumpState(PlayerStateID.Jump, stateMachine);
        Player_FallState fallState = new Player_FallState(PlayerStateID.Fall, stateMachine);
        Player_ClimbState climbState = new Player_ClimbState(PlayerStateID.Climb, stateMachine);
        Player_WallRunState wallRunState = new Player_WallRunState(PlayerStateID.WallRun, stateMachine);

        //SetupStandState
        stateMachine.AddState(standState);
        standState.AddTransition(PlayerTransitionID.Trigger_Jump, PlayerStateID.Jump);
        standState.AddTransition(PlayerTransitionID.Happen_Fall, PlayerStateID.Fall);
        standState.AddTransition(PlayerTransitionID.Into_Climb, PlayerStateID.Climb);
        standState.AddTransition(PlayerTransitionID.Into_WallRun, PlayerStateID.WallRun);

        

        //SetupJumpState
        stateMachine.AddState(jumpState);
        jumpState.AddTransition(PlayerTransitionID.LandOnGround, PlayerStateID.Stand);
        jumpState.AddTransition(PlayerTransitionID.Happen_Fall, PlayerStateID.Fall);
        jumpState.AddTransition(PlayerTransitionID.Into_Climb, PlayerStateID.Climb);


        //SetupFallState
        stateMachine.AddState(fallState);
        fallState.AddTransition(PlayerTransitionID.LandOnGround, PlayerStateID.Stand);
        fallState.AddTransition(PlayerTransitionID.Into_WallRun, PlayerStateID.WallRun);
        fallState.AddTransition(PlayerTransitionID.Into_Climb,PlayerStateID.Climb);

        //SetupClimbState
        stateMachine.AddState(climbState);
        climbState.AddTransition(PlayerTransitionID.LandOnGround, PlayerStateID.Stand);
        climbState.AddTransition(PlayerTransitionID.Happen_Fall, PlayerStateID.Fall);

        //SetupWallRunState
        stateMachine.AddState(wallRunState);
        wallRunState.AddTransition(PlayerTransitionID.LandOnGround, PlayerStateID.Stand);
        wallRunState.AddTransition(PlayerTransitionID.Happen_Fall, PlayerStateID.Fall);
        

        #region Register State Behaviour Event

        standState.RegisterEvent(StateBehaviourType.Enter, BehaviourOpportunityType.Start, wallRunState.ClearLastLeaveData, DelegateLifetime.AlwaysExist);

        

        #endregion
    }
    private void Update()
    {
        
        
        CheckGround();

        SetMoveAxisDirection(Vector2.one);
        stateMachine.Update();
        
        UpdateRoleDirection();
        UpdateController();

        //TODO special -> player 
        UpdateCoordinateAxis();


    }
    private void FixedUpdate()
    {
        //IsInGround =IsInGroundMethod();
        CalculateMovenum();
        if (movement.enabled)
        {
            movement.OnFixedUpdate();
        }
    }
    private void LateUpdate()
    {
        UpdateAnimatorMotion();
        UpdateState();
    }
    #endregion
    #region Unity_AnimationRuntimeCycle
    protected override void OnAnimatorMove()
    {
        base.OnAnimatorMove();
        if (animatorMachine.CheckAnimationStateByName(0, "Climb Up"))
        {
            movement.rigid.position = animatorMachine.animator.rootPosition;
            return;
        }
        if (animatorMachine.animator.GetFloat("MovementID") == 1)
        {
            if (climbAbility.towardHitInfo.collider)
            {
                AlignPositionInClimbLocomotion();
            }
        }
        else if (animatorMachine.animator.GetFloat("MovementID") == 2)
        {
            if (wallRunAbility.towardHitInfo.collider && Vector3.Angle(transform.forward, -wallRunAbility.towardHitInfo.normal) < 90)
            {
                AlignWallRun();
            }
        }
        else
        {
            if (stateMachine.CurrentStateID == PlayerStateID.Stand || animatorMachine.CheckAnimationStateByName(0, "Lower_ClimbOver", "Higher_ClimbOver"))
            {
                movement.deltaPosition += animatorMachine.animator.deltaPosition;
                movement.deltaRotation *= animatorMachine.animator.deltaRotation;
            }
            else if (animatorMachine.CheckAnimationStateByName(0, "Stand To Climb"))
            {
                movement.deltaPosition += animatorMachine.animator.deltaPosition;
                movement.deltaRotation *= animatorMachine.animator.deltaRotation;
            }
            else if (animatorMachine.CheckAnimationStateByName(0, Const_Animation.BackAir_Roll_End))
            {
                movement.deltaPosition += animatorMachine.animator.deltaPosition;
                movement.deltaRotation *= animatorMachine.animator.deltaRotation;
            }
            else if (stateMachine.CurrentStateID == PlayerStateID.Jump || stateMachine.CurrentStateID == PlayerStateID.Fall)
            {
                if (stateMachine.CurrentStateID == PlayerStateID.Jump&&!animatorMachine.CheckAnimationStateByName(0,"Jump"))
                {
                    return;
                }
                
            }
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!footAndHandIK)
        {
            return;
        }
        MatchIK();
        //L_Hand_IKWeight = animatorMachine.animator.GetFloat("L_Hand_IKWeight");
        //R_Hand_IKWeight = animatorMachine.animator.GetFloat("R_Hand_IKWeight");
        L_Hand_IKWeight = 1;
        R_Hand_IKWeight = 1;
        //animatorMachine.animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(t.forward));
        //animatorMachine.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        if (animatorMachine._IKGoal.IsSelectThisEnumInMult(MatchIKGoal.LeftHand))
        {
            if (leftHandMatchTargetInfo.collider != null)
            {
                animatorMachine.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, L_Hand_IKWeight);
                animatorMachine.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMatchTargetInfo.point + leftHandMatchTargetInfo.normal * 0.05f);
                animatorMachine.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMatchTargetInfo.ikRotation);
                animatorMachine.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, L_Hand_IKWeight);
            }
        }

        if (animatorMachine._IKGoal.IsSelectThisEnumInMult(MatchIKGoal.RightHand))
        {
            if (rightHandMatchTargetInfo.collider != null)
            {
                animatorMachine.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animatorMachine.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMatchTargetInfo.point + rightHandMatchTargetInfo.normal * 0.05f);
                animatorMachine.animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMatchTargetInfo.ikRotation);
                animatorMachine.animator.SetIKRotationWeight(AvatarIKGoal.RightHand, R_Hand_IKWeight);
            }
        }
        if (leftFootIKInfo.collider != null)
        {
            animatorMachine.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animatorMachine.animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKInfo.point + leftFootIKInfo.normal * 0.05f);
            animatorMachine.animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootIKInfo.ikRotation);
            animatorMachine.animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
        }
        if (rightFootIKInfo.collider != null)
        {
            animatorMachine.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animatorMachine.animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKInfo.point + rightFootIKInfo.normal * 0.05f);
            animatorMachine.animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootIKInfo.ikRotation);
            animatorMachine.animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        }
    }
    #endregion
    
   
    public void FallUpdate(bool isDrop)
    {
        if (stateMachine.LastStateID==PlayerStateID.WallRun)
        {
            if (InputController.isInputingMove&&isDrop==false)
            {
                //air move
                movement.deltaPosition += transform.forward*GetFixedMovementInputValueMagnitude(movementSmoothValue)*air_movement_MaxSpeed * Time.deltaTime;
            }
        }
    }
    public void JumpStateEnter()
    {
        ClimbType climbType = climbAbility.CheckLowOrHighClimb();
        //TODO 目前只做了规则形状，对于不规则物体需要多打射线检测目标位置。
        if (climbType == ClimbType.LowerClimbOver)
        {
            SetReadyClimbInfo(ClimbType.LowerClimbOver);
            leftHandPosition=climbAbility.ledge_Hit.point + Vector3.Cross(transform.forward, Vector3.up) * 0.3f; 
        }
        else if (climbType == ClimbType.HigherClimbOver)
        {
            SetReadyClimbInfo(ClimbType.HigherClimbOver);
            rightHandPosition=climbAbility.ledge_Hit.point + Vector3.Cross(-transform.forward, Vector3.up) * 0.3f;
            rightFootPosition = climbAbility.ledge_Hit.point + Vector3.down*1.55f;
        }
        else
        {
            Debug.Log("Jump");
            animatorMachine.animator.ResetTrigger("Jump");
            animatorMachine.animator.SetTrigger("Jump");
            verticalVelocity = Mathf.Sqrt(2 * movement.gravity * jumpMaxHeight);
            currentHorizontalMovenum = jump_BaseMovenumInPanel * GetFixedMovementInputValueMagnitude(movementSmoothValue);
        }
        jumpAfterIsHoldPress = true;
        jumpOnPhysicalStart = false;
    }
    public bool Trigger_Jump()
    {
        bool triggerJump = false;
        if (_inputController.JustInputedJumpThisFrame && IsInGround)
        {
            triggerJump = true;
            if (animatorMachine.CheckNextOrCurrentAnimationStateByName(0, "Running Turn 180", "Run_End","FallEnd"))
            {
                triggerJump = false;
            }
            
        }
        return triggerJump;
    }
    private void HandleJump()
    {
        

    }
    private void UpdateCoordinateAxis()
    {
        baseDir_CoordinateAxis = CameraController.Instance.GetMainCameraPlaneCoordAxis(Vector3.up);
    }
    private void UpdateController()
    {
        HandleJump();
        //if (animatorMachine.CheckAnimationStateByName(0, "Jump"))
        //{
        //    movement.vertical = animatorMachine.animator.GetFloat("Vertical");

        //}
        //if (animatorMachine.CheckAnimationStateByName(0, "Fall"))
        //{
        //    movement.vertical = animatorMachine.animator.GetFloat("Vertical");
        //}
    }

    public void UpdateRoleDirection()
    {
        Vector3 movementAxis = new Vector3(movementValue.x, 0, movementValue.y);
        bool isTurn = false;
        //note 输入投射到基准坐标系
        if (movementAxis != Vector3.zero && isCanCtrlRotate)
        {
            movementAxis = CameraController.Instance.GetMainCameraPlaneCoordAxis(movementAxis, Vector3.up);
            bool isContinueMove = animatorMachine.animator.GetFloat("Movement") > 0.1f && !animatorMachine.CheckAnimationStateByName(0, "Running Turn 180") && !animatorMachine.animator.GetNextAnimatorStateInfo(0).IsName("Running Turn 180");
            if (_inputController.JustInputedMoveThisFrame)
            {
                DebugTool.DebugPrint(string.Format($"JustInputedMoveThisFrame:{movementAxis}"));
                if (isContinueMove)
                {
                    //叉乘计算
                    
                    float movementBetweenAngle = Vector3.Angle(lastMovementAxis, movementAxis);
                    Vector3 betweenCross = Vector3.Cross(lastMovementAxis, movementAxis);
                    if (betweenCross.y < 0)
                    {
                        movementBetweenAngle *= -1;
                    }
                    float abs_angle = Mathf.Abs(movementBetweenAngle);
                    // 這裡注意角度是否需要區分正負
                    animatorMachine.animator.SetFloat("TurnAngle", abs_angle);
                    isTurn = abs_angle > 170;
                    
                    
                }
            }
            if (isContinueMove)
            {
                //TODO 转向需优化 1.角度范围限制+插值处理 2.插入动画
                if (!isTurn)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementAxis), Time.deltaTime * movement_angularSpeed);
                }

                lastMovementAxis = movementAxis;
            }

        }
        bool inputingMovement = (_inputController.isInputingMove);
        animatorMachine.animator.SetBool("isInputingMove", inputingMovement);
        //     ---
    }
    private void CalculateGravity()
    {
        if (stateMachine.CurrentStateID==PlayerStateID.Jump||(stateMachine.CurrentStateID==PlayerStateID.Fall&&movement.rigid.isKinematic == false))
        {
            if (stateMachine.CurrentStateID == PlayerStateID.Jump && jumpOnPhysicalStart == false && animatorMachine.CheckAnimationStateByName(0, "Jump"))
            {
                jumpOnPhysicalStart = true;
            }
            if (!IsInGround)
            {
                if (verticalVelocity <= 0 /*|| !InputController.JustInputedJumpThisFrame*/)
                {
                    verticalVelocity -= movement.gravity * fallMultiply * Time.deltaTime;
                }
                else
                {
                    verticalVelocity -= movement.gravity * Time.deltaTime;
                }
            }
            
        }
        else
        {
            if (!IsInGround)
            {
                //verticalVelocity -= movement.gravity * fallMultiply * Time.deltaTime;
            }
            else
            {
                verticalVelocity = -movement.gravity  * Time.deltaTime;
            }
        }
    }

    private void CalculateMovenum()
    {
        CalculateGravity();
        if (stateMachine.CurrentStateID==PlayerStateID.Jump||(stateMachine.CurrentStateID==PlayerStateID.Fall&&movement.rigid.isKinematic == false))
        {
           
            //maybe is applyRootmotion
            //get movement velocity
            Vector3 moveDirection = transform.forward;
            //calculate friction
            //in air 
            //can't rotate
            float _air_Friction = air_Friction;
            currentHorizontalMovenum = Mathf.MoveTowards(currentHorizontalMovenum, 0, Time.deltaTime * _air_Friction);
            moveDirection = moveDirection * currentHorizontalMovenum;
            movement.deltaPosition += new Vector3(moveDirection.x, verticalVelocity, moveDirection.z)*Time.deltaTime;
        }
    }
    public void UpdateState()
    {
        if (animatorMachine.CheckAnimationStateByName(0, "Idle"))
        {
            animatorMachine.animator.SetFloat("TurnAngle", 0);
        }
        if (animatorMachine.CheckAnimationStateByName(0, "Running Turn 180"))
        {
            animatorMachine.animator.SetFloat("TurnAngle", 0);
            lastContinueMovementPosition = transform.position;
            animatorMachine.animator.SetBool("isRunEndStop", false);
        }
        if (animatorMachine.CheckAnimationStateByName(0, "Run_End"))
        {
            animatorMachine.animator.SetBool("isRunEndStop", false);
            animatorMachine.animator.SetFloat("TurnAngle", 0);
        }
        if (animatorMachine.CheckAnimationStateByName(0, "Jump"))
        {

        }
        if (_inputController.JustInputedMoveThisFrame)
        {
            lastContinueMovementPosition = transform.position;
        }
        if (_inputController.JustReleasedMoveThisFrame)
        {
            if (Vector3.Distance(lastContinueMovementPosition, transform.position) > 5 && Physics.Raycast(transform.position, transform.forward, climbAbility.headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))) == false)
            {
                animatorMachine.animator.SetBool("isRunEndStop", true);
            }
        }
        if (jumpAfterIsHoldPress)
        {
            if (InputController.isInputingJump==false)
            {
                jumpAfterIsHoldPress = false;
            }
        }
    }
    public void UpdateAnimatorMotion()
    {
        if (isCanCtrlMove)
        {
            animatorMachine.animator.SetFloat("Movement", GetFixedMovementInputValueMagnitude(movementValue), 0.1f, Time.deltaTime);
        }
    }

    private void CheckGround()
    {
        if (!IsInGround)
        {
            canFall = !Physics.Raycast(transform.position, Vector3.down, fallHeight, ~(1 << LayerMask.NameToLayer("Character")));
        }
    }
    private void GroundStateChangeAfter(bool state)
    {

        animatorMachine.animator.SetBool("isGround", state);
    }
    public void SetMirrorParameter(bool isMirror)
    {
        isMirrorParameter = isMirror;
        animatorMachine.SetMirrorParameter(isMirror);
    }
    public float GetTowardEndOffsetLength()
    {
        return InputController.isInputingMove ? wallRunAbility.run_characterEndOffset : wallRunAbility.current_characterEndOffset;
    }
}
