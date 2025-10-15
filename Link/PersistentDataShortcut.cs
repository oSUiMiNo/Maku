#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;



public static class PersistentDataShortcut
{
    // Assets �����ɍ�郊���N�i���O�͂��D�݂Łj
    private const string LinkPathUnderAssets = "Assets/Link_Persistent";

    [MenuItem("Tools/Create Shortcut/PersistentDataPath -> Assets", priority = 1000)]
    public static void CreatePersistentDataShortcut()
    {
        string target = Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
        string link = Path.GetFullPath(LinkPathUnderAssets);

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
            UnityEngine.Debug.LogWarning($"���������N�̍폜�Ɏ��s: {e.Message}");
        }

        bool ok = false;

#if UNITY_EDITOR_WIN
        // Windows: symlink(/D) �� junction(/J)�B�J���҃��[�h�Ȃ�Ǘ��ҕs�v�� /D ���ʂ邱�Ƃ�����
        ok = RunCmd($"mklink /D \"{link}\" \"{target}\"") || RunCmd($"mklink /J \"{link}\" \"{target}\"");
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        // macOS / Linux: �V���{���b�N�����N
        ok = RunShell($"ln -s \"{target}\" \"{link}\"");
#endif

        AssetDatabase.Refresh();

        if (ok)
        {
            UnityEngine.Debug.Log($"�쐬����: {LinkPathUnderAssets} �� {target}");
        }
        else
        {
            UnityEngine.Debug.LogError(
                $"�����N�쐬�Ɏ��s���܂����B\n" +
                $"�^�[�Q�b�g: {target}\n�����N: {link}\n" +
#if UNITY_EDITOR_WIN
                "Windows �̏ꍇ: �Ǘ��Ҍ����܂��́u�J���҃��[�h�v��L���ɂ��čĎ��s���Ă݂Ă��������B/J�i�W�����N�V�����j�����s���Ă��܂��B\n"
#else
                "�^�[�~�i��������p�X�̌������m�F���Ă��������B\n"
#endif
            );
        }
    }

#if UNITY_EDITOR_WIN
    private static bool RunCmd(string args)
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
            UnityEngine.Debug.LogWarning($"cmd ���s���s: {e.Message}");
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
            UnityEngine.Debug.LogWarning($"�V�F�����s���s: {e.Message}");
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
