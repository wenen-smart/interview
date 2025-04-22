using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClassabc", menuName = "FSMState/CreateNewClassabc", order = 100)]
public class NewClassabc:FsmStateConfig<NewClassabc,SkeletonState,SkeletonTransitionType>{

new public static void SetScriptablePath()
{
scriptablePath = PathDefine.FsmStateConfigPath+"NewClassabc";
}
}
