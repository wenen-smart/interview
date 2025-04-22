using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class MathfTools
{
    public const float InfinitelyZero = 0.00001f;
    /// <summary>
    /// [-1,1]
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static int GetNormalizeValue(this float v)
    {
        int symbol = 1;
        int result=Mathf.Abs(v)>InfinitelyZero?1:0;
        if (result==1)
        {
            if (v<-InfinitelyZero)
            {
                symbol = -1;
            }
            result *= symbol;
        }
        return result;
    }
}

