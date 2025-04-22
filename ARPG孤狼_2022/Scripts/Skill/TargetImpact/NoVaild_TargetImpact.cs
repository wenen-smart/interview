using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NoVaild_TargetImpact : ITargetImpact
{
    public void TargetImpact(SkillDeployer deployer, SkillEntity skillEntity, IDamageable goTarget)
    {
        MyDebug.DebugSupperError("此枚举已经移除 当前枚举配置无效");
    }
}
