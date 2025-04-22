using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class Pd_SetTransMixer : PlayableBehaviour
{
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

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (playerData==null)
        {
            return;
        }
        RoleController actor = playerData as RoleController;
        if (actor)
        {
        Vector3 targetVec = Vector3.zero;
        Vector3 rotaton = Vector3.zero;
            for (int i = 0; i < playable.GetInputCount(); i++)
            {
                float weight = playable.GetInputWeight(i);
                var clipPlayable = playable.GetInput(i);
                var clipMono = ((ScriptPlayable<Pd_SetTransBehaviour>)clipPlayable).GetBehaviour();


                if (((int)(clipMono.setTransDataType) & (1 << (int)SetTransDataType.Position)) != 0)
                {
                    targetVec += clipMono.calcuatePosResult * weight;

                }

                if (((int)(clipMono.setTransDataType) & (1 << (int)SetTransDataType.Rotation)) != 0)
                {

                    rotaton += clipMono.calCuateEulerResult * weight;

                }

            }

            if (actor)
            {
                if (targetVec != Vector3.zero)
                {
                    actor.POSITION = targetVec;
                }

                if (rotaton!=Vector3.zero)
                {
                    actor.transform.eulerAngles = rotaton;
                }
               
            
        }

        }
    }
}
