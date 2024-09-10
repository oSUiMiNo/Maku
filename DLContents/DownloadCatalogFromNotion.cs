using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

// �f�[�^�x�[�X�͂���Notion�y�[�W�ɂ���B
// https://www.notion.so/6795dd70753a495897cb85e0abce95fe?pvs=4


public class DownloadCatalogFromNotion : MonoBehaviour
{
    private const string NotionAccessToken = "secret_OIxSWO69mxnD9FNbmL2US0pcsLWCUmsaglBZBCWPWrC";
    //private const string DatabaseID = "e8777c71cf694fa8bd135fb1d7cb1728";
    private const string DatabaseID = "ee761ae157e346b88ef3fd58ba9146d3";



    [SerializeField]
    public string assetName;


    void Start()
    {
        InputEventHandler.OnDown_D += Execute;
        InputEventHandler.OnDown_Q += async () => await CallNotionAPI_QueryFile("�e�X�g�A�Z�b�g2");

    }



    async void Execute()
    {
        await CallNotionAPI_DownloadFile(await CallNotionAPI_QueryFile("�e�X�g�A�Z�b�g2"));


        if (!string.IsNullOrEmpty(assetName)) await CallNotionAPI_DownloadFile(await CallNotionAPI_QueryFile(assetName));
        else
        {
            Debug.Log(NotionAssetTable.Ins == null);
            foreach (var a in NotionAssetTable.Ins.assetNames)
            {
                await CallNotionAPI_DownloadFile(await CallNotionAPI_QueryFile(a));
                Debug.Log(a);
            }
        }
    }



