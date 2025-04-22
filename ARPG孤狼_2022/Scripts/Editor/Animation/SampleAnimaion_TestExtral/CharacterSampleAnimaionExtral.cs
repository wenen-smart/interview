using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(CharacterAnim),true)]
public class CharacterSampleAnimaionExtral : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CharacterAnim characterAnim=null;
        if (target!=null)
        {
           characterAnim = target as CharacterAnim;
        }
        if (characterAnim)
        {
            AnimationClip animationClip=null;
            object clipObj = serializedObject.FindProperty("animationClip").objectReferenceValue;
            if (clipObj!=null)
            {
                animationClip = clipObj as AnimationClip;
            }
            if (animationClip!=null)
            {
                EditorGUILayout.BeginVertical("Box");
                GUILayout.Label("���Թ���-��������");
                float length=serializedObject.FindProperty("sampleClipLength").floatValue;
                GUILayout.Label("����ʱ�们��");
                EditorGUI.BeginChangeCheck();
                characterAnim.sampleClipLength = GUILayout.HorizontalSlider(length,0,animationClip.length);
                if (EditorGUI.EndChangeCheck())
                {
                    characterAnim.SampleAnimationInCharacter(characterAnim.sampleClipLength);
                }
                EditorGUILayout.Space(20);
                GUILayout.Label("�����ٶȻ���");
                characterAnim.sampleSpeed = GUILayout.HorizontalSlider(serializedObject.FindProperty("sampleSpeed").floatValue,0.01f,2);
                EditorGUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("�Զ���������"))
                {
                    characterAnim.CtrlSampleAnimationInCharacter(true);
                }
                if (GUILayout.Button("��ͣ��������"))
                {
                    characterAnim.CtrlSampleAnimationInCharacter(false);
                }
                if (GUILayout.Button("���ò���"))
                {
                    characterAnim.CtrlSampleAnimationInCharacter(false);
                    characterAnim.ResetSample();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                serializedObject.ApplyModifiedProperties();
            }
            
        }
    }
}
