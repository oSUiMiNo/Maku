using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 生成したスクリプタブルオブジェクトはフォルダResourcesに配置する
[CreateAssetMenu(fileName = "NotionAssetTable")]
public class NotionAssetTable : ScriptableObject
{
    #region == インスタンスの作成===============================================
    // この名前のスクリプタブルオブジェクトが
    // フォルダResources に配置されてないといけない。
    private static readonly string RESOURCE_PATH = "NotionAssetTable";

    private static NotionAssetTable ins = null;
    public static NotionAssetTable Ins
    {
        get
        {
            if (ins == null)
            {
                NotionAssetTable asset = Resources.Load(RESOURCE_PATH) as NotionAssetTable;
                if (asset == null)
                {
                    // アセットが指定のパスに無い。
                    // 誰かが勝手に移動させたか、消しやがったな！
                    Debug.AssertFormat(false, $"{RESOURCE_PATH} ってやつが Resources に無かった");
                    asset = CreateInstance<NotionAssetTable>();
                }

                ins = asset;
            }

            return ins;
        }
    }
    #endregion ==============================

    // ここに、Notion から取りたいAsset名の一覧を書いておく
    [SerializeField]
    public List<string> assetNames;
}
