using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class InventoryManager:CommonSingleTon<InventoryManager>
{
    //暂时不用
    public void PutToInventory(ActorSystem actorSystem,DropItemPacker packer)
    {
        if (actorSystem.roleData==null)
        {
            actorSystem.roleData = new RoleData();
        }
        if (actorSystem.roleDefinition.haveInvertory)
        {
            if (actorSystem.roleData.InventoryData == null)
            {
                actorSystem.roleData.InventoryData = new InventoryData();
            }
            foreach (var dropItemData in packer.dropItemKindDatas)
            {
                actorSystem.roleData.InventoryData.Put(dropItemData.dropItemKindInfo.itemDataConfig,dropItemData.Count);
                if (actorSystem.roleDefinition.roleID == 1)
                {
                    Game.Instance.GainItem(dropItemData.dropItemKindInfo.itemDataConfig.ITEM_Name);
                }
            }
        }
    }
        /// <returns>返回剩余量，0代表不剩于都可以放入背包</returns>
    public int PutToInventory(ActorSystem actorSystem, ItemDataConfig itemDataConfig,int count)
    {
        int remainCount = count;
        if (actorSystem.roleData == null)
        {
            actorSystem.roleData = new RoleData();
        }
        if (actorSystem.roleDefinition.haveInvertory)
        {
            if (actorSystem.roleData.InventoryData == null)
            {
                actorSystem.roleData.InventoryData = new InventoryData();
            }
            remainCount=actorSystem.roleData.InventoryData.Put(itemDataConfig, count);
            if (actorSystem.roleDefinition.roleID == 1)
            {
                Game.Instance.GainItem(itemDataConfig.ITEM_Name);
            }
        }
        return remainCount;
    }
    public void TakeFromInventory(ActorSystem actorSystem, DropItemPacker packer)
    {
        RoleData roleData = null;
        roleData = actorSystem.roleData;
        if (roleData == null)
        {
            MyDebug.DebugWarning($"TakeFromFailed{actorSystem}-RoleData:null");
            return;
        }
        if (actorSystem.roleDefinition.haveInvertory)
        {
            foreach (var dropItemData in packer.dropItemKindDatas)
            {
                roleData.InventoryData.Take(dropItemData.dropItemKindInfo.itemDataConfig, dropItemData.Count);
            }
        }
    }
    public ItemDataConfig Equip(ActorSystem actorSystem,ItemDataConfig itemDataConfig)
    {
        RoleData roleData = null;
        roleData = actorSystem.roleData;
        ItemDataConfig remain = roleData.InventoryData.Equip(itemDataConfig);
        return remain;
    }
    public bool PlayerEquip(WeaponDataConfig itemDataConfig)
    {
        RoleData roleData = null;
        roleData = PlayerFacade.Instance.actorSystem.roleData;
        List<WeaponDataConfig> haveEquipweaponDataConfigs = roleData.InventoryData.FindTheTypeEquipmentAndHaveEquiped(itemDataConfig.GetWeaponType());
        if (haveEquipweaponDataConfigs.Count>0)
        {
            return false;
        }
        roleData.InventoryData.Equip(itemDataConfig);
        return true;
    }
    public bool PlayerEquip(OrnamentDataConfig itemDataConfig)
    {
        RoleData roleData = null;
        roleData = PlayerFacade.Instance.actorSystem.roleData;
        List<OrnamentDataConfig> haveEquipornamentDataConfigs = roleData.InventoryData.FindTheTypeEquipmentAndHaveEquiped(itemDataConfig.GetOrnamentType());
        if (haveEquipornamentDataConfigs.Count > 0)
        {
            return false;
        }
        roleData.InventoryData.Equip(itemDataConfig);
        return true;
    }
    public bool PlayerUnEquip(WeaponDataConfig itemDataConfig)
    {
        RoleData roleData = null;
        roleData = PlayerFacade.Instance.actorSystem.roleData;
        List<WeaponDataConfig> haveEquipweaponDataConfigs = roleData.InventoryData.FindTheTypeEquipmentAndHaveEquiped(itemDataConfig.GetWeaponType());
        if (haveEquipweaponDataConfigs.Count > 0)
        {
            roleData.InventoryData.UnEquip(itemDataConfig);
            return true;
        }
        return false;
    }
    public bool PlayerUnEquip(OrnamentDataConfig itemDataConfig)
    {
        RoleData roleData = null;
        roleData = PlayerFacade.Instance.actorSystem.roleData;
        List<OrnamentDataConfig> haveEquipDataConfigs = roleData.InventoryData.FindTheTypeEquipmentAndHaveEquiped(itemDataConfig.GetOrnamentType());
        if (haveEquipDataConfigs.Count > 0)
        {
            roleData.InventoryData.UnEquip(itemDataConfig);
            return true;
        }
        return false;
    }
    public bool PlayerUnEquip(ItemDataConfig itemDataConfig)
    {
        if ((itemDataConfig is EquipmentDataConfig)==false)
        {
            return false;
        }
        if (itemDataConfig is OrnamentDataConfig)
        {
            return PlayerUnEquip(itemDataConfig as OrnamentDataConfig);
        }
        else
        {
            return PlayerUnEquip(itemDataConfig as WeaponDataConfig);
        }
    }
}

