using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FPS_CameraController : CameraController
{
    public float rotateSpeed = 45;
    private PlayerInputController inputController;
    public CinemachineVirtualCamera camVirtualAim;
    public CinemachineVirtualCamera fps_camera;
    public float MaxVerticalAngle=60;
    public float MinVerticalAngle;
    public bool isListenCtrl = false;
    public float defaultFocalLength=0.45f;
    public void OnValidate()
    {
        SetFPSCameraFOV(defaultFocalLength);
    }
    public void Awake()
    {
        inputController = PlayerInputController.Instance;
    }
    /// <summary>
    /// 根据焦距（focal length）计算field of view值
    /// </summary>
    private float CalculateFieldOfView(CinemachineVirtualCamera camera,float focalLength)
    {
        float sensorSize = camera.m_Lens.SensorSize.y;

        // 根据焦距和传感器大小计算field of view值
        float fov = 2.0f * Mathf.Atan(sensorSize / (2.0f * focalLength)) * Mathf.Rad2Deg;

        return fov;
    }
    public void SetFPSCameraFOV(float focalLength)
    {
        fps_camera.m_Lens.FieldOfView = CalculateFieldOfView(fps_camera,focalLength);
    }
    public void LateUpdate()
    {
        if (isListenCtrl)
        {
            Quaternion camRotation = camVirtualAim.transform.rotation;
            if (Mathf.Abs(inputController.MouseAxisY) > 0.5f)
            {
                camRotation *= Quaternion.AngleAxis(-inputController.MouseAxisY * rotateSpeed * Time.deltaTime, camVirtualAim.transform.right);
            }
            if (Mathf.Abs(inputController.MouseAxisX) > 0.5f)
            {
                camRotation *= Quaternion.AngleAxis(inputController.MouseAxisX * rotateSpeed * Time.deltaTime, Vector3.up);
            }

            Vector3 camEuler = camRotation.eulerAngles;
            camEuler.z = 0;
            camRotation = Quaternion.Euler(camEuler);

            camVirtualAim.ForceCameraPosition(camVirtualAim.transform.position, camRotation);
            LimitRotation();
        }
    }
    private void LimitRotation()
    {
        if ((this.FixedAngle(transform.localEulerAngles.x)) < -MaxVerticalAngle)
        {
            var quaternion = Quaternion.Euler(fps_camera.transform.localEulerAngles.SetX(-MaxVerticalAngle));
            SetRotation(fps_camera, quaternion);
        }
        else if ((this.FixedAngle(transform.localEulerAngles.x)) > -MinVerticalAngle)
        {
            var quaternion = Quaternion.Euler(fps_camera.transform.localEulerAngles.SetX(-MinVerticalAngle));
            SetRotation(fps_camera, quaternion);
        }
        
    }

}
