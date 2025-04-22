using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ItemConfigContainer : ScriptableConfigTon<ItemConfigContainer>
{
    public ItemConfigsJsonInfo JsonInfo;
    public ItemDataConfigInfo<WeaponDataConfig>[] WeaponDataConfigList;
    public ItemDataConfigInfo<AlimentDataConfig>[] AlimentDataConfigList;
    public ItemDataConfigInfo<OrnamentDataConfig>[] OrnamentDataConfigList;
    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.itemConfigDataContainerPathNoSuffix;
    }
}
[Serializable]
public class ItemDataConfigInfo<T> where T:ItemDataConfig
{
    //public string configName;
    [HideInInspector]
    public string guid;
    public T config;
}

