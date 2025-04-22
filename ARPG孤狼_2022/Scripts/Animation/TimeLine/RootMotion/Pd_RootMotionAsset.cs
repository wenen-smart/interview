using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class Pd_RootMotionAsset : PlayableAsset,ITimelineClipAsset
{
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;
    public AnimationCurve zCurve;
    
    public ClipCaps clipCaps => ClipCaps.Blending;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var mono = new Pd_RootMotionBehaviour();
        mono.zDirCurve = zCurve;
        mono.yCurve = yCurve;
        mono.xCurve = xCurve;
        return ScriptPlayable<Pd_RootMotionBehaviour>.Create(graph,mono);
    }
}
