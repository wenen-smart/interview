using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Skeleton_ArchierFSMConfig", menuName = "FSMState/CreateSkeleton_ArchierFSMConfig", order = 9)]
public class Skeleton_ArchierFSMConfig:FsmStateConfig<Skeleton_ArchierFSMConfig,SkeletonState,SkeletonTransitionType>{

new public static void SetScriptablePath()
{
scriptablePath = PathDefine.FsmStateConfigPath+"Skeleton_ArchierFSMConfig";
}
}
