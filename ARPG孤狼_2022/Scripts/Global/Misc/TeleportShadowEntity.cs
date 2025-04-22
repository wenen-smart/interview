using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TeleportShadowEntity:MonoBehaviour
    {

    public float duration;
    public float destoryTime;
    public MeshRenderer meshRenderer;
    public float bianyuanMult;
    

    public void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    public void Update()
    {
        var invteralTime = destoryTime - Time.time;
        if (invteralTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            float scale = invteralTime/duration;
            Color color=meshRenderer.material.GetColor("_RimColor");
            color.a *= scale;
            meshRenderer.material.SetColor("_RimColor", color);
        }
    }
}

