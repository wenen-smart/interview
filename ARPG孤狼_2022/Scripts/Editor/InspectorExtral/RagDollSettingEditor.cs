using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RagDollSetting))]
public class RagDollSettingEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RagDollSetting ragDollSetting = target as RagDollSetting;
        if (ragDollSetting==null)
        {
            return;
        }
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("EnableRagDoll"))
        {
            ragDollSetting.EnableRagDoll();
        }
        EditorGUILayout.Space(15);
        if (GUILayout.Button("DisableRagDoll"))
        {
            ragDollSetting.DisableRagDoll();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}

