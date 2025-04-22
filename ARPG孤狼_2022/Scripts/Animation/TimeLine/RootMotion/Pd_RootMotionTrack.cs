using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
[TrackBindingType(typeof(Animator))]
[TrackClipType(typeof(Pd_RootMotionAsset))]
// A behaviour that is attached to a playable
public class Pd_RootMotionTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {

       var setTransMixer= ScriptPlayable<Pd_RootMotionMixer>.Create(graph);
        setTransMixer.SetInputCount(inputCount);
      
        return setTransMixer;
    }
}
