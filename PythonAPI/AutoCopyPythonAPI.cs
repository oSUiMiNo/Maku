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
    //    // アセットの変更に関わらず、常にコピーを試みる
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
            Debug.LogError("ソースファイルが見つかりません: " + fullSourcePath);
            return;
        }

        try
        {
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
                AssetDatabase.Refresh();
            }

            File.Copy(fullSourcePath, destinationFilePath, true); // trueで上書きを許可
            AssetDatabase.Refresh();
            Debug.Log("ファイルをコピーしました: " + destinationFilePath);
        }
        catch (IOException e)
        {
            // コピーに失敗した場合でも、エラーメッセージを表示するのみで処理を続行
            Debug.LogWarning("ファイルのコピーに失敗しました: " + e.Message);
        }
    }
}
#endif