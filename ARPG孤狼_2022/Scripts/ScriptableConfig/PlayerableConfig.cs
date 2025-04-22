using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayableConfig", menuName = "CreateConfig/CreatePlayable", order = 1)]
public class PlayerableConfig : ScriptableConfigTon<PlayerableConfig>
{
   new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.playableConfigPath;
    }
    [Header("---------SpeedMult------------")]
    public float quickRunMult = 2;
    public float commmonRunMult = 1;
    public float commonFallSpeed = 1;
    public float quickMaxFallSpeed = 2;
    public float hangUpExitBlendSpeed = 20;
    public AnimationCurve hangUpToBlendSpeedCurve;
   

    [Header("---------���Գ˻�------------")]
    [Range(0,1)]
    public float jump_����_Mult = 0.5f;
    [Range(0, 1)]
    public float fall_����_Mult = 0.2f;
    [Range(0, 1)]
    public float fall2Landing_����_Mult = 0.5f;

    [Header("-------Physical Relateive--------")]
    public Vector3 boxCast_closestWall_halfExtent = Vector3.one;
    public float boxCast_closestWall_maxDis = 2f;
    public LayerMask arrowCanTriggerCheckLayer;

    [Header("---------RayCastLengthConfig------------")]
    public float check_InWallForward_MaxDis = 0.5f;
    public float check_InGround_MaxDis = 0.01f;
    public float check_InGroundWhenFall_MaxDis = 0.5f;
    [Range(0.01f,1)]
    public float check_closetWall_HorDirMult = 1;
    public float bowArrowMaxRayDis = 20;//����Ļ���ķ���һ������

    [Header("-------LayMaskConfig------------")]
    public LayerMask allowUpWallLayerMask;
    public LayerMask allowClosetWallBehaviourLayerMask;
    public LayerMask allowHangWallLayerMask;
    public LayerMask ColliderLayermask;
    public LayerMask enemyLayerMask;
    [MultSelectTags]
    public GameObjectTag StairsTag; 

    [Header("---------Hang Check----------")]
    public float closetHeadTop_Dis = 0.289772f;
    public float headTopForward_Offset = 0.25f;
    public float originToHandHeight = 2.355f;

    [Header("---------Strike----------")]
    [Tooltip("����ͻϮ��ɱ���е�һ��")]
    public Vector3 boxCast_hangUpStrick_halfExtent = Vector3.one;
    [Tooltip("����ͻϮ��ɱ���е�ԭ��ƫ��")]
    public Vector3 castFromHeadTopOffset_hangUpStrick = Vector3.zero;
    [Header("---------Picker----------")]
    public Vector3 boxCast_CheckPicker_HalfExtent = Vector3.one;
    public LayerMask pickItemLayerMask;

    [Header("MoveCompoennt")]
    public float noarriveAreasafeDistance=1;//���˾���NavMesh�в��ɵ�������İ�ȫ����
}
