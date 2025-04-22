using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class EndSkill_SelfImapct : ISelfImpact
{
    public void SelfImpact(SkillDeployer deployer, SkillEntity skillData, GameObject goSelf)
    {
        if (skillData.damageableList!=null&&skillData.skillData.endSkillID!=-1)
        {
            ActorSystem selfActorSystem = goSelf.GetComponent<ActorSystem>();
            var weaponManager= selfActorSystem.GetActorComponent<WeaponManager>();
            if (weaponManager!=null)
            {
                if (weaponManager.attackedTarget)
                {
                    //CharacterFacade facade=goSelf.GetComponent<CharacterFacade>();
                    
                    PlayableAsset asset = null;
                    if (goSelf.transform.IsInEnemyBack(weaponManager.attackedTarget?.transform)==false)
                    {
                        asset = EndKillTimeLineConfig.Instance.GetTimeLineAsset(skillData.skillData.endSkillID,selfActorSystem.roleDefinition,weaponManager.attackedTarget.actorSystem.roleDefinition);
                    }
                    else
                    {
                        asset = EndKillTimeLineConfig.Instance.GetBackStabTimeLineConfigs(skillData.skillData.backStabID_endSkill,selfActorSystem.roleDefinition,weaponManager.attackedTarget.actorSystem.roleDefinition);
                    }
                    EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(selfActorSystem.GetActorComponent<RoleController>(), weaponManager.attackedTarget.GetActorComponent<RoleController>(), asset);
                    weaponManager.attackedTarget = null;
                    selfActorSystem.GetComponent<PlayerStateManager>().nextAttackVaildSign = false;
                }

            }
        }
    }

}
