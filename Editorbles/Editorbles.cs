using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;




public class EditorblesHandler : SingletonCompo<EditorblesHandler>
{
    public override bool IsActive { get; protected set; }
    = true;
    //= false;

    public static Editorbles data = Editorbles.Ins;
    public static List<EditorbleHeader> headers = new List<EditorbleHeader>();
    
  
    protected override void Awake0()
    {
        //Debug.Log($"パーシステント {Application.persistentDataPath}");
        //Debug.Log($"{data}");
        LoadFast();
    }


    public void LoadFast()
    {
        data.Load();
        List<string> removeList = new List<string>();
        foreach (var IDList in data.IDListDict.Values)
        {
            foreach (var id in IDList)
            {
                //Debug.Log($"{id}");
                if (!Directory.Exists($"{data.SaveFolderPath}/{id}"))
                {
                    Debug.Log($"ない　{id}");
                    removeList.Add(id);
                    //Remove(id);
                }
            }
        }
        foreach (var id in removeList)
        {
            data.Remove(id);
        }
    }


    public async void Delete(string id)
    {
        data.Remove(id);
        Directory.Delete(@$"{data.SaveFolderPath}/{id}", true);
        
        List<EditorbleHeader> newHeaders = new List<EditorbleHeader>();
        foreach (var header in headers)
        {
            //if (header.id == id) headers.Remove(header);
            if (header.id != id) newHeaders.Add(header);
            Debug.Log(header.id);
        }
        headers = newHeaders;

        //string address = TrimStr_R(id, "__", false);
        string address = id.TrimStr_R("__", false);
        string[] dirs = Directory.GetDirectories(@$"{data.SaveFolderPath}");
        Debug.Log(dirs.Count());
        for (int i = 0; i < dirs.Count(); i++)
        {
            Debug.Log(@$"{dirs[i]}");
        }
        for (int i = 0; i < dirs.Count(); i++)
        {
            //string newDirName = @$"{TrimStr_R(dirs[i], "__", false)}__{i}";
            string newDirName = @$"{dirs[i].TrimStr_R("__", false)}__{i}";
            Debug.Log($"新しいID0 {newDirName}");
            if (dirs[i] != newDirName) Directory.Move(@$"{dirs[i]}", newDirName);

            //string newID = @$"{TrimStr_R(data.IDListDict[address][i], "__", false)}__{i}";
            string newID = @$"{data.IDListDict[address][i].TrimStr_R("__", false)}__{i}";
            Debug.Log($"新しいID1 {newID}");
            data.IDListDict[address][i] = newID;
            headers[i].id = newID;
        }
    }
}



[System.Serializable]
public class Editorbles : SavableSingleton<Editorbles>
{
    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
    public override string SaveFolderPath { get; set; } = $"{Application.persistentDataPath}/Editorbles";
    public override string FileName { get; set; } = $"Editorbles";

    // keyがアドレスで vlueがID
    //[JsonProperty] public static Dictionary<string, List<string>>  IDListDict = new Dictionary<string, List<string>>();
    [JsonProperty] public Dictionary<string, List<string>> IDListDict = new Dictionary<string, List<string>>();
    // [JsonProperty] public static List<string> IDList = new List<string>();


    //public void LoadFast()
    //{
    //    Load();
    //    List<string> removeList = new List<string>();
    //    foreach (var IDList in IDListDict.Values)
    //    {
    //        foreach (var id in IDList)
    //        {
    //            Debug.Log($"{id}");
    //            if (!Directory.Exists($"{SaveFolderPath}/{id}"))
    //            {
    //                Debug.Log($"ない　{id}");
    //                removeList.Add(id);
    //                //Remove(id);
    //            }
    //        }
    //    }
    //    foreach(var id in removeList)
    //    {
    //        Remove(id);
    //    }
    //}

    public void Remove(string id)
    {
        foreach (var key in IDListDict.Keys)
        {
            if (IDListDict[key].Contains(id)) IDListDict[key].Remove(id);
            if (IDListDict[key].Count == 0) IDListDict.Remove(key);
        }
        Save();
    }

    //public void Delete(string id)
    //{
    //    Remove(id);
    //    Directory.Delete(@$"{SaveFolderPath}/{id}", true);
    //    string address = TrimStr_R(id, "__", false);
    //    string[] dirs = Directory.GetDirectories(@$"{SaveFolderPath}");
    //    for(int i = 0; i < dirs.Count(); i++)
    //    {
    //        string newID = @$"{TrimStr_R(dirs[i], "__", false)}__{i}";
    //        Directory.Move(@$"{dirs[i]}", newID);
    //        IDListDict[address][i] = newID;
    //    }
    //}

    //static string GetKeyFromValue(Dictionary<string, List<string>> dictionary, int value)
    //{
    //    return dictionary.FirstOrDefault(kvp => kvp.Value == value).Key;
    //}
}
