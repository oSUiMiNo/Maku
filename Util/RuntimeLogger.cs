using UnityEngine;
using System.IO;
using System;


///*******************************************************<summary>
/// Debug.Log���g���������^�C���i�r���h��j�Ƀ��O���t�@�C���ɏo��
///</summary>******************************************************
public class RuntimeLogger : MonoBehaviour
{
    static RuntimeLogger Ins;
    // ���O�t�@�C���̃p�X
    string LogFile => $"{Application.persistentDataPath}/runtime_log.txt";


    void Awake()
    {
        // �����^�C���i�r���h��j�łȂ���Ζ���
        if (Application.isEditor) return;

        //--------------------------------------
        // �V�[�����܂�����悤�ɃV���O���g����
        //--------------------------------------
        if (Ins == null)
        {
            Ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // �N�����Ƀ��O�t�@�C�����N���A
        ClearLogFile();
        // �W���� logMessageReceived �C�x���g�Ƀ��O������o�^
        Application.logMessageReceived += HandleLog;
        Debug.Log("�����^�C�����O�N������");
    }


    void OnDestroy()
    {
        // �C�x���g�n���h���o�^����
        Application.logMessageReceived -= HandleLog;
    }


    ///==============================================<summary>
    /// ���O���b�Z�[�W���󂯎�����ۂ̏���
    ///</summary>=============================================
    void HandleLog(string logMsg, string stackTrace, LogType type)
    {
        // �����^�C���i�r���h��j�łȂ���Ζ���
        if (Application.isEditor) return;

        try
        {
            // 1���̃��O���쐬
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {logMsg}\n";
            if (type == LogType.Error || type == LogType.Exception)
            {
                logEntry += $"{stackTrace}\n";
            }
            // ���O���t�@�C���ɒǋL
            File.AppendAllText(LogFile, logEntry);
        }
        catch (Exception e)
        {
            Debug.LogError($"�����^�C�����O�������ݎ��s�F{e.Message}");
        }
    }


    ///==============================================<summary>
    /// ���O�t�@�C�����N���A
    ///</summary>=============================================
    void ClearLogFile()
    {
        // �����^�C���i�r���h��j�łȂ���Ζ���
        if (Application.isEditor) return;
     
        try
        {
            // �t�@�C������̕�����ŏ㏑��
            File.WriteAllText(LogFile, string.Empty);
        }
        catch (Exception e)
        {
            Debug.LogError($"���O�t�@�C���̃N���A���s\n�p�X: {LogFile}\n����: {e.Message}");
        }
    }
}