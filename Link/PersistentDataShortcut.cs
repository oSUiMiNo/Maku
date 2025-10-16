#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;



public static class PersistentDataShortcut
{
    // Assets 直下に作るリンク（名前はお好みで）
    const string LinkPath = "Assets/Link_Persistent";

    [MenuItem("Maku/Create Shortcut/Symbolic or Junction/PersistentDataPath", priority = 1000)]
    public static void CreatePersistentDataShortcut()
    {
        string targ = Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
        string link = Path.GetFullPath(LinkPath);

        // 既存を掃除
        try
        {
            if (Directory.Exists(link) || File.Exists(link))
            {
                // シンボリックリンク/ジャンクションでも Directory.Delete で消える
                Directory.Delete(link, true);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"既存リンクの削除に失敗: {e.Message}");
        }

        bool ok = false;

#if UNITY_EDITOR_WIN
        // Windows: symlink(/D) か junction(/J)。開発者モードなら管理者不要で /D が通ることが多い
        ok = RunCmd($"mklink /D \"{link}\" \"{targ}\"") || RunCmd($"mklink /J \"{link}\" \"{targ}\"");
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
        // macOS / Linux: シンボリックリンク
        ok = RunShell($"ln -s \"{targ}\" \"{link}\"");
#endif

        AssetDatabase.Refresh();

        if (ok)
        {
            Debug.Log($"作成完了: {LinkPath} → {targ}");
        }
        else
        {
            Debug.LogError(
                $"リンク作成に失敗しました。\n" +
                $"ターゲット: {targ}\nリンク: {link}\n" +
#if UNITY_EDITOR_WIN
                "Windows の場合: 管理者権限または「開発者モード」を有効にして再試行してみてください。/J（ジャンクション）も試行しています。\n"
#else
                "ターミナル権限やパスの権限を確認してください。\n"
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
            Debug.LogWarning($"cmd 実行失敗: {e.Message}");
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
            Debug.LogWarning($"シェル実行失敗: {e.Message}");
            return false;
        }
    }

    private static string EscapeForShell(string s)
    {
        // 全体をシングルクォートで包み、内部のシングルクォートは安全にエスケープ
        return "'" + s.Replace("'", "'\"'\"'") + "'";
    }
#endif
}
#endif
