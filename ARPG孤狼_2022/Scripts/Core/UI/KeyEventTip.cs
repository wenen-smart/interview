using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KeyEventTip : MonoBehaviour
{
    [SerializeField]
    private UILabel LabelInfo;
    [SerializeField]
    private UISprite containerBG;
    private Sequence animSeq;
    public void ShowTipContainer(string info)
    {
        LabelInfo.text = info;
        ShowTipContainer();
    }
    private void ShowTipContainer()
    {
        if (gameObject.activeSelf==false)
        {
            containerBG.fillAmount = 0;
            containerBG.alpha = 0;
            gameObject.SetActive(true);
            if (animSeq != null)
            {
                animSeq.Kill();
            }
            Sequence sequence = DOTween.Sequence();
            Tweener fillTweener = DOTween.To(() => containerBG.fillAmount, (targetValue) => containerBG.fillAmount = targetValue, 1, 0.1f);
            Tweener alphaTweener = DOTween.To(() => containerBG.alpha, (targetValue) => containerBG.alpha = targetValue, 1, 0.1f);
            sequence.Append(fillTweener);
            sequence.Join(alphaTweener);
            animSeq = sequence;
        }
    }
    public void HideTipContainer()
    {
        gameObject.SetActive(false);
    }
    
}
