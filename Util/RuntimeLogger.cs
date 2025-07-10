using UnityEngine;
using System.IO;
using System;

// Debug.Log���g�����A�����^�C�����s���Ƀ��O���t�@�C���ɏo��
public class RuntimeLogger : MonoBehaviour
{
    private static RuntimeLogger instance;
    private string logFilePath;

    private void Awake()
    {
        // �V���O���g���p�^�[���ŁA�V�[�����܂����ŃC���X�^���X��1�ɕۂ�
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ���O�t�@�C���̃p�X������
        logFilePath = Path.Combine(Application.persistentDataPath, "runtime_log.txt");

        // �A�v���P�[�V�����N�����Ƀ��O�t�@�C�����N���A
        ClearLogFile();

        // logMessageReceived�C�x���g�Ƀn���h����o�^
        Application.logMessageReceived += HandleLog;

        Debug.Log("�����^�C�����O�N��");
    }

    private void OnDestroy()
    {
        // �I�u�W�F�N�g���j�������ۂɃC�x���g�n���h���̓o�^������
        Application.logMessageReceived -= HandleLog;
    }

    //================================================
    // ���O���b�Z�[�W���󂯎�����ۂ̏���
    //================================================
    void HandleLog(string logMsg, string stackTrace, LogType type)
    {
        // �G�f�B�^���s���̓t�@�C���ɏ������܂Ȃ�
        if (Application.isEditor)
        {
            return;
        }

        try
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {logMsg}\n";
            if (type == LogType.Error || type == LogType.Exception)
            {
                logEntry += $"{stackTrace}\n";
            }
            File.AppendAllText(logFilePath, logEntry);
        }
        catch (Exception e)
        {
            // �t�@�C���������݂Ɏ��s�����ꍇ�̃G���[�����i���̃��O�̓t�@�C���ɂ͏�����Ȃ��j
            Debug.LogError($"�����^�C�����O�������ݎ��s: {e.Message}");
        }
    }

    //================================================
    // ���O�t�@�C�����N���A
    //================================================
    private void ClearLogFile()
    {
        // �����^�C���i�r���h��j�ł̂݃t�@�C�����N���A����
        if (Application.isEditor)
        {
            return;
        }

        try
        {
            File.WriteAllText(logFilePath, string.Empty);
        }
        catch (Exception e)
        {
            // �t�@�C������Ɏ��s�����ꍇ
            Debug.LogError($"���O�t�@�C���̃N���A���s\n�p�X: {logFilePath}\n����: {e.Message}");
        }
    }
}