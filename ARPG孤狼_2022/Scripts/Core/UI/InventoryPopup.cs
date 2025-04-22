using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class InventoryPopupDefinition : PopupDefinition
{
    public InventoryData inventoryData;
    public InventoryPopupDefinition(InventoryData inventoryData) : base(UIPanelIdentity.InventoryPanel)
    {
        this.inventoryData = inventoryData;
    }
}
public class InventoryPopup : BaseInventoryPopup
{
    [SerializeField]
    private UIGrid contentGrid;
    protected NGUI_InventorySlot[] soltList;

    private static InventoryPopup _instance;
    public static InventoryPopup Instance;
    public override void Enable()
    {
        base.Enable();
        CollectSlotItem();
        contentGrid.Reposition();
        InventoryPopupDefinition def = Definition as InventoryPopupDefinition;
        LoadSoltData(def.inventoryData);
    }
    public override void Exit()
    {
        base.Exit();
        CollectSlotItem();
        isPicking = false;
        PickItem.ReduceAmount(PickItem.Amount);
    }
    public void CollectSlotItem()
    {
        NGUI_Inventory_Item[] inventoryItems = GetComponentsInChildren<NGUI_Inventory_Item>();
        if (inventoryItems != null)
        {
            foreach (NGUI_Inventory_Item item in inventoryItems)
            {
                GameObjectFactory.Instance.PushItem(item.gameObject);
            }
        }
    }
    public override void Init()
    {
        base.Init();
        Instance = this;
        contentGrid.Reposition();
        int soltCount = contentGrid.transform.childCount;
        soltList = new NGUI_InventorySlot[soltCount];
        for (int i = 0; i < soltCount; i++)
        {
            Transform soltTransform = contentGrid.transform.GetChild(i);
            soltList[i]=soltTransform.GetComponent<NGUI_InventorySlot>();
        }
    }

    public void LoadSoltData(InventoryData inventoryData)
    {
        List<InventoryItemData> itemDatas=inventoryData.itemDatas;
        if (itemDatas==null)
        {
            return;
        }
        foreach (var item in itemDatas)
        {
            IterateSoltAndPut(item);
        }
    }
    public void IterateSoltAndPut(InventoryItemData inventoryItemData)
    {
        //找同类型
        //---容量是否满
        //找空solt
        //背包是否满
        int count = inventoryItemData.count;
        if (inventoryItemData.isHaveAEquip)
        {
            count--;
        }
        if (count>0)
        {
            SetItem(inventoryItemData.itemConfig,count);
        }
    }
    //public NGUI_InventorySlot FindEmptySolt()
    //{
    //    NGUI_InventorySlot solt=null;
    //    for (int i = 0; i < soltList.Length; i++)
    //    {
    //        if (soltList[i].IsEmpty())
    //        {
    //            solt = soltList[i];
    //            break;
    //        }
    //    }
    //    return solt;
    //}
    public void PickDownItemAll()
    {
        PickDownItem(PickItem.Amount);
    }
    public void PickDownItem(int count = 1)
    {
        if (PickItem.gameObject.activeSelf == false)
        {
            return;
        }
        PickItem.ReduceAmount(count);
        if (PickItem.Amount <= 0)
        {
            isPicking = false;
            PickItem.itemDataConfig = null;
        }
    }
    public void PickUpItem(ItemDataConfig item, int amount)
    {
        PickItem.SetItem(item, amount);
        PickItem.ItemSprite.depth = 100;
        isPicking = true;
        PickItem.gameObject.SetActive(true);
    }
    
    public void Update()
    {
        if (isPicking)
        {
            PickItem.transform.position = CameraMgr.ScreentPointToNGUIWorldPoint(Input.mousePosition);
        }
    }
    public void MatchCharacterSlot(EquipmentDataConfig itemDataConfig)
    {
        CharacterPopup characterPopup = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.CharacterPopup) as CharacterPopup;
        if (characterPopup == false || characterPopup.gameObject.activeInHierarchy == false)
        {
            return;
        }
        characterPopup.HighLightEquipment(itemDataConfig,SlotStateEnum.Match);
    }
    public void PutToCharacterPanel(NGUI_InventorySlot slot)
    {
        if (transform.childCount <= 0)
        {
            return;
        }
        CharacterPopup characterPopup = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.CharacterPopup) as CharacterPopup;
        if (characterPopup == false || characterPopup.gameObject.activeInHierarchy == false)
        {
            return;
        }

        NGUI_Inventory_Item currentItemTruly = slot.transform.GetChild(0).GetComponent<NGUI_Inventory_Item>();
        ItemDataConfig itemDataConfig = currentItemTruly.itemDataConfig;
        bool isCanEquip = false;
        
        if (itemDataConfig is EquipmentDataConfig)
        {
            EquipmentDataConfig equipConfig = itemDataConfig as EquipmentDataConfig;
            switch (equipConfig.GetEquipmentType())
            {
                case EquipmentType.Ornament:
                    isCanEquip=InventoryManager.Instance.PlayerEquip(equipConfig as OrnamentDataConfig);
                    characterPopup.EquipTheEquipment(equipConfig);
                        break;
                case EquipmentType.Weapon:
                    isCanEquip=InventoryManager.Instance.PlayerEquip(equipConfig as WeaponDataConfig);
                    characterPopup.EquipTheEquipment(equipConfig);
                    break;
                default:
                    break;
            }
            if (isCanEquip==false)
            {
                
                if (characterPopup)
                {
                    if (characterPopup.gameObject.activeInHierarchy)
                    {
                        characterPopup.HighLightEquipment(equipConfig,SlotStateEnum.Full);
                    }
                }
            }
        }
        if (isCanEquip)
        {
            currentItemTruly.ReduceAmount();
            Game.Instance.EquipItem(itemDataConfig.ITEM_Name);
        }
    }
}
