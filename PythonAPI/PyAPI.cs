using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Text;

public class PyAPI
{
    string PyExeFile;
    string PyDir;

    public PyAPI(string pyDir, string pyExeFile = "")
    {
        PyDir = pyDir;
        if (string.IsNullOrEmpty(pyExeFile)) PyExeFile = $"{pyDir}/.venv/Scripts/python.exe";
        else PyExeFile = pyExeFile ;
    }



    public async UniTask<System.Diagnostics.Process> Idle(string pyFileName, float timeout = 0)
    {
        // Python�t�@�C���p�X
        string pyFile = @$"{PyDir}\{pyFileName}";
        if (!File.Exists(PyExeFile)) Debug.LogError($"���̎��s�t�@�C���͖���{PyExeFile}");
        if (!File.Exists(pyFile)) Debug.LogError($"����Py�t�@�C���͖���{pyFile}");

        await UniTask.SwitchToThreadPool();
        System.Diagnostics.Process process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo(PyExeFile)
            {
                Arguments = $"{pyFile}",
                UseShellExecute = false, // �V�F�����g�p���Ȃ�
                RedirectStandardOutput = true, // �W���o�͂����_�C���N�g
                RedirectStandardInput = true, // �W�����͂����_�C���N�g
                RedirectStandardError = true, // �W���G���[�����_�C���N�g
                CreateNoWindow = true, // PowerShell�E�B���h�E��\�����Ȃ�
            }
        };

        process.Start();
        await UniTask.SwitchToMainThread();
        return process;
    }



    public async UniTask<JObject> Exe(string pyFileName, float timeout = 0) {
        return await Exe(pyFileName, new JObject(), timeout);
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
                Arguments = $"{pyFile} {sendData}",
                UseShellExecute = false, // �V�F�����g�p���Ȃ�
                RedirectStandardOutput = true, // �W���o�͂����_�C���N�g
                RedirectStandardInput = true, // �W�����͂����_�C���N�g
                RedirectStandardError = true, // �W���G���[�����_�C���N�g
                CreateNoWindow = true, // PowerShell�E�B���h�E��\�����Ȃ�
            }
        };

        string output = await process.RunAsync(timeout);
        if (!string.IsNullOrEmpty(output))
        {
            Debug.Log($"Raw Python Output:\n{output}"); // ���̏o�͂����O�o��

            // ���s�R�[�h�𓝈� (���ɏd�v)
            output = output.Replace("\r\n", "\n").Replace("\r", "\n");

            // ���K�\���p�^�[��
            string pattern = @"JSON_OUTPUT_START(.*?)JSON_OUTPUT_END";

            // ���K�\����JSON������𒊏o
            Match match = Regex.Match(output, pattern); // .* ��ǉ�


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
                    Debug.LogError($"JSON�p�[�X�G���[: {ex.Message}");
                    Debug.LogError($"JSON������: {jsonString}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"�\�����ʃG���[: {ex.Message}");
                    Debug.LogError($"JSON������: {jsonString}");
                    return null;
                }
            }
            else
            {
                Debug.LogError("JSON�o�͂�������܂���ł����B");
                return null;
            }
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
    public static async UniTask Send(this System.Diagnostics.Process process, JObject inputJObj)
    {
        await UniTask.SwitchToThreadPool();

        string sendData = JsonConvert.SerializeObject(inputJObj);
        var inputWriter = process.StandardInput;
        //var outputReader = process.StandardOutput;
        inputWriter.WriteLine(sendData);
        inputWriter.Flush();

        // ReadToEnd() �������ŌĂԂƏd�����Ă�΂��B��ŏo�͂�txt�̕����ɂ���
        //var output = outputReader.ReadToEnd();
        await UniTask.SwitchToMainThread();
        //return output;
    }



    public static UniTask<string> RunAsync(this System.Diagnostics.Process process, float timeout = 0)
    {
        var cts = new CancellationTokenSource();
        var exited = new UniTaskCompletionSource<string>();
        string output = "";

        if (timeout != 0)
            UniTask.RunOnThreadPool(() => process.Cancel(timeout, cts.Token)).Forget();
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



    public static async void Cancel(this System.Diagnostics.Process process, float timeout, CancellationToken cancellationToken)
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