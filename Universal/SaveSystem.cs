using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEditor;
using Newtonsoft.Json;
using System.Reflection;

/// <summary>
/// アプリケーション終了時の処理をさせるのに MonoBehaviour が必要なのでシングルトンコンポーネント化するが、
/// 他のクラスから利用する Save() や Load() は直接呼び出したいので static
/// </summary>
public class SaveSystem : SingletonCompo<SaveSystem>
{
    /// <summary>
    /// 特定のクラスにだけ公開したい変数や関数に使う。
    /// 詳しくは以下を参照
    /// https://www.notion.so/private-39bd316213f64e999dc0c2ce353fc814
    /// </summary>
    public interface IFriendWith_SaveSystem
    {
        void CheckFirstLoading();
        void ResetFirstLoading();
        void SetFirstLoading(bool isLoadedAtFirst);
        void SetManagementDictionaty();
        void UpdateIndex();
        List<IFriendWith_SaveSystem> GetAllInstances();
    }
    static IFriendWith_SaveSystem Friend(object obj)
    {
        return (IFriendWith_SaveSystem)obj;
    }

    static IEnumerable<Type> SavableBaseTypes;


    // 消す　＝＝＝＝＝＝＝
    /// <summary>
    /// 【注意】
    /// ここで指定したパスの、セーブデータをしまう用のフォルダを必ず作っておく。
    /// 【メモ】
    /// プロパティとしてやれば、静的な場所でも $" " を使える
    /// </summary>
    //public static string SaveFolderPath => @"C:\Users\vantan\Documents\Unity\Maku\ChessN7\Assets\SaveFiles";
    //public static string SaveFolderPath => $"{Application.persistentDataPath}";
    // 消す　＝＝＝＝＝＝＝


    protected override void SubLateAwake()
    {
        GetSavableBaseTypes();
        InitSavables();
    }
    void OnApplicationQuit()
    {
        ResetSavables();
    }



    #region 初期化
    /// <summary>
    /// 1. IFriendWith_SaveSystem を継承しておりかつ抽象ではない型を全部把握して SavableBaseTypes に記憶しておく
    /// 2. 記憶した全クラスのインスタンスを1つずつ作り、各インスタンスの SetManagementDictionaty() を呼ぶ
    /// </summary>
    static void GetSavableBaseTypes()
    {
        // SavableBaseTypes = System.Reflection.Assembly
        //.GetAssembly(typeof(IFriendWith_SaveSystem))
        //.GetTypes()
        //.Where(t =>
        //{
        //    return
        //        t.GetInterfaces().Contains(typeof(IFriendWith_SaveSystem)) &&
        //        !t.IsAbstract;
        //});

        // 上記処理では他のアセンブリ内のスクリプトに適用されないので新調した
        SavableBaseTypes = AppDomain.CurrentDomain.GetAssemblies() // 全てのアセンブリを取得
        .SelectMany(assembly =>
        {
            try
            {
                return assembly.GetTypes(); // 各アセンブリの全ての型を取得
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 一部の型がロードできなかった場合でも処理を継続
                return ex.Types.Where(t => t != null);
            }
        })
        .Where(t =>
        {
            return t.GetInterfaces().Contains(typeof(IFriendWith_SaveSystem)) && // IFriendWith_SaveSystemインターフェースを実装しているか
                   !t.IsAbstract; // 抽象クラスではない
        });
    }
    static void InitSavables()
    {
        foreach (var a in SavableBaseTypes)
        {
            object obj = Activator.CreateInstance(a);
            Friend(obj).SetManagementDictionaty();
            Debug.Log(a);
        }
    }
    #endregion



    #region セーブ Json.NET版
    public static void Save(ISave data)
    {
        Debug.Log($"---SaveSystem  セーブ---");
        ///<summary>
        ///【StreamWriter の使い方】
        /// sw.Writeとsw.WriteLineと書くことで、テキストに文字を出力することができる。
        /// 改行しないときは、Write
        /// 改行するときは、WriteLine
        /// </summary>
        string Path = data.GetPath();
        // データクラスに循環参照しているパラメータがあるとエラーを吐くので、
        // そのパラメータには[JsonIgnore]をつけてシリアライズを無視してもらう
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        // 書き込み
        StreamWriter streamWriter = new(Path, false);
        streamWriter.WriteLine(jsonData);
        streamWriter.Flush();
        streamWriter.Close();
    }
    #endregion



