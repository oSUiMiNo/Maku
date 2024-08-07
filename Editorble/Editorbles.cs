using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[System.Serializable]
public class Editorbles : SavableSingleton<Editorbles>
{
    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
    public override string SaveFolderPath { get; set; } = $"{Application.persistentDataPath}/Editorbles";
    public override string FileName { get; set; } = $"Editorbles";

    [JsonProperty] public static Dictionary<string, List<string>>  IDListDict = new Dictionary<string, List<string>>();
    //[JsonProperty] public static List<string> IDList = new List<string>();


    public void LoadFast()
    {
        Load();
        List<string> removeList = new List<string>();
        foreach (var IDList in IDListDict.Values)
        {
            foreach (var id in IDList)
            {
                Debug.Log($"{id}");
                if (!Directory.Exists($"{SaveFolderPath}/{id}"))
                {
                    Debug.Log($"Ç»Ç¢Å@{id}");
                    removeList.Add(id);
                    //Remove(id);
                }
            }
        }
        foreach(var id in removeList)
        {
            Remove(id);
        }
    }

    public void Remove(string id)
    {
        IDListDict.Remove(id);
        Save();
    }
}

public class EditorblesHandler : SingletonCompo<EditorblesHandler>
{
    public override bool IsActive { get; protected set; }
    = true;
    //= false;

    protected override void Awake0()
    {
        Debug.Log($"{Editorbles.Ins}");
        Editorbles.Ins.LoadFast();
    }
}
