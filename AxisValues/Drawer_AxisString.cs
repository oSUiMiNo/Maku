using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maku.AxisValues
{ 
    //定義クラス・構造体

    /// <summary>
    /// X, Y, Z 各軸の true / false 構造体
    /// </summary>
    [Serializable]
    public struct AxisString2
    {
        public string a;
        public string b;

        public AxisString2(string a, string b)
        {
            this.a = a;
            this.b = b;
        }
    }

    [Serializable]
    public struct AxisString3
    {
        public string a;
        public string b;
        public string c;

        public AxisString3(string a, string b, string c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }




#if UNITY_EDITOR
    //エディタ拡張(インスペクタ表示)
    [CustomPropertyDrawer(typeof(AxisString2))]
    public class Drawer_AxisLString2 : PropertyDrawer
    {
        static readonly GUIContent LABEL_A = new GUIContent("A");
        static readonly GUIContent LABEL_B = new GUIContent("B");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty property_A = property.FindPropertyRelative("a");
            SerializedProperty property_B = property.FindPropertyRelative("b");

            //名前
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            //ラベル
            contentPosition.width *= 1f / 2f; //2つ並べる場合 (n 個のとき、1 / n)
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 15f;  //ラベル幅(適当)

            //各要素
            EditorGUI.PropertyField(contentPosition, property_A, LABEL_A);
            contentPosition.x += contentPosition.width;

            EditorGUI.PropertyField(contentPosition, property_B, LABEL_B);

            EditorGUI.EndProperty();
        }
    }


    //エディタ拡張(インスペクタ表示)
    [CustomPropertyDrawer(typeof(AxisString3))]
    public class Drawer_AxisLString3 : PropertyDrawer
    {
        static readonly GUIContent LABEL_A = new GUIContent("A");
        static readonly GUIContent LABEL_B = new GUIContent("B");
        static readonly GUIContent LABEL_C = new GUIContent("C");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty property_A = property.FindPropertyRelative("a");
            SerializedProperty property_B = property.FindPropertyRelative("b");
            SerializedProperty property_C = property.FindPropertyRelative("c");

            //名前
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            //ラベル
            contentPosition.width *= 1f / 3f; //3つ並べる場合 (n 個のとき、1 / n)
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 15f;  //ラベル幅(適当)

            //各要素
            EditorGUI.PropertyField(contentPosition, property_A, LABEL_A);
            contentPosition.x += contentPosition.width;

            EditorGUI.PropertyField(contentPosition, property_A, LABEL_B);
            contentPosition.x += contentPosition.width;

            EditorGUI.PropertyField(contentPosition, property_B, LABEL_C);

            EditorGUI.EndProperty();
        }
    }
#endif
}