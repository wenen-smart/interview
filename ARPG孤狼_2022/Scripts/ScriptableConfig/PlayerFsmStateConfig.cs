using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerFsmStateConfig", menuName = "FSMState/Player/CreatePlayerFsmStateConfig", order = 1)]
public class PlayerFsmStateConfig : FsmStateConfig<PlayerFsmStateConfig, PlayerStateType, PlayerTransitionType>
{
    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.FsmStateConfigPath+"PlayerFsmStateConfig";
    }
}
