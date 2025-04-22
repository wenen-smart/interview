using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EquipmentDataConfig:ItemDataConfig
{
    public string _equipmentType { get { return equipmentType.ToString(); } set { equipmentType = (EquipmentType)Enum.Parse(typeof(EquipmentType), value); } }
    [SerializeField]
    private EquipmentType equipmentType;
    public EquipmentType GetEquipmentType()
    {
        return equipmentType;
    }
    public int ad;//物理攻击
    public int ap;//法术伤害
    public int td;//真实伤害
    public int adDef;//物抗
    public int apDef;//魔抗
}
public enum EquipmentType
{
    Ornament,//装饰
    Weapon,
}