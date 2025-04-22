using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class RagDollSettingWindowEditor: EditorWindow
{
     [MenuItem("GameObject/3D Object/RagDollOverride",priority=17)]
    public static void CreateRagDoll()
    {
        OpenWnd();
    }
    public static void OpenWnd()
    {
        
        if (Selection.activeGameObject !=null)
        {
            RagDollSettingWindowEditor ragDollSettingEditor = RagDollSettingWindowEditor.GetWindow<RagDollSettingWindowEditor>(false);
            ragDollSettingEditor.titleContent = new GUIContent("RagDollWindow");
            ragDollSettingEditor.selectObj = Selection.activeGameObject;
            ragDollSettingEditor.Show();
            ragDollSettingEditor.changeCheckWhenClickCreate = false;
        }
        
    }
    public GameObject selectObj;
    public bool changeCheckWhenClickCreate;
    public void OnGUI()
    {
        if (selectObj != null)
        {
            RagDollSetting ragDollSetting = selectObj.GetComponent<RagDollSetting>();
            if (ragDollSetting == null)
            {
                ragDollSetting = selectObj.AddComponent<RagDollSetting>();
            }
            else
            {
                Type ragdollBuilderType = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
                var windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
                if (windows == null || windows.Length == 0)
                {
                    EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
                    windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
                }
                if (windows != null && windows.Length > 0)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal("box");
                    //EditorGUI.BeginDisabledGroup(nextPath == null);  //如果nextPath == null 为真，在Inspector面板上显示，承灰色（即不可操作）
                    EditorGUI.BeginDisabledGroup(true);
                    
                    var content = new GUIContent("Select") { };
                    EditorGUILayout.ObjectField(content,ragDollSetting,ragDollSetting.GetType(),false,GUILayout.ExpandWidth(true),GUILayout.Height(20));
                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                    var ragdollWindow = windows[0] as ScriptableWizard;
                    Rect ragdollWizardrect= ragdollWindow.position;
                    ragdollWizardrect.width = 400;
                    ragdollWizardrect.height = Screen.height-200;
                    ragdollWizardrect.position = new Vector2(Screen.width - ragdollWizardrect.width/2, 0);
                    ragdollWindow.position = ragdollWizardrect;
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginScrollView(new Vector2(this.position.width/2,-this.position.height/2),false,true,GUILayout.ExpandWidth(true),GUILayout.Height(this.position.height/2));
                    
                    
                   ragDollSetting.Pelvis = EditorGUILayout.ObjectField("Pelvis",ragDollSetting.Pelvis,typeof(Transform),allowSceneObjects:true) as Transform;
                    ragDollSetting.LeftHips = EditorGUILayout.ObjectField("LeftHips",ragDollSetting.LeftHips,typeof(Transform),allowSceneObjects:true) as Transform;
                    ragDollSetting.LeftKnee = EditorGUILayout.ObjectField("LeftKnee",ragDollSetting.LeftKnee,typeof(Transform),allowSceneObjects:true) as Transform;
                    ragDollSetting.LeftFoot = EditorGUILayout.ObjectField("LeftFoot", ragDollSetting.LeftFoot, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.RightHips = EditorGUILayout.ObjectField("RightHips", ragDollSetting.RightHips, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.RightKnee = EditorGUILayout.ObjectField("RightKnee", ragDollSetting.RightKnee, typeof(Transform), allowSceneObjects: true) as Transform;
                     ragDollSetting.RightFoot = EditorGUILayout.ObjectField("RightFoot", ragDollSetting.RightFoot, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.LeftArm = EditorGUILayout.ObjectField("LeftArm", ragDollSetting.LeftArm, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.LeftElbow = EditorGUILayout.ObjectField("LeftElbow", ragDollSetting.LeftElbow, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.RightArm = EditorGUILayout.ObjectField("RightArm", ragDollSetting.RightArm, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.RightElbow = EditorGUILayout.ObjectField("RightElbow", ragDollSetting.RightElbow, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.MiddleSpine = EditorGUILayout.ObjectField("MiddleSpine", ragDollSetting.MiddleSpine, typeof(Transform), allowSceneObjects: true) as Transform;
                    ragDollSetting.Head = EditorGUILayout.ObjectField("Head", ragDollSetting.Head, typeof(Transform), allowSceneObjects: true) as Transform;

                    GUILayout.EndScrollView();
                    
                    bool isChanged=EditorGUI.EndChangeCheck();
                    
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button("Create"))
                    {
                        changeCheckWhenClickCreate = isChanged||GUI.changed;
                        if (changeCheckWhenClickCreate)
                        {
                            SetFieldValue(ragdollWindow, "pelvis", ragDollSetting.Pelvis);
                            SetFieldValue(ragdollWindow, "leftHips", ragDollSetting.LeftHips);
                            SetFieldValue(ragdollWindow, "leftKnee", ragDollSetting.LeftKnee);
                            SetFieldValue(ragdollWindow, "leftFoot", ragDollSetting.LeftFoot);
                            SetFieldValue(ragdollWindow, "rightHips", ragDollSetting.RightHips);
                            SetFieldValue(ragdollWindow, "rightKnee", ragDollSetting.RightKnee);
                            SetFieldValue(ragdollWindow, "rightFoot", ragDollSetting.RightFoot);
                            SetFieldValue(ragdollWindow, "leftArm", ragDollSetting.LeftArm);
                            SetFieldValue(ragdollWindow, "leftElbow", ragDollSetting.LeftElbow);
                            SetFieldValue(ragdollWindow, "rightArm", ragDollSetting.RightArm);
                            SetFieldValue(ragdollWindow, "rightElbow", ragDollSetting.RightElbow);
                            SetFieldValue(ragdollWindow, "middleSpine", ragDollSetting.MiddleSpine);
                            SetFieldValue(ragdollWindow, "head", ragDollSetting.Head);
                            var method = ragdollWindow.GetType().GetMethod("CheckConsistency", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            if (method != null)
                            {
                                ragdollWindow.errorString = (string)method.Invoke(ragdollWindow, null);
                                ragdollWindow.isValid = string.IsNullOrEmpty(ragdollWindow.errorString);
                            }
                            
                            if (ragdollWindow.isValid)
                            {
                                 var createMethod = ragdollWindow.GetType().GetMethod("OnWizardCreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                createMethod.Invoke(ragdollWindow,null);
                                ragDollSetting.OnGeneric();
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            
                        }
                    }
                    
                    if (GUILayout.Button("Delete RagDoll"))
                    {
                        ragDollSetting.DeleteRagDoll();
                        Close();
                    }
                    GUILayout.EndHorizontal();
                    if (changeCheckWhenClickCreate==false&&ragDollSetting.IsGeneatedRagDoll)
                    {
                        EditorGUILayout.HelpBox("该物体已经生成布娃娃了，如需重新配置，请先点击删除",MessageType.Warning);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(ragdollWindow.isValid == false ? ragdollWindow.errorString : "创建成功", ragdollWindow.isValid ? MessageType.None : MessageType.Error);
                    }
                    
                    EditorGUILayout.HelpBox("指定对应骨骼 生成布娃娃",MessageType.Info );
                    if (GUILayout.Button("自动映射骨骼"))
                    {
                        var animator = ragDollSetting.GetComponentInChildren<Animator>();

                        ragDollSetting.Pelvis = animator.GetBoneTransform(HumanBodyBones.Hips);
                        ragDollSetting.LeftHips = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                        ragDollSetting.LeftKnee = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                        ragDollSetting.LeftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                        ragDollSetting.RightHips = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                        ragDollSetting.RightKnee = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                        ragDollSetting.RightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                        ragDollSetting.LeftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                        ragDollSetting.LeftElbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                        ragDollSetting.RightArm =  animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                        ragDollSetting.RightElbow = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                        ragDollSetting.MiddleSpine = animator.GetBoneTransform(HumanBodyBones.Spine);
                        ragDollSetting.Head = animator.GetBoneTransform(HumanBodyBones.Head);
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                }
            }
        }
    }
    /// <summary>
    /// Use reflection to set the value of the field.
    /// </summary>
    private void SetFieldValue(ScriptableWizard obj, string name, object value)
    {
        if (value == null)
        {
            return;
        }

        var field = obj.GetType().GetField(name);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
}

