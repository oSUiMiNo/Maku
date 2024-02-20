using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Player1 : SingletonCompo<Player1>
{
    public override bool IsActive { get; protected set; }
    //= false;
    = true;

    [System.Serializable]
    class PlayerData : Savable
    {
        protected override string AffiliatedFolderName { get; set; } = "PlayerData";
        public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
        public static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
        
        // 以下の書き方ではダメ？何で代入しているinstancesはスタティックじゃないといけないの？と思ったが、
        // 恐らく以下のように書くと本クラスのインスタンスが作られる度に管理用のInstancesが増えてしまうが、
        // 今回は１クラス１リストにしたかったため
        // public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = new();

        #region データ
        public string UserName = "Default Name";
        public int UserRank = 0;
        [JsonProperty] // スタティックなデータをシリアアライズしたい場合は[JsonProperty]をつける
        public static string Job = "アルケミスト";
        public List<string> Items = new List<string>() { "薬草", "聖水" };
        public Dictionary<string, Friend> Friends = new Dictionary<string, Friend>();
        #endregion    
    }
    public class Friend
    {
        public string Job = "Default Name";
        public int Rank = 0;
        public Friend(string job, int rank)
        {
            Job = job;
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
            foreach (var a in p0.Friends)
            {
                friends0 += $"{a.Key},{a.Value.Job},{a.Value.Rank} ";
            }
            Debug.Log($"フレンド : {friends0}");
            Debug.Log($"スタティック : {PlayerData.Job}");
            Debug.Log("-----------------------------------------");

            Debug.Log("--------- プレイヤー1 の情報 --------------------------------");
            Debug.Log($"ユーザー名 : {p1.UserName}");
            Debug.Log($"ユーザーランク : {p1.UserRank}");
            string items1 = string.Empty;
            Debug.Log($"アイテム0 : {items1}");
            p1.Items.ForEach(a => items1 += $"{a} ");
            Debug.Log($"アイテム : {items1}");
            string friends1 = string.Empty;
            foreach(var a in p1.Friends)
            {
                friends1 += $"{a.Key},{a.Value.Job},{a.Value.Rank} ";
            }
            Debug.Log($"フレンド : {friends1}");
            Debug.Log($"スタティック : {PlayerData.Job}");
            Debug.Log("-----------------------------------------");
        };
        InputEventHandler.OnDown_X += () =>
        {
            Debug.Log("データ更新した。");
            p0.UserName = "あちゃ";
            p0.UserRank += 10;
            p0.Items.Add("バイキルミン");
            p0.Friends.Add("まく", new Friend("エレメントアーチャー", 8));
            p0.Friends.Add("くまた", new Friend("シールドセージ", 15));
            PlayerData.Job = "ハイセプター";
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
