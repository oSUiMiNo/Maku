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
/// �A�v���P�[�V�����I�����̏�����������̂� MonoBehaviour ���K�v�Ȃ̂ŃV���O���g���R���|�[�l���g�����邪�A
/// ���̃N���X���痘�p���� Save() �� Load() �͒��ڌĂяo�������̂� static
/// </summary>
public class SaveSystem : SingletonCompo<SaveSystem>
{
    /// <summary>
    /// ����̃N���X�ɂ������J�������ϐ���֐��Ɏg���B
    /// �ڂ����͈ȉ����Q��
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


    // �����@��������������
    /// <summary>
    /// �y���Ӂz
    /// �����Ŏw�肵���p�X�́A�Z�[�u�f�[�^�����܂��p�̃t�H���_��K������Ă����B
    /// �y�����z
    /// �v���p�e�B�Ƃ��Ă��΁A�ÓI�ȏꏊ�ł� $" " ���g����
    /// </summary>
    //public static string SaveFolderPath => @"C:\Users\vantan\Documents\Unity\Maku\ChessN7\Assets\SaveFiles";
    //public static string SaveFolderPath => $"{Application.persistentDataPath}";
    // �����@��������������


    protected override void SubLateAwake()
    {
        GetSavableBaseTypes();
        InitSavables();
    }
    void OnApplicationQuit()
    {
        ResetSavables();
    }



    #region ������
    /// <summary>
    /// 1. IFriendWith_SaveSystem ���p�����Ă��肩���ۂł͂Ȃ��^��S���c������ SavableBaseTypes �ɋL�����Ă���
    /// 2. �L�������S�N���X�̃C���X�^���X��1�����A�e�C���X�^���X�� SetManagementDictionaty() ���Ă�
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

