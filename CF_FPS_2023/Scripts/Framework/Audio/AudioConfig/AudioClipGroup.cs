using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName ="NewAudioClipGroup",menuName = "CreateConfig/CreateAudioClipGroup",order = 7)]
public class AudioClipGroup:ScriptableObject
{
    public bool isLoop;
    public List<AudioClipItem> audioClipGroups;
    [Header("空就不重写")]
    public AudioMixerGroup overrideMixerGroup;
}
[Serializable]
public class AudioClipItem
{
    public AudioClipSequenceMode sequenceMode;
    public List<AudioClip> clips;
}
public enum AudioClipSequenceMode
{
    Random,
    RandomNoImmediatelyRepeat,
    Sequence,
}

