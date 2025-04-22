using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "WolfFSMStateConfig", menuName = "FSMState/CreateWolfFSMStateConfig", order = 0)]
public class WolfFSMStateConfig : FsmStateConfig<WolfFSMStateConfig, WolfState, WolfTransitionType>
{

    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.FsmStateConfigPath + "WolfFSMStateConfig";
    }
}
