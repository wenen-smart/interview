using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaggedBox : MonoBehaviour
{
    public int dropItemID;
    public int minKindCount;
    public int maxKindCount;
    private bool isDrop = false;
    private GameDropItemKindListInfo boxDropItemKindListInfo;
    private void Awake()
    {
        OnLoadDropInfo();
    }
    public void OnLoadDropInfo()
    {
        DropItemKindInfo[] dropItemKindInfos = DropItemListContainer.Instance.GetItemKindInfo(dropItemID);
        boxDropItemKindListInfo = new GameDropItemKindListInfo();
        boxDropItemKindListInfo.kindsInfo = dropItemKindInfos;
        boxDropItemKindListInfo.minKindCount = minKindCount;
        boxDropItemKindListInfo.maxKindCount = maxKindCount;
    }
    public void DropItem()
    {
        if (isDrop)
        {
            return;
        }
        GetComponent<AudioClipCue>()?.Play();
        isDrop = true;
        if (boxDropItemKindListInfo.kindsInfo != null && boxDropItemKindListInfo.kindsInfo.Length > 0)
        {
            var itemKindInfo = boxDropItemKindListInfo.RandomGetItemInRange();
            GameObject itemGO = GameObjectFactory.Instance.PopItem("Prefab/ItemPacker");
            itemGO.SetActive(true);
            itemGO.transform.SetParent(null, false);
            itemGO.transform.position = transform.position + Vector3.up * 0.1f;
            DropItemObject _DropItemObject = itemGO.GetComponent<DropItemObject>();
            if (_DropItemObject == null)
            {
                _DropItemObject = itemGO.AddComponent<DropItemObject>();
            }
            DropItemPacker packer = new DropItemPacker();
            packer.relativeObject = itemGO;
            packer.dropItemKindDatas = new List<DropItemData>();
            foreach (var item in itemKindInfo)
            {
                packer.AddItem(new DropItemData(item, item.RandomCount()));
            }
            _DropItemObject.itemPacker = packer;
        }
    }
}
