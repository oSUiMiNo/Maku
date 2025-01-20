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
    static string LogPath => $"{Application.dataPath}/PyLog.txt"; // �Ď�����t�@�C���̃p�X
    static SharedLog Log = new SharedLog(LogPath);
    //IObservable<long> OnRead => existLog.UpdateWhileEqualTo(File.Exists(LogPath), 0.05f);
    IObservable<long> OnRead => logActive.TimerWhileEqualTo(Log.isActive, 0.05f);
    static BoolReactiveProperty logActive = new BoolReactiveProperty(true);

    protected sealed override void Awake0()
    {
        OnRead.Subscribe(_ =>
        {
            if (!File.Exists(LogPath)) return; // �Ȃ񂩃I�y���[�^�����蔲����̂Ńu���b�N���Ƃ�
            //Debug.Log($"���O {File.Exists(LogPath)}");
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
        // �I����ɑ҂������̂ł�����Delay.Second�ł͂���
        await UniTask.Delay(1);
        Log.Close();
        logActive.Dispose();
        //Debug.Log("���O�@���킢��");
    }

    private async void OnDestroy()
    {
        PyAPI.CloseAll();
        // �I����ɑ҂������̂ł�����Delay.Second�ł͂���
        await UniTask.Delay(1);
        Log.Close();
        logActive.Dispose();
        //Debug.Log("���O�@�ł��Ƃ낢");
    }
}



public class PyFnc
{
    System.Diagnostics.Process process;
    string OutPath; // �Ď�����t�@�C���̃p�X
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
        InitLog(pyFile);
    }

    void InitLog(string pyFile)
    {
        // �A�E�g�v�b�g�p�t�@�C���쐬;
        OutPath = $"{pyFile.Replace("\\", "/").Replace(".py", ".txt")}";
        Output = new SharedLog(OutPath);
        OnRead.Subscribe(_ =>
        {
            if (!File.Exists(OutPath)) return; // �Ȃ񂩃I�y���[�^�����蔲����̂Ńu���b�N���Ƃ�
            //Debug.Log($"���O{File.Exists(OutPath)} {Path.GetFileName(OutPath)}");
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
            Debug.LogAssertion("�^�C���A�E�g");
            process.PerfectKill();
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