using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ConfigResFactory: ScriptSingleTon<ScriptableObject>
{
    public static T LoadConifg<T>(string path) where T: UnityEngine.Object
    {
        return PrefabDB.LoadAsset<T>(path);
    }
}

