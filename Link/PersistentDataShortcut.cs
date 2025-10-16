#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;



public static class PersistentDataShortcut
{
    // Assets �����ɍ�郊���N�i���O�͂��D�݂Łj
    const string LinkPath = "Assets/Link_Persistent";

    [MenuItem("Maku/Create Shortcut/Symbolic or Junction/PersistentDataPath", priority = 1000)]
    public static void CreatePersistentDataShortcut()
    {
        string targ = Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
        string link = Path.GetFullPath(LinkPath);

        // ������|��
        try
        {
            if (Directory.Exists(link) || File.Exists(link))
            {
                // �V���{���b�N�����N/�W�����N�V�����ł� Directory.Delete �ŏ�����
                Directory.Delete(link, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"���������N�̍폜�Ɏ��s: {e.Message}");
        }

        bool ok = false;

#if UNITY_EDITOR_WIN
        // Windows: symlink(/D) �� junction(/J)�B�J���҃��[�h�Ȃ�Ǘ��ҕs�v�� /D ���ʂ邱�Ƃ�����
        ok = RunCmd($"mklink /D \"{link}\" \"{targ}\"") || RunCmd($"mklink /J \"{link}\" \"{targ}\"");
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        // macOS / Linux: �V���{���b�N�����N
        ok = RunShell($"ln -s \"{targ}\" \"{link}\"");
#endif

        AssetDatabase.Refresh();

        if (ok)
        {
            Debug.Log($"�쐬����: {LinkPath} �� {targ}");
        }
        else
        {
            Debug.LogError(
                $"�����N�쐬�Ɏ��s���܂����B\n" +
                $"�^�[�Q�b�g: {targ}\n�����N: {link}\n" +
#if UNITY_EDITOR_WIN
                "Windows �̏ꍇ: �Ǘ��Ҍ����܂��́u�J���҃��[�h�v��L���ɂ��čĎ��s���Ă݂Ă��������B/J�i�W�����N�V�����j�����s���Ă��܂��B\n"
#else
                "�^�[�~�i��������p�X�̌������m�F���Ă��������B\n"
#endif
            );
        }
    }

#if UNITY_EDITOR_WIN
    static bool RunCmd(string args)
    {
        try
        {
            var psi = new ProcessStartInfo("cmd.exe", "/c " + args)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var p = Process.Start(psi);
            p.WaitForExit();
            return p.ExitCode == 0;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"cmd ���s���s: {e.Message}");
            return false;
        }
    }
#endif

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
    private static bool RunShell(string command)
    {
        try
        {
            var shell = Application.platform == RuntimePlatform.LinuxEditor ? "/bin/bash" : "/bin/zsh";
            var psi = new ProcessStartInfo(shell, "-lc " + EscapeForShell(command))
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var p = Process.Start(psi);
            p.WaitForExit();
            return p.ExitCode == 0;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"�V�F�����s���s: {e.Message}");
            return false;
        }
    }

    private static string EscapeForShell(string s)
    {
        // �S�̂��V���O���N�H�[�g�ŕ�݁A�����̃V���O���N�H�[�g�͈��S�ɃG�X�P�[�v
        return "'" + s.Replace("'", "'\"'\"'") + "'";
    }
#endif
}
#endif