    async UniTask CallNotionAPI_DownloadFile(JToken contentObj)
    {
        if (contentObj == null || !contentObj.Any()) return;

        // contentObj��JArray�Ȃ̂ŁA�܂��ŏ��̗v�f���擾����
        var firstElement = contentObj[0];

        string downloadURL = "";
        string fileName = "";

        Debug.Log("First Element: " + firstElement.ToString());

        // properties �ɃA�N�Z�X
        var properties = firstElement["properties"] as JObject;

        if (properties == null)
        {
            Debug.LogError("properties �t�B�[���h��������܂���");
            return;
        }

        // �J�^���O�t�@�C���t�B�[���h���m�F
        var catalogFileProperty = properties["�J�^���O�t�@�C��"];
        Debug.Log("Catalog File Property: " + catalogFileProperty?.ToString());

        // �J�^���O�t�@�C�������݂��A���� files �� JArray �ł��邱�Ƃ��m�F
        if (catalogFileProperty != null && catalogFileProperty["files"] is JArray catalogArray && catalogArray.Count > 0)
        {
            downloadURL = catalogArray[0]["file"]["url"].ToString();
            fileName = catalogArray[0]["name"].ToString();
        }
        else
        {
            Debug.LogError("�J�^���O�t�@�C���ɃA�N�Z�X�ł��܂���");
            return;
        }

        string newFilePath = @$"C:\Users\{Environment.UserName}\Downloads\{fileName}";

        // �t�@�C���_�E�����[�h
        using (UnityWebRequest request = new UnityWebRequest(downloadURL))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            await request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                byte[] results = request.downloadHandler.data;
                using (FileStream fs = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(results, 0, results.Length);
                }

                Debug.Log($"�t�@�C���� {newFilePath} �ɕۑ�����܂����B");
            }
        }
    }

    private void OnBecameInvisible()
    {

    }

    async UniTask<JToken> CallNotionAPI_SearchFile(string searchWord)
    {
        WWWForm form = new WWWForm();
        string jsonStr = string.Empty;
        using (UnityWebRequest request = UnityWebRequest.Post($"https://api.notion.com/v1/databases/{DatabaseID}/query", form))
        {
            request.SetRequestHeader("Authorization", $"Bearer {NotionAccessToken}");
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            request.SetRequestHeader("Notion-Version", "2022-02-22");

            await request.SendWebRequest();

            // �G���[����
            switch (request.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("���N�G�X�g��");
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log("���N�G�X�g����");
                    break;

                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log(
                        @"�T�[�o�Ƃ̒ʐM�Ɏ��s�B
                        ���N�G�X�g���ڑ��ł��Ȃ������A
                        �Z�L�����e�B�ŕی삳�ꂽ�`���l�����m���ł��Ȃ������ȂǁB");
                    Debug.LogError(request.error);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(
                        @"�T�[�o���G���[������Ԃ����B
                        �T�[�o�Ƃ̒ʐM�ɂ͐����������A
                        �ڑ��v���g�R���Œ�`����Ă���G���[���󂯎�����B");
                    Debug.LogError(request.error);
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log(
                        @"�f�[�^�̏������ɃG���[�������B
                        ���N�G�X�g�̓T�[�o�Ƃ̒ʐM�ɐ����������A
                        ��M�����f�[�^�̏������ɃG���[�������B
                        �f�[�^���j�����Ă��邩�A�������`���ł͂Ȃ��ȂǁB");
                    Debug.LogError(request.error);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            jsonStr = request.downloadHandler.text;
        }

        // �p�[�X
        JObject responseObj = JObject.Parse(jsonStr);

        // �e�[�u���̑S�v�f
        JArray elements = (JArray)responseObj["results"];
        
        // ����
        List<JToken> errements_Searched = elements.Where(b => new Regex(searchWord).IsMatch(b.ToString())).ToList();

        // �����̌��ʈ�ӂɍi��Ă���ꍇ�͂��̗v�f����(Notion DB �̂Ƃ����s)��Ԃ��B
        // �Y���������������ꍇ�͂��̈ꗗ�������B
        // ���[�U�[�̓l�N�X�g�A�N�V�����Ƃ��āA�ꗗ�̒������ӂɍi�荞�߂錟�����[�h����肵�ēx�����B
        if(errements_Searched.Count() == 1)
        {
            var props = errements_Searched[0]["properties"];
            Debug.Log(
                  $"�A�Z�b�g���F{props["�A�Z�b�g��"]["title"][0]["text"]["content"]}\n" +
                  $"�A�Z�b�g�̐����F{props["�A�Z�b�g�̐���"]["rich_text"][0]["text"]["content"]}\n" +
                  $"�J�^���O�t�@�C���F{props["�J�^���O�t�@�C��"]["files"][0]["name"]}");
            return props;
        }
        else
        {
            string assetsInfo = $"�Y������A�Z�b�g��{errements_Searched.Count()}�������I�ȉ��̒������ӂɍi���悤�Ɍ����������ĂˁB";
            foreach (var c in errements_Searched)
            {
                var props = c["properties"];
                assetsInfo += $"\n" +
                    $"�A�Z�b�g���F{props["�A�Z�b�g��"]["title"][0]["text"]["content"]}\n" +
                    $"�A�Z�b�g�̐����F{props["�A�Z�b�g�̐���"]["rich_text"][0]["text"]["content"]}\n" +
                    $"�J�^���O�t�@�C���F{props["�J�^���O�t�@�C��"]["files"][0]["name"]}\n";
            }
            Debug.Log(assetsInfo);
            return null;
        }
    }



    async UniTask<JToken> CallNotionAPI_QueryFile(string searchWord)
    {
        // �N�G�����\�z
        var queryPayload = new
        {
            filter = new
            {
                property = "�A�Z�b�g��",  // Notion��DB�̃v���p�e�B��
                title = new
                {
                    contains = searchWord // �������[�h���܂ނ��̂��t�B���^
                }
            }
        };

        // JSON�`���ɃV���A���C�Y
        string jsonQuery = JsonConvert.SerializeObject(queryPayload);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonQuery);

        using (UnityWebRequest request = new UnityWebRequest($"https://api.notion.com/v1/databases/{DatabaseID}/query", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {NotionAccessToken}");
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            request.SetRequestHeader("Notion-Version", "2022-02-22");

            await request.SendWebRequest();

            // �G���[�����i�ȗ��j
            switch (request.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("���N�G�X�g��");
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log("���N�G�X�g����");
                    break;

                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log(
                        @"�T�[�o�Ƃ̒ʐM�Ɏ��s�B
                        ���N�G�X�g���ڑ��ł��Ȃ������A
                        �Z�L�����e�B�ŕی삳�ꂽ�`���l�����m���ł��Ȃ������ȂǁB");
                    Debug.LogError(request.error);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(
                        @"�T�[�o���G���[������Ԃ����B
                        �T�[�o�Ƃ̒ʐM�ɂ͐����������A
                        �ڑ��v���g�R���Œ�`����Ă���G���[���󂯎�����B");
                    Debug.LogError(request.error);
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log(
                        @"�f�[�^�̏������ɃG���[�������B
                        ���N�G�X�g�̓T�[�o�Ƃ̒ʐM�ɐ����������A
                        ��M�����f�[�^�̏������ɃG���[�������B
                        �f�[�^���j�����Ă��邩�A�������`���ł͂Ȃ��ȂǁB");
                    Debug.LogError(request.error);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            string jsonStr = request.downloadHandler.text;

            // �p�[�X
            JObject responseObj = JObject.Parse(jsonStr);

            // �e�[�u���̃t�B���^�����O���ꂽ�v�f
            JArray elements = (JArray)responseObj["results"];
            Debug.Log(elements.Count);
            // elements �̒��g�����O�ɕ\��
            foreach (var element in elements)
            {
                Debug.Log(element.ToString());
                Debug.Log($"ID: {element["id"]}, Name: {element["properties"]["�A�Z�b�g��"]["title"][0]["text"]["content"]}");
            }

            return elements;
        }
    }
}