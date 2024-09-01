using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

// �f�[�^�x�[�X�͂���Notion�y�[�W�ɂ���B
// https://www.notion.so/6795dd70753a495897cb85e0abce95fe?pvs=4


public class DownloadCatalogFromNotion : MonoBehaviour
{
    private const string NotionAccessToken = "secret_OIxSWO69mxnD9FNbmL2US0pcsLWCUmsaglBZBCWPWrC";
    private const string DatabaseID = "e8777c71cf694fa8bd135fb1d7cb1728";


    [SerializeField]
    public string assetName;


    void Start()
    {
        InputEventHandler.OnDown_D += Execute;
    }



    async void Execute()
    {
        if (!string.IsNullOrEmpty(assetName)) await CallNotionAPI_DowiloadFile(await CallNotionAPI_SearchFile(assetName));
        else
        {
            Debug.Log(NotionAssetTable.Ins == null);
            foreach (var a in NotionAssetTable.Ins.assetNames)
            {
                await CallNotionAPI_DowiloadFile(await CallNotionAPI_SearchFile(a));
                Debug.Log(a);
            }
        }
    }



    async UniTask CallNotionAPI_DowiloadFile(JToken contentObj)
    {
        if (contentObj == null) return;

        // �f�[�^�x�[�X����擾�����_�E�����[�h�����N
        string downloadURL = contentObj["�J�^���O�t�@�C��"]["files"][0]["file"]["url"].ToString();

        // �f�[�^�x�[�X����擾�����t�@�C����
        string fileName = contentObj["�J�^���O�t�@�C��"]["files"][0]["name"].ToString();

        // �V�K�쐬����t�@�C���̃p�X�Ɩ��O
        string newFilePath = @$"C:\Users\osuim\Downloads\{fileName}";

        //�t�@�C���_�E�����[�h
        using (UnityWebRequest request = new UnityWebRequest(downloadURL))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            await request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                // json�Ƃ��̃e�L�X�g�t�@�C���Ȃ炻�̂܂ܓ��邱�Ƃ��\
                Debug.Log(request.downloadHandler.text); //�J�^���O�̒��g

                // �e�L�X�g�t�@�C���ȊO���_�E�����[�h�������̂Ńo�C�i���f�[�^���畜����������ɂ�����
                byte[] results = request.downloadHandler.data;

                // �o�C�g����t�@�C������
                using (FileStream fs = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    // �V�K�t�@�C���Ƀo�C�g�z���0�����ڂ���Ō�܂ŏ�������
                    fs.Write(results, 0, results.Length);
                }
            }
        }
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
        // ���[�U�[�̓l�N�X�g�A�N�V�����Ƃ��āA�ꗗ�̒������ӂɍi�荞�߂錟�����[�h����肵�ʓx�����B
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
}


