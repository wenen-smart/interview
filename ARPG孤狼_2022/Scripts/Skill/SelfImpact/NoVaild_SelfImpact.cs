using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NoVaild_SelfImpact : ISelfImpact
{
    public void SelfImpact(SkillDeployer deployer, SkillEntity skillEntity, GameObject goSelf)
    {
        MyDebug.DebugSupperError("此枚举已经移除 当前枚举配置无效");
    }
}

