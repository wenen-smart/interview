using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
[System.Obsolete("此类已经弃用，更换为EndSkill_TargetImapct",true)]
public class EndSkill_SelfImpact : ISelfImpact
{
    public void SelfImpact(SkillDeployer deployer, SkillEntity skillData, GameObject goSelf)
    {
        if (skillData.damageableList!=null&&skillData.skillData.endSkillID!=-1)
        {
            ActorSystem selfActorSystem = goSelf.GetComponent<ActorSystem>();
            var weaponManager= selfActorSystem.GetActorComponent<WeaponManager>();
            if (weaponManager!=null)
            {
                PlayableAsset asset = null;
                bool isBackAttack = goSelf.transform.IsInEnemyBack(weaponManager.attackedTarget?.transform);
                int endSkillId = isBackAttack ?skillData.skillData.backStabID_endSkill:skillData.skillData.endSkillID;
                IDamageable target = null;
                //背面攻击几率绝杀更大
                if (weaponManager.soonDiedTarget)
                {
                    target = weaponManager.soonDiedTarget;
                    //CharacterFacade facade=goSelf.GetComponent<CharacterFacade>();
                     asset = EndKillTimeLineConfig.Instance.GetTimeLineAsset(endSkillId,selfActorSystem.roleDefinition,target.actorSystem.roleDefinition);
                    if (asset==null)
                    {
                        endSkillId = isBackAttack==false ?skillData.skillData.backStabID_endSkill:skillData.skillData.endSkillID;
                        EndKillTimeLineConfig.Instance.GetTimeLineAsset(endSkillId,selfActorSystem.roleDefinition,target.actorSystem.roleDefinition);
                    }
                }
                else
                {
                    if (weaponManager.soonAttackedTarget)
                    {
                        if (isBackAttack)
                        {
                            target = weaponManager.soonAttackedTarget;
                            RoleBetweenPlayerableConfig roleBetweenPlayerableConfig = EndKillTimeLineConfig.Instance.GetRoleBetweenConfig(endSkillId, selfActorSystem.roleDefinition, target.actorSystem.roleDefinition);
                            if (roleBetweenPlayerableConfig != null)
                            {
                                if (roleBetweenPlayerableConfig.IsCanEndSkill(target.MaxHP, target.hp))
                                {
                                    if (roleBetweenPlayerableConfig.IsCanAccidentEndSkill())
                                    {
                                        asset = EndKillTimeLineConfig.Instance.GetTimeLineAsset(roleBetweenPlayerableConfig);
                                        if (asset == null)
                                        {
                                            endSkillId = isBackAttack == false ? skillData.skillData.backStabID_endSkill : skillData.skillData.endSkillID;
                                            EndKillTimeLineConfig.Instance.GetTimeLineAsset(endSkillId, selfActorSystem.roleDefinition, target.actorSystem.roleDefinition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (asset)
                {
                    EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(selfActorSystem.GetActorComponent<RoleController>(), target.GetActorComponent<RoleController>(), asset);
                    selfActorSystem.GetComponent<PlayerStateManager>().nextAttackVaildSign = false;
                    weaponManager.soonDiedTarget = null;
                    weaponManager.soonAttackedTarget = null;
                }
            }
        }
    }

}