    #region ロード Json.NET版 ( 通常のクラス用 )
    public static void Load(ISave data)
    {
        Debug.Log($"---SaveSystem  ロード---");
        string Path = data.GetPath();
        if (!File.Exists(Path))
        {
            Debug.Log("-----------最初回ロード-----------");
            ///<summary>
            /// 初回はとりまJsonデータを作りたいので、
            /// 以下の処理の中でセーブも行っている。
            /// ここでセーブしておかないとセーブファイルが無いまんまなのでまたここに来る。
            /// </summary>
            Friend(data).CheckFirstLoading();
        }
        else
        {
            StreamReader streamReader = new(Path);
            string jsonData = streamReader.ReadToEnd();
            streamReader.Close();
            // デシリアライズした結果を既存のクラスに上書きしたい場合はPopulateObject
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                // これをセットしないとロードしたデータを上書きした際、List系のデータが上書きではなく要素の追加をされてしまう
                ObjectCreationHandling = ObjectCreationHandling.Replace, 
            });
            ///<summary>
            /// 以下の処理にはセーブの処理が含まれている。
            /// この処理を、ロードよりも前に持ってきてしまうと、
            /// ロード前のデータがセーブされた状態でロード処理に移ってしまう。
            /// </summary>
            Friend(data).CheckFirstLoading();
        }
    }
    #endregion



    #region セーブ JsonUtility
    //public static void Save(ISave data)
    //{
    //    Debug.Log($"---SaveSystem  セーブ---");
    //    ///<summary>
    //    ///【StreamWriter の使い方】
    //    /// sw.Writeとsw.WriteLineと書くことで、テキストに文字を出力することができる。
    //    /// 改行しないときは、Write
    //    /// 改行するときは、WriteLine
    //    /// </summary>
    //    string Path = data.GetPath();
    //    //string jsonData = EditorJsonUtility.ToJson(data);
    //    Debug.Log($"{data.GetPath()}");
    //    Debug.Log($"{Path}");
    //    Debug.Log($"{((SavableCompo)data).IsLoadedAtFirst}");
    //    string jsonData = JsonUtility.ToJson(data, true);
    //    StreamWriter streamWriter = new(Path, false);
    //    streamWriter.WriteLine(jsonData);
    //    streamWriter.Flush();
    //    streamWriter.Close();
    //}
    #endregion



    #region ロード JsonUtilityp ( 通常のクラス用 )
    //public static void Load(ISave data)
    //{
    //    Debug.Log($"---SaveSystem  ロード---");
    //    string Path = data.GetPath();
    //    if (!File.Exists(Path))
    //    {
    //        Debug.Log("-----------最初回ロード-----------");
    //        ///<summary>
    //        /// 初回はとりまJsonデータを作りたいので、
    //        /// 以下の処理の中でセーブも行っている。
    //        /// ここでセーブしておかないとセーブファイルが無いまんまなのでまたここに来る。
    //        /// </summary>
    //        Friend(data).CheckFirstLoading();
    //    }
    //    else
    //    {
    //        StreamReader streamReader = new(Path);
    //        string jsonData = streamReader.ReadToEnd();
    //        streamReader.Close();
    //        JsonUtility.FromJsonOverwrite(jsonData, data);
    //        ///<summary>
    //        /// 以下の処理にはセーブの処理が含まれている。
    //        /// この処理を、ロードよりも前に持ってきてしまうと、
    //        /// ロード前のデータがセーブされた状態でロード処理に移ってしまう。
    //        /// </summary>
    //        Friend(data).CheckFirstLoading();
    //    }
    //}
    #endregion


    // Vector3 等はjson.netではシリアライズできない。
    // MonoBehaviour をjson.netでシリアライズしようとすると祖先クラスのrigidbodyプロパティなどもシリアライズしようとしてエラーになる。阻止不可能らしい。
    // なので JsonUtility
    #region ロード ( MonoBehaviour用 )
    //public static void Load_Compo(ISave before, ISave after)
    //{
    //    Debug.Log($"---SaveSystem  ロード---");

    //    string Path = before.GetPath();
    //    if (!File.Exists(Path))
    //    {
    //        Debug.Log("-----------初回ロード-----------");
    //        ///<summary>
    //        /// 初回はとりまJsonデータを作りたいので、
    //        /// 以下の処理の中でセーブも行っている。
    //        /// ここでセーブしておかないとセーブファイルが無いまんまなのでまたここに来る。
    //        /// </summary>
    //        Friend(before).CheckFirstLoading();
    //    }
    //    Debug.Log($"パス {Path}");
    //    StreamReader streamReader = new(Path);
    //    string jsonData = streamReader.ReadToEnd();
    //    streamReader.Close();

    //    //JsonUtility.FromJsonOverwrite(jsonData, after);
    //    JsonUtility.FromJsonOverwrite(jsonData, before);

    //    //Friend(after).CheckFirstLoading();
    //    Friend(before).CheckFirstLoading();
    //    //((MonoBehaviour)after).enabled = true;
    //}
    public static void Load_Compo(ISave before)
    {
        Debug.Log($"---SaveSystem  ロード---");

        string Path = before.GetPath();
        if (!File.Exists(Path))
        {
            Debug.Log("-----------初回ロード-----------");
            ///<summary>
            /// 初回はとりまJsonデータを作りたいので、
            /// 以下の処理の中でセーブも行っている。
            /// ここでセーブしておかないとセーブファイルが無いまんまなのでまたここに来る。
            /// </summary>
            Friend(before).CheckFirstLoading();
        }
        Debug.Log($"パス {Path}");
        StreamReader streamReader = new(Path);
        string jsonData = streamReader.ReadToEnd();
        streamReader.Close();

        //JsonUtility.FromJsonOverwrite(jsonData, after);
        JsonUtility.FromJsonOverwrite(jsonData, before);

        //Friend(after).CheckFirstLoading();
        Friend(before).CheckFirstLoading();
        //((MonoBehaviour)after).enabled = true;
    }
    #endregion



    #region セーブ JsonUtility ( MonoBehaviour用 )
    public static void Save_Compo(ISave before)
    {
        Debug.Log($"---SaveSystem  セーブ---");
        ///<summary>
        ///【StreamWriter の使い方】
        /// sw.Writeとsw.WriteLineと書くことで、テキストに文字を出力することができる。
        /// 改行しないときは、Write
        /// 改行するときは、WriteLine
        /// </summary>
        string Path = before.GetPath();
        //string jsonData = EditorJsonUtility.ToJson(data);
        string jsonData = JsonUtility.ToJson(before, true);
        StreamWriter streamWriter = new(Path, false);
        streamWriter.WriteLine(jsonData);
        streamWriter.Flush();
        streamWriter.Close();
    }
    #endregion

    
    public static void UpdateAllIndex()
    {
        foreach (var a in SavableBaseTypes)
        {
            Debug.Log($"後処理１{a}\n==================================");

            object obj = Activator.CreateInstance(a);
            Debug.Log($"後処理２{obj} {Friend(obj)}");
            List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            foreach (var b in savableInss)
            {
                Debug.Log($"======= 後処理３ ======={b}");
                b.UpdateIndex();
            }
        }
    }


    #region 後処理
    static void ResetSavables()
    {
        Debug.Log($"後処理 ====================================================================");

        foreach (var a in SavableBaseTypes)
        {
            Debug.Log($"後処理１{a}\n==================================");

            object obj = Activator.CreateInstance(a);
            Debug.Log($"後処理２{obj} {Friend(obj)}");
            List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            foreach (var b in savableInss)
            {
                Debug.Log($"======= 後処理３ ======={b}");
                b.ResetFirstLoading();
            }
            //if (a.BaseType.Name == "SavableCompo")
            //{

            //}
            //else
            //{
            //    object obj = Activator.CreateInstance(a);
            //    Debug.Log($"後処理２{obj} {Friend(obj)}");
            //    List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            //    foreach (var b in savableInss)
            //    {
            //        Debug.Log($"======= 後処理３ ======={b}");
            //        b.ResetFirstLoading();
            //    }
            //}

            //object obj = Activator.CreateInstance(a);
            //Debug.Log($"後処理２{obj} {Friend(obj)}");
            //List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            //foreach (var b in savableInss)
            //{
            //    Debug.Log($"======= 後処理３ ======={b}");
            //    b.ResetFirstLoading();
            //}
        }
    }
    #endregion
}











