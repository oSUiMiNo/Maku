using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileBrowser
{
    public static string SelectFilePath(string title, string directory, string extension)
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
            Debug.Log($"パスが Null");
            return null;
        }
        else
        {
            Debug.Log($"パスはこれ : {paths[0]}");
            return paths[0];
        }
    }

    public static List<string> SelectFilePath_Multi(string title, string directory, string extension)
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
            Debug.Log($"パスが Null");
            return null;
        }
        else
        {
            Debug.Log($"パスはこれ : {paths[0]}");
            return paths.ToList();
        }
    }
}
