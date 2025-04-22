using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ItemDataConfigManager : IManager
{
    public  static Dictionary<int, ItemDataConfig> itemDataConfigDics = new Dictionary<int, ItemDataConfig>();
    public void Init()
    {
        

    }

    public void LoadScript(object args)
    {
        ItemJsonConfigEleAddDic(ItemConfigContainer.Instance.WeaponDataConfigList);
        ItemJsonConfigEleAddDic(ItemConfigContainer.Instance.AlimentDataConfigList);
        ItemJsonConfigEleAddDic(ItemConfigContainer.Instance.OrnamentDataConfigList);
    }
    public void ItemJsonConfigEleAddDic<T>(ItemDataConfigInfo<T>[] itemDataConfigInfos) where T:ItemDataConfig
    {
        if (itemDataConfigInfos==null||itemDataConfigInfos.Length<=0)
        {
            return;
        }
        foreach (var jsonConfig in itemDataConfigInfos)
        {
            itemDataConfigDics.Add(jsonConfig.config.id, jsonConfig.config);
        }
    }

    public static ItemDataConfig GetItemDataConfig(int itemID)
    {
        ItemDataConfig matchDataConfig = null;
        itemDataConfigDics.TryGetValue(itemID, out matchDataConfig);
        return matchDataConfig;
    }
    public bool ListIsHaveElement(IEnumerable<object> itemDataConfigs)
    {
        return itemDataConfigs != null && itemDataConfigs.Count() > 0;
    }
}

