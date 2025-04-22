using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : InstanceMono<AudioManager>,IManager
{
    public static string MusicSourceManagerName = "MusicCHANNEL";
    public static string SFXSourceManagerName = "SFXCHANNEL";
    private GameObject MusicSourceManagerGO;
    private GameObject SFXSourceManagerGO;
    public List<AudioSource> ActiveAudioSource=new List<AudioSource>();
    public Stack<AudioSource> sourceCompleteStack = new Stack<AudioSource>();
    public override void Awake()
    {
        base.Awake();
        MusicSourceManagerGO=new GameObject(MusicSourceManagerName);
        SFXSourceManagerGO = new GameObject(SFXSourceManagerName);
        MusicSourceManagerGO.transform.SetParent(transform);
        SFXSourceManagerGO.transform.SetParent(transform);
        DontDestroyOnLoad(gameObject);
    }
    public void Init()
    {
        MyDebug.DebugPrint("Init AudioManager ...");
    }
    
    public void LoadScript(object args)
    {
        
    }

    public AudioSource PlayAudio(string clipPath)
    {
        AudioClip clip = PrefabDB.LoadAsset<AudioClip>(clipPath);
        return PlayAudio(clip);
    }
    public AudioSource PlayAudio(AudioClip clip, AudioType audioType = AudioType.SFX_2,Vector3 sourcePoint = default(Vector3),bool isLoop = false,AudioMixerGroup overrideMixerGroup=null)
    {
        AudioSourceConfig config = AudioSourceFactory.Instance.GetSourceConfig(audioType);
        return PlayAudio(clip,config,sourcePoint,isLoop,overrideMixerGroup);
    }
    public AudioSource PlayAudio(AudioClip clip,AudioSourceConfig sourceConfig,Vector3 sourcePoint = default(Vector3),bool isLoop = false,AudioMixerGroup overrideMixerGroup=null)
    {
        AudioSource audioSource = UseAudioSouce(sourceConfig.audioType);
        if (audioSource)
        {
            AudioSourceEntity audioSourceEntity = audioSource.GetComponent<AudioSourceEntity>();
            if (audioSourceEntity==null)
            {
                audioSourceEntity = audioSource.gameObject.AddComponent<AudioSourceEntity>();
            }
            audioSourceEntity?.SetInfo(sourceConfig);
        }
        UpdateAudioSourceSettingData(audioSource,sourceConfig);
        if (overrideMixerGroup!=null)
        {
            audioSource.outputAudioMixerGroup = overrideMixerGroup;
        }
        audioSource.transform.position = sourcePoint;
        audioSource.clip = clip;
        audioSource.loop = isLoop;
        audioSource.Play();
        SourceSyncChannel(audioSource, sourceConfig.audioType);
        return audioSource;
    }
    public void SourceSyncChannel(AudioSource audioSource,AudioType audioType)
    {
        AudioChannelConfig channelConfig = AudioSourceFactory.Instance.GetSourceChannel(audioType);
        audioSource.volume *= channelConfig.Volume;
    }
    //数值发生改变 或首次进入
    public void SyncChannel(AudioType audioType)
    {
        Transform channelManager=null;
        switch (audioType)
        {
            case AudioType.Music_2:
            case AudioType.Music_3:
                channelManager = MusicSourceManagerGO.transform;
                break;
            case AudioType.SFX_2:
            case AudioType.SFX_3:
                channelManager = SFXSourceManagerGO.transform;
                break;
            default:
                break;
        }
        AudioChannelConfig channelConfig = AudioSourceFactory.Instance.GetSourceChannel(audioType);
        foreach (Transform audioSouceTrans in channelManager)
        {
            var audioSourceEntity=audioSouceTrans.GetComponent<AudioSourceEntity>();
            if (audioSourceEntity)
            {
                UpdateAudioSourceSettingData(audioSourceEntity.audioSource,audioSourceEntity.sourceConfig);
                audioSourceEntity.audioSource.volume *= channelConfig.Volume;
            }
            
        }
    }
    public void PlayAudioOnShot()
    {
        
    }

    public void Update()
    {
        if (ActiveAudioSource.Count>0)
        {
            for (int i = 0; i < ActiveAudioSource.Count; i++)
            {
                if (ActiveAudioSource[i].isPlaying == false)
                {
                    AudioSourceFactory.Instance.PushItem(ActiveAudioSource[i]);
                    ActiveAudioSource.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    public AudioSource UseAudioSouce(AudioType audioType)
    {
        string suffix = "";
        switch (audioType)
        {
            case AudioType.Music_2:
            case AudioType.Music_3:
                suffix = MusicSourceManagerName;
                foreach (Transform bgmSourceTrans in MusicSourceManagerGO.transform)
                {
                    AudioSource bgmSource = bgmSourceTrans.GetComponent<AudioSource>();
                    if (bgmSource)
                    {
                         UnUseAudioSource(bgmSource);//BGM只能存在一个
                    }
                   
                }
                break;
            case AudioType.SFX_2:
            case AudioType.SFX_3:
                suffix = SFXSourceManagerName;
                break;
            default:
                break;
        }
        AudioSource audioSource = AudioSourceFactory.Instance.PopItem(transform.name+"/"+suffix);
        ActiveAudioSource.Add(audioSource);
        return audioSource;
    }
    public void UpdateAudioSourceSettingData(AudioSource audioSource,AudioSourceConfig config)
    {
        AudioSource prefabSource=config.audioSource;
        audioSource.outputAudioMixerGroup = prefabSource.outputAudioMixerGroup;
        audioSource.volume = prefabSource.volume;
        audioSource.spread = prefabSource.spread;
        audioSource.playOnAwake = prefabSource.playOnAwake;
        audioSource.pitch = prefabSource.pitch;
        audioSource.panStereo = prefabSource.panStereo;
        audioSource.maxDistance = prefabSource.maxDistance;
        audioSource.minDistance = prefabSource.minDistance;
        audioSource.priority = prefabSource.priority;
        audioSource.reverbZoneMix = prefabSource.reverbZoneMix;
        audioSource.spatialBlend = prefabSource.spatialBlend;
        audioSource.spatialize = prefabSource.spatialize;
        audioSource.spatializePostEffects = prefabSource.spatializePostEffects;
        audioSource.dopplerLevel = prefabSource.dopplerLevel;
    }
    public void UnUseAudioSource(AudioSource audioSource)
    {
        AudioSourceFactory.Instance.PushItem(audioSource);
        ActiveAudioSource.Remove(audioSource);
    }
}
public enum AudioType
{
    Music_2,//背景音乐只能存在一处
    Music_3,
    SFX_2,
    SFX_3,
    BGS,//背景音效 环境音
}
