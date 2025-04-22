using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
/// <summary>
/// 封装Scriptable
/// </summary>
public abstract class ScriptableConfig:ScriptableObject
{

}
/// <summary>
/// 封装Scriptable 且含有单例，只适用单个文件
/// </summary>
public abstract class ScriptableConfigTon<T> : ScriptableConfig where T: ScriptableConfigTon<T>,new()
{
    private static T instance;

    public static T Instance
    {
        
        get
        {
            if (instance==null)
            {
                typeof(T).InvokeMember("SetScriptablePath",BindingFlags.Static|BindingFlags.InvokeMethod|BindingFlags.Public, null,null,null);
                instance = Resources.Load<T>(scriptablePath) as T;
            }
#if UNITY_EDITOR
            if (instance==null)
            {

                InitInstance("Assets/Resources/"+scriptablePath+".asset");
            }
#endif
            return instance;
        }
        set
        {
            instance = value;
        }

    }
    protected static void InitInstance(string path) 
    {
        T scriptableObject = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        if (instance!=null)
        {
            DestroyImmediate(instance);
        }
        if (scriptableObject)
        {
            instance = CreateInstance<T>();
            instance.hideFlags = HideFlags.HideAndDontSave;
        }
        else
        {
            Debug.Log($"scriptableObject  null path:{path} type:{typeof(T)}");
        }
        
    }

    static ScriptableConfigTon()
    {
        SetScriptablePath();
    }

    protected static string scriptablePath;
    public static void SetScriptablePath()
    {
       
    }
    public virtual void OnEnable()
    {
       instance = this as T;
    }

}
