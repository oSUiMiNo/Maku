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

// データベースはこのNotionページにある。
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

        // データベースから取得したダウンロードリンク
        string downloadURL = contentObj["カタログファイル"]["files"][0]["file"]["url"].ToString();

        // データベースから取得したファイル名
        string fileName = contentObj["カタログファイル"]["files"][0]["name"].ToString();

        // 新規作成するファイルのパスと名前
        string newFilePath = @$"C:\Users\osuim\Downloads\{fileName}";

        //ファイルダウンロード
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
                // jsonとかのテキストファイルならそのまま得ることも可能
                Debug.Log(request.downloadHandler.text); //カタログの中身

                // テキストファイル以外もダウンロードしたいのでバイナリデータから復元する方式にしたい
                byte[] results = request.downloadHandler.data;

                // バイトからファイル復元
                using (FileStream fs = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    // 新規ファイルにバイト配列の0文字目から最後まで書き込み
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

            // エラー処理
            switch (request.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Debug.Log("リクエスト中");
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log("リクエスト成功");
                    break;

                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log(
                        @"サーバとの通信に失敗。
                        リクエストが接続できなかった、
                        セキュリティで保護されたチャネルを確立できなかったなど。");
                    Debug.LogError(request.error);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(
                        @"サーバがエラー応答を返した。
                        サーバとの通信には成功したが、
                        接続プロトコルで定義されているエラーを受け取った。");
                    Debug.LogError(request.error);
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log(
                        @"データの処理中にエラーが発生。
                        リクエストはサーバとの通信に成功したが、
                        受信したデータの処理中にエラーが発生。
                        データが破損しているか、正しい形式ではないなど。");
                    Debug.LogError(request.error);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            jsonStr = request.downloadHandler.text;
        }

        // パース
        JObject responseObj = JObject.Parse(jsonStr);

        // テーブルの全要素
        JArray elements = (JArray)responseObj["results"];
        
        // 検索
        List<JToken> errements_Searched = elements.Where(b => new Regex(searchWord).IsMatch(b.ToString())).ToList();

        // 検索の結果一意に絞れている場合はその要素たち(Notion DB のとある一行)を返す。
        // 該当が複数あった場合はその一覧を示す。
        // ユーザーはネクストアクションとして、一覧の中から一意に絞り込める検索ワードを特定し彩度検索。
        if(errements_Searched.Count() == 1)
        {
            var props = errements_Searched[0]["properties"];
            Debug.Log(
                  $"アセット名：{props["アセット名"]["title"][0]["text"]["content"]}\n" +
                  $"アセットの説明：{props["アセットの説明"]["rich_text"][0]["text"]["content"]}\n" +
                  $"カタログファイル：{props["カタログファイル"]["files"][0]["name"]}");
            return props;
        }
        else
        {
            string assetsInfo = $"該当するアセットが{errements_Searched.Count()}個あった！以下の中から一意に絞れるように検索し直してね。";
            foreach (var c in errements_Searched)
            {
                var props = c["properties"];
                assetsInfo += $"\n" +
                    $"アセット名：{props["アセット名"]["title"][0]["text"]["content"]}\n" +
                    $"アセットの説明：{props["アセットの説明"]["rich_text"][0]["text"]["content"]}\n" +
                    $"カタログファイル：{props["カタログファイル"]["files"][0]["name"]}\n";
            }
            Debug.Log(assetsInfo);
            return null;
        }
    }
}


