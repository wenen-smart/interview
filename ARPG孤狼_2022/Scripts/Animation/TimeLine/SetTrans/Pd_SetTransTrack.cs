using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
[TrackBindingType(typeof(RoleController))]
[TrackClipType(typeof(Pd_SetTransAsset))]
[TrackColor(219 * 1.0f / 255, 112 * 1.0f / 255, 147*1.0f/255)]

// A behaviour that is attached to a playable
public class Pd_SetTransTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {

       var setTransMixer= ScriptPlayable<Pd_SetTransMixer>.Create(graph);
        setTransMixer.SetInputCount(inputCount);
        return setTransMixer;
    }
}
