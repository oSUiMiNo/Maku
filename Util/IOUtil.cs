using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class IOUtil
{
    // データの削除や移動も実装したい


    // 後で実装して、CopyFolder() のファイルコピーの処理をこの関数に置き換える
    public static void CopyFile(string sourceFilePath, string destinationFolderPath, bool recursive = true)
    {

        //// ソースディレクトリの情報を取得
        //var dir = new DirectoryInfo(sourceFilePath);

        //// ソースディレクトpリが本当に存在するか確認
        //if (!dir.Exists)
        //    throw new DirectoryNotFoundException($"コピーしたいディレクトリが見つからなかった: {dir.FullName}");

        //// コピーを開始する前にディレクトリをキャッシュする
        //DirectoryInfo[] dirs = dir.GetDirectories();

        //// クローンの器としての空ディレクトリを作成
        //Directory.CreateDirectory(destinationFolderPath);

        //// コピー元ディレクトリのファイルを取得し、コピー先ディレクトリにコピーする。
        //foreach (FileInfo file in dir.GetFiles())
        //{
        //    string targetFilePath = Path.Combine(destinationFolderPath, file.Name);
        //    // 指定したパスのフォルダが既に存在する場合は無視
        //    if (File.Exists(targetFilePath))
        //    {
        //        Debug.LogWarning($"下記のファイルは既に存在するのでコピーしないどきます\n{targetFilePath}");
        //        return;
        //    }
        //    // コピー
        //    file.CopyTo(targetFilePath);
        //}

        //// 再帰的にサブディレクトリをコピーする場合は、再帰的にこのメソッドを呼び出す。
        //if (recursive)
        //{
        //    foreach (DirectoryInfo subDir in dirs)
        //    {
        //        string newDestinationDir = Path.Combine(destinationFolderPath, subDir.Name);
        //        CopyFolder(subDir.FullName, newDestinationDir, true);
        //    }
        //}
    }


    public static void CopyFolder(string sourceFolderPath, string destinationFolderPath, bool recursive = true)
    {
        // ソースディレクトリの情報を取得
        var dir = new DirectoryInfo(sourceFolderPath);

        // ソースディレクトリが本当に存在するか確認
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"コピーしたいディレクトリが見つからなかった: {dir.FullName}");

        // コピーを開始する前にディレクトリをキャッシュする
        DirectoryInfo[] dirs = dir.GetDirectories();

        // クローンの器としての空ディレクトリを作成
        Directory.CreateDirectory(destinationFolderPath);

        // コピー元ディレクトリのファイルを取得し、コピー先ディレクトリにコピーする。
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationFolderPath, file.Name);
            // 指定したパスのフォルダが既に存在する場合は無視
            if (File.Exists(targetFilePath))
            {
                Debug.LogWarning($"下記のファイルは既に存在するのでコピーしないどきます\n{targetFilePath}");
                return;
            }
            // コピー
            file.CopyTo(targetFilePath);
        }

        // 再帰的にサブディレクトリをコピーする場合は、再帰的にこのメソッドを呼び出す。
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationFolderPath, subDir.Name);
                CopyFolder(subDir.FullName, newDestinationDir, true);
            }
        }
    }


    // 指定したパスのものが入っている親フォルダを丸ごとコピー
    public static void CopyParentFolder(string sourcePath, string destinationFolderPath, bool recursive = true)
    {
        // パスのファイル（またはフォルダ）名を抜き取り
        string sourceName = Path.GetFileName(sourcePath);
        // 親フォルダのパスを抜き取り
        string parentFolderPath = sourcePath.Replace(@"\" + sourceName, "");
        // 親フォルダコピー
        CopyFolder(parentFolderPath, destinationFolderPath, recursive);
    }


    //===========================================
    // あるフォルダの内容をサブディレクトリ含めて別フォルダにコピー
    //===========================================
    public static void CopyAll(string sourceDir, string destDir, string[] ignoreFileName = null, string[] ignoreDirName = null)
    {
        if (ignoreFileName == null) ignoreFileName = new string[0];
        if (ignoreDirName == null) ignoreDirName = new string[0];

        // コピー先ディレクトリが無ければ作成
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        // 全ファイルをコピー
        foreach (string filePath in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(filePath);
            string destFilePath = $"{destDir}/{fileName}";

            // 無視対象は無視
            if (ignoreFileName.Contains(fileName))
            {
                Debug.Log($"無視：{filePath}");
                continue;
            }
            // コピー先に同名ディレクトリがあったら警告(困ったら機能追加する)
            if (Directory.Exists(destFilePath))
            {
                Debug.LogAssertion($"ファイルと同名のディレクトリがあった");
                continue;
            }
            // コピー実行(同名ファイルがあったら上書き)
            File.Copy(filePath, destFilePath, true);
        }

        // サブディレクトリを再帰的にコピー
        foreach (string dirPath in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dirPath);
            string destSubDir = $"{destDir}/{dirName}";

            // 無視対象は無視
            if (ignoreDirName.Contains(dirName))
            {
                Debug.Log($"無視：{dirPath}");
                continue;
            }
            // 再帰処理
            CopyAll(dirPath, destSubDir, ignoreFileName, ignoreDirName);
        }
        Debug.Log($"移動完了：{sourceDir}");
    }


    //===========================================
    // あるフォルダの内容をサブディレクトリ含めて別フォルダに移動
    //===========================================
    public static void MoveAll(string sourceDir, string destDir, string[] ignoreFileName = null, string[] ignoreDirName = null)
    {
        if (ignoreFileName == null) ignoreFileName = new string[0];
        if (ignoreDirName == null) ignoreDirName = new string[0];

        // 移動先ディレクトリが無ければ作成
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        // 全ファイルを移動
        foreach (string filePath in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(filePath);
            string destFilePath = $"{destDir}/{fileName}";

            // 無視対象は無視
            if (ignoreFileName.Contains(fileName))
            {
                Debug.Log($"無視：{filePath}");
                continue;
            }
            // 移動先に同名ディレクトリがあったら警告(困ったら機能追加する)
            if (Directory.Exists(destFilePath))
            {
                Debug.LogAssertion($"ファイルと同名のディレクトリがあった");
                continue;
            }
            // 移動先に同名ファイルがあったら削除
            if (File.Exists(destFilePath)) File.Delete(destFilePath);
            // 移動実行
            File.Move(filePath, destFilePath);
        }

        // サブディレクトリを再帰的に移動
        foreach (string dirPath in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dirPath);
            string destSubDir = $"{destDir}/{dirName}";

            // 無視対象は無視
            if (ignoreDirName.Contains(dirName))
            {
                Debug.Log($"無視：{dirPath}");
                continue;
            }
            // 再帰処理
            MoveAll(dirPath, destSubDir, ignoreFileName, ignoreDirName);
        }
        Debug.Log($"移動完了：{sourceDir}");
    }
}
