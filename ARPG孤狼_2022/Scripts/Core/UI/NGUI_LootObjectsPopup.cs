using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootObjectsPopupDefinition : PopupDefinition
{
    public List<DropItemObject> itemObjects;
    public static string itemPrefabPath = "Prefab/UI/LootWndItem";
    public LootObjectsPopupDefinition(List<DropItemObject> itemObjects) : base(UIPanelIdentity.LootItemPanel)
    {
        this.itemObjects = itemObjects;
    }
}
public class NGUI_LootObjectsPopup : BasePanel
{
    [SerializeField]
    private UIGrid Content_Grid;
    [SerializeField]
    private UIButton pickAllItemBtn;
    [SerializeField]
    private UIButton backListBtn;//收缩
    [SerializeField]
    private UIButton spreadListBtn;//展开
    [SerializeField]
    private Animator popupAnim;
    private List<LootPopupItem> lootPopupItems=new List<LootPopupItem>();
    private bool isBack = false;
    public override void Enable()
    {
        base.Enable();
        foreach (var item in lootPopupItems)
        {
            GameObjectFactory.Instance.PushItem(item.gameObject);
        }
        LootObjectsPopupDefinition lootPopupDef = Definition as LootObjectsPopupDefinition;
        List<DropItemObject> itemObjects=lootPopupDef.itemObjects;
        foreach (var itemObject in itemObjects)
        {
            if (itemObject.itemPacker==null)
            {
                MyDebug.DebugWarning($"{itemObject}的pack为null");
                continue;
            }
            List<DropItemData> dropItemKDatas = itemObject.itemPacker.dropItemKindDatas;
            foreach (var itemData in dropItemKDatas)
            {
                GameObject itemGO = GameObjectFactory.Instance.PopItem(LootObjectsPopupDefinition.itemPrefabPath);
                itemGO.transform.SetParent(Content_Grid.transform, false);
                LootPopupItem lootPopupItem = itemGO.GetComponent<LootPopupItem>();
                lootPopupItem.SetInfo(itemObject,itemData,OnPickItem);
                lootPopupItem.SetCount(itemData.Count);
                lootPopupItems.Add(lootPopupItem);
            }
        }
        pickAllItemBtn.onClick.Clear();
        pickAllItemBtn.onClick.Add(new EventDelegate(OnPickAllItem));
        backListBtn.onClick.Clear();
        backListBtn.onClick.Add(new EventDelegate(OnListBack));
        spreadListBtn.onClick.Clear();
        spreadListBtn.onClick.Add(new EventDelegate(OnClickSpreadList));
        Content_Grid.Reposition();
        isBack = false;
        popupAnim.CrossFade("SpreadLootList",0f,-1,0.99f);
    }
    public override void Exit()
    {
        base.Exit();
    }
    private void OnPickItem(LootPopupItem _lootPopupItem)
    {
        if (_lootPopupItem.ItemObject.itemPacker!=null)
        {
            int remain = InventoryManager.Instance.PutToInventory(PlayerFacade.Instance.actorSystem,_lootPopupItem.dropItemData.dropItemKindInfo.itemDataConfig,_lootPopupItem.ChooseCount);
            _lootPopupItem.dropItemData.Reduce(_lootPopupItem.ChooseCount);
            _lootPopupItem.RefreshItem();

            if (_lootPopupItem.dropItemData.IsHave==false)
            {
                GameObjectFactory.Instance.PushItem(_lootPopupItem.gameObject);
                Content_Grid.Reposition();
            }
            PlayAudio(PathDefine.Sound_PickItem);
        }
    }
    public void OnPickAllItem()
    {
        foreach (var item in lootPopupItems)
        {
            OnPickItem(item);
        }
        Hide();
    }
    /// <summary>
    /// 收缩列表
    /// </summary>
    public void OnListBack()
    {
        popupAnim.CrossFade("BackLootList",0.1f);
        isBack = true;
    }
    public void OnClickSpreadList()
    {
        if (isBack)
        {
            isBack = false;
            popupAnim.CrossFade("SpreadLootList",0.1f);
        }
    }
}
