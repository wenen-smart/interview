using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BaseInventoryPopup:BasePanel
{
    #region 常量
    private const int fadeSpeed = 2;
    #endregion

    #region 事件
    #endregion

    #region 字段
    public NGUI_Slot[] slotList;
    public static BaseInventoryPopup CurrentPanelWhenPick;
    public bool isPicking = false;
    private NGUI_Inventory_Item pickItem;

    public NGUI_Inventory_Item PickItem
    {
        get
        {
            if (pickItem==null)
            {
                pickItem = GameObjectFactory.Instance.PopItem(PathDefine.Inventory_ItemPath).GetComponent<NGUI_Inventory_Item>();
                pickItem.gameObject.AddComponent<UIPanel>().depth = 100;
                pickItem.transform.SetParent(transform,false);
            }
            return pickItem;
        }
        set => pickItem = value;
    }
    #endregion

    #region 属性
    #endregion

    #region 方法
    public void SetItem(int id)
    {
        ItemDataConfig itemDataConfig=null;
        itemDataConfig=ItemDataConfigManager.GetItemDataConfig(id);
        SetItem(itemDataConfig);
    }
    public override void Init()
    {
        base.Init();
        slotList = GetComponentsInChildren<NGUI_Slot>();
    }
    public void SetItem(ItemDataConfig item, int amount = 1)
    {
        if (item.capacity != 1)
        {
            NGUI_Slot slot = FindSameItem(item.id);
            if (slot != null)
            {
                NGUI_Inventory_Item itemTruly = slot.transform.GetChild(0).GetComponent<NGUI_Inventory_Item>();

                int remianAmount = itemTruly.itemDataConfig.capacity - itemTruly.Amount;
                if (amount > remianAmount)
                {
                    amount -= remianAmount;//去得到一个 多余量
                    itemTruly.AddAmount(remianAmount);//添加到 当前槽的最大量
                    SetItem(item, amount);//把剩下多出来的再去回调  
                    return;
                }
                itemTruly.AddAmount(amount);//有相同的且容量美满的才去加   否则就是 找空槽去实例化一个物品出来
                return;
            }

        }
        NGUI_Slot emptySlot = FindEmptyItem();
        if (emptySlot == null)
        {
            Debug.LogWarning("满了");
        }
        else
        {
            if (amount > item.capacity)
            {
                amount -= item.capacity;//多余量
                emptySlot.SetItem(item, item.capacity);//把最大的容量设置到当前物品槽中
                SetItem(item, amount);
                return;
            }
            emptySlot.SetItem(item, amount);
        }
    }

    private NGUI_Slot FindSameItem(int id)
    {
        foreach (var slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                NGUI_Inventory_Item itemTruly = slot.transform.GetChild(0).GetComponent<NGUI_Inventory_Item>();
                if (itemTruly.itemDataConfig.id == id)
                {
                    if (itemTruly.itemDataConfig.capacity > itemTruly.Amount)
                    {
                        return slot;
                    }

                }
            }
        }
        return null;
    }
    private NGUI_Slot FindEmptyItem()
    {

        foreach (var slot in slotList)
        {
            if (slot.transform.childCount <= 0)
            {
                return slot;
            }
        }
        return null;
    }
    #endregion

    #region Unity回调
    protected virtual void Start()
    {
        
        //Hide();
    }
    private void Update()
    {
        //if (canvasGroup.alpha != tarAlpha)
        //{
        //    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, tarAlpha, Time.deltaTime * fadeSpeed);
        //    if (Mathf.Abs(canvasGroup.alpha - tarAlpha) < 0.05f)
        //    {
        //        canvasGroup.alpha = tarAlpha;
        //        canvasGroup.blocksRaycasts = canvasGroup.alpha == 0 ? false : true;

        //    }

        //}
    }
}
#endregion