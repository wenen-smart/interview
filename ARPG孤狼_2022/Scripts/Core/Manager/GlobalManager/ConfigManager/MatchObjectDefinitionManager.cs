using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MatchObjectDefinitionManager : IManager
{
    public void Init()
    {
        
    }
    public const string k_RoleObjectDefinition_ConfigPath="_Data_RoleObjectDefinition";

    public void LoadScript(object args)
    {
        InitRoleDefinition();
    }
    public void InitRoleDefinition()
    {
        ExcelDataObject roleDefDatas = ConfigResFactory.LoadConifg<ExcelDataObject>(k_RoleObjectDefinition_ConfigPath);
        if (roleDefDatas==null)
        {
            MyDebug.DebugError("所有角色信息配置加载失败");
        }
        foreach (var rowData in roleDefDatas._Rows)
        {
            RoleObjectDefinition definition = new RoleObjectDefinition();
            definition.roleID.StringToInt(rowData[(int)(_Data_RoleObjectDefinition.ID)]);
            definition.object_Name=(rowData[(int)(_Data_RoleObjectDefinition.Name)]);
            definition.object_Description=(rowData[(int)(_Data_RoleObjectDefinition.Description)]);
            definition.prefab_Path=(rowData[(int)(_Data_RoleObjectDefinition.Prefab_Path)]);
            definition._roleDropItemKindListInfo = new GameDropItemKindListInfo();
            definition.haveInvertory.StringToBoolean(rowData[(int)(_Data_RoleObjectDefinition.HaveInvertory)]);
            string dropItemListIDStr = (rowData[(int)(_Data_RoleObjectDefinition.DropItemListID)]);
            if (string.IsNullOrEmpty(dropItemListIDStr)==false)
            {
                int dropItemListID = int.Parse(dropItemListIDStr);
                string dropListRange = (rowData[(int)(_Data_RoleObjectDefinition.DropKindRandomRange)]);
                int[] dropCountIntRange = new int[2];
                dropListRange = dropListRange.Trim('[', ']');
                string[] dropListRangeArray = dropListRange.Split(',', '，');
                if (dropListRangeArray.Length == 1)
                {
                    dropCountIntRange[0].StringToInt(dropListRangeArray[0]);
                    dropCountIntRange[1] = dropCountIntRange[0];
                }
                else
                {
                    dropCountIntRange[0].StringToInt(dropListRangeArray[0]);
                    dropCountIntRange[1].StringToInt(dropListRangeArray[1]);
                }
                DropItemKindInfo[] dropItemKindInfos = DropItemListContainer.Instance.GetItemKindInfo(dropItemListID);
                if (dropItemKindInfos == null)
                {
                    MyDebug.DebugSupperError($"严重错误，读取不到dropitemListID对应的数据。\n 字典：{DropItemListContainer.Instance.dropItemKindInfoDics}");
                }
                else
                {
                    definition._roleDropItemKindListInfo = new GameDropItemKindListInfo();
                    definition._roleDropItemKindListInfo.kindsInfo = dropItemKindInfos;
                    definition._roleDropItemKindListInfo.minKindCount = dropCountIntRange[0];
                    definition._roleDropItemKindListInfo.maxKindCount = dropCountIntRange[1];
                }
            }
            if (RoleObjectDefinition.Definitions.ContainsKey(definition.roleID))
            {
                MyDebug.DebugWarning("已经包含RoleID:"+definition.roleID+"的配置，本次遍历过滤。请检查角色信息配置表ID是否重复");
                continue;
            }
            RoleObjectDefinition.Definitions.Add(definition.roleID,definition);
        }
    }
}

