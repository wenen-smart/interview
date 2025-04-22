using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SwordAirComboAttackDeploy : SkillDeployer
{
    public override void DelopySkill()
    {
        base.DelopySkill();
        IDamageable[] targets = ResetTarget();
        skillEntity.damageableList = targets;
        listSelfImpact.ForEach(selfImpact => selfImpact.SelfImpact(this, skillEntity, skillEntity.owner.gameObject));
        //Time.timeScale = 0.5f;
        if (targets != null)
        {
            foreach (var item in targets)
            {
                listTargetImpact.ForEach(targetImpact=> targetImpact.TargetImpact(this,skillEntity,item));


                //Excute
                //Vector3 currentTargetPos = item.characterFacade.transform.position;
                //currentTargetPos.y = skillEntity.owner.position.y;
                //item.characterFacade.transform.position = currentTargetPos;
                
                

                Vector3 force = GetAttackDirForce(skillEntity);
                BattleManager.Instance.CastForce(skillEntity.owner.GetComponent<ActorSystem>(), item.actorSystem, force);

            }
            //Effect

        }
    }
}

