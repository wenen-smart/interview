using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GainItemContainer : MonoBehaviour
{
    [SerializeField]
    private UILabel LabelInfo;
    [SerializeField]
    private UISprite containerBG;
    private Sequence animSeq;
    public void ShowGainItemContainer(string info)
    {
        LabelInfo.text = info;
        ShowGainItemContainer();
    }
    private void ShowGainItemContainer()
    {
        gameObject.SetActive(true);
        containerBG.fillAmount = 0;
        containerBG.alpha = 0;
        if (animSeq!=null)
        {
            animSeq.Kill();
        }
        Sequence sequence = DOTween.Sequence();
        Tweener fillTweener = DOTween.To(()=>containerBG.fillAmount,(targetValue)=>containerBG.fillAmount=targetValue,1,1);
        Tweener alphaTweener = DOTween.To(()=>containerBG.alpha,(targetValue)=>containerBG.alpha=targetValue,1,0.5f);
        sequence.Append(fillTweener);
        sequence.Join(alphaTweener);
        sequence.OnComplete(()=> { GameRoot.Instance.AddTimeTask(2000, HideGainItemContainer); });
        animSeq = sequence;
    }
    public void HideGainItemContainer()
    {
        if (animSeq != null)
        {
            animSeq.Kill();
        }
        Sequence sequence = DOTween.Sequence();
        Tweener fillTweener = DOTween.To(() => containerBG.fillAmount, (targetValue) => containerBG.fillAmount = targetValue, 0, 1);
        Tweener alphaTweener = DOTween.To(() => containerBG.alpha, (targetValue) => containerBG.alpha = targetValue, 0, 1f);
        sequence.Append(fillTweener);
        sequence.Join(alphaTweener);
        sequence.OnComplete(()=> { gameObject.SetActive(false); });
        animSeq = sequence;
    }
}