#region 1【セーブしたいクラスに継承させる】 ========================================================================

public interface ISave
{
    string GetPath();
    void Save();
    void Load();
}


#region 2【ノーマル】 ==============================================================

/// <summary>
/// 【注意】
/// これを継承したクラスには必ず引数無しのデフォルトコンストラクタを書いておく。
/// </summary>
[Serializable]
public abstract class Savable : MyExtention,
    SaveSystem.IFriendWith_SaveSystem,
    ISave
{
    #region 全Savable系基底クラス共通の処理
    // 共通データ
    [JsonProperty] protected string _Path { get; private set; } = string.Empty;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
    [JsonProperty] protected int InstanceNumber { get; private set; } = -1;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
    [JsonProperty] protected bool IsLoadedAtFirst { get; private set; } = false;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
                                                                                  // JsonUtility の場合
                                                                                  //[SerializeField] protected string path = string.Empty;  //private にしたら保存されずエラーになる
                                                                                  //[SerializeField] protected int instanceNumber = -1;  //private にしたら保存されずエラーになる
                                                                                  //[SerializeField] protected bool isLoadedAtFirst = false;  //private にしたら保存されずエラーになる

    // データをしまうフォルダの名前。指定しない場合は「Application.persistentDataPath」になる。
    [JsonIgnore] public virtual string SaveFolderPath { get; set; } = string.Empty;
    // データのファイル名。指定しない場合は「クラス名+instanceNumber」になる
    [JsonIgnore] public virtual string FileName { get; set; } = string.Empty;

    // 統括用ディクショナリー
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    // 各セーブ可能クラスに用意する staticなインスタンス管理リストのゲッター
    // Instances が循環参照になっているので[JsonIgnore]をつけてシリアライズされないようにする。継承先でオーバーライドする際は付けなくても正常に動きそう
    [JsonIgnore] public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

    //自分のセーブファイルを保存する場所のパスを返す。
    public string GetPath() { return _Path; }

    /// <summary>
    /// isFirstLoadingは各セーブ可能基底クラスごとに用意しているため抽象化しきれない変数。
    /// セーブ可能インスタンスの型を具体的にキャスとせずに外部から変更する場合に、
    /// IFriendWith_SaveSystem 内用意した変更用関数を使うのが良い感じだった。
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        this.IsLoadedAtFirst = isLoadedAtFirst;
    }

    // 親クラスの、辞書に自分の管理リストを登録する。
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ロードしたデータが初回ロードかどうか確認し、初回であれば初回フラグだけを変更し保存。
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        if (!IsLoadedAtFirst)
        {
            Debug.Log($"{this} はアプリを起動してから１回目のロード");
            IsLoadedAtFirst = true;
            SaveSystem.Save(this);
        }
        else
        {
            Debug.Log($"{this} はアプリを起動後に少なくとも１回ロードされたようです");
        }

        Debug.Log($"{IsLoadedAtFirst}");
    }

    void SaveSystem.IFriendWith_SaveSystem.ResetFirstLoading()
    {
        foreach (var a in InstancesListsManagementDictionaty.Values)
        {
            foreach (var b in a)
            {
                b.SetFirstLoading(false);
                SaveSystem.Save((ISave)b);
            }
        }
    }

    List<SaveSystem.IFriendWith_SaveSystem> SaveSystem.IFriendWith_SaveSystem.GetAllInstances()
    {
        List<SaveSystem.IFriendWith_SaveSystem> list = new();
        foreach (var a in InstancesListsManagementDictionaty.Values)
        {
            //統括ディクショナリを分解して中身のリストの中身に入ったインスタンスを1つのリストにまとめなおす
            list.AddRange(a);
        }
        return list;
    }
    #endregion


    public void Save()
    {
        SaveSystem.Save(this);
        Debug.Log("------セーブした------");
    }
    public void Load()
    {
        //アプリを起動してから1回目のロード
        if (!IsLoadedAtFirst)
        {
            //管理リストに自分を登録
            Instances.Add(this);
            //自分のインスタンス番号と所属フォルダとパスを設定
            InstanceNumber = Instances.Count - 1;
            //string AffiliatedFolderPath = @$"{SaveSystem.SaveFolderPath}/{AffiliatedFolderName}";
            if (string.IsNullOrEmpty(SaveFolderPath)) SaveFolderPath = Application.persistentDataPath;
            // 指定したパスのフォルダが存在しない場合作成
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);
            // 自分の完全なパスを作成
            if (string.IsNullOrEmpty(FileName)) _Path = @$"{SaveFolderPath}/{GetType().Name}{InstanceNumber}.json";
            else _Path = @$"{SaveFolderPath}/{FileName}.json";
        }

        ////ロード
        SaveSystem.Load(this);

        ////管理リスト更新
        Instances[InstanceNumber] = this;  //いらないかも

        Debug.Log("------ロードした------");
    }

    void SaveSystem.IFriendWith_SaveSystem.UpdateIndex()
    {
        InstanceNumber = Instances.IndexOf(this);
    }
}

