using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SkillEntity
    {
    public int skillID;
    [HideInInspector]public string[] attackerTags;
    public IDamageable[] damageableList;//统计整个技能过程中所发现的对象  暂时不确定 是记录被伤害的 还是说在范围内的。
    [HideInInspector]
    public ActorSystem owner;
    [HideInInspector]
    public SkillData skillData;
    [HideInInspector]
    public MyTimer skillCDTimer=new MyTimer();
    [Tooltip("-1该技能不是连击技能，不分配连击组")]
    public int comboGroup = -1;
    [HideInInspector]
    public WeaponManager weaponManager;
    [HideInInspector]
    public Dictionary<IDamageable,double> lastDamageablesInDur=new Dictionary<IDamageable, double>();//记录在本次攻击过程中 各个对象最近一次的时间。
    public void RefreshCD()
    {
        if (skillData.coolTime<=0)
        {
            return;
        }
        skillCDTimer.Go(skillData.coolTime);
    }
    /// <summary>
    /// 技能冷却时间是否结束 
    /// </summary>
    /// <returns></returns>
    public bool GetSkillCDState()
    {
        return skillCDTimer.timerState==MyTimer.TimerState.Finish||skillCDTimer.timerState==MyTimer.TimerState.Idle;
    }
    public bool IsHaveTarget()
    {
        return damageableList != null && damageableList.Length > 0;
    }
    /// <summary>
    /// 记录在本次攻击过程中 各个对象最近一次的时间。
    /// </summary>
    /// <param name="damageable"></param>
    public void RecordDamaged(IDamageable damageable)
    {
        if (damageable==null)
        {
            return;
        }
        if (lastDamageablesInDur.ContainsKey(damageable)==false)
        {
            lastDamageablesInDur.Add(damageable,(Time.realtimeSinceStartup*1000));
            return;
        }
        lastDamageablesInDur[damageable]=(Time.realtimeSinceStartup*1000);
    }
    public void ClearDamageRecord()
    {
        lastDamageablesInDur.Clear();
    }
    /// <summary>
    /// 伤害对象距离上次被攻击时间是否达到 伤害间隔
    /// </summary>
    /// <param name="damageable"></param>
    /// <returns></returns>
    public bool DamagedTimeIsOver(IDamageable damageable)
    {
        bool have = lastDamageablesInDur.ContainsKey(damageable);
        if (have)
        {
            if (skillData.damageInterval <= 0)
            {
                //DamageInterval 没超过0 本次阶段仅对一个对象伤害一次。
                return false;
            }
            return (Time.realtimeSinceStartup*1000-lastDamageablesInDur[damageable])>=skillData. damageInterval;
        }
        return true;
    }
    public bool IsCanAttackGroup()
    {
        return skillData.attackType.IsSelectThisEnumInMult(SkillAttackType.Group);
    }
    /// <summary>
    /// 当前攻击是否有对其他对象造成过伤害
    /// </summary>
    /// <returns></returns>
    public bool CurrentIsHaveDamageAnyObject()
    {
        return lastDamageablesInDur.Count > 0;
    }
    public Vector3 GetAttackDirInWorldMatrix()
    {
        return owner.transform.TransformDirection(skillData.attackDir.normalized);
    }
}

