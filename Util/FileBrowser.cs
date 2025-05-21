using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileBrowser
{
    public static string SelectFilePath(string extension, string title = "", string directory = "")
    {
        //StandaloneFileBrowserを使ってパスの取得
        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, directory, extension, false);

        if (paths.Length == 0)
        {
            Debug.Log($"パス入っていない");
            return null;
        }
        else
        if (string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log($"パスが Nullまたは文字無し");
            return null;
        }
        else
        {
            Debug.Log($"パスはこれ : {paths[0]}");
            return paths[0];
        }
    }



    public static List<string> SelectFilePath_Multi(string extension, string title = "", string directory = "")
    {
        //StandaloneFileBrowserを使ってパスの取得
        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, directory, extension, false);

        if (paths.Length == 0)
        {
            Debug.Log($"パス入っていない");
            return null;
        }
        else
        if (string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log($"パスが Nullまたは文字無し");
            return null;
        }
        else
        {
            Debug.Log($"パスはこれ : {paths[0]}");
            return paths.ToList();
        }
    }



    // 新規ファイルを保存するためのパスを設定
    public static string SetSavePath(string extension, string defaultName = "", string title = "", string directory = "")
    {
        //StandaloneFileBrowserを使ってファイル作りたいファイルのパスを設定。実際にファイルを作成するわけではないらしい。
        string path = StandaloneFileBrowser.SaveFilePanel(title, directory, defaultName, extension);

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"パスが Nullまたは文字無し");
            return null;
        }
        else
        {
            Debug.Log($"パスはこれ : {path}");
            return path;
        }
    }



    // 新規ファイルを作成し保存
    public static string CreateSave(string extension, string defaultName = "", string title = "", string directory = "")
    {
        //StandaloneFileBrowserを使ってファイル作りたいファイルのパスを設定。実際にファイルを作成するわけではないらしい。
        string path = StandaloneFileBrowser.SaveFilePanel(title, directory, defaultName, extension);

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"パスが Nullまたは文字無し");
            return null;
        }
        else
        {
            Debug.Log($"パスはこれ : {path}");
            // 上記で作成したパスのファイルを実際に作成
            File.Create(path);
            return path;
        }
    }
}
