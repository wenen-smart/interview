using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ornament_EquipmentSlot : CharacterItemSlot
{
    public OrnamentType _OrnamentType;

    public override bool Match(EquipmentDataConfig itemDataConfig)
    {
        if (itemDataConfig.GetEquipmentType()!=EquipmentType.Ornament)
        {
            return false;
        }
        OrnamentDataConfig ornamentDataConfig = itemDataConfig as OrnamentDataConfig;
        if (ornamentDataConfig.GetOrnamentType()==_OrnamentType)
        {
            return true;
        }
        return false;
    }
}
