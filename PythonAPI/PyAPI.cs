using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;

public class PyAPI
{
    string PyExeFile;
    string PyDir;

    public PyAPI(string pyExeFile, string pyDir)
    {
        PyExeFile = pyExeFile;
        PyDir = pyDir;
    }


    public async UniTask<JObject> Exe(string pyFileName, JObject inputJObj, float timeout = 0)
    {
        // Pythonファイルパス
        string pyFile = @$"{PyDir}\{pyFileName}";
        if (!File.Exists(PyExeFile)) Debug.LogError($"次の実行ファイルは無い{PyExeFile}");
        if (!File.Exists(pyFile)) Debug.LogError($"次のPyファイルは無い{pyFile}");

        // ["] を [\""] にエスケープしたJson
        string sendData = JsonConvert.SerializeObject(inputJObj).Replace("\"", "\\\"\"");

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo(PyExeFile)
            {
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardOutput = true,
                Arguments = $"{pyFile} {sendData}"
            }
        };

        string output = await process.RunAsync(timeout);
        if (!string.IsNullOrEmpty(output))
        {
            Debug.Log($"Raw Python Output:\n{output}"); // 生の出力をログ出力

            // 改行コードを統一 (非常に重要)
            output = output.Replace("\r\n", "\n").Replace("\r", "\n");

            // 正規表現パターン
            string pattern = @"JSON_OUTPUT_START(.*?)JSON_OUTPUT_END";

            // 正規表現でJSON文字列を抽出
            Match match = Regex.Match(output, pattern); // .* を追加


            if (match.Success)
            {
                string jsonString = match.Groups[1].Value;
                try
                {
                    JObject outputJObj = JObject.Parse(jsonString);
                    return outputJObj;
                }
                catch (JsonReaderException ex)
                {
                    Debug.LogError($"JSONパースエラー: {ex.Message}");
                    Debug.LogError($"JSON文字列: {jsonString}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"予期せぬエラー: {ex.Message}");
                    Debug.LogError($"JSON文字列: {jsonString}");
                    return null;
                }
            }
            else
            {
                Debug.LogError("JSON出力が見つかりませんでした。");
                return null;
            }
        }
        else
        {
            Debug.Log("戻り値が空だった");
            return null;
        }
    }
}


public static class ProcessExtentions
{
    public static UniTask<string> RunAsync(this System.Diagnostics.Process process, float timeout = 0)
    {
        var cts = new CancellationTokenSource();
        var exited = new UniTaskCompletionSource<string>();
        string output = "";

        if (timeout != 0)
            UniTask.RunOnThreadPool(() => process.Cancel(timeout, cts.Token));

        // Exited イベントを有効にする
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) =>
        {
            output = process.StandardOutput.ReadToEnd();
            process.Dispose();
        };

        process.Disposed += (sender, args) =>
        {
            exited.TrySetResult(output);
            cts.Cancel();
        };

        process.Start();

        return exited.Task;
    }

    public static async void Cancel(this System.Diagnostics.Process process, float timeout, CancellationToken cancellationToken)
    {
        try
        {
            await UniTask.WaitForSeconds(timeout, false, PlayerLoopTiming.Update, cancellationToken);
            Debug.LogAssertion("タイムアウト");
            process.Dispose();
        }
        catch
        {
            Debug.Log("タイムアウトがキャンセルされた");
        }
    }
}