#endregion 2【ノーマル】 ===========================================================






#region 2【コンポーネント用】 ==============================================================

[Serializable]
public abstract class SavableCompo : MonoBehaviourMyExtention,
    SaveSystem.IFriendWith_SaveSystem,
    ISave
{
    #region 全Savable系基底クラス共通の処理
    // 共通データ
    //[JsonProperty] protected string _Path { get; private set; } = string.Empty;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
    //[JsonProperty] protected int InstanceNumber { get; private set; } = -1;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
    //[JsonProperty] protected bool IsLoadedAtFirst { get; private set; } = false;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
    // JsonUtility の場合
    [SerializeField] public string _Path = string.Empty;  //private にしたら保存されずエラーになる
    [SerializeField] public int InstanceNumber = -1;  //private にしたら保存されずエラーになる
    [SerializeField] public bool IsLoadedAtFirst = false;  //private にしたら保存されずエラーになる

    // データをしまうフォルダの名前。指定しない場合は「Application.persistentDataPath」になる。
    public virtual string SaveFolderPath { get; set; } = string.Empty;
    // データのファイル名。指定しない場合は「クラス名+instanceNumber」になる
    public virtual string FileName { get; set; } = string.Empty;

    // 統括用ディクショナリー
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    // 各セーブ可能クラスに用意する staticなインスタンス管理リストのゲッター
    // Instances が循環参照になっているので[JsonIgnore]をつけてシリアライズされないようにする。継承先でオーバーライドする際は付けなくても正常に動きそう
    public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }


    //自分のセーブファイルを保存する場所のパスを返す。
    public string GetPath() { return _Path; }

    /// <summary>
    /// isFirstLoadingは各セーブ可能基底クラスごとに用意しているため抽象化しきれない変数。
    /// セーブ可能インスタンスの型を具体的にキャスとせずに外部から変更する場合に、
    /// IFriendWith_SaveSystem 内用意した変更用関数を使うのが良い感じだった。
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        IsLoadedAtFirst = isLoadedAtFirst;
    }

    // 親クラスの、辞書に自分の管理リストを登録する。
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ロードしたデータが初回ロードかどうか確認し、初回であれば初回フラグだけを変更し保存。
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        //IsLoadedAtFirst = true;

        //if (!((SavableCompo)Instances[InstanceNumber]).IsLoadedAtFirst)
        if (!IsLoadedAtFirst)

        {
            Debug.Log($"{this} はアプリを起動してから１回目のロード");
            //((SavableCompo)Instances[InstanceNumber]).IsLoadedAtFirst = true;
            IsLoadedAtFirst = true;

            Debug.Log($"{_Path}");
            SaveSystem.Save_Compo(this);
        }
        else
        {
            Debug.Log($"{this} はアプリを起動後に少なくとも１回ロードされたようです");
        }
    }


    void SaveSystem.IFriendWith_SaveSystem.ResetFirstLoading()
    {
        Debug.Log($"リセットファーストローディング{_Path}");
        foreach (var a in InstancesListsManagementDictionaty.Values)
        {
            foreach (var b in a)
            {
                Debug.Log($"ゲームオブジェクト {((MonoBehaviour)b).gameObject}");
                b.SetFirstLoading(false);
                SaveSystem.Save_Compo((ISave)b);
            }
        }
    }
    //protected void ResetFirstLoading()
    //{
    //    //Debug.Log($"ゲームオブジェクト {gameObject}");
    //    //SaveSystem.Save_Compo(this);

    //    foreach (var a in InstancesListsManagementDictionaty.Values)
    //    {
    //        foreach (var b in a)
    //        {
    //            Debug.Log($"ゲームオブジェクト {((MonoBehaviour)b).gameObject}");
    //            b.SetFirstLoading(false);
    //            SaveSystem.Save_Compo((ISave)b);
    //        }
    //    }
    //}

    List<SaveSystem.IFriendWith_SaveSystem> SaveSystem.IFriendWith_SaveSystem.GetAllInstances()
    {
        List<SaveSystem.IFriendWith_SaveSystem> list = new();
        foreach (var a in InstancesListsManagementDictionaty.Values)
        {
            //統括ディクショナリを分解して中身のリストの中身に入ったインスタンスを1つのリストにまとめなおす
            list.AddRange(a);
        }
        return list;
    }
    #endregion

    //SavableCompo before;
    //SavableCompo after;
    //public int afterCount = 0;

    public void Save()
    {
        //((SavableCompo)Instances[InstanceNumber]).IsLoadedAtFirst = true;
        IsLoadedAtFirst = true;

        //SaveSystem.Save_Compo(((SavableCompo)Instances[InstanceNumber]).gameObject.GetComponent<SavableCompo>());
        SaveSystem.Save_Compo(this);

        Debug.Log("------セーブした------");
    }

    public void Load()
    {
        //アプリを起動してから1回目のロード
        if (!IsLoadedAtFirst)
        {
            Debug.Log($"{this} はアプリを起動してから１回目のロード");
            //管理リストに自分を登録
            Instances.Add(this);
            //自分のインスタンス番号と所属フォルダとパスを設定
            InstanceNumber = Instances.Count - 1;
            //string AffiliatedFolderPath = @$"{SaveSystem.SaveFolderPath}/{AffiliatedFolderName}";
            if (string.IsNullOrEmpty(SaveFolderPath)) SaveFolderPath = Application.persistentDataPath;
            // 指定したパスのフォルダが存在しない場合作成
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);
            // 自分の完全なパスを作成
            if (string.IsNullOrEmpty(FileName)) _Path = @$"{SaveFolderPath}/{GetType().Name}{InstanceNumber}.json";
            else _Path = @$"{SaveFolderPath}/{FileName}.json";
        }

        //管理リスト更新;
        //Instances[InstanceNumber] = (SavableCompo)((SavableCompo)Instances[InstanceNumber]).gameObject.AddComponent(GetType());

        //ロード
        //SaveSystem.Load(this, (ISave)Instances[InstanceNumber]);
        SaveSystem.Load_Compo(this);

        // 自分を消去
        //DestroyImmediate(this);

        ///<summary>
        ///　Load処理の中で現在のこのコンポーネントは破壊されるため、
        ///　多分これ以降の処理は通らないので、ロードを最後に書く
        /// </summary>
        Debug.Log("------ロードした------");
    }

    void SaveSystem.IFriendWith_SaveSystem.UpdateIndex()
    {
        InstanceNumber = Instances.IndexOf(this);
    }


    // MonoBehaviour の関数は同アセンブリ内の子クラスにしか作用しないらしく、
    // パッケージ外で本クラスをの子クラスを実装する場合は、そのクラス内に書く必要がある。
    //void OnApplicationQuit()
    //{
    //    //ResetFirstLoading();
    //}
}

