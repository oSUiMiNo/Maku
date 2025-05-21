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
        // エディタを一時停止する
        //========================================
        public static void Pause()
        {
            Debug.Log("------ 一時停止 ------");
            EditorApplication.isPaused = true;
        }


        //========================================
        // コンソールをクリア
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