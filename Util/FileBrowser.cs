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
            Debug.Log($"�p�X�� Null�܂��͕�������");
            return null;
        }
        else
        {
            Debug.Log($"�p�X�͂��� : {paths[0]}");
            return paths[0];
        }
    }



    public static List<string> SelectFilePath_Multi(string extension, string title = "", string directory = "")
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
            Debug.Log($"�p�X�� Null�܂��͕�������");
            return null;
        }
        else
        {
            Debug.Log($"�p�X�͂��� : {paths[0]}");
            return paths.ToList();
        }
    }



    // �V�K�t�@�C����ۑ����邽�߂̃p�X��ݒ�
    public static string SetSavePath(string extension, string defaultName = "", string title = "", string directory = "")
    {
        //StandaloneFileBrowser���g���ăt�@�C����肽���t�@�C���̃p�X��ݒ�B���ۂɃt�@�C�����쐬����킯�ł͂Ȃ��炵���B
        string path = StandaloneFileBrowser.SaveFilePanel(title, directory, defaultName, extension);

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"�p�X�� Null�܂��͕�������");
            return null;
        }
        else
        {
            Debug.Log($"�p�X�͂��� : {path}");
            return path;
        }
    }



    // �V�K�t�@�C�����쐬���ۑ�
    public static string CreateSave(string extension, string defaultName = "", string title = "", string directory = "")
    {
        //StandaloneFileBrowser���g���ăt�@�C����肽���t�@�C���̃p�X��ݒ�B���ۂɃt�@�C�����쐬����킯�ł͂Ȃ��炵���B
        string path = StandaloneFileBrowser.SaveFilePanel(title, directory, defaultName, extension);

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"�p�X�� Null�܂��͕�������");
            return null;
        }
        else
        {
            Debug.Log($"�p�X�͂��� : {path}");
            // ��L�ō쐬�����p�X�̃t�@�C�������ۂɍ쐬
            File.Create(path);
            return path;
        }
    }
}
