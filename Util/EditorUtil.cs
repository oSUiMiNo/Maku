using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace MyUtil
{
    public class EditorUtil
    {
        //========================================
        // �G�f�B�^���ꎞ��~����
        //========================================
        public static void Pause()
        {
            Debug.Log("------ �ꎞ��~ ------");
            EditorApplication.isPaused = true;
        }


        //========================================
        // �R���\�[�����N���A
        //========================================
        public static void ClearLog()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod(
                "Clear", 
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Public
            );
            clearMethod.Invoke(null, null);
        }
    }
}
#endif