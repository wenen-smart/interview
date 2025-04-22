using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

