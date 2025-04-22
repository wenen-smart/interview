using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomeOptionPopup : BasePanel
{
    [SerializeField] private UIWidget container;
    [SerializeField] private AudioChannelConfig MusicChannelConfig;
    [SerializeField] private AudioChannelConfig SFXChannelConfig;
    [SerializeField] private UISlider MusicVolumeSlider;
    [SerializeField] private UISlider SFXVolumeSlider;
    public override void Init()
    {
        base.Init();
        MusicVolumeSlider.onChange.Add(new EventDelegate(() => { SetMusicVolume();}));
        SFXVolumeSlider.onChange.Add(new EventDelegate(() => { SetSFXVolume();}));
    }
    public override void Enable()
    {
        base.Enable();
        container.alpha = 0;
        transform.localPosition = Vector3.right*-200;
        DOTween.To(()=>container.alpha,(targetAlpha)=> { container.alpha = targetAlpha;},1,2).OnComplete((()=> { })).SetAutoKill();
        DOTween.To(()=>transform.localPosition,(pos)=> { transform.localPosition = pos;},Vector3.zero,1).OnComplete((()=> { })).SetAutoKill();
    }
    public void OnClickNewGame()
    {
        Game.Instance.StartGame(false);
    }
    public void OnClickContinueGame()
    {
        Game.Instance.StartGame(true);
    }
    public void OnClickSetting()
    {

    }
    public override void Exit()
    {
        DOTween.To(()=>container.alpha,(targetAlpha)=> { container.alpha = targetAlpha;},0,1).OnComplete((()=> { base.Exit();})).SetAutoKill();
    }
    public void SetMusicVolume()
    {
        MusicChannelConfig.SetVolume(MusicVolumeSlider.value);
        AudioManager.Instance.SyncChannel(AudioType.Music_2);
        AudioManager.Instance.SyncChannel(AudioType.Music_3);
    }
    public void SetSFXVolume()
    {
        SFXChannelConfig.SetVolume(SFXVolumeSlider.value);
        AudioManager.Instance.SyncChannel(AudioType.SFX_2);
        AudioManager.Instance.SyncChannel(AudioType.SFX_3);
    }
}
