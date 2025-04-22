using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametersBlend
{
    private float target = 0;
    private float speed = 0;
    private bool start = false;
    private Action<float> setValueAction;
    private Func<float> currentValueFunc;
    private Action onComplete;
    public void Update()
    {
        if (start)
        {
            var currentValue = currentValueFunc.Invoke();
            var get = Mathf.Lerp(currentValue,target,Time.deltaTime* speed);
            if (Mathf.Abs(get-target)<0.05f)
            {
                setValueAction.Invoke(target);
                start = false;
                onComplete?.Invoke();
            }
            else
            {
                setValueAction.Invoke(get);
            }
        }
    }

    public void SetTarget(float tar,float speed,Func<float> get,Action<float> set,Action onComplete=null)
    {
        target = tar;
        start = true;
        currentValueFunc = get;
        setValueAction = set;
        this.onComplete = onComplete;
    }

}
