using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NeEntityConfig", menuName = "FSMState/CreateNeEntityConfig", order = 0)]
public class NeEntityConfig:FsmStateConfig<NeEntityConfig,SkeletonState,SkeletonTransitionType>{

new public static void SetScriptablePath()
{
scriptablePath = PathDefine.FsmStateConfigPath+"NeEntityConfig";
}
}
