using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using UniRx;


public class PyAPIHandler : SingletonCompo<PyAPIHandler>
{
    static string LogPath => $"{Application.dataPath}/PyLog.txt"; // �Ď�����t�@�C���̃p�X
    static SharedLog Log = new SharedLog(LogPath);
    static BoolReactiveProperty Readable = new BoolReactiveProperty(true);

    protected sealed override void Awake0()
    {
        Readable.TimerWhileEqualTo(true, 0.011f).Subscribe(_ =>
        {
            Log.ReadLogFileAsync().Forget();
        }).AddTo(this);
        Log.OnLog.Subscribe(msg =>
        {
            Debug.Log(msg);
        }).AddTo(this);
    }

    private void OnApplicationQuit()
    {
        PyAPI.CloseAll();
        Log.Close();
    }

    private void OnDestroy()
    {
        PyAPI.CloseAll();
        Log.Close();
    }
}



public class PyFnc
{
    static BoolReactiveProperty Readable = new BoolReactiveProperty(true);
    System.Diagnostics.Process process;
    SharedLog Output;
    public IObservable<JObject> OnOut => Output.OnLog
    .Select(msg =>
    {
        try
        {
            return JObject.Parse(msg);
        }
        catch (Exception ex)
        {
            // �G���[���� (�K�v�ɉ�����)
            Debug.LogError($"JSON�p�[�X�G���[: {ex.Message}");
            return null;
        }
    })
    .Where(JO => JO != null);

    public PyFnc(string pyInterpFile, string pyFile, string sendData = "")
    {
        // ���s�p�v���Z�X�쐬
        process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo(pyInterpFile)
            {
                Arguments = $"{pyFile} {sendData}",
                UseShellExecute = false, // �V�F�����g�p���Ȃ�
                RedirectStandardOutput = true, // �W���o�͂����_�C���N�g
                RedirectStandardInput = true, // �W�����͂����_�C���N�g
                RedirectStandardError = true, // �W���G���[�����_�C���N�g
                CreateNoWindow = true, // PowerShell�E�B���h�E��\�����Ȃ�
            }
        };

        // �A�E�g�v�b�g�p�t�@�C���쐬;
        Output = new SharedLog($"{pyFile.Replace(".py", ".txt")}");
        Readable.TimerWhileEqualTo(true, 0.01f).Subscribe(_ =>
        {
            Output.ReadLogFileAsync().Forget();
        });
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
        // Python�t�@�C���p�X
        string pyFile = @$"{PyDir}\{pyFileName}";
        if (!File.Exists(PyInterpFile)) Debug.LogError($"���̎��s�t�@�C���͖���{PyInterpFile}");
        if (!File.Exists(pyFile)) Debug.LogError($"����Py�t�@�C���͖���{pyFile}");

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
        // Python�t�@�C���p�X
        string pyFile = @$"{PyDir}\{pyFileName}";
        if (!File.Exists(PyInterpFile)) Debug.LogError($"���̎��s�t�@�C���͖���{PyInterpFile}");
        if (!File.Exists(pyFile)) Debug.LogError($"����Py�t�@�C���͖���{pyFile}");

        // ["] �� [\""] �ɃG�X�P�[�v����Json
        string sendData = JsonConvert.SerializeObject(inputJObj).Replace("\"", "\\\"\"");
        PyFnc pyFnc = new PyFnc(PyInterpFile, pyFile, sendData);
        string output = await pyFnc.RunAsync(timeout);

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
            UniTask.RunOnThreadPool(() => process.Cancel(timeout, cts.Token, fncOnDispose)).Forget();
            //UniTask.RunOnThreadPool(() => process.Cancel(timeout, cts.Token));

        // Exited �C�x���g��L���ɂ���
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) =>
        {
            string error = process.StandardError.ReadToEnd(); // �G���[�ǎ��
            if (!string.IsNullOrEmpty(error)) Debug.LogError($"PowerShell Error: {error}");

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



    public static async void Cancel(this System.Diagnostics.Process process, float timeout, CancellationToken cancellationToken, Action fncOnDispose = null)
    {
        try
        {
            await UniTask.WaitForSeconds(timeout, false, PlayerLoopTiming.Update, cancellationToken);
            Debug.LogAssertion("�^�C���A�E�g");
            process.PerfectKill();
            fncOnDispose?.Invoke();
        }
        catch
        {
            Debug.Log("�^�C���A�E�g���L�����Z�����ꂽ");
        }
    }



    public static void PerfectKill(this System.Diagnostics.Process process)
    {
        process.Kill();
        process.Dispose();
    }

}