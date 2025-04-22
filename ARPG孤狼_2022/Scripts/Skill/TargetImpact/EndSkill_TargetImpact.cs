using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class EndSkill_TargetImpact : ITargetImpact
{
    public void TargetImpact(SkillDeployer deployer, SkillEntity skillEntity, IDamageable goTarget)
    {
        if (skillEntity.damageableList!=null&&skillEntity.skillData.endSkillID!=-1)
        {
            ActorSystem selfActorSystem = skillEntity.owner.GetComponent<ActorSystem>();
            var weaponManager= selfActorSystem.GetActorComponent<WeaponManager>();
            if (weaponManager.soonDiedTarget!=null)
            {
                //CharacterFacade facade=goSelf.GetComponent<CharacterFacade>();

                PlayableAsset asset = null;
                if (skillEntity.owner.transform.IsInEnemyBack(goTarget?.transform) == false)
                {
                    asset = EndKillTimeLineConfig.Instance.GetTimeLineAsset(skillEntity.skillData.endSkillID, selfActorSystem.roleDefinition, goTarget.actorSystem.roleDefinition);
                }
                else
                {
                    asset = EndKillTimeLineConfig.Instance.GetTimeLineAsset(skillEntity.skillData.backStabID_endSkill, selfActorSystem.roleDefinition, goTarget.actorSystem.roleDefinition);
                }
                EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(selfActorSystem.GetActorComponent<RoleController>(), goTarget.GetActorComponent<RoleController>(), asset);
                selfActorSystem.GetComponent<PlayerStateManager>().nextAttackVaildSign = false;
            }
        }
    }
}
