using UnityEngine;
using System.IO;
using System;

// Debug.Logを拡張し、ランタイム実行時にログをファイルに出力
public class RuntimeLogger : MonoBehaviour
{
    private static RuntimeLogger instance;
    private string logFilePath;

    private void Awake()
    {
        // シングルトンパターンで、シーンをまたいでインスタンスを1つに保つ
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

        // ログファイルのパスを決定
        logFilePath = Path.Combine(Application.persistentDataPath, "runtime_log.txt");

        // アプリケーション起動時にログファイルをクリア
        ClearLogFile();

        // logMessageReceivedイベントにハンドラを登録
        Application.logMessageReceived += HandleLog;

        Debug.Log("ランタイムログ起動");
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄される際にイベントハンドラの登録を解除
        Application.logMessageReceived -= HandleLog;
    }

    //================================================
    // ログメッセージを受け取った際の処理
    //================================================
    void HandleLog(string logMsg, string stackTrace, LogType type)
    {
        // エディタ実行時はファイルに書き込まない
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
            // ファイル書き込みに失敗した場合のエラー処理（このログはファイルには書かれない）
            Debug.LogError($"ランタイムログ書き込み失敗: {e.Message}");
        }
    }

    //================================================
    // ログファイルをクリア
    //================================================
    private void ClearLogFile()
    {
        // ランタイム（ビルド後）でのみファイルをクリアする
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
            // ファイル操作に失敗した場合
            Debug.LogError($"ログファイルのクリア失敗\nパス: {logFilePath}\n原因: {e.Message}");
        }
    }
}