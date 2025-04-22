using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public enum AudioType
{
    Music_2,//背景音乐只能存在一处
    Music_3,
    SFX_2,
    SFX_3,
    BGS,//背景音效 环境音
}
public class AudioSourceEntity : MonoBehaviour
{
    public AudioSource audioSource { get { return GetComponent<AudioSource>(); } }
    [HideInInspector]
    public AudioSourceConfig sourceConfig;

    public void SetInfo(AudioSourceConfig sourceConfig)
    {
        this.sourceConfig = sourceConfig;
    }
}

