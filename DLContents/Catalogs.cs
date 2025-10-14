using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using System.IO;
using Maku;


public class NagesenContentsHandler : SingletonCompo<NagesenContentsHandler>
{
    //public static Catalogs Data = new Catalogs();
    public static Catalogs Data = new Catalogs()
    {
        SaveFolderPath = @$"{Application.persistentDataPath}/NagesenContents"
    };

    public override bool IsActive { get; protected set; } = true;


    protected override void Awake0()
    {
        Data.LoadFast();
    }
}

public class DLContentsHandler : SingletonCompo<DLContentsHandler>
{
    //public static Catalogs Data = new Catalogs();
    public static Catalogs Data = new Catalogs()
    {
        SaveFolderPath = @$"{Application.persistentDataPath}/DLContents"
    };

    public override bool IsActive { get; protected set; } = true;


    protected override void Awake0()
    {
        Data.LoadFast();
    }
}





[System.Serializable]
public class Catalogs : Savable//Singleton<DLContents>
{
    #region ====== Savable の仕込み ================================================
    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
    // DBのjsonを保存するパス
    //public override string SaveFolderPath { get; set; } = @$"{Application.persistentDataPath}/DLContents";
    #endregion =======================================

    [JsonProperty] public Dictionary<string, Catalog> ContentsCatalogs = new Dictionary<string, Catalog>();

    public void LoadFast()
    {
        Load();
        List<string> removeList = new List<string>();
        foreach (var contentsName in ContentsCatalogs.Keys)
        {
            if (!Directory.Exists($"{SaveFolderPath}/{contentsName}"))
            {
                Debug.Log($"ない　{contentsName}");
                removeList.Add(contentsName);
            }
        }
        foreach (var contentsName in removeList)
        {
            Remove(contentsName);
        }
    }

    public void Remove(string id)
    {
        // 何故か {Application.persistentDataPath}/com.unity.addressables にhashとcatalogが複製されるのでそれも削除
        if (Directory.Exists($"{Application.persistentDataPath}/com.unity.addressables"))
        {
            Debug.Log($"削除0");
            // ファイル一覧を取得
            string[] hashFiles = Directory.GetFiles($"{Application.persistentDataPath}/com.unity.addressables", "*.hash");
            Debug.Log("Files:");
            foreach (string hashFile in hashFiles)
            {
                Debug.Log($"削除1");
                string hash = File.ReadAllText(hashFile);
                if (hash == ContentsCatalogs[id].Hash)
                {
                    Debug.Log($"削除2");
                    string catalogFile = hashFile.Replace(".hash", ".json");
                    File.Delete(hashFile);
                    File.Delete(catalogFile);
                }
            }
        }
        else
        {
            Debug.LogError($"このフォルダはない {Application.persistentDataPath}/com.unity.addressables");
        }

        ContentsCatalogs.Remove(id);
        Save();
    }
}

[System.Serializable]
public class Catalog
{
    [JsonProperty] public string Hash;
    [JsonProperty] public string Path_Catalog;
    [JsonProperty] public List<string> Paths_Group = new List<string>();
    [JsonProperty] public Dictionary<string, Addresses> Labels = new Dictionary<string, Addresses>();
}

[System.Serializable]
public class Addresses
{
    [JsonProperty] public List<string> BeforeDL = new List<string>();
    [JsonProperty] public List<string> AfterDL = new List<string>();
}