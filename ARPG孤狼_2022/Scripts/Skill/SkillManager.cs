using Buff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillManager : IActorMonoManager
{
    public List<SkillEntity> skillEntity;
    public SkillEntity currentSkillEntity;
    [HideInInspector]public BuffContainer buffContainer;
    public SkillDataConfig dataConfig;

    public override void Init()
    {
        if (buffContainer == null)
        {
            buffContainer = GetComponent<BuffContainer>();
            if (buffContainer == null)
            {
                buffContainer = gameObject.AddComponent<BuffContainer>();
            }
        }
        ReferenceOpposeTag();
    }
    public void ReferenceOpposeTag()
    {
        string[] strs = GetActorComponent<RoleController>().opposeCampTagStrList;
        foreach (var entity in skillEntity)
        {
            entity.attackerTags = strs;
        }
    }
    public override void Start()
    {
        base.Start();
        foreach (var item in skillEntity)
        {
            item.owner = actorSystem;
            item.weaponManager = GetActorComponent<CharacterFacade>().weaponManager;
#if UNITY_EDITOR
            if (dataConfig == null)
            {
                Debug.LogWarning($"{gameObject.name}:Skill配置为空请注意");
            }
#endif
            item.skillData = dataConfig?.GetSkillData(item.skillID);
        }
        GetComponent<CharacterFacade>().attackHandler += ()=> { DeployerSkill(); };
    }

    public SkillEntity PrepareSkill(int skillID)
    {
        SkillEntity entity = skillEntity.Find((s) => {return s.skillID == skillID; });
        if (entity!=null)
        {
            if (entity.skillData==null)
            {
                entity.skillData = dataConfig.GetSkillData(skillID);
            }
            
        }
        return entity;
    }

    public void DeployerSkill()
    {
        //创建预制件  _技能释放
        GameObject deployPrefab= currentSkillEntity.skillData.skillPrefab;
      GameObject deployerGo= GameObjectFactory.Instance.PopItem(deployPrefab);
        //skillEntity传递->技能释放预制件
        if (deployerGo)
        {
            var deployer = deployerGo.GetComponent<SkillDeployer>();
            deployerGo.transform.position = currentSkillEntity.owner.transform.position;
            deployer.skillEntity = currentSkillEntity;
            deployer.Releaser = actorSystem;
            //释放技能
            deployer.PrepareDelopySkill();
            //技能冷却倒计时
        }

    }
    protected static SkillData ReadySkillData;//
    protected static SkillEntity ReadySkillEntity;
}

