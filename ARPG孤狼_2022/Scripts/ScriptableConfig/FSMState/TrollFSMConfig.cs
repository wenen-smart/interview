using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "TrollFSMConfig", menuName = "FSMState/CreateTrollFSMConfig", order = 0)]
public class TrollFSMConfig:FsmStateConfig<TrollFSMConfig,TrollState,TrollTransitionType>{

new public static void SetScriptablePath()
{
scriptablePath = PathDefine.FsmStateConfigPath+"TrollFSMConfig";
}
}
