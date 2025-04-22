using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_EquipmentSlot : CharacterItemSlot
{

    public WeaponType _weaponType;
    public override bool Match(EquipmentDataConfig itemDataConfig)
    {
        if (itemDataConfig.GetEquipmentType() != EquipmentType.Weapon)
        {
            return false;
        }
        WeaponDataConfig weaponDataConfig = itemDataConfig as WeaponDataConfig;
        if (weaponDataConfig.GetWeaponType() == _weaponType)
        {
            return true;
        }
        return false;
    }
}
