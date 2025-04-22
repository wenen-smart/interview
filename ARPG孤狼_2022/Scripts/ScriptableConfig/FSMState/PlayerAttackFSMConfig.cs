using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttackFSMConfig", menuName = "FSMState/CreatePlayerAttackFSMConfig", order = 14)]
public class PlayerAttackFSMConfig:FsmStateConfig<PlayerAttackFSMConfig,AttackType,AttackTransitionType>{

new public static void SetScriptablePath()
{
scriptablePath = PathDefine.FsmStateConfigPath+"PlayerAttackFSMConfig";
}
}
