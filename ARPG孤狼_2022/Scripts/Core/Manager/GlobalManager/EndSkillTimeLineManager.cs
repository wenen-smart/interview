using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;
using UnityEditor;

public class EndSkillTimeLineManager:InstanceMono<EndSkillTimeLineManager>
    {
    public PlayableDirector director;

    public RuntimeAnimatorController attackerRAC;
    public RuntimeAnimatorController hiterRAC;

    public Animator attackerAnimator;
    public Animator hiterAnimator;

    public RoleController attackerController;
    public RoleController hiterController;

    public CinemachineVirtualCamera[] virtualCameras;
    
    private Vector3[] vcPosList;
    private bool endKillTLPlayed = false;
    public bool EndKillTLPlayed { get => TimeLineManager.Instance.TimeLineIsPlay; set => TimeLineManager.Instance.TimeLineIsPlay = value; }
    public Dictionary<string, PlayableBinding> bindingDic = new Dictionary<string, PlayableBinding>();
    public void PlayTimeLine(PlayableAsset playableAsset,double start=0,bool isStartPlay=true)
    {
        director.playableAsset = playableAsset;
        director.time = start;
        if (isStartPlay)
        {
            director.Play();
        }
    }
    public void Start()
    {
        director.played += OnPlayableDirectorPlayed;
        director.stopped += OnPlayableDirectorStopped;
        vcPosList = new Vector3[virtualCameras.Length];
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            vcPosList[i] = virtualCameras[i].transform.localPosition;
        }

    }

    public void AnimatorToTimeLine(Animator anim, PlayableAsset playableAsset, int animLayer,bool isStartPlay)
    {
        bindingDic.Clear();
        var stateInfo = anim.GetCurrentAnimatorStateInfo(animLayer);
        var normalizeTime = stateInfo.normalizedTime;
        double toPos = 0;
        PlayTimeLine(playableAsset, toPos,isStartPlay);             
        //AnimatorClipInfo[] animationClipInfos = anim.GetCurrentAnimatorClipInfo(0);
        //if (animationClipInfos != null&&animationClipInfos.Length>0)
        //{
        //    AnimatorClipInfo currentClip = animationClipInfos[1];

        //    foreach (var item in animationClipInfos)
        //    {
        //        if (item.clip!=null&& item.weight> currentClip.weight)
        //        {
        //            currentClip = item;
        //        }
        //    }
        //foreach (var item in animationClipInfos)
        //{
        //    if (item.clip.name == animationName)
        //    {
        //        currentClip = item;
        //        break;
        //    }
        //}
        //if (currentClip.clip != null)
        //{
        //normalizeTime = Mathf.Repeat(normalizeTime, 1);
        //bool isFind = false;、
        if (playableAsset&&playableAsset.outputs!=null)
        {
            foreach (var item in playableAsset.outputs)
                {
                    if (bindingDic.ContainsKey(item.streamName)==false)
                    {
                        bindingDic.Add(item.streamName, item);
                    }
            //var trackedAsset = item.sourceObject as TrackAsset;
            //if (trackedAsset != null)
            //{
            //    foreach (var clip in trackedAsset.GetClips())
            //    {
                    
                    //        //if (clip.displayName == currentClip.clip.name)
                    //        //{
                    //            var start = clip.start;

                    //            var duration = clip.duration;
                    //            toPos = start + duration * normalizeTime;
                    //            Debug.Log("TimeLine ToPos:" + toPos + " | DisplayName" + clip.displayName);
                    //        //    isFind = true;
                    //        //    break;
                    //        //}
                    //    }

                    //    if (isFind)
                    //    {
                    //        break;
                    //    }
                //}
            }
        }
        //    }
        //}
       
    }


    public void SetEndSkillTimeLine(RoleController attacker, RoleController hiter,PlayableAsset playableAsset,int animLayer=0)
    {
        if (director.state==PlayState.Playing)
        {
            return;
        }
        attackerController = attacker;
        hiterController = hiter;
        attackerAnimator =attacker.anim; 
        attackerAnimator.ResetTrigger(AnimatorParameter.Attack.ToString());
        attackerAnimator.SetInteger(AnimatorParameter.AttackAction.ToString(), 0);
        attackerRAC = attackerAnimator.runtimeAnimatorController;
        //attackerAnimator.runtimeAnimatorController = null;
         hiterAnimator = hiter.anim;
       
        hiterRAC = hiterAnimator.runtimeAnimatorController;
        GameRoot.Instance.AddTimeTask(800,()=> {hiterAnimator.runtimeAnimatorController = null; attackerAnimator.runtimeAnimatorController = null; });
        //hiterAnimator.runtimeAnimatorController = null;

        //如果在绑定前播放会导致Cinemachine片段在asset第一次执行的时候无法切换镜头，我估计是因为片段注册了TimeLinePlay事件，在TimelinePlay时候进行了某些预计算。如果我们未进行绑定就进行播放，会导致相关预计算失败，导致无法切换，按理来说Play的事件执行 应该延迟到实际动画第一帧的时候才去执行，可能官方为了用户更清楚的理解，跟状态标识绑定在一起了。
        //这也就解释了该PlayableAsset在第二次执行时 可以正常切换的问题。因为我们先前绑定过了。在第二次进入播放的时候 asset已经绑定了上一次留有的相机。 （测试：每次播放之前清空一下绑定 结果：证明成功！第N次都不会切换）
        //ClearcBinder(playableAsset);
        //AnimatorToTimeLine(attackerAnimator, playableAsset,animLayer,true);
        //DynameicBinder(attacker,hiter,playableAsset);
        
        AnimatorToTimeLine(attackerAnimator, playableAsset, animLayer, false);//先设置资源到TImeLine
        DynameicBinder(attacker, hiter, playableAsset);//资源设置完才去动态绑定
        Debug.Log("Binder:"+Time.frameCount);
        director.Play();//绑定完再去播放
        Debug.Log("PlayInvoke"+Time.frameCount);
    }
    public void DynameicBinder(RoleController attacker, RoleController hiter,PlayableAsset playableAsset)
    {
        if (!(bindingDic.ContainsKey(Constants.attackerTrackName) && bindingDic.ContainsKey(Constants.hiterTrackName)))
        {
            return;
        }
        director.SetGenericBinding(bindingDic[Constants.attackerTrackName].sourceObject, attackerAnimator);
        director.SetGenericBinding(bindingDic[Constants.hiterTrackName].sourceObject, hiterAnimator);
        if (bindingDic.ContainsKey(Constants.attackerSetTrans))
        {
            director.SetGenericBinding(bindingDic[Constants.attackerSetTrans].sourceObject, attackerController);
        }
        if (bindingDic.ContainsKey(Constants.hiterSetTrans))
        {
            director.SetGenericBinding(bindingDic[Constants.hiterSetTrans].sourceObject, hiterController);
        }
        if (bindingDic.ContainsKey(Constants.hiterRootMotion))
        {
            director.SetGenericBinding(bindingDic[Constants.hiterRootMotion].sourceObject, hiterAnimator);
        }
        if (bindingDic.ContainsKey(Constants.attackerRootMotion))
        {
            director.SetGenericBinding(bindingDic[Constants.attackerRootMotion].sourceObject, attackerAnimator);
        }
        if (bindingDic.ContainsKey(Constants.CinemachineBrainTrack))
        {
            director.SetGenericBinding(bindingDic[Constants.CinemachineBrainTrack].sourceObject, CameraMgr.Instance.brain);
        }

        foreach (var item in playableAsset.outputs)
        {
            var trackAsset = item.sourceObject as TrackAsset;
            if (item.streamName.Equals(Constants.hiterSetTrans))
            {
                Debug.Log("hiterSetTrans");
                foreach (var clipasset in trackAsset.GetClips())
                {
                    var setTransAsset = clipasset.asset as Pd_SetTransAsset;

                    setTransAsset.toAnotherActor.defaultValue = attacker;
                }
            }
            else if (item.streamName.Equals(Constants.attackerSetTrans))
            {
                foreach (var clipasset in trackAsset.GetClips())
                {
                    var setTransAsset = clipasset.asset as Pd_SetTransAsset;

                    setTransAsset.toAnotherActor.defaultValue = hiter;
                }
            }
            else if (item.streamName.Equals(Constants.CinemachineBrainTrack))
            {
                foreach (var clipasset in trackAsset.GetClips())
                {
                    var cinemachineShotAsset = clipasset.asset as CinemachineShot;

                    foreach (var virtualCamera in virtualCameras)
                    {
                        if (clipasset.displayName == virtualCamera.name)
                        {

                            //cinemachineShotAsset.VirtualCamera = new ExposedReference<CinemachineVirtualCameraBase>() { defaultValue = virtualCamera };
                            cinemachineShotAsset.VirtualCamera.defaultValue = virtualCamera;
                        }
                    }
                }
            }
        }
    }
    public void ClearcBinder(PlayableAsset playableAsset)
    {
        if (!(bindingDic.ContainsKey(Constants.attackerTrackName) && bindingDic.ContainsKey(Constants.hiterTrackName)))
        {
            return;
        }
        director.SetGenericBinding(bindingDic[Constants.attackerTrackName].sourceObject, null);
        director.SetGenericBinding(bindingDic[Constants.hiterTrackName].sourceObject, null);
        if (bindingDic.ContainsKey(Constants.attackerSetTrans))
        {
            director.SetGenericBinding(bindingDic[Constants.attackerSetTrans].sourceObject, null);
        }
        if (bindingDic.ContainsKey(Constants.hiterSetTrans))
        {
            director.SetGenericBinding(bindingDic[Constants.hiterSetTrans].sourceObject, null);
        }
        if (bindingDic.ContainsKey(Constants.hiterRootMotion))
        {
            director.SetGenericBinding(bindingDic[Constants.hiterRootMotion].sourceObject, null);
        }
        if (bindingDic.ContainsKey(Constants.attackerRootMotion))
        {
            director.SetGenericBinding(bindingDic[Constants.attackerRootMotion].sourceObject, null);
        }
        if (bindingDic.ContainsKey(Constants.CinemachineBrainTrack))
        {
            director.SetGenericBinding(bindingDic[Constants.CinemachineBrainTrack].sourceObject, null);
        }

        foreach (var item in playableAsset.outputs)
        {
            var trackAsset = item.sourceObject as TrackAsset;
            if (item.streamName.Equals(Constants.hiterSetTrans))
            {
                Debug.Log("hiterSetTrans");
                foreach (var clipasset in trackAsset.GetClips())
                {
                    var setTransAsset = clipasset.asset as Pd_SetTransAsset;

                    setTransAsset.toAnotherActor.defaultValue = null;
                }
            }
            else if (item.streamName.Equals(Constants.attackerSetTrans))
            {
                foreach (var clipasset in trackAsset.GetClips())
                {
                    var setTransAsset = clipasset.asset as Pd_SetTransAsset;

                    setTransAsset.toAnotherActor.defaultValue = null;
                }
            }
            else if (item.streamName.Equals(Constants.CinemachineBrainTrack))
            {
                foreach (var clipasset in trackAsset.GetClips())
                {
                    var cinemachineShotAsset = clipasset.asset as CinemachineShot;

                    cinemachineShotAsset.VirtualCamera.defaultValue = null;
                }
            }
        }
    }
    public void OnPlayableDirectorPlayed(PlayableDirector playableDirector)
    {
        Debug.Log("PlayableDirectorPlayerCallback"+Time.frameCount);
        EndKillTLPlayed = true;

        attackerController.DisableCtrl();

        var attackerAnim = attackerAnimator.GetComponent<CharacterAnim>();
        attackerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
        attackerAnim.SetInt(AnimatorParameter.AttackAction.ToString(), 0);
        attackerAnim.SwitchEnableAttack(0);
        attackerController.ClearIncreaseVelo();
        attackerController.ClearMoveSpeed();

        var hiterAnim = hiterAnimator.GetComponent<CharacterAnim>();
        hiterAnim.SwitchEnableAttack(0);
        hiterController.DisableCtrl();
        hiterController.ClearIncreaseVelo();
        hiterController.ClearMoveSpeed();

        SetTargetGroupTarget(hiterController.centerTrans.transform);
    }

    public void OnPlayableDirectorStopped(PlayableDirector playableDirector)
    {
        playableDirector.Evaluate();
        attackerController.EnableCtrl();
        var attackerAnim = attackerAnimator.GetComponent<CharacterAnim>();
       

        var hiterAnim = hiterAnimator.GetComponent<CharacterAnim>();
     

        hiterController.DisableCtrl(false);
       
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].ForceCameraPosition(virtualCameras[i].transform.parent.TransformPoint(vcPosList[i]), virtualCameras[i].transform.rotation);
        }
        AnimationClip[] clips =  FindLastClipInTimeLineTargetTrack(hiterAnimator,PlayState.Paused);
        playableDirector.playableAsset = null;
        
        attackerAnimator.runtimeAnimatorController = attackerRAC;
        hiterAnimator.runtimeAnimatorController = null;
        hiterController.EnableRagDoll(new IDamageable.DamageData());
        clips[0].SampleAnimation(hiterAnimator.gameObject,1);
        hiterAnimator.enabled = false;
        hiterAnimator.speed = 0;
        GameRoot.Instance.AddTask(20, () =>
         {
             attackerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
             attackerAnim.SetInt(AnimatorParameter.AttackAction.ToString(), 0);
             hiterAnim.SetInt(AnimatorParameter.HitAction.ToString(), 0);
             ResetData();
         });
        attackerAnim.CloseNextAttackVaildSign();
        EndKillTLPlayed = false;

        
    }
    public void ResetData()
    {
        attackerAnimator = null;
        hiterAnimator = null;
        attackerController = null;
        hiterController = null;
        attackerRAC=null;
        hiterRAC=null;
    }

    public CinemachineTargetGroup targetGroup;

   

    public void SetTargetGroupTarget(Transform target)
    {
        targetGroup.m_Targets[1].target = target;
    }
    /// <summary>
    /// -1为当前时间 time[0,1] time
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="state"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public AnimationClip[] GetAnimatorClipInTimeLinePlay(Animator animator)
    {
        AnimationClip[] animationClips = null;
        string playerableTrackTag = "";
        if (animator == attackerAnimator)
        {
            playerableTrackTag = Constants.attackerTrackName;
        }
        else if (animator == hiterAnimator)
        {
            playerableTrackTag = Constants.hiterTrackName;
        }
        if (string.IsNullOrEmpty(playerableTrackTag) == false)
        {
            foreach (var bind in director.playableAsset.outputs)
            {
                TrackAsset trackAsset = bind.sourceObject as TrackAsset;
                if (bind.streamName == playerableTrackTag && director.GetGenericBinding(bind.sourceObject) == animator)
                {
                    if (trackAsset.isEmpty == false && trackAsset.muted == false)
                    {
                        if (director.isActiveAndEnabled && director.state == PlayState.Playing)
                        {
                            double currentTime = director.time;
                            double duration = director.duration;
                            IEnumerable<TimelineClip> timelineClips = trackAsset.GetClips();//动画片段
                            List<TimelineClip> currentClips = new List<TimelineClip>();
                            foreach (var timeLineClip in timelineClips)
                            {
                                if (timeLineClip.start <= currentTime && timeLineClip.end > currentTime)
                                {
                                    //当前所播的片段  
                                    //存在融合片段，还需要计算权重。 这里先不算融合了
                                    currentClips.Add(timeLineClip);
                                }
                            }
                            if (currentClips.Count > 0)
                            {
                                if (currentClips.Count > 1)
                                {
                                    currentClips.Sort();//默认从小到大
                                }
                                animationClips = currentClips.Select<TimelineClip, AnimationClip>((timeLineClip) => { return timeLineClip.animationClip; }).ToArray();
                            }
                        }
                    }
                }
            }
        }
        return animationClips;
    }
