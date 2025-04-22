using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 制定规则 凡是角色的都在角色下创建Audio物体对象 所有Cur挂载到这个audio上
/// </summary>
public class AudioClipCue:MonoBehaviour
{
    public bool isPlayAwake = false;
    [Header("片段标识符")]
    public string identity;
#if UNITY_EDITOR
    [TextArea(0,3),SerializeField]
    private string description="填写描述";
#endif
    public AudioClipGroup ClipConfig;
    private AudioClip clip;
    private int[] clipPlayRecordGroup;
    public AudioSourceConfig SourceConfig;
    private List<AudioSource> useAudioSouces=new List<AudioSource>();
    public void OnEnable()
    {
        if (isPlayAwake)
        {
            GameRoot.Instance?.AddFrameTask(1,()=> { Play();});
        }
    }
    public void Play()
    {
        RefreshUseAudioSouces();
        if (clipPlayRecordGroup==null||clipPlayRecordGroup.Length==0)
        {
            clipPlayRecordGroup = new int[ClipConfig.audioClipGroups.Count];
        }
        for (int i = 0; i < ClipConfig.audioClipGroups.Count; i++)
        {
            var clipItem = ClipConfig.audioClipGroups[i];
            if (clipItem.clips.Count <= 0)
            {
                continue;
            }
            if (clipItem.clips.Count == 1)
            {
               useAudioSouces.Add(AudioManager.Instance.PlayAudio(clipItem.clips[0],SourceConfig,transform.position,ClipConfig.isLoop,ClipConfig.overrideMixerGroup));
            }
            else
            {
                int index = 0;
                switch (clipItem.sequenceMode)
                {
                    case AudioClipSequenceMode.Random:
                        index = RandomClip(clipItem.clips);
                        break;
                    case AudioClipSequenceMode.RandomNoImmediatelyRepeat:
                        index = RandomClipButImmNoReapeat(clipPlayRecordGroup[i],clipItem.clips);
                        break;
                    case AudioClipSequenceMode.Sequence:
                        index = SequenceClip(clipPlayRecordGroup[i],clipItem.clips);
                        break;
                    default:
                        break;
                }
                clipPlayRecordGroup[i] = index;
               useAudioSouces.Add(AudioManager.Instance.PlayAudio(clipItem.clips[clipPlayRecordGroup[i]],SourceConfig,transform.position,ClipConfig.isLoop,ClipConfig.overrideMixerGroup));
            }
        }
    }
    public void RefreshUseAudioSouces()
    {
        for (int i = 0; i < useAudioSouces.Count; i++)
        {
            if (useAudioSouces[i].isPlaying==false)
            {
                AudioManager.Instance.UnUseAudioSource(useAudioSouces[i]);
                useAudioSouces.RemoveAt(i);
                i--;
            }
        }
    }
    public void PauseAudio()
    {
        RefreshUseAudioSouces();
        foreach (var item in useAudioSouces)
        {
            item.Pause();
        }
    }
    public bool IsPlay
    {
        get
        {
            foreach (var item in useAudioSouces)
            {
                if (item.isPlaying == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
    private int RandomClip(List<AudioClip> clips)
    {
        return UnityEngine.Random.Range(0,clips.Count);
    }
    private int RandomClipButImmNoReapeat(int sequenceIndex,List<AudioClip> clips)
    {
        int index = 0;
        if (clips.Count<=2)
        {
            index = sequenceIndex;
            index += 1;
            index /= clips.Count;
        }else
        {
            if (sequenceIndex==clips.Count-1)
            {
                index=UnityEngine.Random.Range(0, sequenceIndex);
            }
            else
            {
                int isLeft=UnityEngine.Random.Range(0,2);//分为两段
                if (isLeft==1)
                {
                    index=UnityEngine.Random.Range(0,sequenceIndex);
                }
                else
                {
                    index=UnityEngine.Random.Range(sequenceIndex+1,clips.Count);
                }
            }
        }
        return index;
    }
    private int SequenceClip(int sequenceIndex,List<AudioClip> clips)
    {
        return (sequenceIndex + 1) % clips.Count;
    }
    public static void Play(Transform caster,string curIdentity,bool isFindChild)
    {
        AudioClipCue[] curList = null;
        if (isFindChild)
        {
            curList=caster.GetComponentsInChildren<AudioClipCue>();
        }
        else
        {
            curList=caster.GetComponents<AudioClipCue>();
        }
        if (curList==null)
        {
            return;
        }
        foreach (var item in curList)
        {
            if (item.identity==curIdentity)
            {
                item.Play();
                return;
            }
        }
    }
}

