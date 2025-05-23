﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public struct RangedFloat
{
    public float minValue;
    public float maxValue;

    public RangedFloat(float minValue, float maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    /// <summary>  Returns a RandomValue between Min and Max </summary>
    public float RandomValue => UnityEngine.Random.Range(minValue, maxValue);

    /// <summary>Is the value in between the min and max of the FloatRange </summary>
    public bool IsInRange(float value) => value >= minValue && value <= maxValue;
}