public AnimationClip[] FindLastClipInTimeLineTargetTrack(Animator animator, PlayState state = PlayState.Playing)
    {
        AnimationClip[] animationClips = null;
        string playerableTrackTag = "";
        if (animator == attackerAnimator)
        {
            playerableTrackTag = Constants.attackerTrackName;
        }
        else if (animator == hiterAnimator)
        {
            playerableTrackTag = Constants.hiterTrackName;
        }
        if (string.IsNullOrEmpty(playerableTrackTag) == false)
        {
            foreach (var bind in director.playableAsset.outputs)
            {
                TrackAsset trackAsset = bind.sourceObject as TrackAsset;
                if (bind.streamName == playerableTrackTag && director.GetGenericBinding(bind.sourceObject) == animator)
                {
                    if (trackAsset.isEmpty == false && trackAsset.muted == false)
                    {
                        if (director.isActiveAndEnabled && (state == director.state))
                        {
                            List<TimelineClip> timelineClips = trackAsset.GetClips().ToList();//动画片段
                            List<TimelineClip> currentClips = new List<TimelineClip>();
                            currentClips.Add(timelineClips[timelineClips.Count-1]);
                            if (currentClips.Count > 0)
                            {
                                if (currentClips.Count > 1)
                                {
                                    currentClips.Sort();//默认从小到大
                                }
                                animationClips = currentClips.Select<TimelineClip, AnimationClip>((timeLineClip) => { return timeLineClip.animationClip; }).ToArray();
                            }
                        }
                    }
                }
            }
        }
        return animationClips;
    }
}

