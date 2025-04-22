using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjectFactory : ResFactory<GameObject, GameObjectFactory>
    {
    public Dictionary<string, GameObjectPool> objectPools = new Dictionary<string, GameObjectPool>();
        
    public Transform root;
    public Transform uiRoot;

    public GameObjectFactory()
    {
        Init();
    }

    public override GameObject PopItem(string path)
    {
        GameObjectPool objectPool = null;

        if (objectPools.ContainsKey(path)==false)
        {
            objectPool = new GameObjectPool();
            objectPool.prefab = LoadPrefab(path);
            if (objectPool.prefab.layer == LayerMask.NameToLayer("UI") == false)
            {
                objectPool.root = root;
            }
            else
            {
                objectPool.root = uiRoot;
            }
            
            objectPools.Add(path,objectPool);
        }
        else
        {
            objectPool=objectPools[path];
        }
        if (objectPool!=null)
        {
          return  objectPool.GetGameObject();
        }
        return null;
    }
    /// <summary>
    /// 用此方法要保证prefab的名字不能重复
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public  GameObject PopItem(GameObject prefab)
    {
        if (prefab==null)
        {
            return null;
        }
        GameObjectPool objectPool = null;
        Debug.Log(prefab);
        string path = prefab.name;
       
        if (objectPools.ContainsKey(path) == false)
        {
            objectPool = new GameObjectPool();
            objectPool.prefab = prefab;
            if (prefab.layer == LayerMask.NameToLayer("UI") == false)
            {
                objectPool.root = root;
            }
            else
            {
                objectPool.root = uiRoot;
            }
            objectPools.Add(path, objectPool);
        }
        else
        {
            objectPool = objectPools[path];
        }
        if (objectPool != null)
        {
            return objectPool.GetGameObject();
        }
        return null;
    }


    public override void PushItem(GameObject objectEntity)
    {
        bool isFindPool = false;
        foreach (var item in objectPools)
        {
            if (item.Value.IsExist(objectEntity))
            {
                item.Value.PushItem(objectEntity);
                isFindPool = true;
                break;
            }
        }
        if (isFindPool==false)
        {
            Debug.LogWarning("对象池 过程中出错，找不到对应的对象池");
            GameObject.Destroy(objectEntity);
        }
    }

    public override void Init()
    {
         root = new GameObject("Pools").transform;
        uiRoot = new GameObject("UIPools").transform;
        GameObject.DontDestroyOnLoad(root.gameObject);
        GameObject.DontDestroyOnLoad(uiRoot.gameObject);
    }
}