#endregion 2【コンポーネント用】 ===========================================================






#region 2【シングルトン用】 ==============================================================

[Serializable]
public abstract class SavableSingleton<SingletonType> : Singleton<SingletonType>,
    SaveSystem.IFriendWith_SaveSystem,
    ISave
     where SingletonType : Singleton<SingletonType>, new()
{
    #region 全Savable系基底クラス共通の処理
    // 共通データ
    [JsonProperty] protected string _Path { get; private set; } = string.Empty;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
    [JsonProperty] protected int InstanceNumber { get; private set; } = -1;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
    [JsonProperty] protected bool IsLoadedAtFirst { get; private set; } = false;  //Json.NET だと public 以外シリアアライズされないが、[JsonProperty]をつけると関係なくいける
                                                                                  // JsonUtility の場合
                                                                                  //[SerializeField] protected string path = string.Empty;  //private にしたら保存されずエラーになる
                                                                                  //[SerializeField] protected int instanceNumber = -1;  //private にしたら保存されずエラーになる
                                                                                  //[SerializeField] protected bool isLoadedAtFirst = false;  //private にしたら保存されずエラーになる

    // データをしまうフォルダの名前
    [JsonIgnore] public virtual string SaveFolderPath { get; set; } = string.Empty;
    // データのファイル名。指定しない場合は「クラス名+instanceNumber」になる
    [JsonIgnore] public virtual string FileName { get; set; } = string.Empty;

    // 統括用ディクショナリー
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    // 各セーブ可能クラスに用意する staticなインスタンス管理リストのゲッター
    // Instances が循環参照になっているので[JsonIgnore]をつけてシリアライズされないようにする。継承先でオーバーライドする際は付けなくても正常に動きそう
    [JsonIgnore] public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

    //自分のセーブファイルを保存する場所のパスを返す。
    public string GetPath() { return _Path; }

    /// <summary>
    /// isFirstLoadingは各セーブ可能基底クラスごとに用意しているため抽象化しきれない変数。
    /// セーブ可能インスタンスの型を具体的にキャスとせずに外部から変更する場合に、
    /// IFriendWith_SaveSystem 内用意した変更用関数を使うのが良い感じだった。
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        this.IsLoadedAtFirst = isLoadedAtFirst;
    }

    // 親クラスの、辞書に自分の管理リストを登録する。
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ロードしたデータが初回ロードかどうか確認し、初回であれば初回フラグだけを変更し保存。
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        if (!IsLoadedAtFirst)
        {
            Debug.Log($"{this} はアプリを起動してから１回目のロード");
            IsLoadedAtFirst = true;
            SaveSystem.Save(this);
        }
        else
        {
            Debug.Log($"{this} はアプリを起動後に少なくとも１回ロードされたようです");
        }

        Debug.Log($"{IsLoadedAtFirst}");
    }

    void SaveSystem.IFriendWith_SaveSystem.ResetFirstLoading()
    {
        Debug.Log($"リセットファーストローディング{_Path}");
        foreach (var a in InstancesListsManagementDictionaty.Values)
        {
            foreach (var b in a)
            {
                b.SetFirstLoading(false);
                SaveSystem.Save((ISave)b);
            }
        }
    }

    List<SaveSystem.IFriendWith_SaveSystem> SaveSystem.IFriendWith_SaveSystem.GetAllInstances()
    {
        List<SaveSystem.IFriendWith_SaveSystem> list = new();
        foreach (var a in InstancesListsManagementDictionaty.Values)
        {
            //統括ディクショナリを分解して中身のリストの中身に入ったインスタンスを1つのリストにまとめなおす
            list.AddRange(a);
        }
        return list;
    }
    #endregion


    public void Save()
    {
        SaveSystem.Save(this);
        Debug.Log("------セーブした------");
    }
    public void Load()
    {
        //アプリを起動してから1回目のロード
        if (!IsLoadedAtFirst)
        {
            //管理リストに自分を登録
            Instances.Add(this);
            //自分のインスタンス番号と所属フォルダとパスを設定
            InstanceNumber = Instances.Count - 1;
            //string AffiliatedFolderPath = @$"{SaveSystem.SaveFolderPath}/{AffiliatedFolderName}";
            if (string.IsNullOrEmpty(SaveFolderPath)) SaveFolderPath = Application.persistentDataPath;
            // 指定したパスのフォルダが存在しない場合作成
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);
            // 自分の完全なパスを作成
            if (string.IsNullOrEmpty(FileName)) _Path = @$"{SaveFolderPath}/{GetType().Name}{InstanceNumber}.json";
            else _Path = @$"{SaveFolderPath}/{FileName}.json";
        }

        ////ロード
        SaveSystem.Load(this);

        ////管理リスト更新
        Instances[InstanceNumber] = this;  //いらないかも
        Debug.Log("------ロードした------");
    }

    void SaveSystem.IFriendWith_SaveSystem.UpdateIndex()
    {
        InstanceNumber = Instances.IndexOf(this);
    }
}

