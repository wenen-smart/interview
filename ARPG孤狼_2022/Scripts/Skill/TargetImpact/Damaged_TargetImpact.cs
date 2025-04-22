using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Damaged_TargetImpact : ITargetImpact
{
    public void TargetImpact(SkillDeployer deployer, SkillEntity skillEntity, IDamageable goTarget)
    {
        if (skillEntity.skillData.isTriggerCheck)
        {
            Debug.Log("技能开启了武器触发检测 检测伤害入口，请在配置中删掉本enum");
            return;
        }
        Vector3 attackDir = skillEntity.GetAttackDirInWorldMatrix();
        goTarget.Hit(10,attackDir,skillEntity.owner.transform,skillEntity.skillData);
        skillEntity.RecordDamaged(goTarget);//记录伤害时间
        BattleManager.Instance.AttackFeetBack(skillEntity,goTarget);
    }
}

