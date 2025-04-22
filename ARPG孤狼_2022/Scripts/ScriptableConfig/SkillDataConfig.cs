using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillDataConfig", menuName = "CreateConfig/CreateSkillDataConfig", order = 1)]
public class SkillDataConfig:ScriptableConfig
    {
    public List<SkillData> skillDataList;
    public SkillData GetSkillData(int skillId)
    {
        SkillData data=null;
        foreach (var skillData in skillDataList)
        {
            if (skillData.skillID==skillId)
            {
                data = skillData;
                break;
            }
        }
        return data;
    }
}