#endregion 2【シングルトン用】 ===========================================================







#region 2【シングルトンコンポーネント用】 ==============================================================

/// <summary>
/// 【注意】
/// まだ実用的ではない。一旦封印
/// </summary>
/// <typeparam name="SingletonType"></typeparam>
//[Serializable]
//public abstract class SavableSingletonCompo<SingletonType> : SingletonCompo<SingletonType>,
//    SaveSystem.IFriendWith_SaveSystem,
//    ISave
//     where SingletonType : SingletonCompo<SingletonType>, new()
//{
//    #region 全Savable系基底クラス共通の処理
//    //共通データ
//    [SerializeField] protected string path = string.Empty;  //private にしたら保存されずエラーになる。
//    [SerializeField] protected int instanceNumber = -1;  //private にしたら保存されずエラーになる。
//    [SerializeField] protected bool isLoadedAtFirst = false;  //private にしたら保存されずエラーになる。


//    //統括用ディクショナリー
//    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

//    //各セーブ可能クラスに用意する staticなインスタンス管理リストのゲッター
//    public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

//    //自分のセーブファイルを保存する場所のパスを返す。
//    public string GetPath() { return path; }

//    /// <summary>
//    /// isFirstLoadingは各セーブ可能基底クラスごとに用意しているため抽象化しきれない変数。
//    /// セーブ可能インスタンスの型を具体的にキャスとせずに外部から変更する場合に、
//    /// IFriendWith_SaveSystem 内用意した変更用関数を使うのが良い感じだった。
//    /// </summary>
//    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
//    {
//        this.isLoadedAtFirst = isLoadedAtFirst;
//    }

