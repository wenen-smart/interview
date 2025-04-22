using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "TrollAttackFSMConfig", menuName = "FSMState/CreateTrollAttackFSMConfig", order = 1)]
public class TrollAttackFSMConfig:FsmStateConfig<TrollAttackFSMConfig,TrollAttackStateType,TrollAttackTransitionType>{

new public static void SetScriptablePath()
{
scriptablePath = PathDefine.FsmStateConfigPath+"TrollAttackFSMConfig";
}
}
