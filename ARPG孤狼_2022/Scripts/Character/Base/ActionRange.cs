using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ActionRange : MonoBehaviour
{
    public float radius;
    public bool IsInActionRange(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) <= radius;
    }
    public void OnDrawGizmosSelected()
    {
        Color color = Color.green;
        color.a = 0.5f;
        Handles.color = color;
        Handles.DrawSolidArc(transform.position,Vector3.up,Vector3.forward,360,radius);
    }
}

