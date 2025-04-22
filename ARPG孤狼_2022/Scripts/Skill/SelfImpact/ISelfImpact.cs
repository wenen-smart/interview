using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ISelfImpact
{
    /// 影响自身的方法
    /// <param name="deployer">技能施放器</param>
    /// <param name="skillEntity">技能数据对象</param>
    /// <param name="goSelf">自身或队友对象</param>
    void SelfImpact(SkillDeployer deployer, SkillEntity skillEntity, GameObject goSelf);
}
