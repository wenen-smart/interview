using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MoveComponent),true)]
public class MoveComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (target==null)
        {
            return;
        }
        MoveComponent moveComponent=target as MoveComponent;
        if (moveComponent.useCustomGravity)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gravity系数",GUIStyle.none);
            
            moveComponent._gravity = EditorGUILayout.FloatField(moveComponent._gravity);
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("_gravity"),true);
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
