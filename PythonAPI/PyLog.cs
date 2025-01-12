using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using Cysharp.Threading.Tasks;

public class PyLog : SingletonCompo<PyLog>
{
    string LogPath; // 監視するファイルのパス
    DateTime lastWriteTime;

    protected sealed override void Awake0() => CreateLogFileAsync().Forget();
    protected sealed override void Update() => ProcessLogFileAsync().Forget();



    async UniTask CreateLogFileAsync()
    {
        await UniTask.SwitchToThreadPool();

        // Assets 直下にログ用txtファイル作成
        LogPath = $"{Application.dataPath}/PyLog.txt";

        // 初期化時にログファイルを削除
        if (File.Exists(LogPath))
        try
        {
            File.Delete(LogPath);
            Debug.Log("ログファイルを削除しました（初期化時）");
        }
        catch (Exception e)
        {
            Debug.LogError($"初期化時のログファイル削除に失敗しました: {e.Message}");
        }

        try
        {
            string directory = Path.GetDirectoryName(LogPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.Create(LogPath).Close();
            Debug.Log($"ログファイル作成: {LogPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"ログファイル作成失敗: {e.Message}");
            enabled = false;
            return;
        }

        try
        {
            lastWriteTime = File.GetLastWriteTime(LogPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"最終更新日時取得失敗: {e.Message}");
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
            Debug.LogWarning($"ログファイルが削除されたため再作成: {LogPath}");
            lastWriteTime = File.GetLastWriteTime(LogPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"ログファイル再作成失敗: {e.Message}");
            enabled = false;
            return;
        }

        DateTime currentWriteTime = File.GetLastWriteTime(LogPath);

        if (currentWriteTime != lastWriteTime)
        {
            lastWriteTime = currentWriteTime;
            try
            {
                // 未処理部分を保持する変数
                string unprocessedLogs = "";

                // Python と同じtxtファイルを操作する際の競合を防止
                using (FileStream fs = new FileStream(LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                {
                    // 区切りごとに分割して処理
                    string[] logs = sr.ReadToEnd().Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string log in logs)
                    {
                        string trimmedLog = log.Trim();
                        if (!string.IsNullOrEmpty(trimmedLog))
                        try
                        {
                            // ログを出力
                            Debug.Log(trimmedLog);
                        }
                        catch
                        {
                            // 処理失敗時は未処理部分に保持
                            unprocessedLogs += "___\n" + log + "\n";
                        }
                    }
                }

                // 未処理部分をファイルに安全に書き戻す
                if (!string.IsNullOrEmpty(unprocessedLogs))
                {
                    using (FileStream fs = new FileStream(LogPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        fs.SetLength(0); // ファイル内容をクリア
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(unprocessedLogs.Trim());
                        }
                    }
                }
                else
                {
                    // 全て処理済みならファイルを完全にクリア
                    using (FileStream fs = new FileStream(LogPath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        // FileMode.Truncate を使用すると、ファイルを開いた瞬間にその内容が自動的に削除され、ファイルサイズが0にリセットされる。この箇所に具体的な処理を書く必要は無い
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ログ読み取りエラー: {e.Message}");
            }
        }
        await UniTask.SwitchToMainThread();
    }



    void OnApplicationQuit()
    {
        // 終了時にログファイルを削除
        if (File.Exists(LogPath))
        try
        {
            File.Delete(LogPath);
            Debug.Log("ログファイルを削除しました（終了時）");
        }
        catch (Exception e)
        {
            Debug.LogError($"終了時のログファイル削除に失敗しました: {e.Message}");
        }
    }
}