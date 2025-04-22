using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShadow : MonoBehaviour
{

    
    [HideInInspector]
    public SkinnedMeshRenderer[] meshRenderer;
    private Vector3 lastPosition;
    public float intervalTime=0.5f;
    private float lastTime;
    public Shader shader;
    public float duration;
    public Color emissionColor;
    public float bianyuanMult;

    public void Start()
    {
        meshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public void Update()
    {

        if (transform.position==lastPosition)
        {
            return;
        }
        lastPosition = transform.position;
        if (Time.time-lastTime<=intervalTime)
        {
            return;
        }
        lastTime = Time.time;
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            Mesh newMesh=new Mesh();
            meshRenderer[i].BakeMesh(newMesh);

            GameObject item = new GameObject();
           var entity=item.AddComponent<TeleportShadowEntity>();
            entity.duration = duration;
            
            entity.bianyuanMult = bianyuanMult;
            entity.destoryTime = Time.time + duration;

            item.hideFlags = HideFlags.HideAndDontSave;
            MeshRenderer newMeshRender =item.AddComponent<MeshRenderer>();

            var filter = item.AddComponent<MeshFilter>();
            filter.mesh = newMesh;
            newMeshRender.material = meshRenderer[i].material;
            newMeshRender.material.shader = shader;
            newMeshRender.material.SetColor("_RimColor",emissionColor);
            newMeshRender.material.SetFloat("_RimIntensity",bianyuanMult);
            item.transform.localScale = meshRenderer[i].transform.localScale;
            item.transform.position = meshRenderer[i].transform.position;
            item.transform.rotation = meshRenderer[i].transform.rotation;

            
        }
    }
}