        // ��L�����ł͑��̃A�Z���u�����̃X�N���v�g�ɓK�p����Ȃ��̂ŐV������
        SavableBaseTypes = AppDomain.CurrentDomain.GetAssemblies() // �S�ẴA�Z���u�����擾
        .SelectMany(assembly =>
        {
            try
            {
                return assembly.GetTypes(); // �e�A�Z���u���̑S�Ă̌^���擾
            }
            catch (ReflectionTypeLoadException ex)
            {
                // �ꕔ�̌^�����[�h�ł��Ȃ������ꍇ�ł��������p��
                return ex.Types.Where(t => t != null);
            }
        })
        .Where(t =>
        {
            return t.GetInterfaces().Contains(typeof(IFriendWith_SaveSystem)) && // IFriendWith_SaveSystem�C���^�[�t�F�[�X���������Ă��邩
                   !t.IsAbstract; // ���ۃN���X�ł͂Ȃ�
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



    #region �Z�[�u Json.NET��
    public static void Save(ISave data)
    {
        Debug.Log($"---SaveSystem  �Z�[�u---");
        ///<summary>
        ///�yStreamWriter �̎g�����z
        /// sw.Write��sw.WriteLine�Ə������ƂŁA�e�L�X�g�ɕ������o�͂��邱�Ƃ��ł���B
        /// ���s���Ȃ��Ƃ��́AWrite
        /// ���s����Ƃ��́AWriteLine
        /// </summary>
        string Path = data.GetPath();
        // �f�[�^�N���X�ɏz�Q�Ƃ��Ă���p�����[�^������ƃG���[��f���̂ŁA
        // ���̃p�����[�^�ɂ�[JsonIgnore]�����ăV���A���C�Y�𖳎����Ă��炤
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        // ��������
        StreamWriter streamWriter = new(Path, false);
        streamWriter.WriteLine(jsonData);
        streamWriter.Flush();
        streamWriter.Close();
    }
    #endregion



    #region ���[�h Json.NET�� ( �ʏ�̃N���X�p )
    public static void Load(ISave data)
    {
        Debug.Log($"---SaveSystem  ���[�h---");
        string Path = data.GetPath();
        if (!File.Exists(Path))
        {
            Debug.Log("-----------�ŏ��񃍁[�h-----------");
            ///<summary>
            /// ����͂Ƃ��Json�f�[�^����肽���̂ŁA
            /// �ȉ��̏����̒��ŃZ�[�u���s���Ă���B
            /// �����ŃZ�[�u���Ă����Ȃ��ƃZ�[�u�t�@�C���������܂�܂Ȃ̂ł܂������ɗ���B
            /// </summary>
            Friend(data).CheckFirstLoading();
        }
        else
        {
            StreamReader streamReader = new(Path);
            string jsonData = streamReader.ReadToEnd();
            streamReader.Close();
            // �f�V���A���C�Y�������ʂ������̃N���X�ɏ㏑���������ꍇ��PopulateObject
            JsonConvert.PopulateObject(jsonData, data, new JsonSerializerSettings
            {
                // ������Z�b�g���Ȃ��ƃ��[�h�����f�[�^���㏑�������ہAList�n�̃f�[�^���㏑���ł͂Ȃ��v�f�̒ǉ�������Ă��܂�
                ObjectCreationHandling = ObjectCreationHandling.Replace, 
            });
            ///<summary>
            /// �ȉ��̏����ɂ̓Z�[�u�̏������܂܂�Ă���B
            /// ���̏������A���[�h�����O�Ɏ����Ă��Ă��܂��ƁA
            /// ���[�h�O�̃f�[�^���Z�[�u���ꂽ��ԂŃ��[�h�����Ɉڂ��Ă��܂��B
            /// </summary>
            Friend(data).CheckFirstLoading();
        }
    }
    #endregion



    #region �Z�[�u JsonUtility
    //public static void Save(ISave data)
    //{
    //    Debug.Log($"---SaveSystem  �Z�[�u---");
    //    ///<summary>
    //    ///�yStreamWriter �̎g�����z
    //    /// sw.Write��sw.WriteLine�Ə������ƂŁA�e�L�X�g�ɕ������o�͂��邱�Ƃ��ł���B
    //    /// ���s���Ȃ��Ƃ��́AWrite
    //    /// ���s����Ƃ��́AWriteLine
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



    #region ���[�h JsonUtilityp ( �ʏ�̃N���X�p )
    //public static void Load(ISave data)
    //{
    //    Debug.Log($"---SaveSystem  ���[�h---");
    //    string Path = data.GetPath();
    //    if (!File.Exists(Path))
    //    {
    //        Debug.Log("-----------�ŏ��񃍁[�h-----------");
    //        ///<summary>
    //        /// ����͂Ƃ��Json�f�[�^����肽���̂ŁA
    //        /// �ȉ��̏����̒��ŃZ�[�u���s���Ă���B
    //        /// �����ŃZ�[�u���Ă����Ȃ��ƃZ�[�u�t�@�C���������܂�܂Ȃ̂ł܂������ɗ���B
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
    //        /// �ȉ��̏����ɂ̓Z�[�u�̏������܂܂�Ă���B
    //        /// ���̏������A���[�h�����O�Ɏ����Ă��Ă��܂��ƁA
    //        /// ���[�h�O�̃f�[�^���Z�[�u���ꂽ��ԂŃ��[�h�����Ɉڂ��Ă��܂��B
    //        /// </summary>
    //        Friend(data).CheckFirstLoading();
    //    }
    //}
    #endregion


    // Vector3 ����json.net�ł̓V���A���C�Y�ł��Ȃ��B
    // MonoBehaviour ��json.net�ŃV���A���C�Y���悤�Ƃ���Ƒc��N���X��rigidbody�v���p�e�B�Ȃǂ��V���A���C�Y���悤�Ƃ��ăG���[�ɂȂ�B�j�~�s�\�炵���B
    // �Ȃ̂� JsonUtility
    #region ���[�h ( MonoBehaviour�p )
    //public static void Load_Compo(ISave before, ISave after)
    //{
    //    Debug.Log($"---SaveSystem  ���[�h---");

    //    string Path = before.GetPath();
    //    if (!File.Exists(Path))
    //    {
    //        Debug.Log("-----------���񃍁[�h-----------");
    //        ///<summary>
    //        /// ����͂Ƃ��Json�f�[�^����肽���̂ŁA
    //        /// �ȉ��̏����̒��ŃZ�[�u���s���Ă���B
    //        /// �����ŃZ�[�u���Ă����Ȃ��ƃZ�[�u�t�@�C���������܂�܂Ȃ̂ł܂������ɗ���B
    //        /// </summary>
    //        Friend(before).CheckFirstLoading();
    //    }
    //    Debug.Log($"�p�X {Path}");
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
        Debug.Log($"---SaveSystem  ���[�h---");

        string Path = before.GetPath();
        if (!File.Exists(Path))
        {
            Debug.Log("-----------���񃍁[�h-----------");
            ///<summary>
            /// ����͂Ƃ��Json�f�[�^����肽���̂ŁA
            /// �ȉ��̏����̒��ŃZ�[�u���s���Ă���B
            /// �����ŃZ�[�u���Ă����Ȃ��ƃZ�[�u�t�@�C���������܂�܂Ȃ̂ł܂������ɗ���B
            /// </summary>
            Friend(before).CheckFirstLoading();
        }
        Debug.Log($"�p�X {Path}");
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



    #region �Z�[�u JsonUtility ( MonoBehaviour�p )
    public static void Save_Compo(ISave before)
    {
        Debug.Log($"---SaveSystem  �Z�[�u---");
        ///<summary>
        ///�yStreamWriter �̎g�����z
        /// sw.Write��sw.WriteLine�Ə������ƂŁA�e�L�X�g�ɕ������o�͂��邱�Ƃ��ł���B
        /// ���s���Ȃ��Ƃ��́AWrite
        /// ���s����Ƃ��́AWriteLine
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
            Debug.Log($"�㏈���P{a}\n==================================");

            object obj = Activator.CreateInstance(a);
            Debug.Log($"�㏈���Q{obj} {Friend(obj)}");
            List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            foreach (var b in savableInss)
            {
                Debug.Log($"======= �㏈���R ======={b}");
                b.UpdateIndex();
            }
        }
    }


    #region �㏈��
    static void ResetSavables()
    {
        Debug.Log($"�㏈�� ====================================================================");

        foreach (var a in SavableBaseTypes)
        {
            Debug.Log($"�㏈���P{a}\n==================================");

            object obj = Activator.CreateInstance(a);
            Debug.Log($"�㏈���Q{obj} {Friend(obj)}");
            List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            foreach (var b in savableInss)
            {
                Debug.Log($"======= �㏈���R ======={b}");
                b.ResetFirstLoading();
            }
            //if (a.BaseType.Name == "SavableCompo")
            //{

            //}
            //else
            //{
            //    object obj = Activator.CreateInstance(a);
            //    Debug.Log($"�㏈���Q{obj} {Friend(obj)}");
            //    List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            //    foreach (var b in savableInss)
            //    {
            //        Debug.Log($"======= �㏈���R ======={b}");
            //        b.ResetFirstLoading();
            //    }
            //}

            //object obj = Activator.CreateInstance(a);
            //Debug.Log($"�㏈���Q{obj} {Friend(obj)}");
            //List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            //foreach (var b in savableInss)
            //{
            //    Debug.Log($"======= �㏈���R ======={b}");
            //    b.ResetFirstLoading();
            //}
        }
    }
    #endregion
}











#region 1�y�Z�[�u�������N���X�Ɍp��������z ========================================================================

public interface ISave
{
    string GetPath();
    void Save();
    void Load();
}


#region 2�y�m�[�}���z ==============================================================

/// <summary>
/// �y���Ӂz
/// ������p�������N���X�ɂ͕K�����������̃f�t�H���g�R���X�g���N�^�������Ă����B
/// </summary>
[Serializable]
public abstract class Savable : MyExtention,
    SaveSystem.IFriendWith_SaveSystem,
    ISave
{
    #region �SSavable�n���N���X���ʂ̏���
    // ���ʃf�[�^
    [JsonProperty] protected string _Path { get; private set; } = string.Empty;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
    [JsonProperty] protected int InstanceNumber { get; private set; } = -1;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
    [JsonProperty] protected bool IsLoadedAtFirst { get; private set; } = false;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
                                                                                  // JsonUtility �̏ꍇ
                                                                                  //[SerializeField] protected string path = string.Empty;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�
                                                                                  //[SerializeField] protected int instanceNumber = -1;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�
                                                                                  //[SerializeField] protected bool isLoadedAtFirst = false;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�

    // �f�[�^�����܂��t�H���_�̖��O�B�w�肵�Ȃ��ꍇ�́uApplication.persistentDataPath�v�ɂȂ�B
    [JsonIgnore] public virtual string SaveFolderPath { get; set; } = string.Empty;
    // �f�[�^�̃t�@�C�����B�w�肵�Ȃ��ꍇ�́u�N���X��+instanceNumber�v�ɂȂ�
    [JsonIgnore] public virtual string FileName { get; set; } = string.Empty;

    // �����p�f�B�N�V���i���[
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    // �e�Z�[�u�\�N���X�ɗp�ӂ��� static�ȃC���X�^���X�Ǘ����X�g�̃Q�b�^�[
    // Instances ���z�Q�ƂɂȂ��Ă���̂�[JsonIgnore]�����ăV���A���C�Y����Ȃ��悤�ɂ���B�p����ŃI�[�o�[���C�h����ۂ͕t���Ȃ��Ă�����ɓ�������
    [JsonIgnore] public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

    //�����̃Z�[�u�t�@�C����ۑ�����ꏊ�̃p�X��Ԃ��B
    public string GetPath() { return _Path; }

    /// <summary>
    /// isFirstLoading�͊e�Z�[�u�\���N���X���Ƃɗp�ӂ��Ă��邽�ߒ��ۉ�������Ȃ��ϐ��B
    /// �Z�[�u�\�C���X�^���X�̌^����̓I�ɃL���X�Ƃ����ɊO������ύX����ꍇ�ɁA
    /// IFriendWith_SaveSystem ���p�ӂ����ύX�p�֐����g���̂��ǂ������������B
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        this.IsLoadedAtFirst = isLoadedAtFirst;
    }

