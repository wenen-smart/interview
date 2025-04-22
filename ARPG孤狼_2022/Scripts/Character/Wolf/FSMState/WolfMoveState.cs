using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class WolfMoveState : IState<WolfState, WolfTransitionType, WolfController>
{
    public WolfMoveState(WolfState stateType, FsmStateMachine<WolfState, WolfTransitionType, WolfController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }
    float followSafeDis = 0;

    public override void Excute(WolfController go, params object[] args)
    {
        followSafeDis = UnityEngine.Random.Range(2,3);
        
    }

    public override void OnUpdate(WolfController go, params object[] args)
    {
        if (go.FollowTarget!=null)
        {
            if (go.TargetIsInRange(go.FollowTarget.transform,followSafeDis)==false)
            {
                Vector3 targetPos = go.transform.position + go.transform.forward;
                go.FixedCharacterLookToTarget(go.FollowTarget.transform);
                if (go.TargetDirCanWalk(go.transform.forward))
                {
                    go.SetNavMeshTarget(targetPos);
                    go.LerpForwardBlend(1f);
                }
                else
                {
                    go.LerpForwardBlend(0f);
                }
                
            }
            else
            {
                go.StopMove();
                //InSafeDis
            }
        }
        go.UpdateTarget(360);
    }
}

