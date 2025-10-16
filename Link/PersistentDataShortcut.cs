#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;



public static class PersistentDataShortcut_WIN
{
    const string LinkPath = "Assets/Link_Persistent";

    [MenuItem("Maku/Create Shortcut (Windows)/PersistentDataPath → Assets", priority = 1000)]
    public static void CreatePersistentDataShortcut()
    {
#if UNITY_EDITOR_WIN
        UniTask.Void(async () =>
        {
            string targ = Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar);
            string link = Path.GetFullPath(LinkPath);

            // persistentDataPath が未作成でも落ちないように確保
            try { Directory.CreateDirectory(targ); } catch { }

            // 既存リンク/フォルダを除去
            try
            {
                // PowerShellでの安全削除（リンク/実体を誤って追わない）
                string rm = $"if (Test-Path -LiteralPath '{link}') {{ Remove-Item -LiteralPath '{link}' -Force }}";
                await PowerShellAPI.Command(rm);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"既存のリンク削除に失敗: {e.Message}");
            }

            bool ok = false;
            string madeType = "?";

            // まず SymbolicLink を試す（開発者モード or 管理者で成功しやすい）
            try
            {
                string cmdSym = $"New-Item -ItemType SymbolicLink -Path '{link}' -Target '{targ}'";
                await PowerShellAPI.Command(cmdSym);
                ok = true;
                madeType = "SymbolicLink";
            }
            catch
            {
                // ダメなら Junction にフォールバック（管理者不要で通ることが多い）
                try
                {
                    string cmdJunc = $"New-Item -ItemType Junction -Path '{link}' -Target '{targ}'";
                    await PowerShellAPI.Command(cmdJunc);
                    ok = true;
                    madeType = "Junction";
                }
                catch (System.Exception e2)
                {
                    Debug.LogError(
                        "リンク作成に失敗しました（SymbolicLink / Junction ともに失敗）。\n" +
                        $"ターゲット: {targ}\nリンク: {link}\n" +
                        $"詳細: {e2.Message}\n" +
                        "※ SymbolicLink は開発者モード or 管理者権限が必要な場合があります。"
                    );
                }
            }

            AssetDatabase.Refresh();

            if (ok)
            {
                Debug.Log($"作成完了: [{madeType}] {LinkPath} → {targ}");
            }
        });
#else
        Debug.LogError("このコマンドは Windows 専用です。");
#endif
    }
}
#endif
