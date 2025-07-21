using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MyUtil;


[System.Serializable]
public class Player2 : SavableCompo
{
    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
    public override string SaveFolderPath { get; set; }

    #region データ
    [SerializeField] public string UserName = "Default Name";
    [SerializeField] public int UserRank = 0;
    [SerializeField] public Vector3 t = Vector3.zero;

    #endregion

    private void Awake()
    {
        SaveFolderPath = $"{Application.persistentDataPath}/PlayerData";
        Debug.Log($"{Application.persistentDataPath}");
    }

    private void Start()
    {
        //InputEventHandler.OnDown_F += LF;
        //InputEventHandler.OnDown_L += L;
        //InputEventHandler.OnDown_S += S;

        InputEventHandler.OnDown_S += () =>
        {
            Save();
        };
        InputEventHandler.OnDown_L += () =>
        {
            Load();
        };
        InputEventHandler.OnDown_C += () =>
        {
            Debug.Log("ランクの表示  ------------------------------------------");
            Debug.Log($"ユーザー名 : {UserName}");
            Debug.Log($"ユーザーランク : {UserRank}");
            Debug.Log("--------------------------------------------------------");
        };
        InputEventHandler.OnDown_X += () =>
        {
            Debug.Log("データ更新した。");
            UserName = "あちゃ";
            UserRank += 10;
        };
        InputEventHandler.OnDown_V += () =>
        {
            foreach (var a in Instances)
            {
                Debug.Log($"{a}");
            }
        };
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    Debug.Log("F");
        //    LoadFirst();
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Load();
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Debug.Log("ランクの表示  ------------------------------------------");
        //    Debug.Log($"ユーザー名 : {UserName}");
        //    Debug.Log($"ユーザーランク : {UserRank}");
        //    Debug.Log("--------------------------------------------------------");
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    Debug.Log("データ更新した。");
        //    UserName = "あちゃ";
        //    UserRank += 10;
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    foreach (var a in Instances)
        //    {
        //        Debug.Log($"{a}");
        //    }
        //}
    }

    //void LF()
    //{
    //    InputEventHandler.OnDown_F -= LF;
    //    LoadFirst();
    //}
    //void L()
    //{
    //    InputEventHandler.OnDown_L -= L;
    //    Load();
    //}
    //void S()
    //{
    //    InputEventHandler.OnDown_S -= S;
    //    Save();
    //}
}
