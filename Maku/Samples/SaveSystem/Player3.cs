using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Player3 : SavableSingletonCompo<Player3>
//{
//    public override bool IsActive { get; protected set; } = false;

//    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
//    public static List<SaveSystem.IFriendWith_SaveSystem> instances = new();

//    #region データ
//    public string UserName = "Default Name";
//    public int UserRank = 0;
//    #endregion

//    protected override void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.S))
//        {
//            Save();
//        }
//        if (Input.GetKeyDown(KeyCode.L))
//        {
//            Load();
//        }
//        if (Input.GetKeyDown(KeyCode.C))
//        {
//            Debug.Log("ランクの表示  ------------------------------------------");
//            Debug.Log($"ユーザー名 : {UserName}");
//            Debug.Log($"ユーザーランク : {UserRank}");
//            Debug.Log("--------------------------------------------------------");
//        }
//        if (Input.GetKeyDown(KeyCode.X))
//        {
//            Debug.Log("データ更新した。");
//            UserName = "あちゃ";
//            UserRank += 10;
//        }
//        if (Input.GetKeyDown(KeyCode.V))
//        {
//            foreach (var a in instances)
//            {
//                Debug.Log($"{a}");
//            }
//        }
//    }
//}
