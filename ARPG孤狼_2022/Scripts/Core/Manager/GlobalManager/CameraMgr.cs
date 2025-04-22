using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraMgr : InstanceMono<CameraMgr>
{


    public CinemachineVirtualCamera camVirtualAim;
    public CinemachineFreeLook freeLook;
    public CinemachineImpulseSource impusleSource;
    public Vector2 defaultFreeLookAxisSpeed;
    public Vector2 defaulAimCamAxisSpeed;
    public CinemachineBrain brain;
    public Transform defaultLookAt;
    public Transform defaultFollowAt;
    public Vector3 offset;
    public Vector3 aimOffset { get; set; }
    public CharacterFacade aimer { get { return PlayerFacade.Instance; } }
    public bool allowControllerVirtualCam { get; set; }
    public bool isControlVirtualCam { get; set; }
    public float Lock_HorMinAngle_WhenNoTarget = 10;
    public float Lock_VerMinAngle_WhenNoTarget = 10;
    public float Lock_FollowDampSmoothTime = 10;
    public float lock_Target_slerpSpeed = 45;
    public float lock_Target_LookAt_MaxVerticalAngle=15;
    public float lock_Target_LoseTarget_MaxVerticalAngle = 85;
    public Vector2 aim_VerticalAngleLimit = new Vector2(-60,80);//x=》上  y=》下
    public Vector3 currentVelocity;
    public ParticleSystem post_fangshemohu;
    public Action canelLockTarget;
    public CinemachineTargetGroup targetGroup;
    public float lock_Target_MaxHorLerpAngle=60;//如果要旋转的值大于这个值的话，就硬切或者加快lerp速度
    private bool lock_NOUpdateLookTarget;//锁定状态下不持续朝向目标

    public override void Awake()
    {
        base.Awake();
        impusleSource=GetComponent<CinemachineImpulseSource>();
    }

    public void Start()
    {
        defaultLookAt = freeLook.LookAt;
        defaultFollowAt = freeLook.Follow;
    }
    public void SetVirtualAim(Vector3 target, Vector3 lookFoward, Transform lookAt = null)
    {
        camVirtualAim.gameObject.SetActive(true);
        camVirtualAim.transform.position = target;
        camVirtualAim.transform.rotation = Quaternion.LookRotation(lookFoward,Vector3.up);
        camVirtualAim.LookAt = lookAt;
        currentVelocity = Vector3.zero;
    }
    public void SetCanControlVirtualAim(Vector3 aimOffset, Vector3 lookFoward)
    {
        this.aimOffset = aimOffset;
        Transform relativeAimer = aimer.characterAnim.GetBodyTransform(HumanBodyBones.Neck);
        Vector3 camPos = relativeAimer.transform.position + aimer.transform.TransformVector(aimOffset);
        SetVirtualAim(camPos,lookFoward);
        allowControllerVirtualCam = true;
    }
    public void CloseVirtualAim()
    {
        camVirtualAim.gameObject.SetActive(false);
        allowControllerVirtualCam = false;
        isControlVirtualCam = false;
        freeLook.ForceCameraPosition(camVirtualAim.transform.position, camVirtualAim.transform.rotation);
    }

    public bool isLockTarget;

    public void SetLockTarget(EnemyController lockTarget)
    {
        if (lockTarget != null)
        {
            lockEnemy = lockTarget;
            isLockTarget = true;
            freeLook.m_XAxis.m_InputAxisName = "";
            freeLook.m_YAxis.m_InputAxisName = ""; 
            targetGroup.m_Targets[1].target = lockTarget.centerTrans.transform;
            freeLook.LookAt = targetGroup.transform;
            freeLook.Follow = null;
            NGUI_GameMainPopup.Instance.OpenForeSinglePoint();
        }
        else
        {
            NGUI_GameMainPopup.Instance.CloseForeSinglePoint();
            if (Vector3.Angle(Vector3.ProjectOnPlane(PlayerFacade.Instance.transform.forward, Vector3.up), Vector3.ProjectOnPlane(transform.forward, Vector3.up)) > Lock_HorMinAngle_WhenNoTarget || Vector3.Angle(Vector3.ProjectOnPlane(PlayerFacade.Instance.transform.forward, transform.right), Vector3.ProjectOnPlane(transform.forward, transform.right)) > Lock_VerMinAngle_WhenNoTarget)
            {

                //freeLook.LookAt = null;
                //freeLook.Follow = null;
                Vector3  probablyPosition= PlayerFacade.Instance.transform.position + PlayerFacade.Instance.transform.TransformVector(offset);

                Vector3 willPosToLookPoint = freeLook.LookAt.position - probablyPosition;
                Vector3 currentPosToLookPoint = freeLook.LookAt.position - freeLook.transform.position;
                //处理y平面上的向量 、 投射 方向角度只与XZ有关 投射到Y平面
                Vector3 willPosToLookPointProject=Vector3.ProjectOnPlane(willPosToLookPoint,Vector3.up);
                Vector3 currentPosToLookPointProject=Vector3.ProjectOnPlane(currentPosToLookPoint,Vector3.up);
                float angle = Vector3.SignedAngle(currentPosToLookPointProject,willPosToLookPointProject,Vector3.up);
                MyDebug.DebugPrint("CameraForceAngle:"+angle);
                MyDebug.DebugLine(freeLook.LookAt.position,probablyPosition,Color.red,5);
                MyDebug.DebugWireSphere(new Vector3[] { probablyPosition,freeLook.transform.position},0.1f,Color.green,5);
                MyDebug.DebugLine(freeLook.LookAt.position,freeLook.transform.position,Color.green,5);
                freeLook.m_XAxis.Value = angle;
                //SetFreeLookRotation(Quaternion.LookRotation(PlayerFacade.Instance.transform.forward));
                freeLook.m_YAxis.Value = 0.5f;
                //for (int i = 0; i < 3; i++)
                //{
                //    freeLook.GetRig(i).ForceCameraPosition(freeLook.transform.position, Quaternion.LookRotation(PlayerFacade.Instance.transform.forward));
                //}
                //    StartCoroutine(GameRoot.Instance.AddTask(50, () =>
                // {
                //     freeLook.PreviousStateIsValid = false;//让虚拟相机在下一帧立即完成Damp

                //     freeLook.LookAt = defaultLookAt;
                //     freeLook.Follow = defaultFollowAt;

                //     //for (int i = 0; i < 3; i++)
                //     //{
                //     //    freeLook.GetRig(i).PreviousStateIsValid = false;
                //     //}

                //     //StartCoroutine(GameRoot.Instance.AddTask(50,()=> {


                //     //}));
                // }
                //));
            }
        }




        //改变速度 FreeLook不能自动避障
        //defaultFreeLookAxisSpeed = new Vector2(freeLook.m_XAxis.m_MaxSpeed, freeLook.m_YAxis.m_MaxSpeed);
        //freeLook.m_XAxis.m_MaxSpeed = 0;
        //freeLook.m_YAxis.m_MaxSpeed = 0;
        //freeLook.LookAt = lockTarget.transform;


        //方案二 改变 Input输入

        //offset = freeLook.transform.InverseTransformVector(PlayerFacade.Instance.transform.position - freeLook.transform.position);

    }
    public void LateUpdate()
    {
        if (isLockTarget && lockEnemy && CurrentFreeLookIsLive())
        {
            NGUI_GameMainPopup.Instance.UpdateForeSinglePointPos(lockEnemy.centerTrans.transform.position);
            //var toTargetDir = lockEnemy.GetTrulyRigBodyCenterPoint() - PlayerFacade.Instance.playerController.GetTrulyRigBodyCenterPoint();
            
            Quaternion tempRotation;
            var camWorldPosition = PlayerFacade.Instance.transform.position+PlayerFacade.Instance.transform.TransformVector(offset);
            var toTargetDir = targetGroup.transform.position - camWorldPosition;
             Vector3 projectDir = Vector3.ProjectOnPlane(toTargetDir, Vector3.up);
            Vector3 projectRight = (Quaternion.Euler(0, 90, 0) * projectDir.normalized).normalized;

            if (Vector3.SignedAngle(projectDir, toTargetDir, projectRight) <= -lock_Target_LookAt_MaxVerticalAngle)
            {
                toTargetDir = Quaternion.Euler(projectRight * -lock_Target_LookAt_MaxVerticalAngle) * projectDir;//
            }

            
            Quaternion targetRotation = Quaternion.LookRotation(toTargetDir);
            if (Quaternion.Angle(targetRotation,transform.rotation)<=lock_Target_MaxHorLerpAngle)
            {
                freeLook.transform.position = Vector3.SmoothDamp(freeLook.transform.position, camWorldPosition, ref currentVelocity, Time.deltaTime * Lock_FollowDampSmoothTime);
                tempRotation = Quaternion.Slerp(transform.rotation,targetRotation, Time.fixedDeltaTime * lock_Target_slerpSpeed);
            }
            else
            {
                freeLook.transform.position = camWorldPosition;
                tempRotation = targetRotation;
            }
            SetFreeLookRotation(tempRotation);
            JudgeLoseTarget(toTargetDir);
            //freeLook.transform.rotation = Quaternion.LookRotation(toTargetDir);
        }
        else if (allowControllerVirtualCam)
        {
            if (brain.IsLive(camVirtualAim)&&CurrentFreeLookIsLive()==false)
            {
                if (isControlVirtualCam==false)
                {
                    isControlVirtualCam = true;
                }
                Transform relativeAimer = aimer.characterAnim.GetBodyTransform(HumanBodyBones.Neck);
                Quaternion camRotation = camVirtualAim.transform.rotation;
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                bool aimPivotUpdate = false;
                if (Mathf.Abs(mouseX)>0.5f)
                {
                    camRotation *=Quaternion.AngleAxis(defaulAimCamAxisSpeed.x*mouseX, Vector3.up);
                    aimPivotUpdate = true;
                }
                if (Mathf.Abs(mouseY) > 0.5f)
                {
                    camRotation*=Quaternion.AngleAxis(mouseY * defaulAimCamAxisSpeed.y, camVirtualAim.transform.right);
                    aimPivotUpdate = true;
                }
                Vector3 camEuler = camRotation.eulerAngles;
                camEuler.z = 0;
                float verticalAngleInInspector = this.FixedAngle(camEuler.x);
                verticalAngleInInspector=Mathf.Clamp(verticalAngleInInspector,aim_VerticalAngleLimit.x,aim_VerticalAngleLimit.y);
                camEuler.x = verticalAngleInInspector;
                camRotation = Quaternion.Euler(camEuler);
                Vector3 camPos = relativeAimer.transform.position + aimer.transform.TransformVector(aimOffset);
                camRotation = Quaternion.Slerp(transform.rotation, camRotation, Time.fixedDeltaTime * lock_Target_slerpSpeed);
                camPos += Vector3.up * (this.FixedAngle(camRotation.eulerAngles.x)/60);//处理位置跟随仰角偏移
                camPos = Vector3.SmoothDamp(camVirtualAim.transform.position,camPos, ref currentVelocity, Time.fixedDeltaTime * Lock_FollowDampSmoothTime);
                camVirtualAim.ForceCameraPosition(camPos, camRotation);
            }
        }
    }
    private void LimitRotation()
    {
        if ((this.FixedAngle(transform.localEulerAngles.x)) <= -lock_Target_LookAt_MaxVerticalAngle)
        {
            var quaternion = Quaternion.Euler(freeLook.transform.localEulerAngles.SetX(-lock_Target_LookAt_MaxVerticalAngle));
            SetFreeLookRotation(quaternion);
        }
    }
    private void JudgeLoseTarget(Vector3 toTargetDir)
    {
        if (Vector3.Angle(Vector3.ProjectOnPlane(toTargetDir,Vector3.up),toTargetDir)>lock_Target_LoseTarget_MaxVerticalAngle)
        {
            CanelLockTarget();
        }
    }
    public void SetFreeLookRotation(Quaternion quaternion)
    {
        //更新同步主相机  应用到主相机
        freeLook.ForceCameraPosition(freeLook.transform.position, quaternion);
    }

    public bool CurrentFreeLookIsLive()
    {
        return brain.IsLive(freeLook);
    }
    public void CanelLockTarget()
    {
        NGUI_GameMainPopup.Instance.CloseForeSinglePoint();
        isLockTarget = false;
        lockEnemy = null;
        //freeLook.m_XAxis.m_MaxSpeed = defaultFreeLookAxisSpeed.x;
        //freeLook.m_YAxis.m_MaxSpeed = defaultFreeLookAxisSpeed.y;
        freeLook.LookAt = defaultLookAt;
        freeLook.Follow = defaultFollowAt;
        SetFreeLookRotation(brain.transform.rotation);
        freeLook.m_XAxis.m_InputAxisName = "Mouse X";
        freeLook.m_YAxis.m_InputAxisName = "Mouse Y";
        canelLockTarget?.Invoke();
    }

    public EnemyController lockEnemy;

    public void GeneralImpusle()
    {
        impusleSource.GenerateImpulse();
    }
    public void OpenPost_FangsheMohu()
    {
        post_fangshemohu.Stop();
        post_fangshemohu.Play();
    }

    public void GetRotateSpeed()
    {
        
    }
    public RaycastHit CamCenterRaycast(LayerMask layerMask,float rayCastDis=1000,QueryTriggerInteraction interaction=QueryTriggerInteraction.UseGlobal)
    {
        Camera camera = Camera.main;
        Ray ray = camera.ScreenPointToRay(new Vector2(Screen.width/2.0f,Screen.height/2.0f));
        RaycastHit raycastHit;
        Physics.Raycast(ray,out raycastHit,rayCastDis,layerMask.value,interaction);
        return raycastHit;
    }
    public static Vector3 WorldPointToNGUIWorldPoint(Vector3 worldPoint)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPoint);
        screenPos = new Vector3(screenPos.x, screenPos.y, 0f);
        Vector3 nguiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
        return nguiPos;
    }
    public static Vector3 WorldPointToNGUILocalPoint(Vector3 worldPoint,Transform nguiTrans)
    {
        return nguiTrans.InverseTransformPoint(WorldPointToNGUIWorldPoint(worldPoint));
    }
    public static Vector3 ScreentPointToNGUIWorldPoint(Vector2 screenPos)
    {
        //Vector3 screenWorldPoint = Camera.main.ScreenToWorldPoint(screenPos);
        //return WorldPointToNGUIWorldPoint(screenWorldPoint);
        Vector3 nguiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
        return nguiPos;
    }
    public static Vector3 ScreentPointToNGUILocalPoint(Vector2 screenPos,Transform nguiTrans)
    {
        return nguiTrans.InverseTransformPoint(ScreentPointToNGUIWorldPoint(screenPos));
    }
    public void SetNoLookAtTargetWhenLock(bool noLook)
    {
        lock_NOUpdateLookTarget = noLook;
    }
}
