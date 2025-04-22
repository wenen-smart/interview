using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ResFactory<PrefabT,InstanceT>:ScriptSingleTon<InstanceT>, IObjectFactory<PrefabT> where PrefabT : UnityEngine.Object,new() where InstanceT:new()
{



    //public Dictionary<string, ObjectPool<PrefabT>> prefabPools=new Dictionary<string, ObjectPool<PrefabT>>();

    public PrefabT LoadPrefab(string path) 
    {
        return PrefabDB.LoadAsset<PrefabT>(path);
    }

    public abstract void PushItem(PrefabT objectEntity);
    public abstract PrefabT PopItem(string path);

    public abstract void Init();

}
public class ResFactory : ResFactory<UnityEngine.Object,ResFactory>
{
    public override void Init()
    {
        
    }

    public override UnityEngine.Object PopItem(string path)
    {
        return LoadPrefab(path);
    }


    public override void PushItem(UnityEngine.Object objectEntity)
    {
        
    }
}



