using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPopupItem : MonoBehaviour
{
    [SerializeField]
    private UILabel NameLabel;
    [SerializeField]
    private UILabel DescriptionLabel;
    [SerializeField]
    private UITexture icon;
    private DropItemObject _itemObject;
    [HideInInspector]
    public DropItemData dropItemData;
    [SerializeField]
    private UIButton pickItemBtn;
    [SerializeField]
    private UILabel countLabel;
    [SerializeField]
    private UIWidget chooseWidget;
    [SerializeField]
    private UIInput chooseCountInputField;
    [SerializeField]
    private UIButton reduceChooseCountBtn;
    [SerializeField]
    private UIButton addChooseCountBtn;
    public int ChooseCount 
    {
        get
        {
            if (string.IsNullOrEmpty(chooseCountInputField.value))
            {
                return dropItemData.Count;
            }
            return int.Parse(chooseCountInputField.value);
        }
        set
        {
             chooseCountInputField.value = value.ToString();
        }
    }

    public DropItemObject ItemObject { get => _itemObject; set => _itemObject = value; }

    public void SetInfo(DropItemObject itemObject,DropItemData dropItemData,Action<LootPopupItem> pickItemAction=null)
    {
        this.ItemObject = itemObject;
        this.dropItemData = dropItemData;
        ChooseCount = dropItemData.Count;
        NameLabel.text = dropItemData.dropItemKindInfo.itemDataConfig.ITEM_Name;
        DescriptionLabel.text = dropItemData.dropItemKindInfo.itemDataConfig.ITEM_Description;
        icon.mainTexture = dropItemData.dropItemKindInfo.itemDataConfig.GetIcon();

        pickItemBtn.onClick.Clear();
        addChooseCountBtn.onClick.Clear();
        reduceChooseCountBtn.onClick.Clear();

        pickItemBtn.onClick.Add(new EventDelegate(()=> { pickItemAction?.Invoke(this); }));
        addChooseCountBtn.onClick.Add(new EventDelegate(()=> { AddChooseCount(1); }));
        reduceChooseCountBtn.onClick.Add(new EventDelegate(()=> { AddChooseCount(-1); }));
        if (ChooseCount>1)
        {
            chooseWidget.gameObject.SetActive(true);
        }
        else
        {
            chooseWidget.gameObject.SetActive(false);
        }
    }
    public void RefreshItem()
    {
        AddChooseCount(0);
        SetCount(dropItemData.Count);
    }
    public void SetCount(int count)
    {
        if (count>1)
        {
            countLabel.text = "X" + count;
        }
        else
        {
            countLabel.text = "";
        }
    }
    public void AddChooseCount(int add)
    {
        ChooseCount += add;
        if (ChooseCount>=dropItemData.Count)
        {
            ChooseCount = dropItemData.Count;
        }
        else
        {
            if (ChooseCount<=1)
            {
                ChooseCount = 1;
            }
        }
    }
}
