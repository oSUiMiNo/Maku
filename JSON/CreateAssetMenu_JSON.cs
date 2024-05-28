using System;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 新規テキストファイルを作成する
/// EditorMenuスクリプトです
/// 
/// 使い方：
///   Project View上で右クリック → Create/TextFile をクリック
///   選択している階層にテキストファイルが生成されます
/// </summary>
public class CreateAssetMenu_JSON
{
    /// <summary>
    /// Projectビュー上で選択している階層に新規JSONファイルを作成します
    /// </summary>
    [MenuItem("Assets/Create/JSON", false, 1)]
    public static void CreateTextFile()
    {
        var path = Application.dataPath;
        var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        // 保存フォルダのパスを整える
        if (selectedPath.Length != 0)
        {
            if (!IsFolder(selectedPath))
            {
                selectedPath = selectedPath.Substring(0, selectedPath.LastIndexOf("/", StringComparison.CurrentCulture));
            }
            path = path.Remove(path.Length - "Assets".Length, "Assets".Length);
            path += selectedPath;
        }

        var fileName = "NewJSON.json";
        path += "/" + fileName;
        int cnt = 0;

        // 同じパスのファイルが既に存在したら、パスの最後に番号を追記する。
        // それも存在したら番号を１増やしてリトライ。ファイル名がかぶらなくなるまで繰り返す。
        while (File.Exists(path))
        {
            if (path.Contains(fileName))
            {
                cnt++;
                var newFileName = "NewJSON" + cnt + ".json";
                path = path.Replace(fileName, newFileName);
                fileName = newFileName;
            }
            else
            {
                Debug.LogError("path dont contain " + fileName);
                break;
            }
        }

        // 空のテキストを書き込む.
        File.WriteAllText(path, "", System.Text.Encoding.UTF8);
        AssetDatabase.Refresh();
    }


    // 指定パスがフォルダかどうかチェック
    public static bool IsFolder(string path)
    {
        try
        {
            return File.GetAttributes(path).Equals(FileAttributes.Directory);
        }
        catch (Exception ex)
        {
            if (ex.GetType() == typeof(FileNotFoundException))
            {
                return false;
            }
            throw ex;
        }
    }
}