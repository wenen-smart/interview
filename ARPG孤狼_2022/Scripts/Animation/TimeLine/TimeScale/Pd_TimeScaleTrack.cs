using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
[TrackClipType(typeof(Pd_TimeScaleAsset))]
public class Pd_TimeScaleTrack:TrackAsset
    {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {

        var mixerPlayable = ScriptPlayable<Pd_TimeScaleMixer>.Create(graph);//其实创建的就是Playable 通过.GetBehaviour() 可以转换成Pd_TimeScaleMixer
        //一个mixer轨道或一个片段就相当与一个Playable
        mixerPlayable.SetInputCount(inputCount);//设置轨道上的片段数量.
        return mixerPlayable;
    }
}

