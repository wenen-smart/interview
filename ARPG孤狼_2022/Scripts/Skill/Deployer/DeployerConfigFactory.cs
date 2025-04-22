using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DeployerConfigFactory
{

    public static Dictionary<string, Type> selfImpactTypeDic=new Dictionary<string, Type>();
    public static Dictionary<string, Type> targetImpactTypeDic = new Dictionary<string, Type>();
    static DeployerConfigFactory()
    {
        string[] selfImpactEnums = Enum.GetNames(typeof(SelfImpartType));
        foreach (var enumName in selfImpactEnums)
        {
            var typeName = enumName + "_SelfImpact";
            Type type = Type.GetType(typeName);
            if (type!=null)
            {
                selfImpactTypeDic.Add(enumName, type);
            }
        }

        string[] targetImpactEnums = Enum.GetNames(typeof(TargetImpact));
        foreach (var enumName in targetImpactEnums)
        {
            var typeName = enumName + "_TargetImpact";
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                targetImpactTypeDic.Add(enumName, type);
            }
        }
    }

    /// <summary>
    /// 工厂方法设计模式：创建目标选择对象的方法 符合 定义
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public static IAttackSelector CreateAttackSelector(SkillData skill)
    {
        IAttackSelector attackSelector = null;
        switch (skill.damageMode)
        {
            case DamageMode.Circle:
                attackSelector = new CircleAttackSelector();
                break;
            case DamageMode.Sector:
                attackSelector = new SectorAttackSelector();
                break;
        }
        return attackSelector;
    }
    /// <summary>
    /// 创建 初始化 自身影响
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public static List<ISelfImpact> CreateSelfImpact(SkillData skill)
    {
        List<ISelfImpact> list = new List<ISelfImpact>();
        //list.Add(new EndSkill_SelfImpact());
        foreach (var enumName in selfImpactTypeDic)
        {
            if (((int)skill.selfImpartEnum).IsSelectThisEnumInMult((int)Enum.Parse(typeof(SelfImpartType),enumName.Key)))
            {
            var selfImpact=Activator.CreateInstance(enumName.Value) as ISelfImpact;
                list.Add(selfImpact);
                Debug.Log("ISelfImpact:" + enumName.Key); 
            }
        }
        //list.Add(new CostSPSelfImpact());
        //list.Add(new ...);
        return list;
    }
    /// <summary>
    ///  创建 初始化 目标影响
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public static List<ITargetImpact> CreateTargetImpact(SkillData skill)
    {
        List<ITargetImpact> list = new List<ITargetImpact>();
        //list.Add(new EndSkill_TargetImpact());
        foreach (var enumName in targetImpactTypeDic)
        {
            if (((int)skill.targetImpartEnum).IsSelectThisEnumInMult((int)Enum.Parse(typeof(TargetImpact), enumName.Key)))
            {
                var targetImpact = Activator.CreateInstance(enumName.Value) as ITargetImpact;
                list.Add(targetImpact);
                Debug.Log("ITargetImpact:" + enumName.Key);
            }
        }
        //list.Add(new CostSPSelfImpact());
        //list.Add(new ...);
        return list;
    }
}
