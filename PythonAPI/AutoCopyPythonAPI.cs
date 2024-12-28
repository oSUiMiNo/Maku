# if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class AutoCopyPythonAPI : AssetPostprocessor
{
    private const string sourceFilePath = "Packages/jp.maku.maku_utillity/PythonAPI/PythonAssets/PyAPI.py";
    private const string destinationFolderName = "PythonAssets";
    private const string destinationFileName = "PyAPI.py";

    static AutoCopyPythonAPI()
    {
        CopyAsset();
    }

    //private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //{
    //    // �A�Z�b�g�̕ύX�Ɋւ�炸�A��ɃR�s�[�����݂�
    //    CopyAsset();
    //}

    private static void CopyAsset()
    {
        string projectPath = Application.dataPath.Replace("/Assets", "");
        string destinationFolderPath = Path.Combine(Application.dataPath, destinationFolderName);
        string destinationFilePath = Path.Combine(destinationFolderPath, destinationFileName);

        string fullSourcePath = Path.Combine(projectPath, sourceFilePath);

        if (!File.Exists(fullSourcePath))
        {
            Debug.LogError("�\�[�X�t�@�C����������܂���: " + fullSourcePath);
            return;
        }

        try
        {
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
                AssetDatabase.Refresh();
            }

            File.Copy(fullSourcePath, destinationFilePath, true); // true�ŏ㏑��������
            AssetDatabase.Refresh();
            Debug.Log("�t�@�C�����R�s�[���܂���: " + destinationFilePath);
        }
        catch (IOException e)
        {
            // �R�s�[�Ɏ��s�����ꍇ�ł��A�G���[���b�Z�[�W��\������݂̂ŏ����𑱍s
            Debug.LogWarning("�t�@�C���̃R�s�[�Ɏ��s���܂���: " + e.Message);
        }
    }
}
#endif