using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;

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
        // Python�t�@�C���p�X
        string pyFile = @$"{PyDir}\{pyFileName}";
        if (!File.Exists(PyExeFile)) Debug.LogError($"���̎��s�t�@�C���͖���{PyExeFile}");
        if (!File.Exists(pyFile)) Debug.LogError($"����Py�t�@�C���͖���{pyFile}");

        // ["] �� [\""] �ɃG�X�P�[�v����Json
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
            JObject outputJObj = new JObject();
            try
            {
                outputJObj = JObject.Parse(output);
                return outputJObj;
            }
            catch
            {
                Debug.Log("�߂�l��Json�ł͂Ȃ�");
                outputJObj["Value"] = output;
            }
            return outputJObj;
        }
        else
        {
            Debug.Log("�߂�l���󂾂���");
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

        // Exited �C�x���g��L���ɂ���
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
            Debug.LogAssertion("�^�C���A�E�g");
            process.Dispose();
        }
        catch
        {
            Debug.Log("�^�C���A�E�g���L�����Z�����ꂽ");
        }
    }
}