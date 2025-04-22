using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class Pd_SetTransAsset : PlayableAsset,ITimelineClipAsset
{
    public ExposedReference<Transform> targetTrans;
    public ExposedReference<RoleController> toAnotherActor;//localPos 相对的对象  
    public Vector3 localPos;//属于 toAnotherActor 坐标系下的局部坐标
    public Vector3 localEuler;
    public SetTransType setTransType;
    [MultSelectTags]
    public SetTransDataType setTransDataType;
    public bool isFollowTargetPoint;
    public ClipCaps clipCaps => ClipCaps.Blending;
    

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var mono = new Pd_SetTransBehaviour();
        mono.targetTrans = targetTrans.Resolve(graph.GetResolver());
        mono.toAnotherActor = toAnotherActor.Resolve(graph.GetResolver());
        mono.setTransType = setTransType;
        mono.localPos = localPos;
        mono.localEuler = localEuler;
        mono.setTransDataType = setTransDataType;
        mono.isFollowTargetPoint = isFollowTargetPoint;
        return ScriptPlayable<Pd_SetTransBehaviour>.Create(graph,mono);
    }
}
[Serializable]
public enum SetTransType
{
    Local,
    TransTarget

}
public enum SetTransDataType
{
    Position,
    Rotation
}
