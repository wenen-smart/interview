using UnityEngine;
using System.Collections;

public class SwordMeleeDeployer : SkillDeployer
{
    public override void DelopySkill()
    {
        base.DelopySkill();
        IDamageable[] targets=ResetTarget();
        skillEntity.damageableList = targets;
       

        listSelfImpact.ForEach(selfImpact=>selfImpact.SelfImpact(this,skillEntity,skillEntity.owner.gameObject));;
        if (targets!=null)
        {
            Debug.Log("listTargetImpact:"+listTargetImpact.Count);
            foreach (var item in targets)
            {
                listTargetImpact.ForEach(targetImpact => targetImpact.TargetImpact(this, skillEntity, item));
            }
        }
        
    }
}
