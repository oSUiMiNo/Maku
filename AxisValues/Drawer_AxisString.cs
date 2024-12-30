using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maku.AxisValues
{ 
    //��`�N���X�E�\����

    /// <summary>
    /// X, Y, Z �e���� true / false �\����
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
    //�G�f�B�^�g��(�C���X�y�N�^�\��)
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

            //���O
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            //���x��
            contentPosition.width *= 1f / 2f; //2���ׂ�ꍇ (n �̂Ƃ��A1 / n)
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 15f;  //���x����(�K��)

            //�e�v�f
            EditorGUI.PropertyField(contentPosition, property_A, LABEL_A);
            contentPosition.x += contentPosition.width;

            EditorGUI.PropertyField(contentPosition, property_B, LABEL_B);

            EditorGUI.EndProperty();
        }
    }


    //�G�f�B�^�g��(�C���X�y�N�^�\��)
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

            //���O
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            //���x��
            contentPosition.width *= 1f / 3f; //3���ׂ�ꍇ (n �̂Ƃ��A1 / n)
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 15f;  //���x����(�K��)

            //�e�v�f
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