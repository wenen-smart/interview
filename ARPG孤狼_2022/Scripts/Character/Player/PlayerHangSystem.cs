using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;

public class PlayerHangSystem : ActorComponent
{
    public CharacterAnim playerAnim;
    public PlayerController playerController;
    public PlayerStateManager stateManager;
    public TwoBoneIKConstraint lefthangUpRig;
    public bool takeHand = false;
    public float blendSpeed = 1f;
    private ActorPhyiscal actorPhyiscal;
    [Header("如果要修改half，记得调整canHangVaildDeltaHeight的值")]
    public Vector3 halfExtent;
    public Collider hangCollider;
    public float canHangVaildDeltaHeight = 0.2f;
    public bool vaildHang = false;
    public Vector3[] checkHangVaildOffsetLocal = new Vector3[] { Vector3.zero, Vector3.right * 0.1f, -Vector3.right * 0.1f, Vector3.forward * 0.1f, Vector3.right * 0.2f, -Vector3.right * 0.2f, Vector3.forward * 0.2f };
    public Vector3 currentVaildOffsetLocal;
    public float closetHeadTop_Dis = 0.289772f;
    public Vector3 point;
    public Vector3 leftHandPoint;
    public Vector3 rightHandPoint;
    public float slideToPointDis = 1;
    public Vector3 bodyPoint;
    public Vector3 pointNormal;
    public Action hangUpPlatfromFinish;
    public Action arriveHighPointHandler;
    public bool arriveHigheTaskIsExcute=false;

