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
    public Vector3 GetMainCameraUp()
    {
        return transform.up;
    }
    public Vector3 LocalDirToWorldDir(Vector3 dir)
    {
        return transform.TransformDirection(dir.normalized);
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
    public void SetRotation(CinemachineVirtualCameraBase virtualCameraBase, Quaternion quaternion)
    {
        //更新同步主相机  应用到主相机
        virtualCameraBase.ForceCameraPosition(virtualCameraBase.transform.position, quaternion);
    }
    public RaycastHit CamCenterRaycast(LayerMask layerMask, float rayCastDis = 1000, QueryTriggerInteraction interaction = QueryTriggerInteraction.UseGlobal)
    {
        Camera camera = Camera.main;
        Ray ray = camera.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
        RaycastHit raycastHit;
        Physics.Raycast(ray, out raycastHit, rayCastDis, layerMask.value, interaction);
        return raycastHit;
    }
    public static Vector3 WorldPointToNGUIWorldPoint(Vector3 worldPoint)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPoint);
        screenPos = new Vector3(screenPos.x, screenPos.y, 0f);
        Vector3 nguiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
        return nguiPos;
    }
    public static Vector3 WorldPointToNGUILocalPoint(Vector3 worldPoint, Transform nguiTrans)
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
    public static Vector3 ScreentPointToNGUILocalPoint(Vector2 screenPos, Transform nguiTrans)
    {
        return nguiTrans.InverseTransformPoint(ScreentPointToNGUIWorldPoint(screenPos));
    }
}
