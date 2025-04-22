using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

   public class Anim2TL_Solve
    {


    public static double SolveSameClipFade(Animator anim,string animationName,AnimatorStateInfo stateInfo,PlayableAsset playableAsset,int animLayer)
    {
        var normalizeTime= stateInfo.normalizedTime;
        double toPos = 0;
      AnimatorClipInfo[] animationClipInfos= anim.GetCurrentAnimatorClipInfo(0);
        if (animationClipInfos!=null)
        {
            AnimatorClipInfo currentClip = animationClipInfos[0];
            foreach (var item in animationClipInfos)
            {
                if (item.clip.name == animationName)
                {
                    currentClip = item;
                    break;
                }
            }
            if (currentClip.clip!=null)
            {
            normalizeTime = Mathf.Repeat(normalizeTime, 1);
                bool isFind = false;
            foreach (var item in playableAsset.outputs)
            {
                //bindingDics.Add(item.streamName,item);
                var trackedAsset = item.sourceObject as TrackAsset;
                if (trackedAsset!=null)
                {
                    foreach (var clip in trackedAsset.GetClips())
                    {
                        if (clip.displayName==animationName)
                        {
                            var start = clip.start;
                            
                            var duration = clip.duration;
                             toPos = start + duration * normalizeTime;
                            Debug.Log("TimeLine ToPos:"+toPos+" | DisplayName"+clip.displayName);
                                isFind = true;
                            break;
                        }
                    }

                        if (isFind)
                        {
                            break;
                        }
                }
            }
            }
        }
        return toPos;
    }
    }

