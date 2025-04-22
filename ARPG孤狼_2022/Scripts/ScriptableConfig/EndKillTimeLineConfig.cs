using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName ="EndKillTimeLineConfig",menuName = "CreateConfig/CreateEndKillTimeLineConfig",order = 3)]
  public  class EndKillTimeLineConfig:ScriptableConfigTon<EndKillTimeLineConfig>
    {
    
    new public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.EndKillTimeLineConfigPath;
    }
    public List<SpecialTimeLineConfig> specialTimeLineConfigs = new List<SpecialTimeLineConfig>();
    private List<SpecialTimeLineConfig> backStabTimeLineConfigs = new List<SpecialTimeLineConfig>();
    public override void OnEnable()
    {
        base.OnEnable();
        backStabTimeLineConfigs= specialTimeLineConfigs.FindAll((config)=>config.endSkillType.IsSelectThisEnumInMult(EndSkillType.BackStab,true));
    }
    public PlayableAsset GetTimeLineAsset(int id,RoleObjectDefinition attackerDef, RoleObjectDefinition hiterDef)
    {
        RoleBetweenPlayerableConfig relativeRoleConfig=GetRoleBetweenConfig(id,attackerDef,hiterDef);
        return GetTimeLineAsset(relativeRoleConfig);
    }
    public PlayableAsset GetTimeLineAsset(RoleBetweenPlayerableConfig relativeRoleConfig)
    {
        if (relativeRoleConfig != null)
        {
            return relativeRoleConfig.playerableAssets[UnityEngine.Random.Range(0, relativeRoleConfig.playerableAssets.Length)];
        }
        return null;
    }

    public RoleBetweenPlayerableConfig GetRoleBetweenConfig(int id,RoleObjectDefinition attackerDef, RoleObjectDefinition hiterDef)
    {
        var configItem = specialTimeLineConfigs.Find(config => config.id == id);
        RoleBetweenPlayerableConfig relativeRoleConfig = null;
        if (configItem!=null)
        {
            foreach (var relativeRole in configItem.relativeRoleList)
            {
                if (relativeRole.IsMatch(attackerDef.roleID, hiterDef.roleID))
                {
                    relativeRoleConfig = relativeRole;
                    break;
                }
            }
        }
        
        return relativeRoleConfig;
    }
    public PlayableAsset GetBackStabTimeLineConfigs(int id,RoleObjectDefinition attackerDef, RoleObjectDefinition hiterDef)
    {
        var configItem = backStabTimeLineConfigs.Find(config => config.id == id);
        RoleBetweenPlayerableConfig relativeRoleConfig = null;
        foreach (var relativeRole in configItem.relativeRoleList)
        {
            if (relativeRole.IsMatch(attackerDef.roleID, hiterDef.roleID))
            {
                relativeRoleConfig = relativeRole;
                break;
            }
        }
        if (relativeRoleConfig != null)
        {
            return relativeRoleConfig.playerableAssets[UnityEngine.Random.Range(0, relativeRoleConfig.playerableAssets.Length)];
        }
        return null;
    }
    public PlayableAsset GetPlayerableAsset(RoleObjectDefinition attackerDef, RoleObjectDefinition hiterDef, EndSkillType endSkillType)
    {
        List<SpecialTimeLineConfig> configs = specialTimeLineConfigs.FindAll((config)=>config.endSkillType.IsSelectThisEnumInMult(endSkillType,true));
        List<RoleBetweenPlayerableConfig> matchConfig = new List<RoleBetweenPlayerableConfig>();
        foreach (var configItem in configs)
        {
            foreach (var relativeRole in configItem.relativeRoleList)
            {
                if (relativeRole.IsMatch(attackerDef.roleID,hiterDef.roleID))
                {
                    matchConfig.Add(relativeRole);
                }
            }
        }
        if (matchConfig.Count>0)
        {
            RoleBetweenPlayerableConfig playableConfig=matchConfig[UnityEngine.Random.Range(0,matchConfig.Count)];
            return playableConfig.playerableAssets[UnityEngine.Random.Range(0, playableConfig.playerableAssets.Length)];
        }
        return null;
    }


}
[Serializable]
public class SpecialTimeLineConfig
{
    public string showerName;
    [MultSelectTags]
    public EndSkillType endSkillType;
    public int id;
    public RoleBetweenPlayerableConfig[] relativeRoleList;
    public string showerDescription;
}
[Serializable]
public class RoleBetweenPlayerableConfig
{
    [Header("发起者"),Tooltip("填写角色名称")]
    public int caster;
    [Header("被影响者"),Tooltip("填写角色名称")]
    public int affector;
    [Header("血量低于触发绝杀"),Range(0, 100)]
    public int hpLowPerCanEndSkill = 1;
    [Header("意外触发绝杀的概率"),Range(0,100)]
    public int accidentEndSkillPer;
    public PlayableAsset[] playerableAssets;//动画资源
    public bool IsMatch(int casterID,int affectorID)
    {
        return (caster == casterID) && (affector == affectorID);
    }
    public bool IsCanAccidentEndSkill()
    {
        int ran = UnityEngine.Random.Range(0,100);
        return ran < accidentEndSkillPer;
    }
    public bool IsCanEndSkill(int maxHp,int hp)
    {
        return (int)(maxHp * hpLowPerCanEndSkill*1.0f/100) >= hp;
    }
}
public enum EndSkillType
{
    FrontStab=1,
    FrontStrike=1<<1,
    BackStab=1<<2,
    BackStrike=1<<3,
    SleepStrike=1<<4,
    Common=1<<5,
}