    // �e�N���X�́A�����Ɏ����̊Ǘ����X�g��o�^����B
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ���[�h�����f�[�^�����񃍁[�h���ǂ����m�F���A����ł���Ώ���t���O������ύX���ۑ��B
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        if (!IsLoadedAtFirst)
        {
            Debug.Log($"{this} �̓A�v�����N�����Ă���P��ڂ̃��[�h");
            IsLoadedAtFirst = true;
            SaveSystem.Save(this);
        }
        else
        {
            Debug.Log($"{this} �̓A�v�����N����ɏ��Ȃ��Ƃ��P�񃍁[�h���ꂽ�悤�ł�");
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
            //�����f�B�N�V���i���𕪉����Ē��g�̃��X�g�̒��g�ɓ������C���X�^���X��1�̃��X�g�ɂ܂Ƃ߂Ȃ���
            list.AddRange(a);
        }
        return list;
    }
    #endregion


    public void Save()
    {
        SaveSystem.Save(this);
        Debug.Log("------�Z�[�u����------");
    }
    public void Load()
    {
        //�A�v�����N�����Ă���1��ڂ̃��[�h
        if (!IsLoadedAtFirst)
        {
            //�Ǘ����X�g�Ɏ�����o�^
            Instances.Add(this);
            //�����̃C���X�^���X�ԍ��Ə����t�H���_�ƃp�X��ݒ�
            InstanceNumber = Instances.Count - 1;
            //string AffiliatedFolderPath = @$"{SaveSystem.SaveFolderPath}/{AffiliatedFolderName}";
            if (string.IsNullOrEmpty(SaveFolderPath)) SaveFolderPath = Application.persistentDataPath;
            // �w�肵���p�X�̃t�H���_�����݂��Ȃ��ꍇ�쐬
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);
            // �����̊��S�ȃp�X���쐬
            if (string.IsNullOrEmpty(FileName)) _Path = @$"{SaveFolderPath}/{GetType().Name}{InstanceNumber}.json";
            else _Path = @$"{SaveFolderPath}/{FileName}.json";
        }

        ////���[�h
        SaveSystem.Load(this);

        ////�Ǘ����X�g�X�V
        Instances[InstanceNumber] = this;  //����Ȃ�����

        Debug.Log("------���[�h����------");
    }

    void SaveSystem.IFriendWith_SaveSystem.UpdateIndex()
    {
        InstanceNumber = Instances.IndexOf(this);
    }
}

#endregion 2�y�m�[�}���z ===========================================================






#region 2�y�R���|�[�l���g�p�z ==============================================================

[Serializable]
public abstract class SavableCompo : MonoBehaviourMyExtention,
    SaveSystem.IFriendWith_SaveSystem,
    ISave
{
    #region �SSavable�n���N���X���ʂ̏���
    // ���ʃf�[�^
    //[JsonProperty] protected string _Path { get; private set; } = string.Empty;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
    //[JsonProperty] protected int InstanceNumber { get; private set; } = -1;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
    //[JsonProperty] protected bool IsLoadedAtFirst { get; private set; } = false;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
    // JsonUtility �̏ꍇ
    [SerializeField] public string _Path = string.Empty;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�
    [SerializeField] public int InstanceNumber = -1;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�
    [SerializeField] public bool IsLoadedAtFirst = false;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�

    // �f�[�^�����܂��t�H���_�̖��O�B�w�肵�Ȃ��ꍇ�́uApplication.persistentDataPath�v�ɂȂ�B
    public virtual string SaveFolderPath { get; set; } = string.Empty;
    // �f�[�^�̃t�@�C�����B�w�肵�Ȃ��ꍇ�́u�N���X��+instanceNumber�v�ɂȂ�
    public virtual string FileName { get; set; } = string.Empty;

    // �����p�f�B�N�V���i���[
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    // �e�Z�[�u�\�N���X�ɗp�ӂ��� static�ȃC���X�^���X�Ǘ����X�g�̃Q�b�^�[
    // Instances ���z�Q�ƂɂȂ��Ă���̂�[JsonIgnore]�����ăV���A���C�Y����Ȃ��悤�ɂ���B�p����ŃI�[�o�[���C�h����ۂ͕t���Ȃ��Ă�����ɓ�������
    public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }


