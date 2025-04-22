using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FSMConfigGenerateTool : ScriptGenerateTool
{
    public string stateEnumType;
    public string transitionEnumType;
    public int menuOrder;
    public override string path { get => base.path; set => base.path = value; }

    public override string defaultPath => "Scripts/ScriptableConfig/FSMState/";

    public override void OnGUI()
    {
        GUILayout.Toolbar(0,new string[] { "配置生成"});
        GUILayout.BeginVertical();
        //脚本名
        GUILayout.BeginHorizontal();
        GUILayout.Label("类名",GUILayout.Width(60));
        scriptName = GUILayout.TextField(scriptName);
         GUILayout.Space(10);
        GUILayout.Label("生成路径", GUILayout.Width(60));
        path = GUILayout.TextField(path);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("状态枚举",GUILayout.Width(60));
        stateEnumType = GUILayout.TextField(stateEnumType);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("转换枚举",GUILayout.Width(60));
        transitionEnumType = GUILayout.TextField(transitionEnumType);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        //目录路径
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("菜单项优先级",GUILayout.Width(80));
        menuOrder = EditorGUILayout.IntSlider(menuOrder,0,100,GUILayout.Width(150));
        GUILayout.Space(50);
        OnClickButton();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    public override void OnGenerateScript()
    {
        bool typeConvert = true;
        string errorMessage="";
        if (string.IsNullOrEmpty(stateEnumType)||string.IsNullOrEmpty(transitionEnumType))
        {
            typeConvert = false;
        }
        else
        {
            Type stateType=AssemblyTool.GetCorrentType(stateEnumType);
            Type transtionType = AssemblyTool.GetCorrentType(transitionEnumType);
            if (stateType==null||stateType.IsEnum==false)
            {
                errorMessage += "状态枚举类名有误，请检查\n";
                typeConvert = false;
            }
            if (transtionType==null||transtionType.IsEnum==false)
            {
                errorMessage += "Transition枚举类名有误，请检查";
                typeConvert = false;
            }
        }
        if (typeConvert==false)
        {
            EditorUtility.DisplayDialog("GenerateScript", errorMessage, "Sure");
            return;
        }
        base.OnGenerateScript();
    }
    public override string GenerateInterface()
    {
        return $":FsmStateConfig<{scriptName},{stateEnumType},{transitionEnumType}>";
    }

    public override string GenerateMember()
    {
        string suojin = haveNameSpace ?"\t":"";
        return "\n"+suojin+"new public static void SetScriptablePath()\n"+suojin+"{\n"+suojin+suojin+"scriptablePath = PathDefine.FsmStateConfigPath+\""+scriptName+"\";\n}";
    }

    public override string GenericClassAttitude()
    {
        return $"[CreateAssetMenu(fileName = \"{scriptName}\", menuName = \"FSMState/Create{scriptName}\", order = {menuOrder})]";
    }

    public override string GenericUSING()
    {
        return "using System;\n" +
            "using System.Collections.Generic;\n" +
            "using System.Linq;\n" +
            "using System.Text;\n" +
            "using System.Threading.Tasks;\n" +
            "using UnityEngine;\n";
    }
    [MenuItem("Tools/FSMState/生成有限状态机配置")]
    static void OpenGenerateWnd()
    {
        FSMConfigGenerateTool _Launcher = GetWindow<FSMConfigGenerateTool>();
        _Launcher.Show();
    }
}