//    //親クラスの、辞書に自分の管理リストを登録する。
//    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
//    {
//        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
//    }


//    // ロードしたデータが初回ロードかどうか確認し、初回であれば初回フラグだけを変更し保存。
//    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
//    {
//        if (!isLoadedAtFirst)
//        {
//            Debug.Log($"{this} はアプリを起動してから１回目のロード");
//            isLoadedAtFirst = true;
//            SaveSystem.Save(this);
//        }
//        else
//        {
//            Debug.Log($"{this} はアプリを起動後に少なくとも１回ロードされたようです");
//        }

//        Debug.Log($"{isLoadedAtFirst}");
//    }

//    void SaveSystem.IFriendWith_SaveSystem.ResetFirstLoading()
//    {
//        foreach (var a in InstancesListsManagementDictionaty.Values)
//        {
//            foreach (var b in a)
//            {
//                b.SetFirstLoading(false);
//                SaveSystem.Save((ISave)b);
//            }
//        }
//    }

//    List<SaveSystem.IFriendWith_SaveSystem> SaveSystem.IFriendWith_SaveSystem.GetAllInstances()
//    {
//        List<SaveSystem.IFriendWith_SaveSystem> list = new();
//        foreach (var a in InstancesListsManagementDictionaty.Values)
//        {
//            //統括ディクショナリを分解して中身のリストの中身に入ったインスタンスを1つのリストにまとめなおす。
//            list.AddRange(a);
//        }
//        return list;
//    }
//    #endregion


