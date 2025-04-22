using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CustomPropertyDrawer(typeof(MultSelectTagsAttribute))]
public class MultSelectTagsDrawer: PropertyDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      
        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
    }

    }

