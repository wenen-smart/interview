using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class Pd_RootMotionMixer : PlayableBehaviour
{
    Animator animator;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {

    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {

    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {

    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {

    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
      
    }

    public RoleController roleController;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
      
        if (animator==null)
        {
            animator = playerData as Animator;
            roleController = animator.GetComponentInParent<RoleController>();
        }
        if (roleController)
        {
            Vector3 increseVec=Vector3.zero;
            for (int i = 0; i < playable.GetInputCount(); i++)
            {
                float weigtht = playable.GetInputWeight(i);

                var clipPlayable = (ScriptPlayable<Pd_RootMotionBehaviour>)playable.GetInput(i);
               double duraction=clipPlayable.GetDuration();
               
               
         var mono=clipPlayable.GetBehaviour();
                var zCurve = mono.zDirCurve;
                var yCurve = mono.yCurve;
                var xCurve = mono.xCurve;
                float clipTime = mono.time;
                float normalizeTime = (float)(clipTime/duraction);
                if (zCurve!=null&&zCurve.length>1)
                {
                    increseVec += zCurve.Evaluate((normalizeTime)) * Vector3.forward * weigtht;
                }
                if (yCurve!=null&&yCurve.length>1)
                {
                    increseVec += yCurve.Evaluate((normalizeTime)) * Vector3.up * weigtht;
                }
                if (xCurve!=null&& xCurve.length>1)
                {
                    increseVec += xCurve.Evaluate((normalizeTime)) * Vector3.right * weigtht;
                }

                
            }
            roleController.IncreaseVelo += roleController.transform.TransformDirection(increseVec);
        }
       
    }
}
