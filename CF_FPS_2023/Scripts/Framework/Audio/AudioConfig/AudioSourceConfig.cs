using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName ="NewAudioSourceConfig",menuName = "CreateConfig/AudioSourceConfig",order = 8)]
public class AudioSourceConfig:ScriptableObject
{
    public AudioSource audioSource;
    public AudioType audioType;
}

