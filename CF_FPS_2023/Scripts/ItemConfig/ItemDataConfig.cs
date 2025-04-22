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
    public int capacity = 1;//����
    public bool isGroupInLoot = true;//��ʰȡ�б�������ж���Ƿ��������ʽ����
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
//��Ʒ����
public enum ItemClassify
{
    Aliment,//ʳ��
    Equipment,//װ��  ������װ�Ρ�����
}
[Serializable]
public class ItemConfigsJsonInfo
{
    public string Version;
}