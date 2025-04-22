using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NGUI_InventorySlot:NGUI_Slot
{
    [SerializeField]
    private GameObject itemTrulyPrefab;
    private NGUI_Inventory_Item itemTruly; 

    
    public override void SetItem(ItemDataConfig itemDataConfig,int count)
    {
        if (transform.childCount>0)
        {
            itemTruly.SetItem(itemDataConfig,count);
        }
        else
        {
            GameObject go = GameObjectFactory.Instance.PopItem(itemTrulyPrefab);
            go.transform.SetParent(transform, false);
            itemTruly =go.GetComponent<NGUI_Inventory_Item>();
            itemTruly.SetItem(itemDataConfig, count);
            go.transform.localPosition = Vector3.zero;
        }
        itemTruly.gameObject.SetActive(false);
        itemTruly.gameObject.SetActive(true);
    }
    public string GetCountStr(int count)
    {
        if (count<=1)
        {
            return "";
        }
        return "x" + count.ToString();
    }
    //public void RenewSolt()
    //{
    //    itemCount = 0;
    //}

    
    public override void OnPointerExit(/*PointerEventData eventData*/)
    {
        //hide tooltip；
        //InventoryManager.Instance.HideToolTip();
    }

    public override void OnPointerEnter(/*PointerEventData eventData*/)
    {
        string content = null;

        if (transform.childCount > 0)
        {
            content = itemTruly.itemDataConfig.GetTextContent();
            //InventoryManager.Instance.ShowToolTip(content);
        }
    }

    public override void OnPointerDown(/*PointerEventData eventData*/)
    {
        if (Input.GetMouseButton(1))
        {
            if (transform.childCount > 0)
            {
                NGUI_Inventory_Item currentItemTruly = transform.GetChild(0).GetComponent<NGUI_Inventory_Item>();

                //CharacterPanel.Instance.MatchSlot(currentItemTruly);
                InventoryPopup.Instance.PutToCharacterPanel(this);

            }
            return;

        }

        //下面有物体
         InventoryPopup im= UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.InventoryPanel) as InventoryPopup;
        if (transform.childCount > 0)
        {

            NGUI_Inventory_Item currentTrulyItem = transform.GetChild(0).GetComponent<NGUI_Inventory_Item>();

            //手上有
            if (im.isPicking)
            {

                //id相同
                if (currentTrulyItem.itemDataConfig.id == im.PickItem.itemDataConfig.id)
                {
                    //判断容量满没满
                    int remainAmount = currentTrulyItem.itemDataConfig.capacity - currentTrulyItem.Amount;
                    if (remainAmount <= 0)
                    {
                        return;//满了就return
                    }

                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        //按下ctrl
                        currentTrulyItem.AddAmount();
                        im.PickDownItem();
                    }
                    else
                    {
                        //没有按ctrl
                        if (remainAmount > im.PickItem.Amount)
                        {
                            currentTrulyItem.AddAmount(im.PickItem.Amount);
                            im.PickDownItem(im.PickItem.Amount);
                        }
                        else
                        {
                            currentTrulyItem.AddAmount(remainAmount);
                            im.PickDownItem(remainAmount);
                        }
                    }
                }
                else
                {
                    currentTrulyItem.Exchange(im.PickItem);
                }
            }
            else
            {
                //手上没有
                ItemDataConfig item = currentTrulyItem.itemDataConfig;
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    int pickAmount = (currentTrulyItem.Amount + 1) / 2;
                    im.PickUpItem(item: currentTrulyItem.itemDataConfig, amount: pickAmount);
                    currentTrulyItem.ReduceAmount(pickAmount);
                }
                else
                {
                    //全部取
                    int _Amount = currentTrulyItem.Amount;
                    if (currentTrulyItem.itemDataConfig is EquipmentDataConfig)
                    {
                        InventoryPopup.Instance.MatchCharacterSlot(currentTrulyItem.itemDataConfig as EquipmentDataConfig);
                    }
                    currentTrulyItem.ReduceAmount(currentTrulyItem
                        .Amount);
                    im.PickUpItem(item, _Amount);
                    
                }
            }
        }
        else
        {
            //没有物体
            if (im.isPicking)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    this.SetItem(im.PickItem.itemDataConfig, count: 1);
                    im.PickDownItem(count: 1);
                }
                else
                {
                    this.SetItem(im.PickItem.itemDataConfig, im.PickItem.Amount);
                    im.PickDownItem(im.PickItem.Amount);
                }
            }
            else
            {
                Debug.LogWarning("手上槽上没有物品");
            }
        }

    }
}

