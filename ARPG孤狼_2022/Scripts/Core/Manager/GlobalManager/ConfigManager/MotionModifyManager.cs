using Buff.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MotionModifyManager : IManager
{

    public static Dictionary<int, MotionModifyClipData> motionClipDataDics = new Dictionary<int, MotionModifyClipData>();
    public void Init()
    {
        MyDebug.DebugPrint("Init-MotionModifyManager");
    }
    public void LoadScript(object args)
    {
        MyDebug.DebugPrint("LoadConfig-MotionModifyManager");
        ExcelDataObject excelDataObject = ConfigResFactory.LoadConifg<ExcelDataObject>(PathDefine.motionModifyConfigPath);
        foreach (var _row in excelDataObject._Rows)
        {
            MotionModifyClipData clip = new MotionModifyClipData();
            clip.MID.StringToInt(_row[(int)_Data_MotionModifyConfig.MID]);
            clip.variableType = (MotionVariableType)Enum.Parse(typeof(MotionVariableType),_row[(int)_Data_MotionModifyConfig.VariableType]);
            switch (clip.variableType)
            {
                case MotionVariableType.Time:
                    clip.targetTime.StringToFloat(_row[(int)_Data_MotionModifyConfig.targetVariable]);
                    break;
                case MotionVariableType.Speed:
                    clip.motionSpeed.StringToFloat(_row[(int)_Data_MotionModifyConfig.targetVariable]);
                    break;
                default:
                    break;
            }
            clip.IsUnInterruptedCalcuateVelocity.StringToBoolean(_row[(int)_Data_MotionModifyConfig.UnInterruptedCalcuateVelocity]);
            clip.isMotionOverride.StringToBoolean(_row[(int)_Data_MotionModifyConfig.MotionOverride]);
            clip.motionModifyType = (MotionModifyType)Enum.Parse(typeof(MotionModifyType),_row[(int)_Data_MotionModifyConfig.MotionType]);
            motionClipDataDics.Add(clip.MID,clip);
        }
    }

    public static MotionModifyClip GetMotionClip(int MID)
    {
        MotionModifyClipData clipData;
        motionClipDataDics.TryGetValue(MID,out clipData);
        MotionModifyClip clip = MotionClipFactory.Instance.GetClip(clipData.motionModifyType);
        clip.data = clipData;
        return clip;
    }
    
}


