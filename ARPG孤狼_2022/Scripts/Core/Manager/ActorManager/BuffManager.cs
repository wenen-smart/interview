using Assets.Scripts.Buff.Event;
using Buff.Base;
using Buff.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BuffManager:CommonSingleTon<BuffManager>,IManager
{
    public BuffManager()
    {
        _Instance = this;
    }

    public void ExcelEdit()
    {

    }

    public Dictionary<int, BuffData> buffDataByBID = new Dictionary<int, BuffData>();
    public Dictionary<int, Dictionary<_Data_BuffEvent,EventCode>> actionIDConfig=new Dictionary<int, Dictionary<_Data_BuffEvent,EventCode>>();
    public void LoadScript(object args)
    {
        Debug.Log("Load Script.... BuffManager");
        LoadRows(ConfigResFactory.LoadConifg<ExcelDataObject>(PathDefine.buffDataConfigPath));
        LoadActionConfig(ConfigResFactory.LoadConifg<ExcelDataObject>(PathDefine.buffActionConfigPath));
    }
    public void LoadRows(ExcelDataObject excelDataObject)
    {
        for (int i = 0; i < excelDataObject._Rows.Count; i++)
        {
            int bid = Convert.ToInt32(excelDataObject._Rows[i][(int)_Data_Buff.BID]);
            BuffData buffData = new BuffData()
            {
                BID = bid
            };
            buffData.buffClassTypeName = excelDataObject._Rows[i][(int)_Data_Buff.BuffClassType];
            buffData.priority.StringToInt(excelDataObject._Rows[i][(int)_Data_Buff.Priority]);
            buffData.actionID.StringToInt(excelDataObject._Rows[i][(int)_Data_Buff.ActionID]);
            buffData.MID.StringToInt(excelDataObject._Rows[i][(int)_Data_Buff.MID]);
            buffData.benefitType = (BuffBenefitType)Enum.Parse(typeof(BuffBenefitType), excelDataObject._Rows[i][(int)_Data_Buff.BuffBenefitType]);
            buffData.buffActionEffect = (BuffActionEffect)Enum.Parse(typeof(BuffActionEffect), excelDataObject._Rows[i][(int)_Data_Buff.BuffActionEffect]);
            buffData.buffImmunityTag = (BuffTag)Enum.Parse(typeof(BuffTag), excelDataObject._Rows[i][(int)_Data_Buff.BuffImmunityTag]);
            buffData.buffTag = (BuffTag)Enum.Parse(typeof(BuffTag), excelDataObject._Rows[i][(int)_Data_Buff.BuffTag]);
            buffData.currency.StringToFloat(excelDataObject._Rows[i][(int)_Data_Buff.Currency]);
            buffData.duration.StringToFloat(excelDataObject._Rows[i][(int)_Data_Buff.Duration]);
            buffData.executeCount.StringToInt(excelDataObject._Rows[i][(int)_Data_Buff.ExecuteCount]);
            buffData.executeType = (BuffExecuteType)Enum.Parse(typeof(BuffExecuteType), excelDataObject._Rows[i][(int)_Data_Buff.BuffExecuteType]);
                buffData.intervalExecute.StringToFloat(excelDataObject._Rows[i][(int)_Data_Buff.IntervalExecute]);
            buffData.maxOverrideLayer.StringToInt(excelDataObject._Rows[i][(int)_Data_Buff.MaxOverrideLayer]);
            buffData.overrideType = (BuffOverrideType)Enum.Parse(typeof(BuffOverrideType), excelDataObject._Rows[i][(int)_Data_Buff.BuffOverrideType]);
            if (buffDataByBID.ContainsKey(bid)==false)
            {
                buffDataByBID.Add(bid,buffData);
            }
            else
            {
                Debug.LogError("已经存在相同的BID："+bid+" 请检查");
            }
        }
    }
    public void LoadActionConfig(ExcelDataObject excelDataObject)
    {
        for (int i = 0; i < excelDataObject._Rows.Count; i++)
        {
            int actionID = Convert.ToInt32(excelDataObject._Rows[i][(int)_Data_BuffEvent.ActionID]);
            Dictionary<_Data_BuffEvent, EventCode> oneBuffAction = new Dictionary<_Data_BuffEvent, EventCode>();
            oneBuffAction.Add(_Data_BuffEvent.OnBuffAwake,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBuffAwake]));
            oneBuffAction.Add(_Data_BuffEvent.OnBuffStart,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBuffStart]));
            oneBuffAction.Add(_Data_BuffEvent.OnBuffRefresh,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBuffRefresh]));
            oneBuffAction.Add(_Data_BuffEvent.OnBuffRemove,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBuffRemove]));
            oneBuffAction.Add(_Data_BuffEvent.OnBuffDestory,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBuffDestory]));

            oneBuffAction.Add(_Data_BuffEvent.OnSkillExecuted,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnSkillExecuted]));

            oneBuffAction.Add(_Data_BuffEvent.OnBeforeGiveDamage,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBeforeGiveDamage]));
            oneBuffAction.Add(_Data_BuffEvent.OnAfterGiveDamage,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnAfterGiveDamage]));

            oneBuffAction.Add(_Data_BuffEvent.OnBeforeTakeDamage,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBeforeTakeDamage]));
            oneBuffAction.Add(_Data_BuffEvent.OnAfterTakeDamage,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnAfterTakeDamage]));

            oneBuffAction.Add(_Data_BuffEvent.OnBeforeDead,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBeforeDead]));
            oneBuffAction.Add(_Data_BuffEvent.OnAfterDead,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnAfterDead]));

            oneBuffAction.Add(_Data_BuffEvent.OnBeforeKill,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnBeforeKill]));
            oneBuffAction.Add(_Data_BuffEvent.OnAfterKill,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnAfterKill]));

            oneBuffAction.Add(_Data_BuffEvent.OnIntervalThink,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnIntervalThink]));
            oneBuffAction.Add(_Data_BuffEvent.OnIntervalThinkLifeFinish,(EventCode)Enum.Parse(typeof(EventCode),excelDataObject._Rows[i][(int)_Data_BuffEvent.OnIntervalThinkLifeFinish]));
            actionIDConfig.Add(actionID,oneBuffAction);
        }
    }

    public EventCode GetEventCode(int actionID, _Data_BuffEvent buffCallBackType)
    {
        if (actionIDConfig.ContainsKey(actionID))
        {
            var buffCallbacks = actionIDConfig[actionID];
            if (buffCallbacks != null && buffCallbacks.ContainsKey(buffCallBackType))
            {
                return buffCallbacks[buffCallBackType];
            }
        }
        return EventCode.None;
    }
    public EventCode GetEventCodeByBID(int BID, _Data_BuffEvent buffCallBackType)
    {
        BuffData buffData = GetBuffData(BID);
        if (buffData!=null)
        {
            return GetEventCode(buffData.BID,buffCallBackType);
        }
        return EventCode.None;
    }
    public BuffData GetBuffData(int bID)
    {
        BuffData buffData;
        buffDataByBID.TryGetValue(bID,out buffData);
        return buffData;
    }

    public void Init()
    {
        
    }
}

