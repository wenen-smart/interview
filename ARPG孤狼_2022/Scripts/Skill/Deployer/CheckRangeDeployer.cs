using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CheckRangeDeployer : SkillDeployer
{
    public override void DelopySkill()
    {
        base.DelopySkill();
        IDamageable[] targets=ResetTarget();
        skillEntity.damageableList = targets;
        selectTargetHandler?.Invoke(targets);
    }
}

