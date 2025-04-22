using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerTriggerComboDeploy:SkillDeployer
{
    public  void FixedUpdate()
    {
        bool attackVaild = skillEntity.weaponManager.CheckAttackTrigger(skillEntity,DelopySkill);
    }
    public override IDamageable[] ResetTarget()
    {
        if (skillEntity.weaponManager.soonAttackedTarget==null)
        {
            return null;
        }
        return new IDamageable[] { skillEntity.weaponManager.soonAttackedTarget };
    }
    public override void DelopySkill()
    {
        base.DelopySkill();
        IDamageable[] targets = ResetTarget();
        skillEntity.damageableList = targets;


        listSelfImpact.ForEach(selfImpact => selfImpact.SelfImpact(this, skillEntity, skillEntity.owner.gameObject)); ;
        if (targets != null)
        {
            foreach (var item in targets)
            {
                listTargetImpact.ForEach(targetImpact => targetImpact.TargetImpact(this, skillEntity, item));
            }
        }
    }
}

