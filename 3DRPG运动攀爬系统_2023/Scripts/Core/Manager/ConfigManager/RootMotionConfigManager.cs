using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RootMotionConfig
{
    public AnimClipJudgeWay judgeWay;
    public RootMotionHandlerAim rootAnyPriority;
    public int priority;
    public string clipName;
    public string clipTag;
    public float motionMult;
    public CalcuateRootMotionModal posCalcuateModal;
    public CalcuateRootMotionModal rotationCalcuateModal;
    public RootMotionProcessClear posClearPriorityInProcess;
    public RootMotionProcessClear rotationClearPriorityInProcess;
    //
}
public class RootMotionConfigManager : IManager
{
    public void Init()
    {

    }
    public static Dictionary<string, List<RootMotionConfig>> rootMotionConfigDic = new Dictionary<string, List<RootMotionConfig>>();
    public static Dictionary<string, string> animatorRelationConfigDic = new Dictionary<string, string>();//animator名称对应rootmotionconfig
    public string _Data_KAssetName = "_Data_AnimatorRelationRootMotion";
    public void LoadScript(object args)
    {
        ExcelDataObject _data = ConfigResFactory.LoadConifg<ExcelDataObject>(_Data_KAssetName);
        LoadAllRootMotionConfig(_data._Rows);
    }
    public void LoadAllRootMotionConfig(List<ExcelRowData> rows)
    {
        foreach (var row in rows)
        {
            animatorRelationConfigDic.Add(row[(int)_Data_AnimatorRelationRootMotion.Animator_Name],row[(int)_Data_AnimatorRelationRootMotion.RootMotionConfig]);
        }
        foreach (var config in animatorRelationConfigDic)
        {
            if (rootMotionConfigDic.ContainsKey(config.Value))
            {
                continue;
            }
            ExcelDataObject _data = ConfigResFactory.LoadConifg<ExcelDataObject>(config.Value);
             LoadRow(config.Value, _data._Rows);
        }
    }
    public void LoadRow(string assetName, List<ExcelRowData> rows)
    {
        List<RootMotionConfig> rootMotionConfigs = new List<RootMotionConfig>();
        foreach (var row in rows)
        {

            RootMotionConfig rootMotionConfig = new RootMotionConfig();
            rootMotionConfig.clipName = row[(int)_Data_PlayerRootMotion.Name];
            rootMotionConfig.clipTag = row[(int)_Data_PlayerRootMotion.Tag];
            rootMotionConfig.judgeWay = (AnimClipJudgeWay)Enum.Parse(typeof(AnimClipJudgeWay), row[(int)_Data_PlayerRootMotion.AnimClipJudgeWay]);
            string multStr = row[(int)_Data_PlayerRootMotion.Mult];
            if (string.IsNullOrEmpty(multStr) == false)
                rootMotionConfig.motionMult = float.Parse(row[(int)_Data_PlayerRootMotion.Mult]);

            string posCalcuateModal = row[(int)_Data_PlayerRootMotion.PosCalcuateModel];
            if (string.IsNullOrEmpty(posCalcuateModal) == false)
                rootMotionConfig.posCalcuateModal = (CalcuateRootMotionModal)Enum.Parse(typeof(CalcuateRootMotionModal), posCalcuateModal);

            string rotationCalcuateModal = row[(int)_Data_PlayerRootMotion.RotationCalculateModel];
            if (string.IsNullOrEmpty(rotationCalcuateModal) == false)
                rootMotionConfig.rotationCalcuateModal = (CalcuateRootMotionModal)Enum.Parse(typeof(CalcuateRootMotionModal), rotationCalcuateModal);

            string posClearPriorityInProcess = row[(int)_Data_PlayerRootMotion.PosProcessClear];
            if (string.IsNullOrEmpty(posClearPriorityInProcess) == false)
                rootMotionConfig.posClearPriorityInProcess = (RootMotionProcessClear)Enum.Parse(typeof(RootMotionProcessClear), posClearPriorityInProcess);

            string rotationClearPriorityInProcess = row[(int)_Data_PlayerRootMotion.RotationProcessClear];
            if (string.IsNullOrEmpty(rotationClearPriorityInProcess) == false)
                rootMotionConfig.rotationClearPriorityInProcess = (RootMotionProcessClear)Enum.Parse(typeof(RootMotionProcessClear), rotationClearPriorityInProcess);

            string rootAnyPriority = row[(int)_Data_PlayerRootMotion.HandlerAim];
            if (string.IsNullOrEmpty(rootAnyPriority) == false)
                rootMotionConfig.rootAnyPriority = (RootMotionHandlerAim)Enum.Parse(typeof(RootMotionHandlerAim), rootAnyPriority);

            string priority = row[(int)_Data_PlayerRootMotion.Priority];
            if (string.IsNullOrEmpty(priority) == false)
            {
                rootMotionConfig.priority = int.Parse(priority);
            }
            rootMotionConfigs.Add(rootMotionConfig);
        }
        rootMotionConfigs.Sort((item1, item2) =>
        {
            if (item1.priority < item2.priority)
            {
                return 1;//降序排序
            }
            else if (item1.priority == item2.priority)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        });
        rootMotionConfigDic.Add(assetName, rootMotionConfigs);
    }
    public static void GetRootMotionConfigsByRolePrefab(string prefab_Name)
    {

    }
    public static List<RootMotionConfig> GetRootMotionConfigsByAssetName(string AssetName)
    {
        List<RootMotionConfig> configs = null;
        rootMotionConfigDic.TryGetValue(AssetName, out configs);
        return configs;
    }
    public static List<RootMotionConfig> GetRootMotionConfigsByAnimator(Animator animator)
    {
        string AssetName=null;//
        if (animator&&animator.runtimeAnimatorController!=null)
        {
            animatorRelationConfigDic.TryGetValue(animator.runtimeAnimatorController.name,out AssetName);
        }
        if (string.IsNullOrEmpty(AssetName))
        {
            DebugTool.DebugWarning($"{animator.runtimeAnimatorController.name}:获取不到对应的RootMotion数据表名字");
            return null;
        }
        List<RootMotionConfig> configs = null;
        rootMotionConfigDic.TryGetValue(AssetName, out configs);
        return configs;
    }
}
