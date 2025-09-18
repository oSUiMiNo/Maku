using UnityEngine;
using System.IO;
using System;


///*******************************************************<summary>
/// Debug.Logを拡張しランタイム（ビルド後）にログをファイルに出力
///</summary>******************************************************
public class RuntimeLogger : MonoBehaviour
{
    static RuntimeLogger Ins;
    // ログファイルのパス
    string LogFile => $"{Application.persistentDataPath}/runtime_log.txt";


    void Awake()
    {
        // ランタイム（ビルド後）でなければ無視
        if (Application.isEditor) return;

        //--------------------------------------
        // シーンをまたげるようにシングルトン化
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
        // 起動時にログファイルをクリア
        ClearLogFile();
        // 標準の logMessageReceived イベントにログ処理を登録
        Application.logMessageReceived += HandleLog;
        Debug.Log("ランタイムログ起動完了");
    }


    void OnDestroy()
    {
        // イベントハンドラ登録解除
        Application.logMessageReceived -= HandleLog;
    }


    ///==============================================<summary>
    /// ログメッセージを受け取った際の処理
    ///</summary>=============================================
    void HandleLog(string logMsg, string stackTrace, LogType type)
    {
        // ランタイム（ビルド後）でなければ無視
        if (Application.isEditor) return;

        try
        {
            // 1つ分のログ文作成
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {logMsg}\n";
            if (type == LogType.Error || type == LogType.Exception)
            {
                logEntry += $"{stackTrace}\n";
            }
            // ログをファイルに追記
            File.AppendAllText(LogFile, logEntry);
        }
        catch (Exception e)
        {
            Debug.LogError($"ランタイムログ書き込み失敗：{e.Message}");
        }
    }


    ///==============================================<summary>
    /// ログファイルをクリア
    ///</summary>=============================================
    void ClearLogFile()
    {
        // ランタイム（ビルド後）でなければ無視
        if (Application.isEditor) return;
     
        try
        {
            // ファイルを空の文字列で上書き
            File.WriteAllText(LogFile, string.Empty);
        }
        catch (Exception e)
        {
            Debug.LogError($"ログファイルのクリア失敗\nパス: {LogFile}\n原因: {e.Message}");
        }
    }
}