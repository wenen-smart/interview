using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class FsmStateConfig<T,StateT,TransitionT> : ScriptableConfigTon<T> where T:FsmStateConfig<T,StateT,TransitionT>,new() where StateT:Enum where TransitionT:Enum
{
    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.FsmStateConfigPath;
    }

    public FsmStateData<StateT, TransitionT>[] FsmStateTypeData;
}