    //�����̃Z�[�u�t�@�C����ۑ�����ꏊ�̃p�X��Ԃ��B
    public string GetPath() { return _Path; }

    /// <summary>
    /// isFirstLoading�͊e�Z�[�u�\���N���X���Ƃɗp�ӂ��Ă��邽�ߒ��ۉ�������Ȃ��ϐ��B
    /// �Z�[�u�\�C���X�^���X�̌^����̓I�ɃL���X�Ƃ����ɊO������ύX����ꍇ�ɁA
    /// IFriendWith_SaveSystem ���p�ӂ����ύX�p�֐����g���̂��ǂ������������B
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        IsLoadedAtFirst = isLoadedAtFirst;
    }

    // �e�N���X�́A�����Ɏ����̊Ǘ����X�g��o�^����B
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ���[�h�����f�[�^�����񃍁[�h���ǂ����m�F���A����ł���Ώ���t���O������ύX���ۑ��B
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        //IsLoadedAtFirst = true;

        //if (!((SavableCompo)Instances[InstanceNumber]).IsLoadedAtFirst)
        if (!IsLoadedAtFirst)

        {
            Debug.Log($"{this} �̓A�v�����N�����Ă���P��ڂ̃��[�h");
            //((SavableCompo)Instances[InstanceNumber]).IsLoadedAtFirst = true;
            IsLoadedAtFirst = true;

            Debug.Log($"{_Path}");
            SaveSystem.Save_Compo(this);
        }
        else
        {
            Debug.Log($"{this} �̓A�v�����N����ɏ��Ȃ��Ƃ��P�񃍁[�h���ꂽ�悤�ł�");
        }
    }


    void SaveSystem.IFriendWith_SaveSystem.ResetFirstLoading()
    {
        Debug.Log($"���Z�b�g�t�@�[�X�g���[�f�B���O{_Path}");
        foreach (var a in InstancesListsManagementDictionaty.Values)
        {
            foreach (var b in a)
            {
                Debug.Log($"�Q�[���I�u�W�F�N�g {((MonoBehaviour)b).gameObject}");
                b.SetFirstLoading(false);
                SaveSystem.Save_Compo((ISave)b);
            }
        }
    }
    //protected void ResetFirstLoading()
    //{
    //    //Debug.Log($"�Q�[���I�u�W�F�N�g {gameObject}");
    //    //SaveSystem.Save_Compo(this);

    //    foreach (var a in InstancesListsManagementDictionaty.Values)
    //    {
    //        foreach (var b in a)
    //        {
    //            Debug.Log($"�Q�[���I�u�W�F�N�g {((MonoBehaviour)b).gameObject}");
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
            //�����f�B�N�V���i���𕪉����Ē��g�̃��X�g�̒��g�ɓ������C���X�^���X��1�̃��X�g�ɂ܂Ƃ߂Ȃ���
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

        Debug.Log("------�Z�[�u����------");
    }

    public void Load()
    {
        //�A�v�����N�����Ă���1��ڂ̃��[�h
        if (!IsLoadedAtFirst)
        {
            Debug.Log($"{this} �̓A�v�����N�����Ă���P��ڂ̃��[�h");
            //�Ǘ����X�g�Ɏ�����o�^
            Instances.Add(this);
            //�����̃C���X�^���X�ԍ��Ə����t�H���_�ƃp�X��ݒ�
            InstanceNumber = Instances.Count - 1;
            //string AffiliatedFolderPath = @$"{SaveSystem.SaveFolderPath}/{AffiliatedFolderName}";
            if (string.IsNullOrEmpty(SaveFolderPath)) SaveFolderPath = Application.persistentDataPath;
            // �w�肵���p�X�̃t�H���_�����݂��Ȃ��ꍇ�쐬
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);
            // �����̊��S�ȃp�X���쐬
            if (string.IsNullOrEmpty(FileName)) _Path = @$"{SaveFolderPath}/{GetType().Name}{InstanceNumber}.json";
            else _Path = @$"{SaveFolderPath}/{FileName}.json";
        }

        //�Ǘ����X�g�X�V;
        //Instances[InstanceNumber] = (SavableCompo)((SavableCompo)Instances[InstanceNumber]).gameObject.AddComponent(GetType());

        //���[�h
        //SaveSystem.Load(this, (ISave)Instances[InstanceNumber]);
        SaveSystem.Load_Compo(this);

        // ����������
        //DestroyImmediate(this);

        ///<summary>
        ///�@Load�����̒��Ō��݂̂��̃R���|�[�l���g�͔j�󂳂�邽�߁A
        ///�@��������ȍ~�̏����͒ʂ�Ȃ��̂ŁA���[�h���Ō�ɏ���
        /// </summary>
        Debug.Log("------���[�h����------");
    }

    void SaveSystem.IFriendWith_SaveSystem.UpdateIndex()
    {
        InstanceNumber = Instances.IndexOf(this);
    }


    // MonoBehaviour �̊֐��͓��A�Z���u�����̎q�N���X�ɂ�����p���Ȃ��炵���A
    // �p�b�P�[�W�O�Ŗ{�N���X���̎q�N���X����������ꍇ�́A���̃N���X���ɏ����K�v������B
    //void OnApplicationQuit()
    //{
    //    //ResetFirstLoading();
    //}
}

