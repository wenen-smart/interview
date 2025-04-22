using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FloatDialogPopupDefinition : PopupDefinition
{
    public float showTime;
    public DialogItemData dialogData;
    public FloatDialogPopupDefinition(DialogItemData itemData,float showTime,UILayerType uILayerType=UILayerType.TopPopup,bool isPauseOtherPanel=true) : base(UIPanelIdentity.FloatDialogPanel,uILayerType,isPauseOtherPanel)
    {
        dialogData = itemData;
        this.showTime = showTime;
    }
}

public class FloatDialogWnd : BasePanel
{
    private int dialogIndex;
    private float a_showTime;
    private DialogItemData dialogData;
    [SerializeField]
    private UILabel floatLabel;
    public Action DialogFinish;
    private Sequence sequence=null;
    public override void Enable()
    {
        base.Enable();
        floatLabel.alpha = 0;
        var def = Definition as FloatDialogPopupDefinition;
        this.dialogData = def.dialogData;
        a_showTime = def.showTime;
        dialogIndex = -1;
        UpdateDialog();
    }
    public override void Pause()
    {
        base.Pause();
    }
    public override void Exit()
    {
        base.Exit();
        if (sequence != null)
        {
            sequence.Kill();
        }
    }
    public void UpdateDialog()
    {
        if (dialogIndex < 0)
        {
            dialogIndex = 0;
        }
        else
        {
            dialogIndex += 1;
        }
        if (dialogIndex>=dialogData.Count)
        {
            DialogFinish?.Invoke();
            Hide();
            return;
        }
        floatLabel.text = dialogData.dialogs[dialogIndex];
        if (sequence!=null)
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();
        var show = DOTween.To(()=>floatLabel.alpha,(targetAlpha)=> { floatLabel.alpha = targetAlpha; },1,a_showTime*0.1f);
        var disappear=DOTween.To(()=>floatLabel.alpha,(targetAlpha)=> { floatLabel.alpha = targetAlpha; },0,1f).SetDelay(a_showTime-a_showTime*0.1f).OnComplete(UpdateDialog);
        sequence.Append(show);
        sequence.Append(disappear);
        sequence.SetAutoKill(true);
    }
}