public class InventoryData
{
    public List<InventoryItemData> itemDatas=new List<InventoryItemData>();
    public List<EquipmentDataConfig> equipmentDataConfigs=new List<EquipmentDataConfig>();
    //public Dictionary<EquipmentType, List<InventoryItemData>> equipmentInventoryItemDataDics = new Dictionary<EquipmentType, List<InventoryItemData>>();
    //public Dictionary<OrnamentType, List<InventoryItemData>> ornamentInventoryItemDataDics = new Dictionary<OrnamentType, List<InventoryItemData>>();
    //public Dictionary<WeaponType, List<InventoryItemData>> weaponInventoryItemDataDics = new Dictionary<WeaponType, List<InventoryItemData>>();

    public int Capacity = 60;
    private int OccupyCapacity=-1;
    public bool isFull
    {
        get
        {
            if (OccupyCapacity==-1)
            {
                OccupyCapacity=CalcuateCurrentOccupyCapacity();
            }
            return OccupyCapacity >= Capacity;
        }
    }
    public int CalcuateCurrentOccupyCapacity()
    {
        int currentCapacity = 0;
        foreach (var item in itemDatas)
        {
            currentCapacity += item.count;
        }
        return currentCapacity;
        
    }
    public bool IsExist(ItemDataConfig dataConfig)
    {
        foreach (var itemData in itemDatas)
        {
            if (itemData.itemConfig == dataConfig)
            {
                return true;
            }
        }
        return false;
    }
    public InventoryItemData GetInventoryItemData(ItemDataConfig itemDataConfig)
    {
        foreach (var itemData in itemDatas)
        {
            if (itemData.itemConfig == itemDataConfig)
            {
                return itemData;
            }
        }
        return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemDataConfig"></param>
    /// <param name="count"></param>
    /// <returns>返回剩余量，0代表不剩于都可以放入背包</returns>
    public int Put(ItemDataConfig itemDataConfig,int count=1)
    {
        if (itemDatas==null)
        {
            itemDatas = new List<InventoryItemData>();
        }
        if (isFull)
        {
            return count;
        }
        int remainCapacity = Capacity - OccupyCapacity;
        int remainCount = 0;
        int putCount = count;
        if (count>=remainCapacity)
        {
            remainCount=count - remainCapacity;
            putCount = remainCapacity;
        }
        if (putCount==0)
        {
            return count;
        }
        InventoryItemData inventoryItemData = GetInventoryItemData(itemDataConfig);
        if (inventoryItemData == null)
        {
            inventoryItemData = new InventoryItemData(itemDataConfig);
            itemDatas.Add(inventoryItemData);
        }
        inventoryItemData.AddCount(putCount);
        return remainCount;
    }
    public void Take(ItemDataConfig itemDataConfig,int count=1)
    {
        if (IsExist(itemDataConfig)==false)
        {
            MyDebug.DebugWarning($"当前背包数据没有这个item{itemDataConfig.ITEM_Name}，你移除它干嘛！");
            return;
        }
        GetInventoryItemData(itemDataConfig).RemoveCount(count);
    }
    /// <summary>
    ///这块只是修改数据，如果没装配上，返回原数据，装配上返回null。
    /// </summary>
    /// <param name="itemDataConfig"></param>
    /// <returns></returns>
    public ItemDataConfig Equip(ItemDataConfig itemDataConfig)
    {
        if (itemDataConfig.ItemIsCanEquip()==false)
        {
            return itemDataConfig;
        }
        if (IsExist(itemDataConfig) == false)
        {
            MyDebug.DebugWarning($"当前背包数据没有这个item{itemDataConfig.ITEM_Name}，装配它干嘛！");
            return itemDataConfig;
        }
        InventoryItemData inventoryItemData = GetInventoryItemData(itemDataConfig);
        if (itemDataConfig.ItemIsCanEquip())
        {
            inventoryItemData.isHaveAEquip = true;
            EquipmentDataConfig equipmentDataConfig = inventoryItemData.itemConfig as EquipmentDataConfig;
            if (equipmentDataConfigs.Contains(equipmentDataConfig)==false)
            {
                equipmentDataConfigs.Add(equipmentDataConfig);
            }
            return null;
        }
        return itemDataConfig;
    }
    public void UnEquip(ItemDataConfig itemDataConfig)
    {
        if (IsExist(itemDataConfig) == false)
        {
            MyDebug.DebugWarning($"当前背包数据没有这个item{itemDataConfig.ITEM_Name}，卸载它干嘛！");
            return;
        }
        InventoryItemData inventoryItemData = GetInventoryItemData(itemDataConfig);
        if (itemDataConfig.ItemIsCanEquip())
        {
            inventoryItemData.isHaveAEquip = false;
            equipmentDataConfigs.Remove(inventoryItemData.itemConfig as EquipmentDataConfig);
            return;
        }
        return;
    }

    public bool HaveEquiped(ItemDataConfig itemDataConfig)
    {
        if (itemDataConfig.ItemIsCanEquip() == false)
        {
            return false;
        }
        if (IsExist(itemDataConfig) == false)
        {
            MyDebug.DebugWarning($"当前背包数据没有这个item{itemDataConfig.ITEM_Name}");
            return false;
        }
        InventoryItemData inventoryItemData = GetInventoryItemData(itemDataConfig);
        return inventoryItemData.isHaveAEquip;
    }

    public List<OrnamentDataConfig> FindTheTypeEquipmentAndHaveEquiped(params OrnamentType[] ornamentTypes)
    {
        List<OrnamentDataConfig> equipItemDatas=new List<OrnamentDataConfig>();
        foreach (var item in itemDatas)
        {
            if (item.isHaveAEquip==true)
            {
                if (item.itemConfig is OrnamentDataConfig)
                {
                    OrnamentDataConfig ornamentDataConfig = item.itemConfig as OrnamentDataConfig;
                    if (ornamentTypes.Any((o)=> {return  o == ornamentDataConfig.GetOrnamentType(); }))
                    {
                        if (equipItemDatas.Contains(ornamentDataConfig)==false)
                        {
                            equipItemDatas.Add(ornamentDataConfig);
                        }
                    }
                }
            }
        }
        return equipItemDatas;
    }
    public List<WeaponDataConfig> FindTheTypeEquipmentAndHaveEquiped(params WeaponType[] weaponTypes)
    {
        List<WeaponDataConfig> equipItemDatas = new List<WeaponDataConfig>();
        foreach (var item in itemDatas)
        {
            if (item.isHaveAEquip == true)
            {
                if (item.itemConfig is WeaponDataConfig)
                {
                    WeaponDataConfig weaponDataConfig = item.itemConfig as WeaponDataConfig;
                    if (weaponTypes.Any((o) => { return o == weaponDataConfig.GetWeaponType(); }))
                    {
                        if (equipItemDatas.Contains(weaponDataConfig) == false)
                        {
                            equipItemDatas.Add(weaponDataConfig);
                        }
                    }
                }
            }
        }
        return equipItemDatas;
    }
    //public  bool TheTypeEquipmentHaveEquiped(OrnamentType ornamentType)
    //{
        
    //}
    //public bool TheTypeEquipmentHaveEquiped(WeaponType weaponType)
    //{

    //}
}
public class InventoryItemData
{
    public ItemDataConfig itemConfig;
    public int count = 0;
    public bool isHaveAEquip=false;//默认同类型物品只能装配一个

    public InventoryItemData(ItemDataConfig itemConfig)
    {
        this.itemConfig = itemConfig;
    }
    public void AddCount(int count=1)
    {
        this.count += count;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <returns return result : True that Current Item Count is zero,otherwise that CurrentItem has exist></returns>
    public bool RemoveCount(int count=1)
    {
        if (count<=0)
        {
            return true;
        }
        this.count -= count;
        return count <= 0;
    }
}
