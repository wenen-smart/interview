﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IActor : MonoBehaviour, I_Init
{
    public abstract void Init();

    public virtual void OnDestory() { }
}

