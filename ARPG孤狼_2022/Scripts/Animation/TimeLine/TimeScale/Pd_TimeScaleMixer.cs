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
            float weight=playable.GetInputWeight(i);//��ǰƬ������֡��ռ��Ȩ��  ָ����µ�Ȩ��
            //playable.GetInput();//���AssetƬ��
            var timeScaleClip= (ScriptPlayable<Pd_TimeScaleBehaviour>)playable.GetInput(i);//playable����ֱ��ת��Behaviour ��Ҫ��ת��Scriptableת��Scriptplayable���Behaviour
            var timeScaleBehaviour = timeScaleClip.GetBehaviour();//�õ����Ƭ�ε�behaviour�ű�
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
