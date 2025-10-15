#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;


///*******************************************************<summary>
/// 
///</summary>******************************************************
public static class AssetUtil
{
    ///==============================================<summary>
    /// 指定したフォルダ（Assets直下のフォルダなど）の直下にあるアセット数を取得
    /// サブフォルダ内のアセットはカウントされない
    ///</summary>=============================================
    public static int CountInDir(string dir)
    {
        // 指定フォルダ配下の全アセットを検索（サブフォルダ含む）
        string[] guids = AssetDatabase.FindAssets("", new[] { dir });
        int count = 0;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 取得したアセットが指定フォルダの直下にあるかどうかをチェック
            // 直下の場合、assetPathは "Assets/YourFolder/ファイル名" のようになっているはずです
            string directory = Path.GetDirectoryName(assetPath);
            if (directory.Replace("\\", "/") == dir)
            {
                count++;
            }
        }
        return count;
    }


    ///==============================================<summary>
    /// 指定フォルダ内のアセットを全取得 
    ///</summary>=============================================
    public static List<Object> GetAssetsInDir(string dir)
    {
        var assets = new List<Object>();
        // 指定フォルダ以下（サブフォルダ含む）のすべてのGUIDを検索
        var guids = AssetDatabase.FindAssets("", new[] { dir });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) continue;

            // ---- フォルダはスキップする ----
            if (AssetDatabase.IsValidFolder(path))
            {
                // path がフォルダなら continue
                continue;
            }

            // 実際のアセットをロード
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            if (asset == null) continue;

            assets.Add(asset);
        }
        return assets;
    }


    ///==============================================<summary>
    /// 絶対パスから "Assets/..." の相対パスを取得する
    /// （プロジェクトフォルダ内にあるファイル専用）
    ///</summary>=============================================
    public static string FullPathToProjectPath(this string fileFullPath)
    {
        // プロジェクトのルートフォルダパス (例: C:/xxx/Unity/MyProject)
        string projectRootPath = Path.GetFullPath(Application.dataPath + "/..");

        // OS依存のディレクトリセパレータを '/' に統一
        projectRootPath = projectRootPath.Replace("\\", "/");
        fileFullPath = fileFullPath.Replace("\\", "/");

        // プロジェクトの外なら null (または空文字列など) を返す
        if (!fileFullPath.StartsWith(projectRootPath))
        {
            Debug.LogError($"[AbsoluteToAssetsRelativePath] プロジェクト外のパスです: {fileFullPath}");
            return null;
        }

        // ルート部分を削って、先頭を "Assets" に合わせる
        // 例: C:/.../MyProject/Assets/SomeFolder/MyPrefab.prefab -> Assets/SomeFolder/MyPrefab.prefab
        string relativePath = fileFullPath.Substring(projectRootPath.Length + 1);

        return relativePath;
    }
}
#endif