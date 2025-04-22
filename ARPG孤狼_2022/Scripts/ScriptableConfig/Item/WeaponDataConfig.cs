using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
/// <summary>
/// 数据类
/// </summary>
[CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "CreateConfig/CreateWeaponDataConfig", order = 1)]
public class WeaponDataConfig : EquipmentDataConfig
{
    public string _weaponType { get { return weaponType.ToString();} set { weaponType = (WeaponType)Enum.Parse(typeof(WeaponType), value); } }
    [SerializeField]
    private WeaponType weaponType;
    public int lv;//等级
    public WeaponType GetWeaponType()
    {
        return weaponType;
    }
}

