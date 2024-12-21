using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Threading.Tasks;

public class PyAPI
{
    string PyExeFile;
    string PyDir;

    public PyAPI(string pyExeFile, string pyDir)
    {
        PyExeFile = pyExeFile;
        PyDir = pyDir;
    }

    
    public async UniTask<JObject> Exe(string pyFileName, JObject inputJObj, float timeout = 15)
    {
        // Pythonファイルパス
        string pyFile = @$"{PyDir}\{pyFileName}";
        // ["] を [\""] にエスケープしたJson
        string sendData = JsonConvert.SerializeObject(inputJObj).Replace("\"", "\\\"\"");

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo(PyExeFile)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = $"{pyFile} {sendData}"
            }
        };

        string output = await process.RunAsync(timeout);
        if (!string.IsNullOrEmpty(output))
        {
            JObject outputJObj = JObject.Parse(output);
            return outputJObj;
        }
        else
        {
            Debug.LogAssertion("戻り値が空だった");
            return null;
        }
    }
}


public static class ProcessExtentions
{
    public static UniTask<string> RunAsync(this System.Diagnostics.Process process, float timeout)
    {
        var cts = new CancellationTokenSource();
        var exited = new UniTaskCompletionSource<string>();
        string output = "";

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
        try {
            await Task.Delay(TimeSpan.FromSeconds(timeout), cancellationToken);
            Debug.LogAssertion("タイムアウト");
            process.Dispose();
        }
        catch (TaskCanceledException) {
            //Debug.Log("タイムアウトがキャンセルされた");
        }
    }
}