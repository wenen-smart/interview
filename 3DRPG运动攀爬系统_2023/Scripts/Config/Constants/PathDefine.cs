using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathDefine
{
    //Used
    public const string UIItemPath = "Prefab/UI/";
    private const string scriptableConfigPath = "Config/Scriptable/";
    public const string scriptManagerConfigPath = "_Data_ScriptManagerConfig";
    public static string gameEnumScriptPath = "Assets/Resolution/Scripts/Config/Constants/GameConfigData.cs";
    public const string TemplateFileDirectory = "Assets/Resolution/ConfigFolder/Template/";
    public static string gameEnumTemplateName = "Template_GameConfigEnum.txt";
    //

    public const string playableConfigPath = scriptableConfigPath + "PlayableConfig";
    public const string playerAnimatorConfigPath = scriptableConfigPath + "PlayerAnimatorConfig";
    public const string SkillDataConfigConfigPath = scriptableConfigPath + "SkillDataConfig";
    public const string EndKillTimeLineConfigPath = scriptableConfigPath + "EndKillTimeLineConfig";
    public const string FsmStateConfigPath = scriptableConfigPath + "FSM/";
    public const string SkillDataConfigFolder = scriptableConfigPath + "SkillData/";


    public const string airComboAttackConfigPath = "_Data_AirComboAttack";

    public const string buffDataConfigPath = "_Data_Buff";
    public const string buffActionConfigPath = "_Data_BuffEvent";
    public const string motionModifyConfigPath = "_Data_MotionModifyConfig";
    public const string UIPanelJsonInfo = "Config/UIPanelPathJson";

    public const string itemConfigDataContainerPathNoSuffix = scriptableConfigPath + "Item/ItemConfigDataContainer";
    public const string itemConfigDataContainerPath = scriptableConfigPath + "Item/ItemConfigDataContainer.asset";
    public const string weaponDataConfigFolder = scriptableConfigPath + "Item/Weapon";//武器装备
    public const string alimentDataConfigFolder = scriptableConfigPath + "Item/Aliment";
    public const string OrnamentDataConfigFolder = scriptableConfigPath + "Item/Ornament";//装饰物装备

    public const string DropItemllistContainerPath = scriptableConfigPath+"Item/DropItemListContainer";
    public const string GraphDataConfigPath = scriptableConfigPath + "GraphDataConfig/GraphDataConfig";
    public const string DialogDataConfigPath = scriptableConfigPath + "DialogDataConfig";

    public const string MusicChannelSOPath = scriptableConfigPath + "Audio/Channel/MusicChannelSO";

    public const string SFXChannelSOPath = scriptableConfigPath + "Audio/Channel/SFXChannelSO";
    public const string Music_2D_SouceConfigSOPath = scriptableConfigPath + "Audio/Source/2DMusicSource";

    public const string Music_3D_SouceConfigSOPath = scriptableConfigPath + "Audio/Source/3DMusicSource";
    public const string SFX_2D_SouceConfigSOPath = scriptableConfigPath + "Audio/Source/2DSFXSource";

    public const string SFX_3D_SouceConfigSOPath = scriptableConfigPath + "Audio/Source/3DSFXSource";

    public const string Inventory_ItemPath = "Prefab/UI/UI_Inventory_Item";

    public const string AudioRes = "Audio/";
    public const string Sound_PickItem=AudioRes+"PickItem";
}

