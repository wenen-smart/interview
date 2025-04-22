using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LifeTimer:MonoBehaviour
{
    [HideInInspector] public bool isAwakeDelayDestory=true;
    public float lifeTime = 10;

    public void Start()
    {
        if (isAwakeDelayDestory)
        {
            Destroy(gameObject,lifeTime);
        }
    }
}