//    public void Save()
//    {
//        SaveSystem.Save(this);
//        Debug.Log("------セーブした------");
//    }
//    public void Load()
//    {
//        //アプリを起動してから1回目のロード
//        if (!isLoadedAtFirst)
//        {
//            //管理リストに追加。
//            Instances.Add(this);

//            //自分のインスタンス番号とパスを設定する。
//            instanceNumber = Instances.Count - 1;
//            Debug.Log($"インスタンスナンバーが    {instanceNumber}  になった");
//            path = @$"{SaveSystem.SaveFolderPath}{GetType().Name}{instanceNumber}.json";
//        }


//        //管理リスト更新
//        MonoBehaviour afterData = (MonoBehaviour)gameObject.AddComponent(GetType());
//        Instances[instanceNumber] = (SaveSystem.IFriendWith_SaveSystem)afterData;

//        //ロード
//        SaveSystem.Load(this, afterData);
//        ///<summary>
//        ///　Load処理の中で現在のこのコンポーネントは破壊されるため、
//        ///　多分これ以降の処理は通らないので、ロードを最後に書く
//        /// </summary>

//        Debug.Log("------ロードした------");
//    }
//}

#endregion 2【シングルトンコンポーネント用】 ===========================================================


#endregion 1【セーブしたいクラスに継承させる】 =====================================================================
