using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maku.AxisValues  //�����O�͔C��
{
    //��`�N���X�E�\����

    /// <summary>
    /// X, Y, Z �e���� true / false �\����
    /// </summary>
    [Serializable]
    public struct AxisBool2
    {
        public bool x;
        public bool y;
        public bool z;

        public AxisBool2(bool x, bool y, bool z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

#if UNITY_EDITOR
    //�G�f�B�^�g��(�C���X�y�N�^�\��)
    [CustomPropertyDrawer(typeof(AxisBool2))]
    public class Drawer_AxisBool3 : PropertyDrawer
    {
        static readonly GUIContent X_LABEL = new GUIContent("X");
        static readonly GUIContent Y_LABEL = new GUIContent("Y");
        static readonly GUIContent Z_LABEL = new GUIContent("Z");

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var xProperty = property.FindPropertyRelative("x");
            var yProperty = property.FindPropertyRelative("y");
            var zProperty = property.FindPropertyRelative("z");

            //���O
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            //���x��
            contentPosition.width *= 1f / 3f; //3���ׂ�ꍇ (n �̂Ƃ��A1 / n)
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 15f;  //���x����(�K��)

            //�e�v�f
            EditorGUI.PropertyField(contentPosition, xProperty, X_LABEL);
            contentPosition.x += contentPosition.width;

            EditorGUI.PropertyField(contentPosition, yProperty, Y_LABEL);
            contentPosition.x += contentPosition.width;

            EditorGUI.PropertyField(contentPosition, zProperty, Z_LABEL);

            EditorGUI.EndProperty();
        }
    }
#endif
}