using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class Pd_RootMotionBehaviour : PlayableBehaviour
{
   public AnimationCurve zDirCurve;
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;
    public float time;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        time += info.deltaTime;
    }
}
