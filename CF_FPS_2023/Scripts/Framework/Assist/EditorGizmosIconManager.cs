using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 对象GizmosIcon管理器
/// </summary>

public class EditorGizmosIconManager
{
    #region 数据定义
    private static GUIContent[] labelIcons;
    private static GUIContent[] largeIcons;

    public enum LabelIcon
    {
        Gray = 0,
        Blue,
        Teal,
        Green,
        Yellow,
        Orange,
        Red,
        Purple
    }

    public enum Icon
    {
        CircleGray = 0,
        CircleBlue,
        CircleTeal,
        CircleGreen,
        CircleYellow,
        CircleOrange,
        CircleRed,
        CirclePurple,
        DiamondGray,
        DiamondBlue,
        DiamondTeal,
        DiamondGreen,
        DiamondYellow,
        DiamondOrange,
        DiamondRed,
        DiamondPurple
    }
    #endregion

    public static void SetIcon(GameObject node, LabelIcon icon)
    {
        if (labelIcons == null)
        {
            labelIcons = GetTextures("sv_label_", string.Empty, 0, 8);
        }
        SetIcon(node, labelIcons[(int)icon].image as Texture2D);
    }
    public static void SetIcon(GameObject node, Icon icon)
    {
        if (largeIcons == null)
        {
            largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
        }
        SetIcon(node, largeIcons[(int)icon].image as Texture2D);
    }
    private static void SetIcon(GameObject node, Texture2D texture)
    {
        #if UNITY_EDITOR
        EditorGUIUtility.SetIconForObject(node, texture);
        #endif
    }
    private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
    {

        GUIContent[] guiContentArray = new GUIContent[count];
        #if UNITY_EDITOR
        for (int index = 0; index < count; ++index)
        {
            guiContentArray[index] = EditorGUIUtility.IconContent(baseName + (startIndex + index) + postFix);
        }
        #endif
        return guiContentArray;
    }
}