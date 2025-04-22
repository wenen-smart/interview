using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point 
{
    public int index;
    public int x;
    public int y;
    public float f;
    public float g;
    public float h;
    public bool isObstacle;
    public int parentIndex;
    public void CalculateF()
    {
        f = h + g;
    }
}
