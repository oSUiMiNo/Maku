using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using UniRx;


public class PyAPIHandler : SingletonCompo<PyAPIHandler>
{
    static string LogPath => $"{Application.dataPath}/PyLog.txt"; // 監視するファイルのパス
    static SharedLog Log = new SharedLog(LogPath);
    //IObservable<long> OnRead => existLog.UpdateWhileEqualTo(File.Exists(LogPath), 0.05f);
    IObservable<long> OnRead => logActive.TimerWhileEqualTo(Log.isActive, 0.05f);
    static BoolReactiveProperty logActive = new BoolReactiveProperty(true);

    protected sealed override void Awake0()
    {
        OnRead.Subscribe(_ =>
        {
            if (!File.Exists(LogPath)) return; // なんかオペレータをすり抜けるのでブロックしとく
            //Debug.Log($"ログ {File.Exists(LogPath)}");
            Log.ReadLogFile();
        }).AddTo(this);

        Log.OnLog.Subscribe(msg =>
        {
            Debug.Log(msg);
        }).AddTo(this);
    }

    private async void OnApplicationQuit()
    {
        PyAPI.CloseAll();
        // 終了後に待ちたいのでここはDelay.Secondではだめ
        await UniTask.Delay(1);
        Log.Close();
        logActive.Dispose();
        //Debug.Log("ログ　くわいと");
    }

    private async void OnDestroy()
    {
        PyAPI.CloseAll();
        // 終了後に待ちたいのでここはDelay.Secondではだめ
        await UniTask.Delay(1);
        Log.Close();
        logActive.Dispose();
        //Debug.Log("ログ　ですとろい");
    }
}



public class PyFnc
{
    System.Diagnostics.Process process;
    string OutPath; // 監視するファイルのパス
    SharedLog Output;
    IObservable<long> OnRead => logActive.TimerWhileEqualTo(Output.isActive, 0.01f);
    static BoolReactiveProperty logActive = new BoolReactiveProperty(true);
    public IObservable<JObject> OnOut => Output.OnLog
    .Select(msg =>
    {
        try
        {
            return JObject.Parse(msg);
        }
        catch (Exception ex)
        {
            // エラー処理 (必要に応じて)
            Debug.LogError($"JSONパースエラー: {ex.Message}");
            return null;
        }
    })
    .Where(JO => JO != null);

    public PyFnc(string pyInterpFile, string pyFile, string sendData = "")
    {
        // 実行用プロセス作成
        process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo(pyInterpFile)
            {
                Arguments = $"{pyFile} {sendData}",
                UseShellExecute = false, // シェルを使用しない
                RedirectStandardOutput = true, // 標準出力をリダイレクト
                RedirectStandardInput = true, // 標準入力をリダイレクト
                RedirectStandardError = true, // 標準エラーをリダイレクト
                CreateNoWindow = true, // PowerShellウィンドウを表示しない
            }
        };
        InitLog(pyFile);
    }

    void InitLog(string pyFile)
    {
        // アウトプット用ファイル作成;
        OutPath = $"{pyFile.Replace("\\", "/").Replace(".py", ".txt")}";
        Output = new SharedLog(OutPath);
        OnRead.Subscribe(_ =>
        {
            if (!File.Exists(OutPath)) return; // なんかオペレータをすり抜けるのでブロックしとく
            //Debug.Log($"ログ{File.Exists(OutPath)} {Path.GetFileName(OutPath)}");
            Output.ReadLogFile();
        }).AddTo(PyAPIHandler.Compo);
    }

    public void Start()
    {
        process.Start();
    }

    public async void Close()
    {
        await process.Command("Close");
        process.PerfectKill();
        Output.Close();
        logActive.Dispose();
    }

    public async UniTask Exe(JObject inputJObj)
    {
        await process.Exe(inputJObj);
    }

    public async UniTask<string> RunAsync(float timeout = 0)
    {
        return await process.RunAsync(timeout, () => Output.Close());
    }
}




public class PyAPI
{
    string PyInterpFile;
    string PyDir;
    static List<PyFnc> IdlongFncs = new List<PyFnc>();

