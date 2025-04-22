using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class OrnamentDataConfig:EquipmentDataConfig
{
    public string _ornamentType { get { return ornamentType.ToString(); } set { ornamentType = (OrnamentType)Enum.Parse(typeof(OrnamentType), value); } }
    [SerializeField]
    private OrnamentType ornamentType;
    public OrnamentType GetOrnamentType()
    {
        return ornamentType;
    }
}

public enum OrnamentType
{
    Head,//头部
    Neck,//项链
    Shoulder,//肩甲
    Jacket,//上衣
    Trousers,//裤子
    Bracers,//手腕
    Gloves,//手套
    Shoes,//鞋子
}
