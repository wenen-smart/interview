using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : ScriptSingleTon<ItemContainer>,IManager
{
    public const string ItemDataConfigPath="Config/Scriptable/Item/";
    public Dictionary<int, GunDataConfig> gunDataConfigDic = new Dictionary<int, GunDataConfig>();
    public Dictionary<int, KnifeDataConfig> knifeDataConfigDic = new Dictionary<int, KnifeDataConfig>();
    public Dictionary<int, ThrowItemDataConfig> throwItemDataConfigDic = new Dictionary<int, ThrowItemDataConfig>();
    public void Init()
    {
        LoadItem<GunDataConfig>(gunDataConfigDic);
        LoadItem<KnifeDataConfig>(knifeDataConfigDic);
        LoadItem<ThrowItemDataConfig>(throwItemDataConfigDic);
    }

    public void LoadItem<T>(Dictionary<int,T> dics) where T:ItemDataConfig
    {
        T[] datas = Resources.LoadAll<T>(ItemDataConfigPath);
        foreach (var item in datas)
        {
            if (IdIsExist(item.id))
            {
                DebugTool.DebugError(string.Format("存在配置错误，物品id重复。id：", item.id));
                continue;
            }
            dics.Add(item.id, item);
        }
    }

    public GunDataConfig GetGunData(int id)
    {
        if (gunDataConfigDic.ContainsKey(id))
        {
            return gunDataConfigDic[id];
        }
        return null;
    }
    public KnifeDataConfig GetKnifeData(int id)
    {
        if (knifeDataConfigDic.ContainsKey(id))
        {
            return knifeDataConfigDic[id];
        }
        return null;
    }
    public ThrowItemDataConfig GetThrowItemData(int id)
    {
        if (throwItemDataConfigDic.ContainsKey(id))
        {
            return throwItemDataConfigDic[id];
        }
        return null;
    }
    public bool IdIsExist(int id)
    {
        return gunDataConfigDic.ContainsKey(id) || knifeDataConfigDic.ContainsKey(id) || throwItemDataConfigDic.ContainsKey(id);
    }

    public void LoadScript(object args)
    {
        Init();
    }
}

