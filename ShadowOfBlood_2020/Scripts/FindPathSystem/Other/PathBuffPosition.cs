using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[InternalBufferCapacity(20)]
public struct PathBuffPosition : IBufferElementData
{
    public int2 pathPos;
}
