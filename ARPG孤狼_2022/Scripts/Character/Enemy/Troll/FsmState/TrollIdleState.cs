using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TrollIdleState : IState<TrollState, TrollTransitionType, TrollController>
{
    public TrollIdleState(TrollState stateType, FsmStateMachine<TrollState, TrollTransitionType, TrollController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {
    }
    public int idleBlend = 0;
    public override void Enter(TrollController go, params object[] args)
    {
        base.Enter(go, args);
        go.lockTarget = null;
        idleBlend = go.RandomIdleBlend();
        go.facade.characterAnim.SetInt(AnimatorParameter.Forward.ToString(),0);
    }
    public override void Exit(TrollController go, params object[] args)
    {
        base.Exit(go, args);
        go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(),0,1,true);
    }
    public override void Excute(TrollController go, params object[] args)
    {
        
    }

    public override void OnUpdate(TrollController go, params object[] args)
    {
        go.LerpForwardBlend(0);
        go.UpdateTarget();
        go.LerpAnimationFloatVlaue(AnimatorParameter.IdleBlend.ToString(),idleBlend,Time.deltaTime*5,false);
    }
}