    #region ClimbLadder
    private bool vaildClimbLadder;
    public bool rightHandClimbLadder;
    public Vector3 rightFeetPoint;
    public Vector3 leftFeetPoint;
    public float toLadderZDis=0.5f;//z轴方向的距离  对齐梯子
    public int currentLadderStageIndex = 0;
    public Ladder currentLadder;
    public int lastCLHash;
    #endregion
    public override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        playerAnim = PlayerFacade.Instance.characterAnim;
        actorPhyiscal = PlayerFacade.Instance.actorPhyiscal;
        stateManager = PlayerFacade.Instance.playerStateManager as PlayerStateManager;
        playerAnim.RegisterReceiveIK(this);
        playerController = PlayerFacade.Instance.playerController;
        //playerAnim.RegisterReceiveIK(this);
    }
    public void HandlerArriveHighPoint()
    {
        arriveHighPointHandler?.Invoke();
        arriveHigheTaskIsExcute = true;
    }
    public bool CheckHangLadder()
    {
        if (playerController.stateManager.CheckState(ActorStateFlag.allowClimbLadder.ToString())==false)
        {
            if (currentLadder)
            {
                currentLadder = null;
                vaildClimbLadder = false;
                GameRoot.Instance.LeaveHandlerMainEvent(EventCode.ClimbUpLadder);
            }
           
            return false;
        }
        var origin = actorPhyiscal.GetCheckPoint(CheckPointType.HeadTop).position + Vector3.up * closetHeadTop_Dis + transform.forward * PlayerableConfig.Instance.headTopForward_Offset;
        Collider[] colliders = Physics.OverlapBox(origin, halfExtent, Quaternion.LookRotation(transform.forward), LayerMask.GetMask("Ladder"));
        bool findLadder = false;
        if (colliders != null && colliders.Length > 0)
        {
            Collider ladderCollider = transform.FindBestClosetT<Collider>(colliders);
            if (ladderCollider != null)
            {
                Ladder ladder = ladderCollider.GetComponent<Ladder>();
                if (ladder != null)
                {
                    if (vaildClimbLadder==false)
                    {
                        vaildClimbLadder = true;
                        currentLadder = ladder;
                        GameRoot.Instance.SignHandlerMainEvent(new EventInfo("爬楼梯", EventCode.ClimbUpLadder, KeyBehaviourType.RoleBehaviour), () => { return HandleStartLadder(ladder); });
                        return true;
                    }
                    findLadder = true;
                }
            }
        }
        if (findLadder==false)
        {
            if (currentLadder)
            {
                currentLadder = null;
                vaildClimbLadder = false;
                GameRoot.Instance.LeaveHandlerMainEvent(EventCode.ClimbUpLadder);
            }
        }
        return findLadder;
    }
    public bool HandleStartLadder(Ladder ladder)
    {
        LadderStagePointData ladderStagePointData = ladder.GetUpStartPointData();
        //先右再左。
        rightHandClimbLadder = true;
        leftFeetPoint = transform.position;
        rightFeetPoint = ladderStagePointData.rightPoint;
        //leftFeetPoint = ladder.GetStagePointData(ladderStagePointData.stageIndex + 1).LeftPoint;
        rightHandPoint = ladder.GetStagePointData(ladderStagePointData.stageIndex+2).rightPoint;
        leftHandPoint = ladder.GetStagePointData(ladderStagePointData.stageIndex + 1).LeftPoint;
        playerAnim.OpenHandAndFeetIK();
        currentLadderStageIndex = ladderStagePointData.stageIndex + 2;
        //同手同脚间相差2否则3格  两手间距一格 右手开始-》手放置第三格 脚为第一格
        playerController.ExChangeMoveMode(ActorMoveMode.ClimbLadder);
        playerController.DisableCtrl(true);
        playerController.CtrlEnableGravity(false);
        playerController.SetCharacterColliderEnable(false);
        playerAnim.SetInt(AnimatorParameter.HangAction.ToString(),10);
        transform.rotation = Quaternion.LookRotation(ladder.transform.forward);
        transform.position = (ladder.transform.position - ladder.transform.forward*toLadderZDis);
        GameRoot.Instance.LeaveHandlerMainEvent(EventCode.ClimbUpLadder);
        arriveHigheTaskIsExcute = false;
        return true;
    }
    public void ClimbLadderOnUpdate()
    {
        CalcuateClimbLadderMove();
    }
    public void HangToPlatform()
    {
        PlayerFacade.Instance.HandleHangToClimb();
        GameRoot.Instance.Leave_Key_HandlerEvent(KeyCode.Space, EventCode.HangDown);
        GameRoot.Instance.LeaveHandlerMainEvent(EventCode.HangToClimb);
    }
    public void ClimbLadderToPlaform()
    {
        playerAnim.SetInt(AnimatorParameter.HangAction.ToString(), 13);
        playerAnim.CloseAllFeetIK();
        playerAnim.CloseAllHandIK();
        rightFeetPoint = Vector3.zero;
        leftFeetPoint = Vector3.zero;
        leftHandPoint = Vector3.zero;
        rightHandPoint = Vector3.zero;
        currentLadder = null;
        currentLadderStageIndex = 0;
        lastCLHash = 0;
        vaildClimbLadder = false;
    }
    private void CalcuateClimbLadderMove()
    {
        if (currentLadder==null)
        {
            return;
        }
        float vert = InputSystem.Instance.inputDir.y;
        leftPointNormal = Vector3.up;
        rightPointNormal = Vector3.up;
        bool canCtrl = false;
        AnimatorStateInfo stateInfo = playerAnim.anim.GetCurrentAnimatorStateInfo(0);
        if ((stateInfo.IsName("LeftHandClimbing")||stateInfo.IsName("RightHandClimbing")||stateInfo.IsName("ClimbingLadder")))
        {
            if (stateInfo.normalizedTime > 1f&&stateInfo.shortNameHash!=lastCLHash)
            {
                canCtrl = true;
            }
            else
            {
                if (stateInfo.normalizedTime <= 1f)
                {
                    
                    var targetPosition = transform.position;
                    //targetPosition.y = (rightHandClimbLadder ? rightFeetPoint : leftFeetPoint).y;
                    //上面代码逻辑是对的，但是游戏中 原点与前一个脚跟接近，所以这边要改成
                    targetPosition.y = (rightHandClimbLadder ? leftFeetPoint : rightFeetPoint).y;
                    float normalizedTime = Mathf.Repeat(stateInfo.normalizedTime, 1);
                    //这边逻辑错误，因为在此时执行之前，left与right点数据都更新了，而我应该获取的是上一次人物的位置。反推
                    //var lastfeet = rightHandClimbLadder ? leftFeetPoint : rightFeetPoint;
                    var lastfeetY = targetPosition.y-Mathf.Abs(leftFeetPoint.y-rightFeetPoint.y);//反推 需要保证每阶段梯格间距一样
                    var lastFeetPos = transform.position;
                    lastFeetPos.y = lastfeetY;
                   
                    transform.position = Vector3.Lerp(lastFeetPos,targetPosition, normalizedTime*1.5f);
                    Debug.Log($"PositionY:{transform.position.y} leftFeet:{leftFeetPoint.y} rightFeet:{rightFeetPoint.y} normalizedTime:{normalizedTime} targetPosition:{targetPosition} lastFeetPos{lastFeetPos}");
                }
            }
        }
        if (canCtrl)
        {
            if (vert != 0)
            {
                bool arriveHighPoint = false;
                LadderStagePointData lastLadderStagePointData = currentLadder.GetStagePointData(currentLadderStageIndex);
                //上次碰到最高的梯子index，无论是哪只手  
                if (rightHandClimbLadder == false)
                {
                    //上次是左手在上，这次变化右手与右脚
                    var rightPointData = currentLadder.GetStagePointData(lastLadderStagePointData.stageIndex + 1);
                    if (rightPointData.stageIndex != -1)
                    {
                        rightHandPoint = rightPointData.rightPoint;
                        rightFeetPoint = currentLadder.GetStagePointData(rightPointData.stageIndex - 2).rightPoint;
                        currentLadderStageIndex = rightPointData.stageIndex;
                    }
                    else
                    {
                        Debug.Log("到达最高点");
                        arriveHighPoint = true;
                    }
                }
                else
                {
                    var leftPointData = currentLadder.GetStagePointData(lastLadderStagePointData.stageIndex + 1);
                    if (leftPointData.stageIndex != -1)
                    {
                        leftHandPoint = leftPointData.LeftPoint;
                        leftFeetPoint = currentLadder.GetStagePointData(leftPointData.stageIndex - 2).LeftPoint;
                        currentLadderStageIndex = leftPointData.stageIndex;
                    }
                    else
                    {
                        Debug.Log("到达最高点");
                        arriveHighPoint = true;
                    }
                }

                //同手同脚间相差2格  两手间距一格 右手开始-》手放置第三格 脚为第一格
                if (arriveHighPoint == false)
                {
                    lastCLHash=stateInfo.shortNameHash;
                    rightHandClimbLadder = !rightHandClimbLadder;
                    playerAnim.anim.SetFloat(AnimatorParameter.AnimSpeed.ToString(),1);
                    if (rightHandClimbLadder)
                    {
                        playerAnim.SetInt(AnimatorParameter.HangAction.ToString(), 12);
                    }
                    else
                    {
                        playerAnim.SetInt(AnimatorParameter.HangAction.ToString(), 11);
                    }

                }
                else
                {
                    ClimbLadderToPlaform();
                }
            }
            else
            {
                //if (bodyPoint==Vector3.zero)
                //{
                //    Debug.Log("BodyPointZero");
                //}
                playerController.InputKeyUpWhenMove();
            }

        }
    }
    public bool IsArriveLadderBestHigh()
    {
        if (currentLadder==null)
        {
            return false;
        }
        bool arriveHighPoint = false;
        LadderStagePointData lastLadderStagePointData = currentLadder.GetStagePointData(currentLadderStageIndex);
        //上次碰到最高的梯子index，无论是哪只手  
        var rightPointData = currentLadder.GetStagePointData(lastLadderStagePointData.stageIndex + 2);
        if (rightPointData.stageIndex == -1)
        {
            arriveHighPoint = true;
        }
        return arriveHighPoint;
    }
    // Update is called once per frame
   public void OnUpdate()
    {
        if (playerController.moveMode == (ActorMoveMode.Shimmy))
        {
            CalcuateMove();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            takeHand = !takeHand;
            var target = takeHand ? 1 : 0;
            StartCoroutine(RigBlend(target, blendSpeed));
        }
       
    }
    public void HangCheckUpdate()
    {
        if (stateManager.allowHangCheck)
        {

            if (InputSystem.Instance.key_E_MainEvent.OnPressed)
            {
                StopAllCoroutines();
                StartCoroutine(RigBlend(1, blendSpeed));
            }
            var origin = actorPhyiscal.GetCheckPoint(CheckPointType.HeadTop).position + Vector3.up * closetHeadTop_Dis + transform.forward * PlayerableConfig.Instance.headTopForward_Offset;
            Collider[] colliders = Physics.OverlapBox(origin, halfExtent, Quaternion.LookRotation(transform.forward), PlayerableConfig.Instance.allowUpWallLayerMask);
            if (colliders != null && colliders.Length > 0)
            {
                hangCollider = transform.FindBestClosetT<Collider>(colliders);
                if (hangCollider != null)
                {
                    float outDis;
                    bool allNoVaild = true;
                    //Bounds bounds=new Bounds(Vector3.zero,Vector3.zero);
                    //if (hangCollider is MeshCollider)
                    //{
                    //    MeshCollider meshCollider = hangCollider as MeshCollider;
                    //    MeshRenderer meshFilter = meshCollider.GetComponent<MeshRenderer>();
                    //    if (meshFilter)
                    //    {
                    //        bounds = meshFilter.bounds;

                    //    }

                    //}
                    //if (bounds.size == Vector3.zero)
                    //{
                    //    bounds = hangCollider.bounds;
                    //}
                    foreach (var item in checkHangVaildOffsetLocal)
                    {
                        Color debuColor = Color.red;
                        var worldOrigin = origin + transform.TransformVector(item);
                        Ray ray = new Ray { origin = worldOrigin, direction = Vector3.down };



                        RaycastHit hitInfo;

                        if (Physics.Raycast(ray, out hitInfo, canHangVaildDeltaHeight, PlayerableConfig.Instance.allowHangWallLayerMask.value, QueryTriggerInteraction.UseGlobal))
                        {
                            if (vaildHang == false)
                            {

                                //UnityEditor.EditorApplication.isPaused = true;
                                if (stateManager.isShimmy == false)
                                {
                                    GameRoot.Instance.SignHandlerMainEvent(new EventInfo { eventTips = "Will hang Collider", eventCode = EventCode.HangUp,keyBehaviourType=KeyBehaviourType.RoleBehaviour }, () => { return HandleHang(hitInfo.point); });

                                }

                                vaildHang = true;
                                UpdatePoint(hitInfo.point);
                                currentVaildOffsetLocal = item;
                                allNoVaild = false;
                                Debug.Log(Time.time);
                                debuColor = Color.green;

                                Debug.DrawLine(ray.origin, ray.origin + Vector3.down * canHangVaildDeltaHeight, debuColor, 2);
                                break;
                            }

                        }

                        Debug.DrawLine(ray.origin, ray.origin + Vector3.down * canHangVaildDeltaHeight, debuColor, 2);

                    }
                    if (allNoVaild)
                    {

                    }


                }

            }
            else
            {
                if (hangCollider != null)
                {
                    hangCollider = null;
                    GameRoot.Instance.LeaveHandlerMainEvent(EventCode.HangUp);
                    Debug.Log("NoVaild " + Time.time);
                }
                vaildHang = false;
            }
        }
        else
        {
            if (hangCollider != null)
            {
                hangCollider = null;
                GameRoot.Instance.LeaveHandlerMainEvent(EventCode.HangUp);
                Debug.Log("NoVaild " + Time.time);
                vaildHang = false;
            }
        }
        Check_CastToDown_HangUp();
    }

    public float align_DirOffestDis = 2;
    public float align_UpOffssetDis = 0.25f;
    public float align_sphereCheckRadius = 0.2f;
    public float algin_sphereCheckDis = 2;
    public Vector3 leftPointNormal;
    public Vector3 rightPointNormal;
    public bool HandleHang(Vector3 hitInfo)
    {
        if (vaildHang)
        {
            //transform.position = bodyPoint;
            PlayerFacade.Instance.HandleHangUpWall();
            UpdatePoint(hitInfo);
            RigBlend(0, 10);
            playerAnim.SetInt(AnimatorParameter.HangAction.ToString(), 1);
            arriveHigheTaskIsExcute = false;
            //HangDown

            //HangToClimb
            Debug.Log("上墙成功");
        }
        else
        {
            Debug.Log("上墙失败");
        }
        return true;
    }

    public void UpdatePoint(Vector3 hitInfo)
    {
        if (playerController.moveMode != ActorMoveMode.Shimmy)
        {
            return;
        }

        RaycastHit raycastHit;
        var dir = (hitInfo - transform.position).normalized;
        dir.y = 0;
        var origin = hitInfo - dir * align_DirOffestDis - Vector3.up * align_UpOffssetDis;
        Debug.DrawLine(origin, origin + dir, Color.green, 100);
        if (Physics.SphereCast(origin, align_sphereCheckRadius, dir, out raycastHit, algin_sphereCheckDis, PlayerableConfig.Instance.allowUpWallLayerMask))
        {
            pointNormal = raycastHit.normal;
            Quaternion newQuaternion = Quaternion.LookRotation(-pointNormal);
            if (Vector3.Angle(-pointNormal,Vector3.up)<45)
            {
                return;
            }
            if (Vector3.Angle(-pointNormal, Vector3.down) < 45)
            {
                return;
            }
            if (Quaternion.Angle(newQuaternion, transform.rotation) > 90)
            {
                return;
            }
            transform.rotation = newQuaternion;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            var hangPoint = raycastHit.point;
            hangPoint.y = hitInfo.y;


            point = hangPoint;
            CalcuateCharacterPosByHandPoint(point);

            transform.position = bodyPoint;


            RaycastHit leftHitInfo;
            GetPoint(hitInfo, -1, out leftHitInfo);
            if (leftHitInfo.collider != null)
            {
                leftHandPoint = leftHitInfo.point;
                leftHandPoint.y = point.y;
                leftPointNormal = leftHitInfo.normal;

            }
            else
            {
                leftHandPoint = point;
            }
            RaycastHit rightHitInfo;
            GetPoint(hitInfo, 1, out rightHitInfo);
            if (rightHitInfo.collider != null)
            {
                rightHandPoint = rightHitInfo.point;
                rightHandPoint.y = point.y;
                rightPointNormal = rightHitInfo.normal;
            }
            else
            {
                rightHandPoint = point;
            }


        }

    }

    //public float slide_DirOffestDis = 1;
    public float slide_UpOffsetDis = 0.2f;
    public float slide_forwardOffset = 1;
    public float[] slide_angleOffsets = {45,20};
    public float slide_checkDis = 1;


    public void StandToHangGetPoint(Vector3 checkDir, int dir, out RaycastHit hitInfo, Vector3 originOffset = default(Vector3))
    {
        var leftDir = checkDir.normalized;

        leftDir.y = 0;
            hitInfo = new RaycastHit();
        foreach (var slide_angleOffset in slide_angleOffsets)
        {

            leftDir = Quaternion.Euler(0, slide_angleOffset * dir, 0) * leftDir;
            leftDir = leftDir.normalized;
            //Debug.Log(leftDir);
            //Debug.DrawRay(point, leftDir, Color.yellow, 100);
            var origin = point + originOffset;
            origin.y = point.y;

            var originLeftOffset = origin - Vector3.up * slide_UpOffsetDis;

            Debug.DrawRay(originLeftOffset, leftDir, Color.blue, 100);
            if (Physics.SphereCast(originLeftOffset, 0.02f, leftDir, out hitInfo, slide_checkDis, PlayerableConfig.Instance.allowHangWallLayerMask))
            {
                break;
            }
        }
     
    }
    public void GetPoint(Vector3 hitPoint, int dir, out RaycastHit hitInfo, Vector3 originOffset = default(Vector3))
    {
        var leftDir = (point - transform.position).normalized;
        //if (hitPoint ==transform.position)
        //{
        //    leftDir = transform.forward.normalized;
        //    Debug.Log("origin");
        //}
        leftDir.y = 0;
        hitInfo = new RaycastHit();
        foreach (var slide_angleOffset in slide_angleOffsets)
        {
            
           
            leftDir = Quaternion.Euler(0, slide_angleOffset * dir, 0) * leftDir;
            leftDir = leftDir.normalized;
            //Debug.Log(leftDir);
            //Debug.DrawRay(point, leftDir, Color.yellow, 100);
            var origin = transform.position + originOffset;
            origin.y = point.y;

            var originLeftOffset = origin - Vector3.up * slide_UpOffsetDis;

            Debug.DrawRay(originLeftOffset, leftDir, Color.blue, 100);
            if (Physics.SphereCast(originLeftOffset, 0.02f, leftDir, out hitInfo, slide_checkDis, PlayerableConfig.Instance.allowHangWallLayerMask))
            {
                Debug.DrawRay(originLeftOffset, leftDir, Color.yellow, 100);
                break;
            }
        }
       
    }
    public IEnumerator RigBlend(float target, float speed)
    {
        while (lefthangUpRig)
        {
            lefthangUpRig.weight = Mathf.MoveTowards(lefthangUpRig.weight, target, Time.deltaTime * speed);
            if (Mathf.Abs(lefthangUpRig.weight - target) < 0.01f)
            {
                lefthangUpRig.weight = target;
                break;
            }
            yield return 0;
        }
        yield return new WaitForSeconds(0.5f);
        while (lefthangUpRig)
        {
            lefthangUpRig.weight = Mathf.MoveTowards(lefthangUpRig.weight, 0, Time.deltaTime * speed);
            if (Mathf.Abs(lefthangUpRig.weight - 0) < 0.01f)
            {
                lefthangUpRig.weight = 0;
                break;
            }
            yield return 0;
        }
    }


    public void OnAnimatorIK(int layerIndex)
    {
        if ((point != Vector3.zero && playerAnim.stateInfo.IsTag("Shimmy")))
        {

            //playerAnim.anim.MatchTarget(leftHandPoint, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), playerAnim.anim.GetFloat("MatchStart"), playerAnim.anim.GetFloat("MatchEnd"));
            //playerAnim.anim.MatchTarget(rightHandPoint, Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), playerAnim.anim.GetFloat("MatchStart"), playerAnim.anim.GetFloat("MatchEnd"));
            //playerAnim.anim.MatchTarget(bodyPoint, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), playerAnim.anim.GetFloat("MatchStart"), playerAnim.anim.GetFloat("MatchEnd"));
            if (playerAnim.leftHandIK)
            {
                playerAnim.anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPoint);
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.7f);
            }
            if (playerAnim.rightHandIK)
            {
                playerAnim.anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPoint);
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.8f);
            }
              
           
        }
        if (playerAnim.stateInfo.IsName("HangToClimb"))
        {
            if (playerAnim.leftHandIK)
            {
                playerAnim.anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPoint);
                playerAnim.anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(transform.forward));
                playerAnim.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.5f);
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
            }
            if (playerAnim.rightHandIK)
            {

                playerAnim.anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPoint);
                playerAnim.anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(transform.forward, Vector3.up));
                playerAnim.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.5f);
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);
            }

        }
        if (playerAnim.stateInfo.IsTag("ClimbLadder"))
        {
            if (playerAnim.leftHandIK)
            {
                playerAnim.anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPoint);
                playerAnim.anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(transform.forward));
                playerAnim.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, playerAnim.anim.GetFloat(AnimatorParameter.LeftHandIKWeight.ToString()));
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, playerAnim.anim.GetFloat(AnimatorParameter.LeftHandIKWeight.ToString()));
            }
            if (playerAnim.rightHandIK)
            {

                playerAnim.anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPoint);
                playerAnim.anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(transform.forward, Vector3.up));
                playerAnim.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, playerAnim.anim.GetFloat(AnimatorParameter.RightHandIKWeight.ToString()));
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, playerAnim.anim.GetFloat(AnimatorParameter.RightHandIKWeight.ToString()));
            }
            if (playerAnim.leftFeetIK)
            {
                playerAnim.anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFeetPoint);
                playerAnim.anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward));
                playerAnim.anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot,playerAnim.anim.GetFloat(AnimatorParameter.LeftFeetIKWeight.ToString()));
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, playerAnim.anim.GetFloat(AnimatorParameter.LeftFeetIKWeight.ToString()));
            }
            if (playerAnim.rightFeetIK)
            {

                playerAnim.anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFeetPoint);
                playerAnim.anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, Vector3.up));
                playerAnim.anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, playerAnim.anim.GetFloat(AnimatorParameter.RightFeetIKWeight.ToString()));
                playerAnim.anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, playerAnim.anim.GetFloat(AnimatorParameter.RightFeetIKWeight.ToString()));
            }
        }

        //playerAnim.anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(test.forward));
        //playerAnim.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        //playerAnim.anim.SetIKHintPosition(AvatarIKHint.LeftElbow, test.position);
        //playerAnim.anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow,1);

    }

    public Transform test;
    public float shimmyMoveSpeed = 10;
    public void CalcuateMove()
    {
        int right = playerController.CalcuateInputDirInCam();
        var target = Vector3.zero;
        var normal = Vector3.zero;
        if (right != 0)
        {
            if (right == 1)
            {
                target = rightHandPoint;
                normal = rightPointNormal;
            }
            else
            {
                target = leftHandPoint;
                normal = leftPointNormal;
            }
            target = CalcuateCharacterPosByHandPoint(target);
            if (Vector3.Distance(target,transform.position)<5)//距离安全效验
            {
                playerController.SetCharacterPosition(target, Vector3.up * 0.02f, shimmyMoveSpeed, true);//TODO 平滑
                playerController.SetCharacterQuaternion(Quaternion.LookRotation(Vector3.ProjectOnPlane(-normal, Vector3.up)));
            }

            var origin = actorPhyiscal.GetCheckPoint(CheckPointType.HeadTop).position + Vector3.up * closetHeadTop_Dis + transform.forward * PlayerableConfig.Instance.headTopForward_Offset;
            Collider[] colliders = Physics.OverlapBox(origin, halfExtent, Quaternion.LookRotation(transform.forward), PlayerableConfig.Instance.allowUpWallLayerMask);
            if (colliders != null && colliders.Length > 0)
            {
                Debug.Log("Check overlayBox");
                bool isRay = false;
                foreach (var item in checkHangVaildOffsetLocal)
                {
                    Color debuColor = Color.red;
                    var worldOrigin = origin + transform.TransformVector(item);
                    Ray ray = new Ray { origin = worldOrigin, direction = Vector3.down };

                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo, canHangVaildDeltaHeight, PlayerableConfig.Instance.allowHangWallLayerMask.value, QueryTriggerInteraction.UseGlobal))
                    {
                        Debug.Log("Check Hang");
                        UpdatePoint(hitInfo.point); 
                        currentVaildOffsetLocal = item;
                        debuColor = Color.green;
                        Debug.DrawLine(ray.origin, ray.origin + Vector3.down * canHangVaildDeltaHeight, debuColor, 2);
                        isRay = true;
                        break;
                    }

                    Debug.DrawLine(ray.origin, ray.origin + Vector3.down * canHangVaildDeltaHeight, debuColor, 2);

                }
                //if (isRay==false)
                //{
                //    Debug.Log("Ray false");
                //}
            }
            else
            {
                //Debug.Log("NoCollider");
            }
        }
        else
        {
            //if (bodyPoint==Vector3.zero)
            //{
            //    Debug.Log("BodyPointZero");
            //}
            playerController.InputKeyUpWhenMove();
        }

    }
    public void ResetDefault()
    {
        point = Vector3.zero;
        leftHandPoint = Vector3.zero;
        rightHandPoint = Vector3.zero;
        leftPointNormal = Vector3.zero;
        rightPointNormal = Vector3.zero;
        vaildHang = false;
        hangCollider = null;
        GameRoot.Instance.LeaveHandlerMainEvent(EventCode.HangUp);
        GameRoot.Instance.LeaveHandlerMainEvent(EventCode.StandToDownHang);
    }
    public Vector3 CalcuateCharacterPosByHandPoint(Vector3 headPoint)
    {
        bodyPoint = headPoint + -Vector3.ProjectOnPlane(pointNormal,Vector3.up) * -1 * (PlayerFacade.Instance.playerController.GetCharacterRadius()) - Vector3.up * PlayerableConfig.Instance.originToHandHeight;
        return bodyPoint;
    }

    public Vector3 cast_ToDown_Hang_halfExtent;
    public float cast_ToDown_Hang_Dis = 5;
    public Vector3[] cast_ToDown_Hang_OffestVec;


    public bool isSignStandToHang = false;
    public void Check_CastToDown_HangUp()
    {
        if (stateManager.isShimmy&&stateManager.allowHangCheck)
        {
            if (isSignStandToHang)
            {
                isSignStandToHang = false;
                GameRoot.Instance.LeaveHandlerMainEvent(EventCode.StandToDownHang);
            }
            return;
        }
        if (stateManager.isJump||stateManager.isFall||playerController.moveComponent.IsInputVelocity()==true)
        {
            return;
        }
        Collider touchCollider = actorPhyiscal.GetCurrentTouchCollider();
        if (stateManager.IsGround && touchCollider != null && (touchCollider.gameObject.layer & (1 << PlayerableConfig.Instance.allowHangWallLayerMask.value)) != 0)
        {
            RaycastHit hitInfo;
            bool isHaveObj = true;
            //find dir  angle< 90  >80
            foreach (var item in cast_ToDown_Hang_OffestVec)
            {

                var worldVec = transform.TransformVector(item);

                var angle = Vector3.Angle(worldVec, transform.forward);
                if (angle < 20 && angle > -10)
                {

                    angle = this.FixedAngle(angle);
                    var offsetWorld = actorPhyiscal.GetCheckPoint(CheckPointType.Feet).transform.position + worldVec;
                    Debug.DrawRay(offsetWorld, Vector3.down);
                    isHaveObj = Physics.CheckBox(offsetWorld, cast_ToDown_Hang_halfExtent, Quaternion.identity, PlayerableConfig.Instance.ColliderLayermask, QueryTriggerInteraction.Ignore);
                    //actorPhyiscal.BoxCast(offsetWorld, cast_ToDown_Hang_halfExtent, Vector3.down, Quaternion.identity, PlayerableConfig.Instance.allowColliderLayermask, out hitInfo, cast_ToDown_Hang_Dis, QueryTriggerInteraction.Collide);
                    if (isHaveObj == false)
                    {

                        var Dir = -transform.TransformDirection(Vector3.ProjectOnPlane(item, Vector3.up));
                        Debug.DrawRay(offsetWorld, Dir, Color.green, 5);

                        RaycastHit raycastHit;
                        var dir = -transform.forward;
                        if (Physics.SphereCast(offsetWorld, align_sphereCheckRadius, dir, out raycastHit, algin_sphereCheckDis, PlayerableConfig.Instance.allowUpWallLayerMask))
                        {
                            pointNormal = raycastHit.normal;

                            var hangPoint = raycastHit.point;
                            if (hangPoint==Vector3.zero||Vector3.Distance(transform.position,hangPoint)>5)
                            {
                                hangPoint = transform.position;//强制修正
                            }
                            hangPoint.y = transform.position.y;


                            point = hangPoint;
                            CalcuateCharacterPosByHandPoint(point);

                            //transform.position = bodyPoint;


                            RaycastHit leftHitInfo;
                            StandToHangGetPoint(-transform.forward, -1, out leftHitInfo, transform.forward * 0.5f);
                            if (leftHitInfo.collider != null)
                            {
                                leftHandPoint = leftHitInfo.point;
                                leftHandPoint.y = point.y;
                                leftPointNormal = leftHitInfo.normal;

                            }
                            RaycastHit rightHitInfo;
                            StandToHangGetPoint(-transform.forward, 1, out rightHitInfo, transform.forward * 0.5f);
                            if (rightHitInfo.collider != null)
                            {
                                rightHandPoint = rightHitInfo.point;
                                rightHandPoint.y = point.y;
                                rightPointNormal = rightHitInfo.normal;

                            }
                        }
                        if (isSignStandToHang == false)
                        {
                            GameRoot.Instance.SignHandlerMainEvent(new EventInfo { eventCode = EventCode.StandToDownHang, eventTips = "StandToDownHang",keyBehaviourType=KeyBehaviourType.RoleBehaviour }, () =>
                            {
                                PlayerFacade.Instance.HandleStandToHang();
                                return true;
                            });
                            Debug.Log("isSignStandToHang");

                            isSignStandToHang = true;
                        }
                        break;
                    }
                }
            }
            if (isHaveObj)
            {
                isSignStandToHang = false;
                GameRoot.Instance.LeaveHandlerMainEvent(EventCode.StandToDownHang);
               
            }

        }
        else
        {
            if (isSignStandToHang)
            {
                isSignStandToHang = false;
                GameRoot.Instance.LeaveHandlerMainEvent(EventCode.StandToDownHang);
            }
        }
    }
    public void SignHandleDown()
    {
        GameRoot.Instance.LeaveHandlerMainEvent(EventCode.StandToDownHang);
        GameRoot.Instance.Sign_Key_HandleEvent(KeyCode.Space, new EventInfo { eventTips = "Will hang down", eventCode = EventCode.HangDown,keyBehaviourType=KeyBehaviourType.RoleBehaviour }, () => { GameRoot.Instance.LeaveHandlerMainEvent(EventCode.HangToClimb); PlayerFacade.Instance.HandlerHangDown(); return true; });
        if (bodyPoint!=Vector3.zero)
        {
            playerController.LerpToTargetOnUpdate(bodyPoint, PlayerableConfig.Instance.hangUpExitBlendSpeed, false);
        }
    }
    public void SingHandleToCllimb()
    {
      
        GameRoot.Instance.SignHandlerMainEvent(new EventInfo { eventTips = "Will HangToClimb", eventCode = EventCode.HangToClimb,keyBehaviourType=KeyBehaviourType.RoleBehaviour}, () => { PlayerFacade.Instance.HandleHangToClimb(); GameRoot.Instance.Leave_Key_HandlerEvent
            (KeyCode.Space,EventCode.HangDown); return true; });
    }

    

    public void OnDrawGizmos()
    {
       

        if (Application.isPlaying)
        {
             var origin = actorPhyiscal.GetCheckPoint(CheckPointType.HeadTop).position + Vector3.up * closetHeadTop_Dis + transform.forward * PlayerableConfig.Instance.headTopForward_Offset;
            var todown_HangUp = actorPhyiscal.GetCheckPoint(CheckPointType.Waist).transform.position;
            Gizmos.DrawWireCube(todown_HangUp, cast_ToDown_Hang_halfExtent * 2);
            Gizmos.DrawWireCube(origin, halfExtent * 2);
        }
        
        if (point != Vector3.zero)
        {
            Gizmos.DrawSphere(point, 0.2f);
            Gizmos.DrawSphere(bodyPoint, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(leftHandPoint, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rightHandPoint, 0.1f);
        }
    }
}

[SerializeField]
public class BoneInfo
{
    public Transform target;
    public Transform hint;
}