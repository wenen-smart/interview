using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseShadowOfRenderer : MonoBehaviour
{
    public bool isAllInHierarchy = false;
    public void Awake()
    {
        Renderer[] renderers;
        if (isAllInHierarchy)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
        else
        {
            renderers = GetComponents<Renderer>();
        }
        foreach (var item in renderers)
        {
            item.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
