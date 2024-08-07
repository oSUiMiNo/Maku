using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using System.IO;



public class CatalogDataHandler : SingletonCompo<CatalogDataHandler>
{
    public static CatalogData Data = new CatalogData();
    public override bool IsActive { get; protected set; } = true;


    protected override void Awake0()
    {
        Data.LoadFast();
    }
}



[System.Serializable]
public class CatalogData : Savable
{
    #region ====== Savable の仕込み ================================================
    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
    // DBのjsonを保存するパス
    public override string SaveFolderPath { get; set; } = @$"{Application.persistentDataPath}/DLContents";
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
        ContentsCatalogs.Remove(id);
        Save();
    }
}

[System.Serializable]
public class Catalog
{
    //public Catalog(string path_Catalog, List<string> paths_Group, string labels, List<string> afterDL)
    //{
    //    Path_Catalog = path_Catalog;
    //    Paths_Group = paths_Group;
    //    _labels = new Dictionary<string, Addresses>();
    //    Addresses addresses = new Addresses(afterDL, new List<string>() { "a", "b" });
    //    Debug.Log($"{labels}");
    //    Debug.Log($"{addresses.AfterDL[0]}");
    //    Debug.Log($"{addresses.BeforeDL[0]}");
    //    _labels.Add(labels, addresses);
    //}
    [JsonProperty] public string Path_Catalog;
    [JsonProperty] public List<string> Paths_Group = new List<string>();
    [JsonProperty] public Dictionary<string, Addresses> Labels = new Dictionary<string, Addresses>();
}

[System.Serializable]
public class Addresses
{
    //public Addresses(List<string> beforeDL, List<string> afterDL) 
    //{
    //    BeforeDL = beforeDL;
    //    AfterDL = afterDL;
    //}
    [JsonProperty] public List<string> BeforeDL = new List<string>();
    [JsonProperty] public List<string> AfterDL = new List<string>();
}