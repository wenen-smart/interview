using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    public class SkillSystem:InstanceMono<SkillSystem>
    {
    //SkillEntity currentSkillEntity;
    //CharacterFacade characterFacade;
    public  override void Awake()
    {
        base.Awake();
        //characterFacade = GetComponent<CharacterFacade>();
        //characterFacade.attackHandler += DeployerSkill;
    }
    //public void DeployerSkill()
    //{
    //    if (characterFacade.skillManager!=null)
    //    {
    //        characterFacade.skillManager.DeployerSkill(currentSkillEntity);
    //    }
    //}
    public void AttackSkillHandler(RoleController roleController,int skillID,Func<bool> beforeAction=null,Action<IDamageable[]> selectTargetsHandler=null)
    {
        //播放Owner动画
        var skillentity = roleController.skillManager.PrepareSkill(skillID);
        bool isCool = !skillentity.GetSkillCDState();
        if (isCool)
        {
            return;
        }
        skillentity.RefreshCD();
        skillentity.ClearDamageRecord();
        roleController.skillManager.currentSkillEntity = skillentity;
        var facade = roleController.GetComponent<CharacterFacade>();
        if (facade&& facade.weaponManager)
        {
            facade.weaponManager.triggerCheck = roleController.skillManager.currentSkillEntity.skillData.isTriggerCheck;
        }
      
        //SelectTarget(roleController);
        bool excuteNext;
        excuteNext = !(beforeAction!=null&&beforeAction.Invoke());
        if (excuteNext)
        {
            roleController.anim.SetInteger(AnimatorParameter.AttackAction.ToString(), skillentity.skillData.anim_attackAction);
            roleController.anim.SetTrigger(AnimatorParameter.Attack.ToString()); 
        }
        if (skillentity.skillData.isAutoLockTarget==true)
        {
           Transform target=SelectTarget(roleController);
            if (target)
            {
                roleController.FixedCharacterLookToTarget(target);
            }
        }
    }
    /// <summary>
    /// 用于特殊技能使用 非攻击类的
    /// </summary>
    /// <param name="roleController"></param>
    /// <param name="skillID"></param>
    /// <param name="beforeAction"></param>
    /// <param name="selectTargetsHandler"></param>
    public SkillEntity SkillHandler(RoleController roleController, int skillID,bool immediatelyDeploy=false, Func<bool> beforeAction = null, Action<IDamageable[]> selectTargetsHandler = null)
    {
        //播放Owner动画
        var skillentity = roleController.skillManager.PrepareSkill(skillID);
        bool isCool = !skillentity.GetSkillCDState();
        if (isCool)
        {
            return null;
        }
        skillentity.RefreshCD();
        skillentity.ClearDamageRecord();
        roleController.skillManager.currentSkillEntity = skillentity;
        if (immediatelyDeploy==true)
        {
            roleController.skillManager.DeployerSkill();
        }
        var facade = roleController.GetComponent<CharacterFacade>();
        if (facade && facade.weaponManager)
        {
            facade.weaponManager.triggerCheck = roleController.skillManager.currentSkillEntity.skillData.isTriggerCheck;
        }

        //SelectTarget(roleController);
        bool excuteNext;
        excuteNext = !(beforeAction != null && beforeAction.Invoke());
        if (excuteNext)
        {
            roleController.anim.SetInteger(AnimatorParameter.AttackAction.ToString(), skillentity.skillData.anim_attackAction);
            roleController.anim.SetTrigger(AnimatorParameter.Attack.ToString());
        }
        if (skillentity.skillData.isAutoLockTarget == true)
        {
            Transform target = SelectTarget(roleController);
            if (target)
            {
                roleController.FixedCharacterLookToTarget(target);
            }
        }
        return skillentity;
    }
    public IDamageable[] Track(RoleController roleController,int nextSkillID)
    {
       var skillEntity = roleController.skillManager.PrepareSkill(nextSkillID);
       var  attackSelector = DeployerConfigFactory.
                     CreateAttackSelector(skillEntity.skillData);
        return attackSelector.InTrackTarget(skillEntity,roleController.transform);
        
    }
    public Transform SelectTarget(RoleController roleController)
    {
        List<RoleController> listTargets = new List<RoleController>();
        for (int i = 0; i < roleController.skillManager.currentSkillEntity.attackerTags.Length; i++)
        {
          //var targets= GameObject.FindGameObjectsWithTag(roleController.skillManager.currentSkillEntity.attackerTags[i]);

          var targets= MapManager.Instance.FindActors(roleController.skillManager.currentSkillEntity.attackerTags[i]);
            if (targets != null && targets.Count > 0)
            { listTargets.AddRange(targets); }
        }
        //过滤
        float minDistance = Mathf.Infinity;
      var enemys= listTargets.FindAll(
            (target)=> {
                var dis = Vector3.Distance(roleController.transform.position, target.transform.position);
                
                return dis < roleController.skillManager.currentSkillEntity.skillData.attackDistance && (roleController.skillManager.currentSkillEntity.skillData.attackangle == 0 || Vector3.Angle(roleController.transform.forward, target.transform.position - roleController.transform.position) < roleController.skillManager.currentSkillEntity.skillData.attackangle); }
        
        );
        Transform enemy = null;
        if (enemys!=null&&enemys.Count>0)
        {
          
            for (int i = 0; i < enemys.Count; i++)
            {
                var dis = Vector3.Distance(roleController.transform.position, enemys[i].transform.position);
                if (dis<minDistance)
                {
                    minDistance = dis;
                    enemy = enemys[i]?.transform ;
                }
            }
        }
        return enemy;
    }
    public void RandomAttackSkillHandler(List<SkillData> skillList)
    {

    }

    /// <summary>
    /// 综合计算得到最终的力
    /// </summary>
    /// <param name="damageTargetForce"></param>
    /// <param name="senderForce"></param>
    /// <returns></returns>
    public float CalcuateUpPickForce(float damageTargetForce,float senderForce)
    {

        return 0;
    }
}
