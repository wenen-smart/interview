using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public partial class PlayerController:RoleController
{
    [HideInInspector] public ClimbAbility climbAbility;

    [HideInInspector] public WallRunAbility wallRunAbility;
    #region  Climb
    [HideInInspector]
    public ClimbType readyClimbType { get; private set; }
    public ClimbProcessState currentClimbProcessState { get; private set; }
    public void ClearReadyClimbInfo()
    {
        SetReadyClimbInfo(ClimbType.None);
    }
    public void SetReadyClimbInfo(ClimbType climbType)
    {
        readyClimbType = climbType;
    }
    public void ClimbStateEnter()
    {
        currentClimbProcessState = ClimbProcessState.InProcess;
    }
    public void ClimbStateExit()
    {
        currentClimbProcessState = ClimbProcessState.None;
        climbAbility.readyClimbForecastHitinfo = default(BaseRaycastHit);
        ClearAllIKGoalCache();
    }
    public void ClimbUpdate()
    {
        MatchClimbTarget();
        #region CLIMBLocomotion
        if (currentMovementIDParameter == climbMovementParam)
        {

            transform.position = GetTargetPositionInClimb();

            if (Mathf.Abs(movementSmoothValue.y) >= 0.5f)
            {
                animatorMachine.animator.SetFloat("Forward", movementSmoothValue.y, 0.1f, Time.deltaTime);
            }
            else
            {
                animatorMachine.animator.SetFloat("Forward", movementSmoothValue.y/*, 0.1f, Time.deltaTime * 3*/);
            }
            MatchIKTargetInClimbLocomotion();
            if (_inputController.MovementValue != Vector2.zero)
            {
                ClimbLocomotionUpdate();
            }

            if (Mathf.Abs(movementSmoothValue.x) >= 1)
            {
                animatorMachine.animator.SetFloat("Horizontal", movementSmoothValue.x, 0.1f, Time.deltaTime);
            }
            else
            {
                animatorMachine.animator.SetFloat("Horizontal", 0);
            }

            return;
        }
        #endregion
        #region STANDTOCLIMB
        if (currentMovementIDParameter != climbMovementParam)
        {

            if (animatorMachine.animator.GetAnimatorTransitionInfo(0).IsName("Stand To Climb -> MotionTree"))
            {
                currentMovementIDParameter = climbMovementParam;
                animatorMachine.animator.SetFloat("MovementID", currentMovementIDParameter);
            }
            else if (animatorMachine.CheckAnimationStateByName(0, "Stand To Climb"))
            {
                bool turntoRight = isMirrorParameter;
                Vector3 targetHandPosition;
                AvatarTarget handAvatar;
                if (turntoRight) { targetHandPosition = rightHandPosition; handAvatar = AvatarTarget.RightHand; }
                else { targetHandPosition = leftHandPosition; handAvatar = AvatarTarget.LeftHand; }

                animatorMachine.animator.MatchTarget(targetHandPosition + climbAbility.readyClimbForecastHitinfo.normal * 0.1f, Quaternion.identity, handAvatar, new MatchTargetWeightMask(Vector3.one, 0), 0.38f, 0.65f);
                animatorMachine.animator.MatchTarget(transform.position, Quaternion.LookRotation(-climbAbility.readyClimbForecastHitinfo.normal), AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1), 0.3f, 0.68f);
                animatorMachine.animator.MatchTarget(GetTargetPositionInClimb(), Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0), 0.65f, 0.75f);
                return;
            }
        }
        #endregion

        #region CLIMBLTOTOP
        if (GetFixedMovementInputValueMagnitude(movementSmoothValue) < 1 || animatorMachine.CheckAnimationStateByName(0, "Fall", "Idle", "Climb Up"))
        {
            if (animatorMachine.CheckAnimationStateByName(0, "Climb Up") && !animatorMachine.animator.IsInTransition(0))
            {
                if (leftHandMatchTargetInfo.collider)
                {
                    DebugTool.DrawWireSphere(leftHandMatchTargetInfo.point + Vector3.up * 0.15f, 2f, Color.blue, 2, "ClimbTopMatchTarget");
                    animatorMachine.animator.MatchTarget(leftHandMatchTargetInfo.point + Vector3.up * 0.15f, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), 0f, 0.05f);
                    animatorMachine.animator.MatchTarget(leftHandMatchTargetInfo.point + Vector3.up * 0.4f, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0), 0.05f, 0.45f);
                }
                else
                {
                    if (rightHandMatchTargetInfo.collider)
                    {
                        DebugTool.DrawWireSphere(rightHandMatchTargetInfo.point + Vector3.up * 0.15f, 2f, Color.blue, 2);
                        animatorMachine.animator.MatchTarget(rightHandMatchTargetInfo.point + Vector3.up * 0.15f, Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0), 0f, 0.05f);
                    }
                }
            }
            if (animatorMachine.CheckAnimationStateByName(0, "Climb Up") && animatorMachine.animator.IsInTransition(0))
            {
                ClearAllIKGoalCache();
            }
            return;
        }
        #endregion;
    }

    public Vector3 GetTargetPositionInClimb()
    {
        //Test
        if (climbAbility.climbForecastHitinfo.collider)
        {
            var temp = transform.InverseTransformPoint(climbAbility.climbForecastHitinfo.point);
            temp.y = 0;
            temp.x = 0;
            return transform.TransformPoint(temp) - transform.forward * climbAbility.characterEndOffset;
        }
        return transform.position;
    }
    public void ContinueClimbStateEnter()
    {
        climbAbility.climbForecastHitinfo = climbAbility.readyClimbForecastHitinfo;
        //distance between  upward forecast point and  headPoint ;
        //float forecastOffect = Vector3.Magnitude(climbAbility.readyClimbForecastHitinfo.point - HeadPoint.position);
        float forecastOffect = Vector3.Magnitude(climbAbility.readyClimbForecastHitinfo.point - HeadPoint.position);
        Vector3 climbPointNormal = climbAbility.readyClimbForecastHitinfo.normal;
        Vector3 climbPointNormalOnPlane = Vector3.up;
        leftHandPosition = climbAbility.readyClimbForecastHitinfo.point + climbPointNormalOnPlane * 0.375f + Vector3.left * 0.3f;
        rightHandPosition = climbAbility.readyClimbForecastHitinfo.point + climbPointNormalOnPlane * 0.375f + Vector3.right * 0.3f;
        DebugTool.DrawWireSphere(leftHandPosition, 0.2f, Color.yellow, 2, "StandToClimbMatchTarget_LeftHand");
        DebugTool.DrawWireSphere(rightHandPosition, 0.2f, Color.yellow, 2, "StandToClimbMatchTarget_RightHand");
        if (stateMachine.IsMatchLastStateStack(PlayerStateID.Fall, PlayerStateID.WallRun))
        {
            //wallRun->Fall-press jump->transition to climb ->climb 
            //TODO:还需要修改fall状态下检测climb的射线逻辑，当前射线逻辑仅有普通站立状态下。增加一条掉落状态下的射线，差不多在手的位置？
            animatorMachine.animator.CrossFade("Wall_Jump_Cross_End_R", 0.15f);
        }
        else
        {
            animatorMachine.animator.CrossFade("Stand To Climb", 0f);
        }

        //transform.rotation = Quaternion.LookRotation(-climbAbility.readyClimbForecastHitinfo.normal);
        Vector3 cross = Vector3.Cross(transform.forward, -climbAbility.readyClimbForecastHitinfo.normal);
        bool turnToRight = cross.y > 0;
        SetMirrorParameter(turnToRight);
        movement.EnableGravity(false);
        CloseRotateSwitch();
        movement.EnableKinematic(true);
        currentMovementIDParameter = commonMovementParam;
        DebugTool.DrawLine(climbAbility.readyClimbForecastHitinfo.point, climbAbility.readyClimbForecastHitinfo.point + climbAbility.readyClimbForecastHitinfo.normal, Color.green, 10);

    }
    public void LowClimbOverStateEnter()
    {
        CloseRotateSwitch();
        movement.EnableKinematic(true);
        Vector3 fixedBodyDir = ProjectOnSelfXZPlane(-climbAbility.readyClimbForecastHitinfo.normal);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(fixedBodyDir), 100);
    }
    public void HighClimbOverStateEnter()
    {
        CloseRotateSwitch();
        movement.EnableKinematic(true);
        Vector3 fixedBodyDir = ProjectOnSelfXZPlane(-climbAbility.readyClimbForecastHitinfo.normal);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(fixedBodyDir), 100);
    }


    public void AlignPositionInClimbLocomotion()
    {
        //TODO 优化
        transform.position = animatorMachine.animator.rootPosition;
        //movement.deltaPosition += animatorMachine.animator.deltaPosition;
        if (Physics.Raycast(transform.position + transform.up * 0.9f, transform.forward, out RaycastHit hitInfo, climbAbility.headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))) & climbAbility.climbForecastHitinfo.ConvertTo(hitInfo))
        {
            DebugTool.DrawLine(transform.position + transform.up * 0.9f, climbAbility.climbForecastHitinfo.point, Color.green, 0);
            var temp = transform.InverseTransformPoint(climbAbility.climbForecastHitinfo.point);
            temp.y = 0;
            transform.position = transform.TransformPoint(temp) + climbAbility.climbForecastHitinfo.normal * climbAbility.characterEndOffset;
            transform.rotation = Quaternion.LookRotation(-climbAbility.climbForecastHitinfo.normal);

        }
        else
        {
            DebugTool.DrawLine(transform.position + transform.up * 0.9f, transform.position + transform.up * 0.9f + transform.forward * climbAbility.headPointEndOffset, Color.red, 0);
        }
    }

    private void ClimbToPlatform()
    {

    }


    private void MatchClimbTarget()
    {
        //DebugTool.DrawWireSphere(animatorMachine.animator.GetIKPosition(AvatarIKGoal.LeftHand),0.1f,Color.red,0);
        //DebugTool.DrawWireSphere(animatorMachine.animator.GetIKPosition(AvatarIKGoal.RightHand),0.1f,Color.red,0);
        //DebugTool.DrawWireSphere(animatorMachine.animator.GetIKPosition(AvatarIKGoal.LeftFoot),0.1f,Color.red,0);
        //DebugTool.DrawWireSphere(animatorMachine.animator.GetIKPosition(AvatarIKGoal.RightFoot),0.1f,Color.red,0);

        DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftHand).position, 0.1f, Color.red, 0);
        DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightHand).position, 0.1f, Color.red, 0);
        DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, 0.1f, Color.red, 0);
        DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightFoot).position, 0.1f, Color.red, 0);

    }
    private void ClimbLocomotionUpdate()
    {
        //TODO 设置多种climbAbility.climbTowardOffset 方案。可以使用批处理或队列策略。 队列策略更优，若当前方案不可行，将新方案加入队列，待下帧尝试采用。
        if (Mathf.Abs(movementSmoothValue.x) > 0 && Mathf.Abs(movementSmoothValue.y) > 0)
        {
            if (movementSmoothValue.x > 0)
            {
                //R
                if (movementSmoothValue.y > 0)
                {
                    //Up
                }
                else
                {

                }
            }
            else
            {
                //L
                if (movementSmoothValue.y > 0)
                {
                    //Up

                }
                else
                {

                }
            }

        }
        else
        {
            if (movementSmoothValue.x > 0)
            {
                //R
                if (!Physics.Raycast(transform.position + transform.up * 0.45f, transform.right, climbAbility.climbTowardOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                {
                    DebugTool.DrawLine(transform.position + transform.up * 0.45f, transform.position + transform.up * 0.45f + transform.right * climbAbility.climbTowardOffset, Color.yellow, 0);
                    if (Physics.Raycast(transform.position + transform.up * 0.45f + transform.right * climbAbility.climbTowardOffset, transform.forward, out climbAbility.towardHitInfo, climbAbility.headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        //move toward target point

                        DebugTool.DrawLine(climbAbility.towardHitInfo.point, climbAbility.towardHitInfo.point + climbAbility.towardHitInfo.normal * 5, Color.blue, 0);
                        //UnityEditor.EditorApplication.isPaused = true;
                        DebugTool.DrawLine(transform.position + transform.up * 0.45f + transform.right * 0.5f, transform.position + transform.up * 0.45f + transform.right * climbAbility.climbTowardOffset + transform.forward * climbAbility.headPointEndOffset, Color.green, 0);

                    }
                    else
                    {
                        DebugTool.DrawLine(transform.position + transform.up * 0.45f + transform.right * climbAbility.climbTowardOffset, transform.position + transform.up * 0.45f + transform.right * climbAbility.climbTowardOffset + transform.forward * climbAbility.headPointEndOffset, Color.red, 0);
                    }
                }
                else
                {
                    DebugTool.DrawLine(transform.position + transform.up * 0.45f, transform.position + transform.up * 0.45f + transform.right * climbAbility.climbTowardOffset, Color.red, 0);
                }
            }
            else if (movementSmoothValue.x < 0)
            {
                //L
                if (!Physics.Raycast(transform.position + transform.up * 0.45f, -transform.right, climbAbility.climbTowardOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                {
                    DebugTool.DrawLine(transform.position + transform.up * 0.45f, transform.position + transform.up * 0.45f - transform.right * climbAbility.climbTowardOffset, Color.yellow, 0);
                    if (Physics.Raycast(transform.position + transform.up * 0.45f - transform.right * climbAbility.climbTowardOffset, transform.forward, out climbAbility.towardHitInfo, climbAbility.headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        //move toward target point

                        DebugTool.DrawLine(climbAbility.towardHitInfo.point, climbAbility.towardHitInfo.point + climbAbility.towardHitInfo.normal * 5, Color.blue, 0);
                        //UnityEditor.EditorApplication.isPaused = true;
                        DebugTool.DrawLine(transform.position + transform.up * 0.45f - transform.right * climbAbility.climbTowardOffset, transform.position + transform.up * 0.45f - transform.right * climbAbility.climbTowardOffset + transform.forward * climbAbility.headPointEndOffset, Color.green, 0);

                    }
                    else
                    {
                        DebugTool.DrawLine(transform.position + transform.up * 0.45f - transform.right * climbAbility.climbTowardOffset, transform.position + transform.up * 0.45f - transform.right * climbAbility.climbTowardOffset + transform.forward * climbAbility.headPointEndOffset, Color.red, 0);
                    }
                }
                else
                {
                    DebugTool.DrawLine(transform.position + transform.up * 0.45f, transform.position + transform.up * 0.45f - transform.right * climbAbility.climbTowardOffset, Color.red, 0);
                }
            }
            if (movementSmoothValue.y > 0)
            {
                //Up
                if (!Physics.Raycast(HeadPoint.transform.position, transform.up, climbAbility.climbToTopOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                {
                    DebugTool.DrawLine(HeadPoint.transform.position, HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset, Color.yellow, 0);
                    if (Physics.Raycast(HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset, transform.forward, out climbAbility.towardHitInfo, climbAbility.headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        //move toward target point

                        DebugTool.DrawLine(climbAbility.towardHitInfo.point, climbAbility.towardHitInfo.point + climbAbility.towardHitInfo.normal * 5, Color.blue, 0);
                        //UnityEditor.EditorApplication.isPaused = true;
                        DebugTool.DrawLine(HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset, HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset + transform.forward * climbAbility.headPointEndOffset, Color.green, 0);


                    }
                    else
                    {
                        DebugTool.DrawLine(HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset, HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset + transform.forward * climbAbility.headPointEndOffset, Color.red, 0);

                        //TODO 获取平台平面顶点
                        //TODO  检测平面是否可站
                        Vector3 climbUpTargetPos = HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset + transform.forward * climbAbility.climbtoTop_EndOffset /*climbAbility.headPointEndOffset*/;
                        Physics.Raycast(climbUpTargetPos, -transform.up, out climbAbility.climbUpRootHitInfo, characterCollider.height, ~(1 << LayerMask.NameToLayer("Character")));
                        //检测顶部区域是否可站
                        Vector3 capsule_point_1 = climbAbility.climbUpRootHitInfo.point + Vector3.up + Vector3.up * characterCollider.radius;
                        Vector3 capsule_point_2 = climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius);
                        //check hand is collision other collider;
                        bool isCanStandIn = true;
                        if (climbAbility.climbUpRootHitInfo.collider == null)
                        {
                            isCanStandIn = false;
                        }
                        if (isCanStandIn)
                        {
                            isCanStandIn = Physics.RaycastNonAlloc(capsule_point_2, transform.up, null, characterCollider.radius, ~(1 << LayerMask.NameToLayer("Character"))) == 0;
                            //isCanStandIn==false if hand collision collider,auto crouch or jump upward
                        }
                        if (isCanStandIn && Physics.OverlapCapsule(capsule_point_1, capsule_point_2, characterCollider.radius, ~(1 << LayerMask.NameToLayer("Character"))).Length == 0)
                        {
                            DebugTool.DebugPrint("ClimbUp");
                            currentClimbProcessState = ClimbProcessState.ClimbWholeFinish;
                            DebugTool.DrawLine(climbUpTargetPos + Vector3.up * characterCollider.radius, climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius), Color.green, 0.1f);
                            DebugTool.DrawWireSphere(climbUpTargetPos + Vector3.up * characterCollider.radius, characterCollider.radius, Color.green, 0.1f);
                            DebugTool.DrawWireSphere(climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius), characterCollider.radius, Color.green, 0.1f);
                            //Grab Object Edge
                            //distance from hand to head  0.3f

                            float onceRayDelta = 0.1f;
                            RaycastHit _leftHandHitInfo;
                            RaycastHit _rightHandHitInfo;
                            for (int i = 1; i <= 5; i++)
                            {
                                Vector3 origin = transform.InverseTransformPoint(capsule_point_2);
                                origin.z = 0;
                                origin = transform.TransformPoint(origin + Vector3.left * climbAbility.climbToTop_hand_delta) + transform.forward * onceRayDelta * i;
                                DebugTool.DrawLine(origin, origin - Vector3.up * (characterCollider.height), Color.red, 2f);
                                if (Physics.Raycast(origin, -transform.up, out _leftHandHitInfo, characterCollider.height, ~(1 << LayerMask.NameToLayer("Character"))) & leftHandMatchTargetInfo.ConvertTo(_leftHandHitInfo.point - transform.forward * 0.1f, _leftHandHitInfo.normal, _leftHandHitInfo.collider, Quaternion.LookRotation(transform.forward, _leftHandHitInfo.normal)))
                                {
                                    DebugTool.DrawWireSphere(leftHandMatchTargetInfo.point, 0.2f, Color.green, 5f);
                                    break;
                                }
                            }
                            for (int i = 1; i <= 5; i++)
                            {
                                Vector3 origin = transform.InverseTransformPoint(capsule_point_2);
                                origin.z = 0;
                                origin = transform.TransformPoint(origin - Vector3.left * climbAbility.climbToTop_hand_delta) + transform.forward * onceRayDelta * i;
                                DebugTool.DrawLine(origin, origin - Vector3.up * (characterCollider.height), Color.red, 2f);
                                if (Physics.Raycast(origin, -transform.up, out _rightHandHitInfo, characterCollider.height, ~(1 << LayerMask.NameToLayer("Character"))) & rightHandMatchTargetInfo.ConvertTo(_rightHandHitInfo.point - transform.forward * 0.1f, _rightHandHitInfo.normal, _rightHandHitInfo.collider, Quaternion.LookRotation(transform.forward, _rightHandHitInfo.normal)))
                                {
                                    DebugTool.DrawWireSphere(rightHandMatchTargetInfo.point, 0.2f, Color.green, 5f);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            DebugTool.DrawLine(climbUpTargetPos + Vector3.up * characterCollider.radius, climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius), Color.red, 0.1f);
                            DebugTool.DrawWireSphere(climbUpTargetPos + Vector3.up * characterCollider.radius, characterCollider.radius, Color.red, 0.1f);
                            DebugTool.DrawWireSphere(climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius), characterCollider.radius, Color.red, 0.1f);
                        }
                    }
                }
                else
                {
                    DebugTool.DrawLine(HeadPoint.transform.position, HeadPoint.transform.position + transform.up * climbAbility.climbToTopOffset, Color.red, 0);
                }
            }
            else if (movementSmoothValue.y < 0)
            {
                //Down
                if (!Physics.Raycast(transform.position, -transform.up, climbAbility.climbTowardOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                {
                    DebugTool.DrawLine(transform.position, transform.position - transform.up * climbAbility.climbTowardOffset, Color.yellow, 0);
                    if (Physics.Raycast(transform.position - transform.up * climbAbility.climbTowardOffset, transform.forward, out climbAbility.towardHitInfo, climbAbility.headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        //move toward target point
                        DebugTool.DrawLine(climbAbility.towardHitInfo.point, climbAbility.towardHitInfo.point + climbAbility.towardHitInfo.normal * 5, Color.blue, 0);
                        //UnityEditor.EditorApplication.isPaused = true;
                        DebugTool.DrawLine(transform.position - transform.up * climbAbility.climbTowardOffset, transform.position - transform.up * climbAbility.climbTowardOffset + transform.forward * climbAbility.headPointEndOffset, Color.green, 0);

                    }
                    else
                    {
                        DebugTool.DrawLine(transform.position - transform.up * climbAbility.climbTowardOffset, transform.position - transform.up * climbAbility.climbTowardOffset + transform.forward * climbAbility.headPointEndOffset, Color.red, 0);
                        if (movementSmoothValue.y < -0.8f)
                        {
                            currentClimbProcessState = ClimbProcessState.CancelClimb;
                        }
                    }
                }
                else
                {
                    DebugTool.DrawLine(transform.position, transform.position - transform.up * climbAbility.climbTowardOffset, Color.red, 0);
                }
            }
        }
    }
    public void ClimbToIdle()
    {
        ClearAllIKGoalCache();
        currentMovementIDParameter = commonMovementParam;
        animatorMachine.animator.SetFloat("MovementID", currentMovementIDParameter);
        animatorMachine.animator.CrossFade("Idle", 0f);
        movement.EnableGravity(true);
        movement.EnableKinematic(false);
        var temp = transform.InverseTransformPoint(climbAbility.climbForecastHitinfo.point);
        temp.y = 0;
        movement.rigid.position = transform.TransformPoint(temp) + climbAbility.climbForecastHitinfo.normal * (climbAbility.characterEndOffset + 0.2f);
    }
    public void ClimbToFall()
    {
        ClearAllIKGoalCache();
        currentMovementIDParameter = commonMovementParam;
        animatorMachine.animator.SetFloat("MovementID", currentMovementIDParameter);
    }
    public void ClimbUpToTop()
    {
        currentMovementIDParameter = commonMovementParam;
        animatorMachine.animator.SetFloat("MovementID", currentMovementIDParameter);
        animatorMachine.animator.CrossFade("Climb Up", 0f);

    }
    public void ClearAllIKGoalCache()
    {
        animatorMachine.ClearIKGoal();
        leftHandMatchTargetInfo = default(IKInfo);
        rightHandMatchTargetInfo = default(IKInfo);
        //GetComponent<RigBuilder>().layers[0].rig.weight = 0;
    }

    public void MatchFoot()
    {
        animatorMachine.animator.MatchTarget(leftHandIKTargetParent.position, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), 0, 0.69f);

    }
    private void MatchIKTargetInClimbLocomotion(/*MatchIKGoal matchIKGoal*/)
    {
        RaycastHit _leftHandHitInfo;
        RaycastHit _rightHandHitInfo;
        if (animatorMachine._IKGoal.IsSelectThisEnumInMult(MatchIKGoal.LeftHand))
        {
            if (leftHandMatchTargetInfo.collider == null)
            {
                if (Physics.SphereCast(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftHand).position - transform.forward * 0.2f, 0.1f, transform.forward, out _leftHandHitInfo, 0.2f + climbAbility.leftHandDis, ~(1 << LayerMask.NameToLayer("Character"))) & leftHandMatchTargetInfo.ConvertTo(_leftHandHitInfo, Quaternion.LookRotation(transform.up, _leftHandHitInfo.normal)))
                {
                    //GetComponent<RigBuilder>().layers[0].rig.weight = 1;
                }
                else
                {
                    //GetComponent<RigBuilder>().layers[0].rig.weight = 0;
                    DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftHand).position, 0.1f, Color.red, 0);
                }
            }
            else
            {
                DebugTool.DrawWireSphere(leftHandMatchTargetInfo.point, 0.1f, Color.yellow, 0);
            }

        }
        else
        {
            leftHandMatchTargetInfo = default(IKInfo);
        }
        if (animatorMachine._IKGoal.IsSelectThisEnumInMult(MatchIKGoal.RightHand))
        {
            if (rightHandMatchTargetInfo.collider == null)
            {
                if (Physics.SphereCast(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightHand).position - transform.forward * 0.2f, 0.1f, transform.forward, out _rightHandHitInfo, 0.2f + climbAbility.rightHandDis, ~(1 << LayerMask.NameToLayer("Character"))) & rightHandMatchTargetInfo.ConvertTo(_rightHandHitInfo, Quaternion.LookRotation(transform.up, _rightHandHitInfo.normal)))
                {
                    DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightHand).position, 0.1f, Color.green, 0);
                }
                else
                {

                    DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightHand).position, 0.1f, Color.red, 0);
                }
            }
            else
            {
                DebugTool.DrawWireSphere(rightHandMatchTargetInfo.point, 0.1f, Color.yellow, 0);
            }
        }
        else
        {
            rightHandMatchTargetInfo = default(IKInfo);
        }
        //MatchIKTarget(animatorMachine._IKGoal,leftHandMatchTargetInfo.normal);
    }

    //private void MatchIKTarget(MatchIKGoal matchIKGoal,Vector3 dir)
    //{
    //    if (matchIKGoal.IsSelectThisEnumInMult(MatchIKGoal.LeftHand))
    //    {
    //        Transform ikTarget = animatorMachine.t.transform.GetChild(0);
    //        Vector3 ikTargetLocalEuler = ikTarget.localEulerAngles;
    //        ikTarget.SetParent(animatorMachine.t.transform.parent);
    //        animatorMachine.t.transform.rotation = Quaternion.LookRotation(dir);
    //        ikTarget.SetParent(animatorMachine.t.transform);
    //        ikTarget.transform.localEulerAngles = ikTargetLocalEuler;
    //        DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftHand).position, 0.1f, Color.green, 0);
    //    }
    //}
    //private void MatchIKTarget()
    //{

    //}
    private void MatchIK()
    {
        if (!animatorMachine.CheckAnimationStateByName(0, "Idle"))
        {
            leftFootIKInfo = default(IKInfo);
            rightFootIKInfo = default(IKInfo);
            return;
        }
        RaycastHit _leftFootHitInfo;
        RaycastHit _rightFootHitInfo;
        if (Physics.Raycast(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, Vector3.down, out _leftFootHitInfo, 0.4f, ~(1 << LayerMask.NameToLayer("Character"))) & leftFootIKInfo.ConvertTo(_leftFootHitInfo.point + _leftFootHitInfo.normal * 0.05f, _leftFootHitInfo.normal, _leftFootHitInfo.collider, Quaternion.LookRotation(transform.forward, _leftFootHitInfo.normal)))
        {
            DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, 0.1f, Color.green, 0);
        }
        else
        {

            DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, 0.1f, Color.red, 0);
        }

        if (Physics.Raycast(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightFoot).position, Vector3.down, out _rightFootHitInfo, 0.4f, ~(1 << LayerMask.NameToLayer("Character"))) & rightFootIKInfo.ConvertTo(_rightFootHitInfo.point + _rightFootHitInfo.normal * 0.05f, _rightFootHitInfo.normal, _rightFootHitInfo.collider, Quaternion.LookRotation(transform.forward, _rightFootHitInfo.normal)))
        {
            DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightFoot).position, 0.1f, Color.green, 0);
        }
        else
        {

            DebugTool.DrawWireSphere(animatorMachine.animator.GetBoneTransform(HumanBodyBones.RightFoot).position, 0.1f, Color.red, 0);
        }
    }
    #endregion

    #region WallRun
    public void WallRunStateEnter()
    {
        currentClimbProcessState = ClimbProcessState.InProcess;
        currentMovementIDParameter = wallRunMovementParam;
        animatorMachine.animator.SetFloat("MovementID", currentMovementIDParameter);
        animatorMachine.animator.CrossFadeInFixedTime("WallRunMotionTree", 0, 0, 0f);


        movement.EnableGravity(false);
        CloseRotateSwitch();
        movement.EnableKinematic(true);
        wallRunAbility.climbForecastHitinfo = wallRunAbility.readyClimbForecastHitinfo;
        wallRunAbility.current_characterEndOffset = wallRunAbility.hold_characterEndOffset;
        wallRunAbility.lastWall = wallRunAbility.readyClimbForecastHitinfo.collider;
        transform.rotation = Quaternion.LookRotation(-wallRunAbility.climbForecastHitinfo.normal, Vector3.ProjectOnPlane(transform.up, wallRunAbility.climbForecastHitinfo.normal));
        movement.SetCastDirection(-transform.up);
    }
    public void WallRunStateExit()
    {
        currentMovementIDParameter = commonMovementParam;
        currentClimbProcessState = ClimbProcessState.None;
        animatorMachine.animator.SetFloat("MovementID", currentMovementIDParameter);
        movement.EnableGravity(true);
        movement.EnableKinematic(false);
        //animatorMachine.animator.CrossFade("Idle", 0f);
        movement.SetCastDirection(Vector3.down);
        movement.verticalMovement = 0;
        transform.rotation = Quaternion.LookRotation(ProjectOnSelfXZPlane(transform.forward), Vector3.up);
    }

    public void WallRunLocomotionUpdate()
    {
        float target_characterEndOffset = InputController.isInputingMove && wallRunAbility.towardHitInfo.collider ? wallRunAbility.run_characterEndOffset : wallRunAbility.hold_characterEndOffset;
        wallRunAbility.current_characterEndOffset = Mathf.Lerp(wallRunAbility.current_characterEndOffset, target_characterEndOffset, Time.deltaTime * wallRunAbility.c_EndOffset_TransitionSpeed);

        if (wallRunAbility.climbForecastHitinfo.collider)
        {
            var temp = transform.InverseTransformPoint(wallRunAbility.climbForecastHitinfo.point);
            temp.y = 0;
            transform.position = transform.TransformPoint(temp) - transform.forward * (wallRunAbility.current_characterEndOffset - 0.01f);
            if (Mathf.Abs(target_characterEndOffset - wallRunAbility.current_characterEndOffset) > 0.01f)
            {
                if (Physics.Raycast(transform.position + transform.up * wallRunAbility.hold_alignCastHeight, transform.forward, out RaycastHit hitInfo, wallRunAbility.GetForwardcastLen(wallRunAbility.hold_alignCastHeight), ~(1 << LayerMask.NameToLayer("Character"))) & wallRunAbility.climbForecastHitinfo.ConvertTo(hitInfo))
                {

                }
            }
        }
        if (InputController.isInputingMove == false)
        {
            if (IsInGround)
            {
                currentClimbProcessState = ClimbProcessState.CancelClimb;
                return;
            }
            if (wallRunAbility.autofall && movementSmoothValue == Vector2.zero)
            {
                //nature todown force
                CloseKinematic();
                movement.verticalMovement -= 2 * Time.deltaTime;
            }
            if (wallRunAbility.towardHitInfo.collider != null)
            {
                wallRunAbility.towardHitInfo = default(RaycastHit);
                return;
            }
        }
        else
        {
            if (movement.rigid.isKinematic == false)
            {
                OpenKinematic();
                movement.verticalMovement = 0;
            }
        }
        DebugTool.DrawWireSphere(wallRunAbility.climbForecastHitinfo.point, 0.1f, Color.red, 1, wallRunAbility.lastWall.gameObject.name);
        //from headpoint cast a ray
        if (Mathf.Abs(movementSmoothValue.x) > 0 && Mathf.Abs(movementSmoothValue.y) > 0)
        {
            if (movementSmoothValue.x > 0)
            {
                //R
                if (movementSmoothValue.y > 0)
                {
                    //Up
                }
                else
                {

                }
            }
            else
            {
                //L
                if (movementSmoothValue.y > 0)
                {
                    //Up

                }
                else
                {

                }
            }

        }
        else
        {
            if (movementSmoothValue.x > 0)
            {
                //R
                if (!Physics.Raycast(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight, transform.right, wallRunAbility.climbTowardOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                {
                    DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight, transform.position + transform.up * 0.45f + transform.right * wallRunAbility.climbTowardOffset, Color.yellow, 0);
                    if (Physics.Raycast(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight + transform.right * wallRunAbility.climbTowardOffset, transform.forward, out wallRunAbility.towardHitInfo, GetTowardEndOffsetLength(), ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        //move toward target point

                        DebugTool.DrawLine(wallRunAbility.towardHitInfo.point, wallRunAbility.towardHitInfo.point + wallRunAbility.towardHitInfo.normal * 5, Color.blue, 0);
                        //UnityEditor.EditorApplication.isPaused = true;
                        DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight + transform.right * 0.5f, transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight + transform.right * wallRunAbility.climbTowardOffset + transform.forward * GetTowardEndOffsetLength(), Color.green, 0);

                    }
                    else
                    {
                        DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight + transform.right * wallRunAbility.climbTowardOffset, transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight + transform.right * wallRunAbility.climbTowardOffset + transform.forward * GetTowardEndOffsetLength(), Color.red, 0);
                    }
                }
                else
                {
                    DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight, transform.position + transform.up * 0.45f + transform.right * wallRunAbility.climbTowardOffset, Color.red, 0);
                }
            }
            else if (movementSmoothValue.x < 0)
            {
                //L
                if (!Physics.Raycast(transform.position + transform.up */*0.45f*/ wallRunAbility.run_alignCastHeight, -transform.right, wallRunAbility.climbTowardOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                {
                    DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight, transform.position + transform.up * 0.45f - transform.right * wallRunAbility.climbTowardOffset, Color.yellow, 0);
                    if (Physics.Raycast(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight - transform.right * wallRunAbility.climbTowardOffset, transform.forward, out wallRunAbility.towardHitInfo, GetTowardEndOffsetLength(), ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        //move toward target point

                        DebugTool.DrawLine(wallRunAbility.towardHitInfo.point, wallRunAbility.towardHitInfo.point + wallRunAbility.towardHitInfo.normal * 5, Color.blue, 0);
                        //UnityEditor.EditorApplication.isPaused = true;
                        DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight - transform.right * wallRunAbility.climbTowardOffset, transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight - transform.right * wallRunAbility.climbTowardOffset + transform.forward * GetTowardEndOffsetLength(), Color.green, 0);

                    }
                    else
                    {
                        DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight - transform.right * wallRunAbility.climbTowardOffset, transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight - transform.right * wallRunAbility.climbTowardOffset + transform.forward * GetTowardEndOffsetLength(), Color.red, 0);
                    }
                }
                else
                {
                    DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight, transform.position + transform.up * /*0.45f*/ wallRunAbility.run_alignCastHeight - transform.right * wallRunAbility.climbTowardOffset, Color.red, 0);
                }
            }
            if (movementSmoothValue.y > 0)
            {
                //Up
                Vector3 headPoint = transform.position + transform.up * wallRunAbility.run_alignCastHeight;
                if (!Physics.Raycast(headPoint, transform.up, wallRunAbility.climbToTopOffset, ~(1 << LayerMask.NameToLayer("Character"))))
                {
                    DebugTool.DrawLine(headPoint, headPoint + transform.up * wallRunAbility.climbToTopOffset, Color.yellow, 0);
                    float forwardLen = wallRunAbility.headPointEndOffset + wallRunAbility.GetForwardcastLen(wallRunAbility.climbToTopOffset);
                    if (Physics.Raycast(headPoint + transform.up * wallRunAbility.climbToTopOffset, transform.forward, out wallRunAbility.towardHitInfo, forwardLen, ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        //move toward target point

                        DebugTool.DrawLine(wallRunAbility.towardHitInfo.point, wallRunAbility.towardHitInfo.point + wallRunAbility.towardHitInfo.normal * 5, Color.blue, 0);
                        //UnityEditor.EditorApplication.isPaused = true;
                        DebugTool.DrawLine(headPoint + transform.up * wallRunAbility.climbToTopOffset, headPoint + transform.up * wallRunAbility.climbToTopOffset + transform.forward * forwardLen, Color.green, 0);


                    }
                    else
                    {

                        DebugTool.DrawLine(headPoint + transform.up * wallRunAbility.climbToTopOffset, HeadPoint.transform.position + transform.up * wallRunAbility.climbToTopOffset + transform.forward * forwardLen, Color.red, 0);

                        //TODO 获取平台平面顶点
                        //TODO  检测平面是否可站
                        Vector3 climbUpTargetPos = HeadPoint.transform.position + transform.up * wallRunAbility.climbToTopOffset + transform.forward * wallRunAbility.climbtoTop_EndOffset /*wallRunAbility.headPointEndOffset*/;
                        Physics.Raycast(climbUpTargetPos, -transform.up, out wallRunAbility.climbUpRootHitInfo, characterCollider.height, ~(1 << LayerMask.NameToLayer("Character")));
                        //检测顶部区域是否可站
                        Vector3 capsule_point_1 = wallRunAbility.climbUpRootHitInfo.point + Vector3.up + Vector3.up * characterCollider.radius;
                        Vector3 capsule_point_2 = climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius);
                        //check hand is collision other collider;
                        bool isCanStandIn = true;
                        if (wallRunAbility.climbUpRootHitInfo.collider == null)
                        {
                            isCanStandIn = false;
                        }
                        if (isCanStandIn)
                        {
                            isCanStandIn = Physics.RaycastNonAlloc(capsule_point_2, transform.up, null, characterCollider.radius, ~(1 << LayerMask.NameToLayer("Character"))) == 0;
                            //isCanStandIn==false if hand collision collider,auto crouch or jump upward
                        }
                        if (isCanStandIn && Physics.OverlapCapsule(capsule_point_1, capsule_point_2, characterCollider.radius, ~(1 << LayerMask.NameToLayer("Character"))).Length == 0)
                        {

                        }
                        else
                        {
                            DebugTool.DrawLine(climbUpTargetPos + Vector3.up * characterCollider.radius, climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius), Color.red, 0.1f);
                            DebugTool.DrawWireSphere(climbUpTargetPos + Vector3.up * characterCollider.radius, characterCollider.radius, Color.red, 0.1f);
                            DebugTool.DrawWireSphere(climbUpTargetPos + Vector3.up * (characterCollider.height - characterCollider.radius), characterCollider.radius, Color.red, 0.1f);
                        }
                    }
                }
                else
                {
                    DebugTool.DrawLine(headPoint, headPoint + transform.up * wallRunAbility.climbToTopOffset, Color.red, 0);
                }
            }
            else if (movementSmoothValue.y < 0)
            {
                //Down

            }
        }
    }
    public void WallRunToStand()
    {
        currentMovementIDParameter = commonMovementParam;
        animatorMachine.animator.SetFloat("MovementID", currentMovementIDParameter);
    }
    public void AlignWallRun()
    {
        if (stateMachine.CurrentStateID != PlayerStateID.WallRun)
        {
            return;
        }

        Player_WallRunState player_WallRunState = (Player_WallRunState)stateMachine.GetCurrentState();

        var rootPosition = animatorMachine.GetRootPosition(RootMotionProcessClear.ClearZ);
        var offset = rootPosition - transform.position;
        offset.y = 0;
        //局部 y z是一样的。则offset大小表示 x的长度
        //防止运动靠近边缘时 坐标超出边缘。
        var localOffset = transform.InverseTransformVector(offset);

        if (movementValue.x > 0)
        {
            if (localOffset.x < 0)
            {
                return;
            }
        }
        if (movementValue.x < 0)
        {
            if (localOffset.x > 0)
            {
                return;
            }
        }
        transform.position = rootPosition;
        if (Physics.Raycast(transform.position + transform.up * wallRunAbility.run_alignCastHeight, transform.forward, out RaycastHit hitInfo, wallRunAbility.headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))) & wallRunAbility.climbForecastHitinfo.ConvertTo(hitInfo))
        {
            Vector3 pos = transform.position;
            //TODO:judge angle
            if (Vector3.Angle(transform.forward, -hitInfo.normal) >= 90)
            {
                return;
            }
            var hitNormalOntoPlane = Vector3.ProjectOnPlane(hitInfo.normal, Vector3.up);
            //DebugTool.DrawWireSphere(hitInfo.point+hitInfo.normal,0.2f,Color.green,10,"hitNormal");
            //DebugTool.DrawLine(hitInfo.point,hitInfo.point+hitInfo.normal,Color.green,10);
            //DebugTool.DrawLine(hitInfo.point,hitInfo.point+hitNormalOntoPlane,Color.red,10);
            //DebugTool.DebugLogger(MyDebugType.Print,string.Format($"hitNormalOntoZ_Magnitude:{hitNormalOntoPlane.magnitude}"));
            //DebugTool.DebugLogger(MyDebugType.Print,string.Format($"maxangle_Magnitude:{Mathf.Cos(Mathf.Deg2Rad*65)}"));
            if (hitNormalOntoPlane.magnitude <= Mathf.Cos(Mathf.Deg2Rad * 65))
            {
                //    //to stand
                //judge down or up
                if (Vector3.Angle(hitInfo.normal, Vector3.up) > 90)
                {
                    //down
                    var rotation = transform.rotation;
                    stateMachine.PerformTransition(PlayerStateID.WallRun, PlayerTransitionID.Happen_Fall,
                        new FSMStateBehaviourEventMembers(StateDirection.Next, StateBehaviourType.Enter, BehaviourOpportunityType.End, BackAirRollWhenWallRunDrop, DelegateLifetime.ExecuteAfterImmDie),
                        new FSMStateBehaviourEventMembers(StateDirection.Current, StateBehaviourType.Leave, BehaviourOpportunityType.End, () => { transform.rotation = rotation; }, DelegateLifetime.ExecuteAfterImmDie)
                        )
                        ;
                }
                else
                {
                    //up
                    animatorMachine.animator.CrossFade("MotionTree", 0.1f, 0);
                    transform.rotation = Quaternion.LookRotation(ProjectOnSelfXZPlane(transform.forward), Vector3.up);
                    WallRunToStand();
                }

                DebugTool.DebugLogger(MyDebugType.Print, "Over  Angle of wallrun");
                return;
            }
            Vector3 hitNormalInPlane = ProjectOnSelfXZPlane(hitInfo.normal);
            DebugTool.DrawWireSphere(hitInfo.point, 0.2f, Color.blue, 10, "A");
            DebugTool.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal, Color.red, 10);
            transform.rotation = Quaternion.LookRotation(-hitInfo.normal, Vector3.ProjectOnPlane(transform.up, hitInfo.normal));
            var temp = transform.InverseTransformPoint(hitInfo.point);
            temp.y = 0;
            transform.position = transform.TransformPoint(temp) + hitInfo.normal * (wallRunAbility.current_characterEndOffset - 0.01f);
        }
        movement.SetCastDirection(-transform.up);
    }

    public void BackAirRollWhenWallRunDrop()
    {
        if (stateMachine.LastStateID == PlayerStateID.WallRun)
        {

            var fallState = (Player_FallState)stateMachine.GetCurrentState();
            fallState.HappenDrop();
            //cast ray  down 
            DebugTool.DrawWireSphere(transform.position, 0.2f, Color.green, 10, "player");
            DebugTool.DrawLine(transform.position, transform.position + transform.up * movement.centerHeight, Color.green, 10);
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitifo1, 6, ~(1 << LayerMask.NameToLayer("Character")), QueryTriggerInteraction.Ignore))
            {
                if (hitifo1.collider != null)
                {
                    CloseMoveAndDirectionSwitch();
                    OpenKinematic();
                    movement.EnableGravity(false);
                    targetRootPosition = hitifo1.point;
                    animatorMachine.animator.CrossFade(Const_Animation.BackAir_Roll_End, 0f, 0, 0);
                    animatorMachine.animator.SetBool("Happen_Fall", false);
                    DebugTool.DebugPrint("BackAirRollWhenWallRunDrop");
                    DebugTool.DrawWireSphere(hitifo1.point, 0.2f, Color.green, 10, "BackAirRollWhenWallRunDrop");
                }
            }
            else
            {
                animatorMachine.animator.SetBool("Happen_Fall", true);
            }


        }
    }

    #endregion
}

