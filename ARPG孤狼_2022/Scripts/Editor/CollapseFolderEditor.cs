using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class CollapseFolderEditor
{
    public static void CollapseFolder(string path)
    {
        var assembly = Assembly.GetAssembly(typeof(Editor));
        var type = assembly.GetType("UnityEditor.ProjectBrowser");

        if (type == null)
            return;

        var browserField = type.GetField("s_LastInteractedProjectBrowser", BindingFlags.Public | BindingFlags.Static);
        var browser = browserField.GetValue(null);

        if (browser == null)
            return;

        // 确定窗口模式是否是单列
        var modeField = type.GetField("m_ViewMode", BindingFlags.NonPublic | BindingFlags.Instance);
        bool isOne = (int)modeField.GetValue(browser) == 0;

        // 获取文件夹树
        var treeField = type.GetField(isOne ? "m_AssetTree" : "m_FolderTree", BindingFlags.NonPublic | BindingFlags.Instance);
        var tree = treeField.GetValue(browser);

        var dataProperty = treeField.FieldType.GetProperty("data", BindingFlags.Instance | BindingFlags.Public);
        var data = dataProperty.GetValue(tree, null);

        var getRowsMethod = dataProperty.PropertyType.GetMethod("GetRows", BindingFlags.Instance | BindingFlags.Public);
        var setExpandedMethods = dataProperty.PropertyType.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList().FindAll(method => method.Name == "SetExpanded");
        var setExpandedMethod = setExpandedMethods[0];

        var rows = (IEnumerable)getRowsMethod.Invoke(data, null);
        bool first = true;
        UnityEngine.Object folderObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        // 遍历当前的行将其折叠
        foreach (var obj in rows)
        {
            //if (first && !isOne)
            //{
            //    var itemType = obj.GetType();
            //    var nameField = itemType.GetField("m_DisplayName", BindingFlags.Instance | BindingFlags.NonPublic);
            //    if (nameField != null)
            //    {
            //        string name = (string)nameField.GetValue(obj);
            //        if (name == "Assets")
            //        {
            //            first = false;
            //            setExpandedMethod.Invoke(data, new object[] { obj, true });
            //            continue;
            //        }
            //    }
            //}
            var itemType = obj.GetType().BaseType.BaseType;
            var nameField = itemType.GetProperty("displayName", BindingFlags.Instance | BindingFlags.Public);
            if (nameField != null)
            {
                string name = (string)nameField.GetValue(obj);
                if (folderObject.name == name)
                {
                    setExpandedMethod.Invoke(data, new object[] { obj, false });

                }
            }
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/折叠所有文件夹 &c", false, 30)]
    public static void SetProjectBrowserFoldersCollapsed()
    {
        var assembly = Assembly.GetAssembly(typeof(Editor));
        var type = assembly.GetType("UnityEditor.ProjectBrowser");

        if (type == null)
            return;

        var browserField = type.GetField("s_LastInteractedProjectBrowser", BindingFlags.Public | BindingFlags.Static);
        var browser = browserField.GetValue(null);

        if (browser == null)
            return;

        // 确定窗口模式是否是单列
        var modeField = type.GetField("m_ViewMode", BindingFlags.NonPublic | BindingFlags.Instance);
        bool isOne = (int)modeField.GetValue(browser) == 0;

        // 获取文件夹树
        var treeField = type.GetField(isOne ? "m_AssetTree" : "m_FolderTree", BindingFlags.NonPublic | BindingFlags.Instance);
        var tree = treeField.GetValue(browser);

        var dataProperty = treeField.FieldType.GetProperty("data", BindingFlags.Instance | BindingFlags.Public);
        var data = dataProperty.GetValue(tree, null);

        var getRowsMethod = dataProperty.PropertyType.GetMethod("GetRows", BindingFlags.Instance | BindingFlags.Public);
        var setExpandedMethods = dataProperty.PropertyType.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList().FindAll(method => method.Name == "SetExpanded");
        var setExpandedMethod = setExpandedMethods[0];

        var rows = (IEnumerable)getRowsMethod.Invoke(data, null);
        bool first = true;
        // 遍历当前的行将其折叠
        foreach (var obj in rows)
        {
            if (first && !isOne)
            {
                var itemType = obj.GetType();
                var nameField = itemType.GetField("m_DisplayName", BindingFlags.Instance | BindingFlags.NonPublic);
                if (nameField != null)
                {
                    string name = (string)nameField.GetValue(obj);
                    if (name == "Assets")
                    {
                        first = false;
                        setExpandedMethod.Invoke(data, new object[] { obj, true });
                        continue;
                    }
                }
            }
            setExpandedMethod.Invoke(data, new object[] { obj, false });
        }
        AssetDatabase.Refresh();
    }

}

