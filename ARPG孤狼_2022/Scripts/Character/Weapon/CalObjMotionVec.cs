using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CalObjMotionVec:MonoBehaviour
    {

    public int deltaFrame=5;
    protected int frameCounter = 0;
    public Vector3 motionVec;
    public Vector3 lastMotionPos;
    public void Update()
    {
        frameCounter++;
        if (lastMotionPos==Vector3.zero)
        {
            lastMotionPos = transform.position;
        }
        if (frameCounter>=deltaFrame)
        {
            frameCounter = 0;
            motionVec = transform.position - lastMotionPos;
            lastMotionPos = transform.position;
        }
    }
    public void CloseAndRenew()
    {
        this.enabled = false;
        frameCounter = 0;
        //this.motionVec = Vector3.zero;
        this.lastMotionPos = Vector3.zero;
    }
    public virtual Vector3 CalculateCurrentMotionVec()
    {
        if (lastMotionPos==Vector3.zero)
        {
            return Vector3.zero;
        }
        return transform.position - lastMotionPos;
    }
}