    public PyAPI(string pyDir, string pyInterpFile = "")
    {
        PyDir = pyDir;
        if (string.IsNullOrEmpty(pyInterpFile)) PyInterpFile = $"{pyDir}/.venv/Scripts/python.exe";
        else PyInterpFile = pyInterpFile ;
    }


    public static void CloseAll()
    { 
        foreach (var fnc in IdlongFncs)
        {
            fnc.Close();
        }
    }


    public PyFnc Idle(string pyFileName, float timeout = 0)
    {
        // Pythonファイルパス
        string pyFile = @$"{PyDir}\{pyFileName}";
        if (!File.Exists(PyInterpFile)) Debug.LogError($"次の実行ファイルは無い{PyInterpFile}");
        if (!File.Exists(pyFile)) Debug.LogError($"次のPyファイルは無い{pyFile}");

        PyFnc pyFnc = new PyFnc(PyInterpFile, pyFile);
        IdlongFncs.Add(pyFnc);
        pyFnc.Start();

        return pyFnc;
    }


    public async UniTask<JObject> Exe(string pyFileName, float timeout = 0) {
        return await Exe(pyFileName, new JObject(), timeout);
    }

    public async UniTask<JObject> Exe(string pyFileName, JObject inputJObj, float timeout = 0)
    {
        // Pythonファイルパス
        string pyFile = @$"{PyDir}\{pyFileName}";
        if (!File.Exists(PyInterpFile)) Debug.LogError($"次の実行ファイルは無い{PyInterpFile}");
        if (!File.Exists(pyFile)) Debug.LogError($"次のPyファイルは無い{pyFile}");

        // ["] を [\""] にエスケープしたJson
        string sendData = JsonConvert.SerializeObject(inputJObj).Replace("\"", "\\\"\"");
        PyFnc pyFnc = new PyFnc(PyInterpFile, pyFile, sendData);
        IdlongFncs.Add(pyFnc);
        string output = await pyFnc.RunAsync(timeout);
        IdlongFncs.Remove(pyFnc);

        return null;
    }
}


public static class ProcessExtentions
{
    public static async UniTask Exe(this System.Diagnostics.Process process, JObject inputJObj)
    {
        string sendData = JsonConvert.SerializeObject(inputJObj);
        var inputWriter = process.StandardInput;
        inputWriter.WriteLine(sendData);
        inputWriter.Flush();
    }


    public static async UniTask Command(this System.Diagnostics.Process process, string command)
    {
        var inputWriter = process.StandardInput;
        inputWriter.WriteLine(command);
        inputWriter.Flush();
    }



    public static UniTask<string> RunAsync(this System.Diagnostics.Process process, float timeout = 0, Action fncOnDispose = null)
    {
        var cts = new CancellationTokenSource();
        var exited = new UniTaskCompletionSource<string>();
        string output = "";

        if (timeout != 0)
            UniTask.RunOnThreadPool(() => process.Timeout(timeout, cts.Token)).Forget();

        // Exited イベントを有効にする
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) =>
        {
            string error = process.StandardError.ReadToEnd(); // エラー読取り
            if (!string.IsNullOrEmpty(error)) Debug.LogError($"PowerShell Error: {error}");

            output = process.StandardOutput.ReadToEnd();
            process.Dispose();
        };

        process.Disposed += (sender, args) =>
        {
            exited.TrySetResult(output);
            cts.Cancel();
            fncOnDispose?.Invoke();
        };

        process.Start();

        return exited.Task;
    }



    public static async void Timeout(this System.Diagnostics.Process process, float timeout, CancellationToken cancellationToken)
    {
        try
        {
            await UniTask.WaitForSeconds(timeout, false, PlayerLoopTiming.Update, cancellationToken);
            Debug.LogAssertion("タイムアウト");
            process.PerfectKill();
        }
        catch
        {
            Debug.Log("タイムアウトがキャンセルされた");
        }
    }



    public static void PerfectKill(this System.Diagnostics.Process process)
    {
        process.Kill();
        process.Dispose();
    }

}