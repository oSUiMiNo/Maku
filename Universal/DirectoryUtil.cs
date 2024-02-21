using System.IO;
using UnityEngine;


public class DirectoryUtil
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
}
