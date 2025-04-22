using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using LitJson;

public abstract class ItemDataConfig : ScriptableObject
{
    public int id;
    public string ITEM_Name;
    public string ITEM_Description;
    public int buy_Price;
    public int sale_Price;

    public Texture icon;

    public GameObject _itemPrefab;
    public int capacity = 1;//容量
    public bool isGroupInLoot = true;//在拾取列表中如果有多个是否以组的形式存在
    public Texture GetIcon()
    {
        return icon;
    }
    public GameObject GetItemPrefab()
    {
        return _itemPrefab;
    }
    public string GetTextContent()
    {
        return "";
    }

}
//物品分类
public enum ItemClassify
{
    Aliment,//食物
    Equipment,//装备  武器、装饰、防具
}
[Serializable]
public class ItemConfigsJsonInfo
{
    public string Version;
}