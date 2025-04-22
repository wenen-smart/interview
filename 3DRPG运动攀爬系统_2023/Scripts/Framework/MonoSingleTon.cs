using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleTon<T> : MonoBehaviour where T:MonoBehaviour
{
protected static T instance;
public static T Instance
{
    get{
        if(instance==null){
            instance=FindObjectOfType<T>();
        }
        return instance;
    }
        protected set { instance = value; }
}
    public virtual void Awake()
    {
        if (Instance)
        {
            instance=FindObjectOfType<T>();
        }
    }
}
