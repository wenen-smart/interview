using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKMotionBlendAnim:MonoBehaviour
    {
    public Transform excuteRig;
    public Vector3 defaultPos;
    public Transform TargetPos;
    public bool motionIK;

    public void Start()
    {
        defaultPos = excuteRig.transform.position;
    }
    public void Update()
    {
       
    }
}