#endregion 2�y�R���|�[�l���g�p�z ===========================================================






#region 2�y�V���O���g���p�z ==============================================================

[Serializable]
public abstract class SavableSingleton<SingletonType> : Singleton<SingletonType>,
    SaveSystem.IFriendWith_SaveSystem,
    ISave
     where SingletonType : Singleton<SingletonType>, new()
{
    #region �SSavable�n���N���X���ʂ̏���
    // ���ʃf�[�^
    [JsonProperty] protected string _Path { get; private set; } = string.Empty;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
    [JsonProperty] protected int InstanceNumber { get; private set; } = -1;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
    [JsonProperty] protected bool IsLoadedAtFirst { get; private set; } = false;  //Json.NET ���� public �ȊO�V���A�A���C�Y����Ȃ����A[JsonProperty]������Ɗ֌W�Ȃ�������
                                                                                  // JsonUtility �̏ꍇ
                                                                                  //[SerializeField] protected string path = string.Empty;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�
                                                                                  //[SerializeField] protected int instanceNumber = -1;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�
                                                                                  //[SerializeField] protected bool isLoadedAtFirst = false;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�

    // �f�[�^�����܂��t�H���_�̖��O
    [JsonIgnore] public virtual string SaveFolderPath { get; set; } = string.Empty;
    // �f�[�^�̃t�@�C�����B�w�肵�Ȃ��ꍇ�́u�N���X��+instanceNumber�v�ɂȂ�
    [JsonIgnore] public virtual string FileName { get; set; } = string.Empty;

    // �����p�f�B�N�V���i���[
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    // �e�Z�[�u�\�N���X�ɗp�ӂ��� static�ȃC���X�^���X�Ǘ����X�g�̃Q�b�^�[
    // Instances ���z�Q�ƂɂȂ��Ă���̂�[JsonIgnore]�����ăV���A���C�Y����Ȃ��悤�ɂ���B�p����ŃI�[�o�[���C�h����ۂ͕t���Ȃ��Ă�����ɓ�������
    [JsonIgnore] public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

    //�����̃Z�[�u�t�@�C����ۑ�����ꏊ�̃p�X��Ԃ��B
    public string GetPath() { return _Path; }

    /// <summary>
    /// isFirstLoading�͊e�Z�[�u�\���N���X���Ƃɗp�ӂ��Ă��邽�ߒ��ۉ�������Ȃ��ϐ��B
    /// �Z�[�u�\�C���X�^���X�̌^����̓I�ɃL���X�Ƃ����ɊO������ύX����ꍇ�ɁA
    /// IFriendWith_SaveSystem ���p�ӂ����ύX�p�֐����g���̂��ǂ������������B
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        this.IsLoadedAtFirst = isLoadedAtFirst;
    }

    // �e�N���X�́A�����Ɏ����̊Ǘ����X�g��o�^����B
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ���[�h�����f�[�^�����񃍁[�h���ǂ����m�F���A����ł���Ώ���t���O������ύX���ۑ��B
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        if (!IsLoadedAtFirst)
        {
            Debug.Log($"{this} �̓A�v�����N�����Ă���P��ڂ̃��[�h");
            IsLoadedAtFirst = true;
            SaveSystem.Save(this);
        }
        else
        {
            Debug.Log($"{this} �̓A�v�����N����ɏ��Ȃ��Ƃ��P�񃍁[�h���ꂽ�悤�ł�");
        }

        Debug.Log($"{IsLoadedAtFirst}");
    }

    void SaveSystem.IFriendWith_SaveSystem.ResetFirstLoading()
    {
        Debug.Log($"���Z�b�g�t�@�[�X�g���[�f�B���O{_Path}");
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
            //�����f�B�N�V���i���𕪉����Ē��g�̃��X�g�̒��g�ɓ������C���X�^���X��1�̃��X�g�ɂ܂Ƃ߂Ȃ���
            list.AddRange(a);
        }
        return list;
    }
    #endregion


    public void Save()
    {
        SaveSystem.Save(this);
        Debug.Log("------�Z�[�u����------");
    }
    public void Load()
    {
        //�A�v�����N�����Ă���1��ڂ̃��[�h
        if (!IsLoadedAtFirst)
        {
            //�Ǘ����X�g�Ɏ�����o�^
            Instances.Add(this);
            //�����̃C���X�^���X�ԍ��Ə����t�H���_�ƃp�X��ݒ�
            InstanceNumber = Instances.Count - 1;
            //string AffiliatedFolderPath = @$"{SaveSystem.SaveFolderPath}/{AffiliatedFolderName}";
            if (string.IsNullOrEmpty(SaveFolderPath)) SaveFolderPath = Application.persistentDataPath;
            // �w�肵���p�X�̃t�H���_�����݂��Ȃ��ꍇ�쐬
            if (!Directory.Exists(SaveFolderPath)) Directory.CreateDirectory(SaveFolderPath);
            // �����̊��S�ȃp�X���쐬
            if (string.IsNullOrEmpty(FileName)) _Path = @$"{SaveFolderPath}/{GetType().Name}{InstanceNumber}.json";
            else _Path = @$"{SaveFolderPath}/{FileName}.json";
        }

        ////���[�h
        SaveSystem.Load(this);

        ////�Ǘ����X�g�X�V
        Instances[InstanceNumber] = this;  //����Ȃ�����
        Debug.Log("------���[�h����------");
    }

    void SaveSystem.IFriendWith_SaveSystem.UpdateIndex()
    {
        InstanceNumber = Instances.IndexOf(this);
    }
}

