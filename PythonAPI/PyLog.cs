using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using Cysharp.Threading.Tasks;

public class PyLog : SingletonCompo<PyLog>
{
    string LogPath; // �Ď�����t�@�C���̃p�X
    DateTime lastWriteTime;

    protected sealed override void Awake0() => CreateLogFileAsync().Forget();
    protected sealed override void Update() => ProcessLogFileAsync().Forget();



    async UniTask CreateLogFileAsync()
    {
        await UniTask.SwitchToThreadPool();

        // Assets �����Ƀ��O�ptxt�t�@�C���쐬
        LogPath = $"{Application.dataPath}/PyLog.txt";

        // ���������Ƀ��O�t�@�C�����폜
        if (File.Exists(LogPath))
        try
        {
            File.Delete(LogPath);
            Debug.Log("���O�t�@�C�����폜���܂����i���������j");
        }
        catch (Exception e)
        {
            Debug.LogError($"���������̃��O�t�@�C���폜�Ɏ��s���܂���: {e.Message}");
        }

        try
        {
            string directory = Path.GetDirectoryName(LogPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.Create(LogPath).Close();
            Debug.Log($"���O�t�@�C���쐬: {LogPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"���O�t�@�C���쐬���s: {e.Message}");
            enabled = false;
            return;
        }

        try
        {
            lastWriteTime = File.GetLastWriteTime(LogPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"�ŏI�X�V�����擾���s: {e.Message}");
            enabled = false;
            return;
        }
        await UniTask.SwitchToMainThread();
    }



    async UniTask ProcessLogFileAsync()
    {
        await UniTask.SwitchToThreadPool();
        if (!File.Exists(LogPath))
        try
        {
            File.Create(LogPath).Close();
            Debug.LogWarning($"���O�t�@�C�����폜���ꂽ���ߍč쐬: {LogPath}");
            lastWriteTime = File.GetLastWriteTime(LogPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"���O�t�@�C���č쐬���s: {e.Message}");
            enabled = false;
            return;
        }

        DateTime currentWriteTime = File.GetLastWriteTime(LogPath);

        if (currentWriteTime != lastWriteTime)
        {
            lastWriteTime = currentWriteTime;
            try
            {
                // ������������ێ�����ϐ�
                string unprocessedLogs = "";

                // Python �Ɠ���txt�t�@�C���𑀍삷��ۂ̋�����h�~
                using (FileStream fs = new FileStream(LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                {
                    // ��؂育�Ƃɕ������ď���
                    string[] logs = sr.ReadToEnd().Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string log in logs)
                    {
                        string trimmedLog = log.Trim();
                        if (!string.IsNullOrEmpty(trimmedLog))
                        try
                        {
                            // ���O���o��
                            Debug.Log(trimmedLog);
                        }
                        catch
                        {
                            // �������s���͖����������ɕێ�
                            unprocessedLogs += "___\n" + log + "\n";
                        }
                    }
                }

                // �������������t�@�C���Ɉ��S�ɏ����߂�
                if (!string.IsNullOrEmpty(unprocessedLogs))
                {
                    using (FileStream fs = new FileStream(LogPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        fs.SetLength(0); // �t�@�C�����e���N���A
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(unprocessedLogs.Trim());
                        }
                    }
                }
                else
                {
                    // �S�ď����ς݂Ȃ�t�@�C�������S�ɃN���A
                    using (FileStream fs = new FileStream(LogPath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        // FileMode.Truncate ���g�p����ƁA�t�@�C�����J�����u�Ԃɂ��̓��e�������I�ɍ폜����A�t�@�C���T�C�Y��0�Ƀ��Z�b�g�����B���̉ӏ��ɋ�̓I�ȏ����������K�v�͖���
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"���O�ǂݎ��G���[: {e.Message}");
            }
        }
        await UniTask.SwitchToMainThread();
    }



    void OnApplicationQuit()
    {
        // �I�����Ƀ��O�t�@�C�����폜
        if (File.Exists(LogPath))
        try
        {
            File.Delete(LogPath);
            Debug.Log("���O�t�@�C�����폜���܂����i�I�����j");
        }
        catch (Exception e)
        {
            Debug.LogError($"�I�����̃��O�t�@�C���폜�Ɏ��s���܂���: {e.Message}");
        }
    }
}