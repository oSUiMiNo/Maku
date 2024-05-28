using System;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// �V�K�e�L�X�g�t�@�C�����쐬����
/// EditorMenu�X�N���v�g�ł�
/// 
/// �g�����F
///   Project View��ŉE�N���b�N �� Create/TextFile ���N���b�N
///   �I�����Ă���K�w�Ƀe�L�X�g�t�@�C������������܂�
/// </summary>
public class CreateAssetMenu_JSON
{
    /// <summary>
    /// Project�r���[��őI�����Ă���K�w�ɐV�KJSON�t�@�C�����쐬���܂�
    /// </summary>
    [MenuItem("Assets/Create/JSON", false, 1)]
    public static void CreateTextFile()
    {
        var path = Application.dataPath;
        var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        // �ۑ��t�H���_�̃p�X�𐮂���
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

        // �����p�X�̃t�@�C�������ɑ��݂�����A�p�X�̍Ō�ɔԍ���ǋL����B
        // ��������݂�����ԍ����P���₵�ă��g���C�B�t�@�C���������Ԃ�Ȃ��Ȃ�܂ŌJ��Ԃ��B
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

        // ��̃e�L�X�g����������.
        File.WriteAllText(path, "", System.Text.Encoding.UTF8);
        AssetDatabase.Refresh();
    }


    // �w��p�X���t�H���_���ǂ����`�F�b�N
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