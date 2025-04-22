using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
public abstract class CommonAttackState<StateT,TransitionT,EntityT> : IState<StateT, TransitionT, EntityT> where StateT:Enum where TransitionT:Enum where EntityT:RoleController
{
    public CommonAttackState(StateT stateType, FsmStateMachine<StateT, TransitionT, EntityT> fsmStateMachine) : base(stateType, fsmStateMachine)
    {

    }
}