#endregion 2�y�V���O���g���p�z ===========================================================







#region 2�y�V���O���g���R���|�[�l���g�p�z ==============================================================

/// <summary>
/// �y���Ӂz
/// �܂����p�I�ł͂Ȃ��B��U����
/// </summary>
/// <typeparam name="SingletonType"></typeparam>
//[Serializable]
//public abstract class SavableSingletonCompo<SingletonType> : SingletonCompo<SingletonType>,
//    SaveSystem.IFriendWith_SaveSystem,
//    ISave
//     where SingletonType : SingletonCompo<SingletonType>, new()
//{
//    #region �SSavable�n���N���X���ʂ̏���
//    //���ʃf�[�^
//    [SerializeField] protected string path = string.Empty;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B
//    [SerializeField] protected int instanceNumber = -1;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B
//    [SerializeField] protected bool isLoadedAtFirst = false;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B


//    //�����p�f�B�N�V���i���[
//    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

//    //�e�Z�[�u�\�N���X�ɗp�ӂ��� static�ȃC���X�^���X�Ǘ����X�g�̃Q�b�^�[
//    public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

//    //�����̃Z�[�u�t�@�C����ۑ�����ꏊ�̃p�X��Ԃ��B
//    public string GetPath() { return path; }

//    /// <summary>
//    /// isFirstLoading�͊e�Z�[�u�\���N���X���Ƃɗp�ӂ��Ă��邽�ߒ��ۉ�������Ȃ��ϐ��B
//    /// �Z�[�u�\�C���X�^���X�̌^����̓I�ɃL���X�Ƃ����ɊO������ύX����ꍇ�ɁA
//    /// IFriendWith_SaveSystem ���p�ӂ����ύX�p�֐����g���̂��ǂ������������B
//    /// </summary>
//    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
//    {
//        this.isLoadedAtFirst = isLoadedAtFirst;
//    }

