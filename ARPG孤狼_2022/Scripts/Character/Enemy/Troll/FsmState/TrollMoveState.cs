using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TrollMoveState : IState<TrollState, TrollTransitionType, TrollController>
{
    SkillManager skillManager;
    public TrollMoveState(TrollState stateType, FsmStateMachine<TrollState, TrollTransitionType, TrollController> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }
    public override void Enter(TrollController go, params object[] args)
    {
        base.Enter(go, args);
        skillManager = go.skillManager;
    }
    public override void Exit(TrollController go, params object[] args)
    {
        base.Exit(go, args);
    }

    public override void Excute(TrollController go, params object[] args)
    {
        
    }

    public override void OnUpdate(TrollController go, params object[] args)
    {
        if (go.IsSeeTarget() && go.TargetIsInFollowRange())
        {
            go.LerpForwardBlend(1);
            go.FixedCharacterLookToTarget(go.lockTarget.transform);
        }
    }
}

