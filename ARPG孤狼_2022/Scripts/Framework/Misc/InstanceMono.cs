using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstanceMono<T> : MonoBehaviour where T:InstanceMono<T>
{

    private static T instance;

    public static T Instance {

        get
        {
            if (instance==null)
            {
                instance = FindObjectOfType<T>();
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    
    }
    public virtual void Awake()
    {
        instance = this as T;
    }

    
}
