using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class ScriptGenerateTool : EditorWindow
{
    private string _path;
    public abstract string defaultPath { get;}
    public virtual string path { get => string.IsNullOrEmpty(_path)?defaultPath:_path; set { _path = value; } }
    public string scriptName = "NewClass";
    public bool haveNameSpace = false;
    public virtual void OnGUI()
    {
        GUILayout.BeginVertical();
        //脚本名
        GUILayout.Label("类名");
        scriptName = GUILayout.TextField(scriptName);
        //目录路径
        OnClickButton();
        GUILayout.EndVertical();
    }
    public virtual void OnClickButton()
    {
        if (GUILayout.Button("Generate"))
        {
            OnGenerateScript();
        }
    }
    public virtual void OnGenerateScript()
    {
        bool isGenerate = true;
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.dataPath, path));
        if (directoryInfo.Exists == false)
        {
            directoryInfo.Create();
        }
        string fileName = scriptName;
        string filePath = Path.Combine(directoryInfo.FullName, scriptName + ".cs");
        if (File.Exists(filePath))
        {
            if (!EditorUtility.DisplayDialog("GenerateScript", "已经存在同名脚本，是否覆盖?", "Sure", "Cancel"))
            {
                isGenerate = false;
                return;
            }
        }

        if (isGenerate)
        {
            StringBuilder fileContent = new StringBuilder();
            fileContent.AppendLine(GenericUSING());
            fileContent.AppendLine(GenericClassAttitude());
            fileContent.Append(GenerateNameSpace());
            haveNameSpace = false;
            if (string.IsNullOrEmpty(GenerateNameSpace()) == false)
            {
                fileContent.Append("\n{\n\t");
                haveNameSpace = true;
            }
            fileContent.Append(GenerateClassModule());
            fileContent.Append(GenerateInterface());
            fileContent.AppendLine("{");
            fileContent.AppendLine(GenerateMember());
            fileContent.AppendLine("}");
            if (haveNameSpace)
            {
                fileContent.AppendLine("}");
            }
            FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
            byte[] data = Encoding.UTF8.GetBytes(fileContent.ToString());
            fileStream.Write(data,0,data.Length);
            fileStream.Close();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("GenerateScript", "脚本已经生成完成，正在编译", "Sure");
        }
    }

    public abstract string GenericUSING();
    public virtual string GenerateNameSpace()
    {
        return "";
    }
    public virtual string GenerateClassModule()
    {
        return $"public class {scriptName}";
    }
    public abstract string GenericClassAttitude();
    public abstract string GenerateInterface();
    public abstract string GenerateMember();
}

