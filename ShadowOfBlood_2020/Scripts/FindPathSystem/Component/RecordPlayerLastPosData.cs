﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RecordPlayerLastPosData : IComponentData
{
    public float3 lastPos;
}
