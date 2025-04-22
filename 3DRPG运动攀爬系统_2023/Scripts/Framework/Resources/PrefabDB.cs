using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PrefabDB
{
    public static Dictionary<string, UnityEngine.Object> prefabPools = new Dictionary<string, UnityEngine.Object>();
    public static T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        T prefab = null;
        if (path == null)
        {
            return null;
        }
        if (prefabPools.ContainsKey(path))
        {
            prefab = prefabPools[path] as T;
        }
        if (prefab==null)
        {
            prefab = Resources.Load<T>(path);
            if (prefab != null)
            {
                prefabPools.Add(path, prefab);
            }
        }
        else
        {
            Debug.LogWarning("无法加载资源：路径错误： " + path);
        }
        return prefab;
    }
}

