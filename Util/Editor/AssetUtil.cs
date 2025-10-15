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
    /// �w�肵���t�H���_�iAssets�����̃t�H���_�Ȃǁj�̒����ɂ���A�Z�b�g�����擾
    /// �T�u�t�H���_���̃A�Z�b�g�̓J�E���g����Ȃ�
    ///</summary>=============================================
    public static int CountInDir(string dir)
    {
        // �w��t�H���_�z���̑S�A�Z�b�g�������i�T�u�t�H���_�܂ށj
        string[] guids = AssetDatabase.FindAssets("", new[] { dir });
        int count = 0;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // �擾�����A�Z�b�g���w��t�H���_�̒����ɂ��邩�ǂ������`�F�b�N
            // �����̏ꍇ�AassetPath�� "Assets/YourFolder/�t�@�C����" �̂悤�ɂȂ��Ă���͂��ł�
            string directory = Path.GetDirectoryName(assetPath);
            if (directory.Replace("\\", "/") == dir)
            {
                count++;
            }
        }
        return count;
    }


    ///==============================================<summary>
    /// �w��t�H���_���̃A�Z�b�g��S�擾 
    ///</summary>=============================================
    public static List<Object> GetAssetsInDir(string dir)
    {
        var assets = new List<Object>();
        // �w��t�H���_�ȉ��i�T�u�t�H���_�܂ށj�̂��ׂĂ�GUID������
        var guids = AssetDatabase.FindAssets("", new[] { dir });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) continue;

            // ---- �t�H���_�̓X�L�b�v���� ----
            if (AssetDatabase.IsValidFolder(path))
            {
                // path ���t�H���_�Ȃ� continue
                continue;
            }

            // ���ۂ̃A�Z�b�g�����[�h
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            if (asset == null) continue;

            assets.Add(asset);
        }
        return assets;
    }


    ///==============================================<summary>
    /// ��΃p�X���� "Assets/..." �̑��΃p�X���擾����
    /// �i�v���W�F�N�g�t�H���_���ɂ���t�@�C����p�j
    ///</summary>=============================================
    public static string FullPathToProjectPath(this string fileFullPath)
    {
        // �v���W�F�N�g�̃��[�g�t�H���_�p�X (��: C:/xxx/Unity/MyProject)
        string projectRootPath = Path.GetFullPath(Application.dataPath + "/..");

        // OS�ˑ��̃f�B���N�g���Z�p���[�^�� '/' �ɓ���
        projectRootPath = projectRootPath.Replace("\\", "/");
        fileFullPath = fileFullPath.Replace("\\", "/");

        // �v���W�F�N�g�̊O�Ȃ� null (�܂��͋󕶎���Ȃ�) ��Ԃ�
        if (!fileFullPath.StartsWith(projectRootPath))
        {
            Debug.LogError($"[AbsoluteToAssetsRelativePath] �v���W�F�N�g�O�̃p�X�ł�: {fileFullPath}");
            return null;
        }

        // ���[�g����������āA�擪�� "Assets" �ɍ��킹��
        // ��: C:/.../MyProject/Assets/SomeFolder/MyPrefab.prefab -> Assets/SomeFolder/MyPrefab.prefab
        string relativePath = fileFullPath.Substring(projectRootPath.Length + 1);

        return relativePath;
    }
}
#endif