//    //�e�N���X�́A�����Ɏ����̊Ǘ����X�g��o�^����B
//    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
//    {
//        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
//    }


//    // ���[�h�����f�[�^�����񃍁[�h���ǂ����m�F���A����ł���Ώ���t���O������ύX���ۑ��B
//    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
//    {
//        if (!isLoadedAtFirst)
//        {
//            Debug.Log($"{this} �̓A�v�����N�����Ă���P��ڂ̃��[�h");
//            isLoadedAtFirst = true;
//            SaveSystem.Save(this);
//        }
//        else
//        {
//            Debug.Log($"{this} �̓A�v�����N����ɏ��Ȃ��Ƃ��P�񃍁[�h���ꂽ�悤�ł�");
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
//            //�����f�B�N�V���i���𕪉����Ē��g�̃��X�g�̒��g�ɓ������C���X�^���X��1�̃��X�g�ɂ܂Ƃ߂Ȃ����B
//            list.AddRange(a);
//        }
//        return list;
//    }
//    #endregion


//    public void Save()
//    {
//        SaveSystem.Save(this);
//        Debug.Log("------�Z�[�u����------");
//    }
//    public void Load()
//    {
//        //�A�v�����N�����Ă���1��ڂ̃��[�h
//        if (!isLoadedAtFirst)
//        {
//            //�Ǘ����X�g�ɒǉ��B
//            Instances.Add(this);

//            //�����̃C���X�^���X�ԍ��ƃp�X��ݒ肷��B
//            instanceNumber = Instances.Count - 1;
//            Debug.Log($"�C���X�^���X�i���o�[��    {instanceNumber}  �ɂȂ���");
//            path = @$"{SaveSystem.SaveFolderPath}{GetType().Name}{instanceNumber}.json";
//        }


//        //�Ǘ����X�g�X�V
//        MonoBehaviour afterData = (MonoBehaviour)gameObject.AddComponent(GetType());
//        Instances[instanceNumber] = (SaveSystem.IFriendWith_SaveSystem)afterData;

//        //���[�h
//        SaveSystem.Load(this, afterData);
//        ///<summary>
//        ///�@Load�����̒��Ō��݂̂��̃R���|�[�l���g�͔j�󂳂�邽�߁A
//        ///�@��������ȍ~�̏����͒ʂ�Ȃ��̂ŁA���[�h���Ō�ɏ���
//        /// </summary>

//        Debug.Log("------���[�h����------");
//    }
//}

#endregion 2�y�V���O���g���R���|�[�l���g�p�z ===========================================================


#endregion 1�y�Z�[�u�������N���X�Ɍp��������z =====================================================================
