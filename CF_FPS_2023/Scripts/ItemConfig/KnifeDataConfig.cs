using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New_KnifeSO", menuName = "CreateWeaponSO/CreateKnifeSO")]
public class KnifeDataConfig : MeleeWeaponDataConfig
{

}

[Serializable]
public class MeleeCheckWay
{
    public List<MeleeCheckWayData> meleeCheckWayData;
    [Header("自动计算检测的起点")]
    public bool StartPosIsByCameraCenterAutoCalculate = true;
    [Header("检测的起点")]
    public Vector3 startPosOffsetInCameraCoord = new Vector3(0, 0,0);
}
[Serializable]
public enum RayCastTrackType
{
    SingleDir,
    FromPointToTwoSide,
}
[Serializable]
public struct MeleeCheckWayData
{
    public RayCastTrackType rayCastTrackType;
    public Vector2 checkDir;
    public float checkAngle;
    public float checkDis;
    public float eachcheckIntervalAngle;
    public float meleeCheckTime;
}
[Serializable]
public struct MeleeAttackData
{
    //[Header("是否存在预动画,等待输入改变执行攻击动画")]
    //public bool isHavePrepare;
    //public string prepareAttackAnimationName;
    public string animationName;
    public DistinctData attackCheckDistinct;
    public DistinctData ComboCheckDistinct;
    public int nextComboID;
    public MeleeCheckWay checkWayData;
}
[Serializable]
public struct DistinctData
{
    [Range(0,1)]
    public float StartNormalizeTime;
    [Range(0,1)]
    public float FinishNormalizeTime;
}