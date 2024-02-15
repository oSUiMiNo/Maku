using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FileBrowser
{
    public static string SelectFilePath(string title, string directory, string extension)
    {
        //StandaloneFileBrowser���g���ăp�X�̎擾
        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, directory, extension, false);

        if (paths.Length == 0)
        {
            Debug.Log($"�p�X�����Ă��Ȃ�");
            return null;
        }
        else
        if (string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log($"�p�X�� Null");
            return null;
        }
        else
        {
            Debug.Log($"�p�X�͂��� : {paths[0]}");
            return paths[0];
        }
    }

    public static List<string> SelectFilePath_Multi(string title, string directory, string extension)
    {
        //StandaloneFileBrowser���g���ăp�X�̎擾
        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, directory, extension, false);

        if (paths.Length == 0)
        {
            Debug.Log($"�p�X�����Ă��Ȃ�");
            return null;
        }
        else
        if (string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log($"�p�X�� Null");
            return null;
        }
        else
        {
            Debug.Log($"�p�X�͂��� : {paths[0]}");
            return paths.ToList();
        }
    }
}
