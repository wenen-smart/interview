using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "SettingSO", menuName = "CreateSettingFile")]
public class SettingSO:ScriptableConfigTon<SettingSO>
{
    [Header("鼠标灵敏度"),Range(30,100)]
    public float mouseSensitivity=60;
    [Header("狙击镜灵敏度"),Range(30,100)]
    public float mouseSensitivityInSnipeScope=40;

    public new static void SetScriptablePath()
    {
        scriptablePath = PathDefine.settingConfigPath;
    }
}

