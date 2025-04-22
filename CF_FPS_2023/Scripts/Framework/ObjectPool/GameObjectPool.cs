using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameObjectPool: IObjectPool<GameObject>
{
    public Transform root;
    
    public GameObject GetGameObject()
    {
        return GetObject();
    }
    public GameObject FindNoActive()
    {
        GameObject objectGo = null;
        foreach (var item in objectPool)
        {
            if (item!=null&&item.activeSelf==false)
            {
                objectGo = item;
                break;
            }
        }
        return objectGo;
    }

    public GameObject NewBuildGameObject()
    {
        GameObject go=  GameObject.Instantiate(prefab,root);
        objectPool.Add(go);
        return go;
    }
    
    public override void PushItem(GameObject go)
    {
        if (go!=root)
        {
            go.transform.SetParent(root, false);//不变换局部比例，就是比例数值仍然还是原先那个，不经过世界变换
            go.gameObject.SetActive(false);
        }
       
    }

    public override GameObject GetObject(Type type)
    {
        GameObject go = FindNoActive();
        if (go == null)
        {
            go = NewBuildGameObject();
        }
        go.gameObject.SetActive(true);
        return go;
    }
}

