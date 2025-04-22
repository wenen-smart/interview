using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RenderStateListener:MonoBehaviour
{
    public event Action intoRenderer;
    public event Action exitRenderer;
    [HideInInspector]
    public bool isRenderObject;
    public void OnBecameVisible()
    {
        if (isRenderObject == false)
        {
            isRenderObject = true;
            intoRenderer?.Invoke();
        }
    }
    public void OnBecameInvisible()
    {
        if (isRenderObject == true)
        {
            isRenderObject = false;
            exitRenderer?.Invoke();
        }
    }
}

