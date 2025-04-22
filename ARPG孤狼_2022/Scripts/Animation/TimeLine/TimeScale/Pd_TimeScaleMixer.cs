using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class Pd_TimeScaleMixer : PlayableBehaviour
{

    public float time_Scale;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        float finalScale = 0;
        for (int i = 0; i < playable.GetInputCount(); i++)
        {
            float weight=playable.GetInputWeight(i);//当前片段所在帧所占的权重  指混合下的权重
            //playable.GetInput();//获得Asset片段
            var timeScaleClip= (ScriptPlayable<Pd_TimeScaleBehaviour>)playable.GetInput(i);//playable不能直接转成Behaviour 需要先转成Scriptable转用Scriptplayable获得Behaviour
            var timeScaleBehaviour = timeScaleClip.GetBehaviour();//得到这个片段的behaviour脚本
            finalScale += timeScaleBehaviour.time_Scale*weight;
        }

        if (finalScale>0)
        {
            Time.timeScale = finalScale;
        }
        else
        {
            Time.timeScale = 1;
        }
        
    }
}
