using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Player1 : SingletonCompo<Player1>
{
    public override bool IsActive { get; protected set; } = false;

    [System.Serializable]
    class PlayerData : Savable
    {
        [JsonIgnore] // Instances が循環参照になっているので[JsonIgnore]をつけてシリアライズされないようにする
        public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
        public static List<SaveSystem.IFriendWith_SaveSystem> instances = new();


        #region データ
        public string UserName = "Default Name";
        public int UserRank = 0;
        public List<string> Items = new List<string>() { "薬草", "聖水" };
        public List<Friend> Friends = new List<Friend>();
        [JsonProperty] // スタティックなデータをシリアアライズしたい場合は[JsonProperty]をつける
        public static string TestStatic = "スタティックデータのテスト";
        #endregion    
    }
    public class Friend
    {
        public string Name = "Default Name";
        public int Rank = 0;
        public Friend(string name, int rank)
        {
            Name = name;
            Rank = rank;
        }
    }




    readonly PlayerData p0 = new();
    readonly PlayerData p1 = new();


    protected override void Start()
    {
        InputEventHandler.OnDown_S += () =>
        {
            p0.Save();
            p1.Save();
        };
        InputEventHandler.OnDown_L += () =>
        {
            p0.Load();
            p1.Load();
        };
        InputEventHandler.OnDown_C += () =>
        {
            Debug.Log("--------- プレイヤー0 の情報 --------------------------------");
            Debug.Log($"ユーザー名 : {p0.UserName}");
            Debug.Log($"ユーザーランク : {p0.UserRank}");
            string items0 = string.Empty;
            p0.Items.ForEach(a => items0 += $"{a} ");
            Debug.Log($"アイテム : {items0}");
            string friends0 = string.Empty;
            p0.Friends.ForEach(a => friends0 += $"{a.Name},{a.Rank} ");
            Debug.Log($"フレンド : {friends0}");
            Debug.Log($"スタティック : {PlayerData.TestStatic}");
            Debug.Log("-----------------------------------------");

            Debug.Log("--------- プレイヤー1 の情報 --------------------------------");
            Debug.Log($"ユーザー名 : {p1.UserName}");
            Debug.Log($"ユーザーランク : {p1.UserRank}");
            string items1 = string.Empty;
            Debug.Log($"アイテム0 : {items1}");
            p1.Items.ForEach(a => items1 += $"{a} ");
            Debug.Log($"アイテム : {items1}");
            string friends1 = string.Empty;
            p1.Friends.ForEach(a => friends1 += $"{a.Name},{a.Rank} ");
            Debug.Log($"フレンド : {friends1}");
            Debug.Log($"スタティック : {PlayerData.TestStatic}");
            Debug.Log("-----------------------------------------");
        };
        InputEventHandler.OnDown_X += () =>
        {
            Debug.Log("データ更新した。");
            p0.UserName = "あちゃ";
            p0.UserRank += 10;
            p0.Items.Add("バイキルミン");
            p0.Friends.Add(new Friend("まく", 8));
            p0.Friends.Add(new Friend("くまた", 15));
            PlayerData.TestStatic = "スタティックデータ変更した";
        };
        InputEventHandler.OnDown_V += () =>
        {
            if (PlayerData.instances.Count == 0)
            {
                Debug.Log("セーブデータのインスタンス無し");
                return;
            }
            foreach (var a in PlayerData.instances)
            {
                Debug.Log($"{a}");
            }
        };
    }
}
