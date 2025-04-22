using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoSingleTon<CameraController>
{
    public CinemachineFreeLook freelook;
    public Vector3 GetMainCameraForward()
    {
        return transform.forward;
    }
    /** 
     * @Description: 获取到以相机正前方、法线所在平面为基准的方向坐标系
     * @param       planeNormal [Vector3]:
     * @return      [*]
     * @author     : Miracle
     */       
    public Vector3 GetMainCameraPlaneCoordAxis(Vector3 planeNormal)
    {
        return Vector3.ProjectOnPlane(GetMainCameraForward(),planeNormal);
    }
    public Vector3 GetMainCameraPlaneCoordAxis(Vector3 localDir,Vector3 planeNormal)
    {
        return Vector3.ProjectOnPlane(transform.TransformDirection(localDir),planeNormal);
    }
    private void  OnDrawGizmos() {
        Gizmos.color=Color.red;
        Gizmos.DrawRay(new Ray(transform.position,GetMainCameraForward()));
        Gizmos.DrawRay(new Ray(transform.position,GetMainCameraPlaneCoordAxis(Vector3.up)));
    }
}
