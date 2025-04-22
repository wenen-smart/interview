using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SkillDeployer : MonoBehaviour
    {

    private SkillEntity m_skillEntity;
    private float lifeTime;
    public ActorSystem Releaser;

    protected List<ISelfImpact> listSelfImpact = new List<ISelfImpact>();
    protected List<ITargetImpact> listTargetImpact = new List<ITargetImpact>();
    protected IAttackSelector attackSelector;
    public Action<IDamageable[]> selectTargetHandler;

    public SkillEntity skillEntity { get {return m_skillEntity; }
        set {m_skillEntity = value;
        attackSelector = DeployerConfigFactory.
                    CreateAttackSelector(m_skillEntity.skillData);
                listSelfImpact = DeployerConfigFactory.
                    CreateSelfImpact(m_skillEntity.skillData);
                listTargetImpact = DeployerConfigFactory.
                    CreateTargetImpact(m_skillEntity.skillData);
        }
    }
    public bool triggerCheck = false;
    private bool isDelopyed = false;
    public virtual void PrepareDelopySkill()
    {
        triggerCheck = false;
        if (skillEntity.skillData.isTriggerCheck==false)
        {
            DelopySkill();
            isDelopyed = true;
        }
        else
        {
            triggerCheck = skillEntity.skillData.isTriggerCheck;
            isDelopyed = true;
        }
         CollectSkill();
    }
    public virtual void DelopySkill()
    {
       
    }
    public virtual void Update()
    {
        if (isDelopyed&&triggerCheck==false)
        {
            DelopySkill();
        }
    }
    //public virtual void FixedUpdate()
    //{
    //    if (isDelopyed&&triggerCheck)
    //    {
    //        bool attackVaild = skillEntity.weaponManager.CheckAttackTrigger(skillEntity,DelopySkill);
    //        if (attackVaild)
    //        {
    //            //DelopySkill(); //由于CheckAttackTrigger 中攻击有效会立即扣除敌人血量。此时在函数结束再去调用释放技能有bug，应该在扣除血量前去释放技能
    //        }
    //    }
    //}

    public virtual void CollectSkill()
    {
        lifeTime = skillEntity.skillData.durationTime;
        GameRoot.Instance.AddTask((int)(lifeTime), () =>
         {
             triggerCheck = false;
             gameObject.SetActive(false);
             isDelopyed = false;
         }
        );
    }

    public virtual IDamageable[] ResetTarget()
    {
        var targets = attackSelector.SelectTarget(skillEntity, transform);
        if (targets != null && targets.Length > 0) return targets;
        return null;
    }
    public Vector3 GetAttackDirForce(SkillEntity skillEntity)
    {
        SkillData skillData = skillEntity.skillData;
        Vector3 force = Vector3.zero;
        if (skillData==null)
        {
            MyDebug.DebugSupperError("传入SkillEntity.skillData无效，请检查");
            return force;
        }
        if (skillData.IsAutoAttackDir)
        {
            force = skillEntity.owner.GetComponent<CharacterFacade>().weaponManager.GetAttackDir().normalized;
            force *= skillEntity.skillData.force.magnitude;
        }
        else
        {
            force = skillData.force;//攻击者方向上的力
            //Convert to world
            force = skillEntity.owner.transform.TransformDirection(force);
        }
        return force;
    }
}

