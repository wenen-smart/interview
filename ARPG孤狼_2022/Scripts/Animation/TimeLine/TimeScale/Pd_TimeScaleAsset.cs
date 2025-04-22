using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class Pd_TimeScaleAsset : PlayableAsset, ITimelineClipAsset
{
    public float timeScale;

    public ClipCaps clipCaps => ClipCaps.Blending;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {

        Pd_TimeScaleBehaviour timeScaleBehaviour = new Pd_TimeScaleBehaviour();
        timeScaleBehaviour.time_Scale = timeScale;

        return  ScriptPlayable<Pd_TimeScaleBehaviour>.Create(graph,timeScaleBehaviour);
    }
}
