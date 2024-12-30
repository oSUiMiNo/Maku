using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maku.AxisValues
{
    // string と int を並べて表示する構造体
    [Serializable]
    public struct AxisStringInt
    {
        public string strValue;
        public int intValue;

        public AxisStringInt(string strValue, int intValue)
        {
            this.strValue = strValue;
            this.intValue = intValue;
        }
    }

#if UNITY_EDITOR
    //エディタ拡張(インスペクタ表示)
    [CustomPropertyDrawer(typeof(AxisStringInt))]
    public class Drawer_StringIntPair : PropertyDrawer
    {
        static readonly GUIContent LABEL_STRING = new GUIContent("String");
        static readonly GUIContent LABEL_INT = new GUIContent("Int");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty property_String = property.FindPropertyRelative("strValue");
            SerializedProperty property_Int = property.FindPropertyRelative("intValue");

            //名前
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            //ラベル
            contentPosition.width *= 1f / 2f; //2つ並べる場合
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 40f; //ラベル幅を調整

            //各要素
            EditorGUI.PropertyField(contentPosition, property_String, LABEL_STRING);
            contentPosition.x += contentPosition.width;

            EditorGUI.PropertyField(contentPosition, property_Int, LABEL_INT);

            EditorGUI.EndProperty();
        }
    }
#endif
}