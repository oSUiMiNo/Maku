using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player1 : SingletonCompo<Player1>
{
    public override bool IsActive { get; protected set; } = false;

    [System.Serializable]
    class PlayerData : Savable
    {
        public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
        public static List<SaveSystem.IFriendWith_SaveSystem> instances = new();

        #region データ
        public string UserName = "Default Name";
        public int UserRank = 0;
        #endregion
    }



    readonly PlayerData p0 = new();
    readonly PlayerData p1 = new();


    protected override void Start()
    {
        InputEventHandler.OnKeyDown_S += () =>
        {
            p0.Save();
            p1.Save();
        };
        InputEventHandler.OnKeyDown_L += () =>
        {
            p0.Load();
            p1.Load();
        };
        InputEventHandler.OnKeyDown_C += () =>
        {
            Debug.Log("ランクの表示 0 -----------------------------------------");
            Debug.Log($"ユーザー名 : {p0.UserName}");
            Debug.Log($"ユーザーランク : {p0.UserRank}");
            Debug.Log("--------------------------------------------------------");

            Debug.Log("ランクの表示 1 -----------------------------------------");
            Debug.Log($"ユーザー名 : {p1.UserName}");
            Debug.Log($"ユーザーランク : {p1.UserRank}");
            Debug.Log("--------------------------------------------------------");
        };
        InputEventHandler.OnKeyDown_X += () =>
        {
            Debug.Log("データ更新した。");
            p0.UserName = "あちゃ";
            p0.UserRank += 10;
        };
        InputEventHandler.OnKeyDown_V += () =>
        {
            foreach (var a in PlayerData.instances)
            {
                Debug.Log($"{a}");
            }
        };
    //protected override void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        p0.Save();
    //        p1.Save();
    //    }
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        p0.Load();
    //        p1.Load();
    //    }
    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        Debug.Log("ランクの表示 0 -----------------------------------------");
    //        Debug.Log($"ユーザー名 : {p0.UserName}");
    //        Debug.Log($"ユーザーランク : {p0.UserRank}");
    //        Debug.Log("--------------------------------------------------------");

    //        Debug.Log("ランクの表示 1 -----------------------------------------");
    //        Debug.Log($"ユーザー名 : {p1.UserName}");
    //        Debug.Log($"ユーザーランク : {p1.UserRank}");
    //        Debug.Log("--------------------------------------------------------");
    //    }
    //    if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        Debug.Log("データ更新した。");
    //        p0.UserName = "あちゃ";
    //        p0.UserRank += 10;
    //    }
    //    if (Input.GetKeyDown(KeyCode.V))
    //    {
    //        foreach (var a in PlayerData.instances)
    //        {
    //            Debug.Log($"{a}");
    //        }
    //    }
    //}
    }
}
