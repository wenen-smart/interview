using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IObjectPool<T>  where T:class
{
    public List<T> objectPool = new List<T>();
    public T prefab;
    public string symbol;

    public virtual  T GetObject<T1>() where T1:T
    {
        return GetObject(typeof(T1));
    }
    public virtual T GetObject()
    {
        return GetObject<T>();
    }
    public abstract T GetObject(Type type);
    public abstract void PushItem(T go);

    public bool IsExist(T go)
    {
        return objectPool.Find(g => g.Equals(go)) != null;
    }
}

public class ObjectPool<T>:IObjectPool<T> where T:class,new()
{
    public override void PushItem(T go)
    {
        objectPool.Add(go);
    }

    public override T GetObject(Type type)
    {
        if (type == null || (type.IsSubclassOf(typeof(T)) == false)&&type!=(typeof(T)))
        {
            MyDebug.DebugError($"-[objectPool]传入的Type{type}不是T{typeof(T)}的继承类");
            return null;
        }
        if (objectPool.Count == 0)
        {
            return Activator.CreateInstance(type) as T;
        }
        T newObj = objectPool[objectPool.Count - 1];
        objectPool.RemoveAt(objectPool.Count - 1);
        return newObj;
    }
}
