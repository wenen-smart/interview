using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public struct BaseRaycastHit
{
    public Vector3 point;
    public Vector3 normal;
    public Collider collider;
    public BaseRaycastHit(RaycastHit hitInfo)
    {
        this.point = hitInfo.point;
        this.normal = hitInfo.normal;
        this.collider = hitInfo.collider;
    }
    public BaseRaycastHit(Vector3 point, Vector3 normal, Collider collider)
    {
        this.point = point;
        this.normal = normal;
        this.collider = collider;
    }
    public bool ConvertTo(Vector3 point, Vector3 normal, Collider collider)
    {
        this.point = point;
        this.normal = normal;
        this.collider = collider;
        return true;
    }
    public bool ConvertTo(RaycastHit hitInfo)
    {
        this.point = hitInfo.point;
        this.normal = hitInfo.normal;
        this.collider = hitInfo.collider;
        return true;
    }
}
