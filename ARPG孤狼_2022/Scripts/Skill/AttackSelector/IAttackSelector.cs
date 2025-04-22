using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// 攻击选择接口【算法】：选择  什么范围/区域 中的敌人 作为攻击目标 ，
///                                 例如：圆形范围/区域 中或扇形范围/区域 中
/// </summary>
public abstract class IAttackSelector
{

    /// <summary>
    /// 选择目标方法：选择 哪些敌人作为要攻击的 目标
    /// </summary>
    /// <param name="skillData">技能对象</param>
    /// <param name="transform">变换对象：选择时的参考点 ；技能拥有者</param>
    /// <returns></returns>
    public IDamageable[] SelectTarget(SkillEntity skillData, Transform skillTransform) { return SelectTarget(skillData,skillTransform,skillData.skillData.attackangle,skillData.skillData.attackDistance); }
    public  IDamageable[] InTrackTarget(SkillEntity skillData, Transform skillTransform)
    {
        IDamageable[] inTrackRangeTargets = SelectTarget(skillData,skillTransform,skillData.skillData.attackangle,skillData.skillData.trackingRange);
        List<IDamageable> canTrackingTargets = new List<IDamageable>();
        if (inTrackRangeTargets!=null&&inTrackRangeTargets.Length>0)
        {
            IDamageable[] inAttackRangeTargets = SelectTarget(skillData,skillTransform,skillData.skillData.attackangle,skillData.skillData.attackDistance);
            foreach (var item in inTrackRangeTargets)
            {
                if (inAttackRangeTargets==null||inAttackRangeTargets.Contains(item)==false)
                {
                    canTrackingTargets.Add(item);
                }
            }
        }
        if (canTrackingTargets.Count==0)
        {
            return null;
        }
        return canTrackingTargets.ToArray();
    }
    public abstract IDamageable[] SelectTarget(SkillEntity skillData, Transform skillTransform,float angle,float distance);
}

