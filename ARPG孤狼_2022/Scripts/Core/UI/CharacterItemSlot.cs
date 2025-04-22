using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterItemSlot : NGUI_Slot
{
    [SerializeField]
    private UISprite SihouetteICON;
    [SerializeField]
    private UISprite slotFrame;
    [SerializeField]
    private GameObject itemTrulyPrefab;
    private NGUI_Inventory_Item itemTruly;
    [SerializeField]
    private UISprite markSprite;
    public override void SetItem(ItemDataConfig itemDataConfig, int count)
    {
        if (itemTruly!=null)
        {
            itemTruly.SetItem(itemDataConfig, count);
        }
        else
        {
            GameObject go = GameObjectFactory.Instance.PopItem(itemTrulyPrefab);
            go.transform.SetParent(transform,false);
            itemTruly = go.GetComponent<NGUI_Inventory_Item>();
            itemTruly.SetItem(itemDataConfig, count);
            itemTruly.ItemSprite.depth = 40;
            go.transform.localPosition = Vector3.zero;
        }
    }
    public override void ReduceItem(int count)
    {
        if (itemTruly)
        {
            if (itemTruly.ReduceAmount(count))
            {
                itemTruly = null;
            }
        }
    }
    public abstract bool Match(EquipmentDataConfig itemDataConfig);
    public Tweener highLightTween;
    public void HighLighting(SlotStateEnum slotHighLightEnum)
    {
        if (highLightTween!=null)
        {
            highLightTween.Kill();
        }
        switch (slotHighLightEnum)
        {
            case SlotStateEnum.Common:
                slotFrame.color = Color.white;
                break;
            case SlotStateEnum.Match:
                highLightTween = DOTween.To(()=>slotFrame.color,(targetColor)=> { slotFrame.color = targetColor; },Color.green,1).SetLoops(4,LoopType.Yoyo).OnComplete(()=> { slotFrame.color = Color.white;markSprite.gameObject.SetActive(false);});
                markSprite.gameObject.SetActive(false);
                markSprite.gameObject.SetActive(true);
                break;
            case SlotStateEnum.Full:
                highLightTween = DOTween.To(()=>slotFrame.color,(targetColor)=> { slotFrame.color = targetColor; },Color.red,1).SetLoops(4,LoopType.Yoyo).OnComplete(()=> { slotFrame.color = Color.white;markSprite.gameObject.SetActive(false);});
                markSprite.gameObject.SetActive(false);
                markSprite.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public override void OnPointerDown()
    {
        base.OnPointerDown();
        if (Input.GetMouseButtonDown(1)&&itemTruly!=null)
        {
            (UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.CharacterPopup) as CharacterPopup)?.UnEquipAndUpdateInventoryPopup(itemTruly.itemDataConfig);
            if (itemTruly.ReduceAmount(1))
            {
                itemTruly = null;
            }
        }
    }
}
public enum SlotStateEnum
{
    Common,
    Match,
    Full,
}
