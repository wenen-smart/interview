using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPopup : BasePanel
{
    public CharacterItemSlot[] CharacterItemSlotList;


    public override void Enable()
    {
        base.Enable();
        InventoryData inventoryData = PlayerFacade.Instance.actorSystem.roleData.InventoryData;//临时
        List<OrnamentDataConfig> ornamentDataConfigs = inventoryData.FindTheTypeEquipmentAndHaveEquiped(OrnamentType.Head,OrnamentType.Jacket,OrnamentType.Neck,OrnamentType.Shoes,OrnamentType.Shoulder,OrnamentType.Trousers,OrnamentType.Bracers,OrnamentType.Gloves);
        List<WeaponDataConfig> weaponDataConfigs = inventoryData.FindTheTypeEquipmentAndHaveEquiped(WeaponType.Bow,WeaponType.Sword);
        foreach (var slot in CharacterItemSlotList)
        {
            //待优化
            if (slot is Ornament_EquipmentSlot)
            {
                Ornament_EquipmentSlot equipSlot = slot as Ornament_EquipmentSlot;
                OrnamentDataConfig equipDataInthisSlot = ornamentDataConfigs.FirstOrDefault((ic) => ic.GetOrnamentType() == equipSlot._OrnamentType);
                if (equipDataInthisSlot!=null)
                {
                    equipSlot.SetItem(equipDataInthisSlot,1);
                }
            }
            else
            {
                Weapon_EquipmentSlot equipSlot = slot as Weapon_EquipmentSlot;
                WeaponDataConfig equipDataInthisSlot = weaponDataConfigs.FirstOrDefault((ic) => ic.GetWeaponType() == equipSlot._weaponType);
                if (equipDataInthisSlot != null)
                {
                    equipSlot.SetItem(equipDataInthisSlot, 1);
                }
            }
        }
    }
    /// <summary>
    /// 找格子然后格子高亮
    /// </summary>
    public CharacterItemSlot MatchEquipment(EquipmentDataConfig equipmentDataConfig)
    {
        CharacterItemSlot matchSlot = null;
        foreach (var slot in CharacterItemSlotList)
        {
            if (slot.Match(equipmentDataConfig))
            {
                matchSlot = slot;
                break;
            }
        }
        return matchSlot;
    }
    public void HighLightEquipment(EquipmentDataConfig equipmentDataConfig,SlotStateEnum slotStateEnum)
    {
        MatchEquipment(equipmentDataConfig).HighLighting(slotStateEnum);
    }
    public void HighLightEquipment(CharacterItemSlot slot, EquipmentDataConfig equipmentDataConfig, SlotStateEnum slotStateEnum)
    {
        slot.HighLighting(slotStateEnum);
    }
    public bool EquipTheEquipment(EquipmentDataConfig equipmentDataConfig)
    {
        var matchSlot = MatchEquipment(equipmentDataConfig);
        if (matchSlot!=null)
        {
            matchSlot.HighLighting(SlotStateEnum.Match);
            matchSlot.SetItem(equipmentDataConfig,1);
        }
        return false;
    }
    public void UnEquipAndUpdateInventoryPopup(ItemDataConfig itemDataConfig)
    {
        InventoryManager.Instance.PlayerUnEquip(itemDataConfig);
        if (InventoryPopup.Instance!=null&&InventoryPopup.Instance.gameObject.activeInHierarchy)
        {
            InventoryPopup.Instance.LoadSoltData(PlayerFacade.Instance.actorSystem.roleData.InventoryData);
        }
    }
}
