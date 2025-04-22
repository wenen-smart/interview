using Buff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UpPickAttackDeployer:SkillDeployer
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
            Debug.Log("listTargetImpact:" + listTargetImpact.Count);
            foreach (var item in targets)
            {
                listTargetImpact.ForEach(targetImpact => targetImpact.TargetImpact(this, skillEntity, item));
                var anim = item.characterFacade?.playerStateManager?.characterAnim;
                Vector3 force = GetAttackDirForce(skillEntity);
                BattleManager.Instance.CastForce(skillEntity.owner.GetComponent<ActorSystem>(), item.actorSystem, force);
                //if (anim)
                //{
                //    item.GetComponent<RoleController>().UpPickAttacked();
                //}
            }
            //GameTimeScaleCtrl.Instance.SetTimeSacle(0.8f,500,1,()=> { CollectSkill(); });
        }



    }
}

