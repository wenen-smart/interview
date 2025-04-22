using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//目标影响算法抽象【接口】
public interface ITargetImpact
{
    /// 影响目标的操作【方法】     
    /// <param name="deployer">技能施放器</param>
    /// <param name="skillEntity">技能数据对象</param>
    /// <param name="goSelf">目标对象</param>
    void TargetImpact(SkillDeployer deployer, SkillEntity skillEntity, IDamageable goTarget);
}