using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AudioSourceFactory : ScriptSingleTon<AudioSourceFactory>,IObjectFactory<AudioSource>
{
    public void Init()
    {
        
    }
    public List<AudioSource> AllAudioSources=new List<AudioSource>();
    public AudioSource PopItem(string sourceParentName)
    {
        if (AllAudioSources==null)
        {
            AllAudioSources=new List<AudioSource>();
        }
        AudioSource popSource=null;
        if (AllAudioSources.Count>0)
        {
            foreach (var source in AllAudioSources)
            {
                if (source.gameObject.activeSelf==false)
                {
                    popSource = source;
                    break;
                }
            }
        }
        if (popSource==null)
        {
            GameObject sourceParentGO = GameObject.Find(sourceParentName);
            if (sourceParentGO==null)
            {
                sourceParentGO = new GameObject(sourceParentName);
            }
            GameObject sourceGO = new GameObject($"AudioSource_Emitter_{AllAudioSources.Count}");
            sourceGO.transform.SetParent(sourceParentGO.transform);
            popSource = sourceGO.AddComponent<AudioSource>();
            AllAudioSources.Add(popSource);
            popSource.playOnAwake = false;
        }
                popSource.gameObject.SetActive(true);
        return popSource;
    }

    public void PushItem(AudioSource objectEntity)
    {
        objectEntity.playOnAwake = false;
        objectEntity.Stop();
        objectEntity.gameObject.SetActive(false);
    }
    public AudioChannelConfig GetSourceChannel(AudioType audioType)
    {
        string path;
        switch (audioType)
        {
            case AudioType.Music_2:
            case AudioType.Music_3:
            default:
                path = PathDefine.MusicChannelSOPath;
                break;
            case AudioType.SFX_2:
            case AudioType.SFX_3:
                path = PathDefine.SFXChannelSOPath;
                break;
        }
        return ConfigResFactory.LoadConifg<AudioChannelConfig>(path);
    }
    public AudioSourceConfig GetSourceConfig(AudioType audioType)
    {
        string path;
        switch (audioType)
        {
            case AudioType.Music_2:
                path = PathDefine.Music_2D_SouceConfigSOPath;
                break;
            case AudioType.Music_3:
                path = PathDefine.Music_3D_SouceConfigSOPath;
                break;
            case AudioType.SFX_2:
                path = PathDefine.SFX_2D_SouceConfigSOPath;
                break;
            case AudioType.SFX_3:
                path = PathDefine.SFX_3D_SouceConfigSOPath;
                break;
            default:
                path = PathDefine.Music_2D_SouceConfigSOPath;
                break;
        }
        return ConfigResFactory.LoadConifg<AudioSourceConfig>(path);
    }
}

