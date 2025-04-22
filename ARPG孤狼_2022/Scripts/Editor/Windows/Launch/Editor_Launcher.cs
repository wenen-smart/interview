using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Editor_Launcher:EditorWindow
{
    private string launchScene;
    private string testScene;
    private string mapScene;
    static bool IsOpenDefaultLaunch
    {
        get
        {
            return EditorPrefs.GetBool(Application.productName+"_IsOpenDefaultLaunch", false);
        }
        set
        {
            EditorPrefs.SetBool(Application.productName+"_IsOpenDefaultLaunch", value);
        }
    }
    public static string DefaultLaunch
    {
        get
        {
            return EditorPrefs.GetString(Application.productName+"_DefaultLaunchKey", "GameLoader");
        }
        set
        {
            EditorPrefs.SetString(Application.productName+"_DefaultLaunchKey", value);
        }
    }
    public static string DefaultTestScene
    {
        get
        {
            return EditorPrefs.GetString(Application.productName+"_DefaultTestSceneKey", "Test");
        }
        set
        {
            EditorPrefs.SetString(Application.productName + "_DefaultTestSceneKey", value);
        }
    }
    public static string DefaultMapScene
    {
        get
        {
            return EditorPrefs.GetString(Application.productName + "_DefaultMapSceneKey", "Map1");
        }
        set
        {
            EditorPrefs.SetString(Application.productName + "_DefaultMapSceneKey", value);
        }
    }
    [MenuItem("Tools/Launch/开启强制默认启动场景")]
    static void OpenLaunchSceneSetting()
    {
        IsOpenDefaultLaunch = true;
    }
    [MenuItem("Tools/Launch/关闭强制默认启动场景")]
    static void CloseLaunchSceneSetting()
    {
        IsOpenDefaultLaunch = false;
    }
    [MenuItem("Tools/Launch/设置默认启动场景")]
    static void OpenLaunchSettingWnd()
    {
        Editor_Launcher _Launcher= GetWindow<Editor_Launcher>();
        _Launcher.launchScene = DefaultLaunch;
        _Launcher.testScene = DefaultTestScene;
        _Launcher.mapScene = DefaultMapScene;
        _Launcher.Show();
    }
    public void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("默认场景：",GUILayout.Width(100));
        launchScene = EditorGUILayout.TextField(launchScene,GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("设置默认场景"))
        {
            DefaultLaunch = launchScene;
            Debug.Log(DefaultLaunch);
            Close();
        }
        if (GUILayout.Button("Reset"))
        {
           EditorPrefs.DeleteKey(Application.productName+"_DefaultLaunchKey");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("测试场景：", GUILayout.Width(100));
        testScene = EditorGUILayout.TextField(testScene, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("设置测试场景"))
        {
            DefaultTestScene = testScene;
            Debug.Log(DefaultTestScene);
            Close();
        }
        
        if (GUILayout.Button("Reset"))
        {
           EditorPrefs.DeleteKey(Application.productName+"_DefaultTestSceneKey");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("地图场景：", GUILayout.Width(100));
        mapScene = EditorGUILayout.TextField(mapScene, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("设置地图场景"))
        {
            DefaultMapScene = mapScene;
            Debug.Log(DefaultMapScene);
            Close();
        }
        if (GUILayout.Button("Reset"))
        {
            EditorPrefs.DeleteKey(Application.productName+"_DefaultMapSceneKey");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}

