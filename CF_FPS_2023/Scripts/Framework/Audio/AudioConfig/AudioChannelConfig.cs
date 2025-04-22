using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="NewAudioChannelSO",menuName = "CreateConfig/CreateAudioChannelSO",order = 6)]
public class AudioChannelConfig : ScriptableObject
{
    [Range(0,1)]
    public float Volume=1;


    public void SetVolume(float v)
    {
        Volume = Mathf.Clamp01(v);
    }
}
