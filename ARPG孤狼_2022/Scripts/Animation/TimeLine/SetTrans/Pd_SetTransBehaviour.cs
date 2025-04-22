using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class Pd_SetTransBehaviour : PlayableBehaviour
{
    public Transform targetTrans;
    public SetTransType setTransType;
    public SetTransDataType setTransDataType;
    public Vector3 localPos;
    public RoleController toAnotherActor;
    public Vector3 localEuler;


    public Vector3 calcuatePosResult;
    public Vector3 calCuateEulerResult;
    public bool isFollowTargetPoint;
    private bool isPlay = false;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {

    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (isPlay == false)
        {
            isPlay = true;

            ExcuteCalcauate();
        }
        if (isFollowTargetPoint)
        {
            ExcuteCalcauate();
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        ExcuteCalcauate();
    }
    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
    }
    public override void OnPlayableDestroy(Playable playable)
    {
        ExcuteCalcauate();
    }
    public void ExcuteCalcauate()
    {
        if (((int)(setTransDataType) & (1 << (int)SetTransDataType.Position)) != 0)
        {
            if (setTransType == SetTransType.TransTarget)
            {
                calcuatePosResult = targetTrans.transform.position;
            }
            else
            {
                if (toAnotherActor)
                {
                    calcuatePosResult = toAnotherActor.transform.TransformPoint(localPos);
                }
            }
            //CorrentPosResult
            Vector3 rayOrigin = calcuatePosResult;
            rayOrigin += Vector3.up;
            RaycastHit[] hitInfos = Physics.RaycastAll(new Ray(rayOrigin,Vector3.down),1.5f,PlayerableConfig.Instance.ColliderLayermask,QueryTriggerInteraction.Collide);
            if (hitInfos!=null&&hitInfos.Length>0)
            {
                foreach (var hitinfo in hitInfos)
                {
                    calcuatePosResult = hitinfo.point;
                    break;
                }
            }

        }

        if (((int)(setTransDataType) & (1 << (int)SetTransDataType.Rotation)) != 0)
        {
            if (setTransType == SetTransType.TransTarget)
            {
                calCuateEulerResult = targetTrans.transform.eulerAngles;
            }
            else
            {
                if (toAnotherActor)
                {
                    calCuateEulerResult = (toAnotherActor.transform.eulerAngles + localEuler);
                }

            }
        }

    }
}
