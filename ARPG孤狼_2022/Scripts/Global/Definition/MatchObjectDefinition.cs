using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MatchObjectDefinition
{
    public string prefab_Path;
    public string object_Name;
    public string object_Description;

#region Runtime Set
    public GameObject prefab { get; protected set; }
    public string Prefab_FileName { get { return prefab.name; } }
    public string Prefab_Tag { get { return prefab.tag; } }
    #endregion
    /// <summary>
    /// 调用前先对所有配置字段完成赋值操作；
    /// </summary>
    public void LoadPrefab()
    {
        if (string.IsNullOrEmpty(prefab_Path))
        {
            MyDebug.DebugError("ObjectDefinition:调用前先对所有配置字段完成赋值操作；prefab_Path没赋值");
            return;
        }
        prefab = GameObjectFactory.Instance.LoadPrefab(prefab_Path);
    }
}
