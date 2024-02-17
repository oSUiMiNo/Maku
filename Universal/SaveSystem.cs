using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEditor;
using Unity.Plastic.Newtonsoft.Json;
//using PlasticGui.Configuration.CloudEdition;

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
        List<IFriendWith_SaveSystem> GetAllInstances();
    }
    static IFriendWith_SaveSystem Friend(object obj)
    {
        return (IFriendWith_SaveSystem)obj;
    }

    static IEnumerable<Type> SavableBaseTypes;

    /// <summary>
    /// �y���Ӂz
    /// �����Ŏw�肵���p�X�́A�Z�[�u�f�[�^�����܂��p�̃t�H���_��K������Ă����B
    /// �y�����z
    /// �v���p�e�B�Ƃ��Ă��΁A�ÓI�ȏꏊ�ł� $" " ���g����
    /// </summary>
    //public static string SaveFolderPath => @"C:\Users\vantan\Documents\Unity\Maku\ChessN7\Assets\SaveFiles\";
    public static string SaveFolderPath => $"{Application.persistentDataPath}/";

    protected override void SubLateAwake()
    {
        //DebugView.Log($"�Z�[�u�t�@�C���ǂ�    {SaveFolderPath}");
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
    void GetSavableBaseTypes()
    {
        SavableBaseTypes = System.Reflection.Assembly
           .GetAssembly(typeof(IFriendWith_SaveSystem))
           .GetTypes()
           .Where(t =>
           {
               return
                   t.GetInterfaces().Contains(typeof(IFriendWith_SaveSystem)) &&
                   !t.IsAbstract;
           });
    }
    void InitSavables()
    {
        foreach (var a in SavableBaseTypes)
        {
            object obj = Activator.CreateInstance(a);
            Friend(obj).SetManagementDictionaty();
        }
    }
    #endregion



    #region �Z�[�u Json.NET��
    //public static void Save(ISave data)
    //{
    //    DebugView.Log($"---SaveSystem  �Z�[�u---");
    //    ///<summary>
    //    ///�yStreamWriter �̎g�����z
    //    /// sw.Write��sw.WriteLine�Ə������ƂŁA�e�L�X�g�ɕ������o�͂��邱�Ƃ��ł���B
    //    /// ���s���Ȃ��Ƃ��́AWrite
    //    /// ���s����Ƃ��́AWriteLine
    //    /// </summary>
    //    string Path = data.GetPath();
    //    string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented); // �z�Q�Ɓi�o�����̑��ݎQ�Ɓj�����Ƃ����ăG���[�������o�Ă��܂�
    //    StreamWriter streamWriter = new(Path, false);
    //    streamWriter.WriteLine(jsonData);
    //    streamWriter.Flush();
    //    streamWriter.Close();
    //}
    #endregion



    #region ���[�h Json.NET�� ( �ʏ�̃N���X�p )
    //public static void Load(ISave data)
    //{
    //    DebugView.Log($"---SaveSystem  ���[�h---");
    //    string Path = data.GetPath();
    //    if (!File.Exists(Path))
    //    {
    //        DebugView.Log("-----------�ŏ��񃍁[�h-----------");
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
    //        JsonConvert.PopulateObject(jsonData, data);
    //        ///<summary>
    //        /// �ȉ��̏����ɂ̓Z�[�u�̏������܂܂�Ă���B
    //        /// ���̏������A���[�h�����O�Ɏ����Ă��Ă��܂��ƁA
    //        /// ���[�h�O�̃f�[�^���Z�[�u���ꂽ��ԂŃ��[�h�����Ɉڂ��Ă��܂��B
    //        /// </summary>
    //        Friend(data).CheckFirstLoading();
    //    }
    //}
    #endregion



    #region �Z�[�u
    public static void Save(ISave data)
    {
        DebugView.Log($"---SaveSystem  �Z�[�u---");
        ///<summary>
        ///�yStreamWriter �̎g�����z
        /// sw.Write��sw.WriteLine�Ə������ƂŁA�e�L�X�g�ɕ������o�͂��邱�Ƃ��ł���B
        /// ���s���Ȃ��Ƃ��́AWrite
        /// ���s����Ƃ��́AWriteLine
        /// </summary>
        string Path = data.GetPath();
        //string jsonData = EditorJsonUtility.ToJson(data);
        string jsonData = JsonUtility.ToJson(data, true);
        StreamWriter streamWriter = new(Path, false);
        streamWriter.WriteLine(jsonData);
        streamWriter.Flush();
        streamWriter.Close();
    }
    #endregion



    #region ���[�h ( �ʏ�̃N���X�p )
    public static void Load(ISave data)
    {
        DebugView.Log($"---SaveSystem  ���[�h---");
        string Path = data.GetPath();
        if (!File.Exists(Path))
        {
            DebugView.Log("-----------�ŏ��񃍁[�h-----------");
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
            JsonUtility.FromJsonOverwrite(jsonData, data);
            ///<summary>
            /// �ȉ��̏����ɂ̓Z�[�u�̏������܂܂�Ă���B
            /// ���̏������A���[�h�����O�Ɏ����Ă��Ă��܂��ƁA
            /// ���[�h�O�̃f�[�^���Z�[�u���ꂽ��ԂŃ��[�h�����Ɉڂ��Ă��܂��B
            /// </summary>
            Friend(data).CheckFirstLoading();
        }
    }
    #endregion



    #region ���[�h ( MonoBehaviour�p )
    //public static void Load(MonoBehaviour beforeData, MonoBehaviour afterData)
    //{
    //    Debug.Log($"---SaveSystem  ���[�h---");
    //    ISave before = (ISave)beforeData;
    //    ISave after = (ISave)afterData;

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

    //    StreamReader streamReader = new(Path);
    //    string jsonData = streamReader.ReadToEnd();
    //    streamReader.Close();
    //    Destroy(beforeData.gameObject.GetComponent(beforeData.GetType()));
    //    EditorJsonUtility.FromJsonOverwrite(jsonData, after);

    //    Friend(after).CheckFirstLoading();
    //    afterData.enabled = true;
    //}
    #endregion
    


    #region �㏈��
    void ResetSavables()
    {
        foreach (var a in SavableBaseTypes)
        {
            object obj = Activator.CreateInstance(a);
            List<IFriendWith_SaveSystem> savableInss = Friend(obj).GetAllInstances();

            foreach (var b in savableInss)
            {
                b.ResetFirstLoading();
            }
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
    //���ʃf�[�^
    [SerializeField] protected string path = string.Empty;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B
    [SerializeField] protected int instanceNumber = -1;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B
    [SerializeField] protected bool isLoadedAtFirst = false;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B


    //�����p�f�B�N�V���i���[
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    //�e�Z�[�u�\�N���X�ɗp�ӂ��� static�ȃC���X�^���X�Ǘ����X�g�̃Q�b�^�[
    public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

    //�����̃Z�[�u�t�@�C����ۑ�����ꏊ�̃p�X��Ԃ��B
    public string GetPath() { return path; }

    /// <summary>
    /// isFirstLoading�͊e�Z�[�u�\���N���X���Ƃɗp�ӂ��Ă��邽�ߒ��ۉ�������Ȃ��ϐ��B
    /// �Z�[�u�\�C���X�^���X�̌^����̓I�ɃL���X�Ƃ����ɊO������ύX����ꍇ�ɁA
    /// IFriendWith_SaveSystem ���p�ӂ����ύX�p�֐����g���̂��ǂ������������B
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        this.isLoadedAtFirst = isLoadedAtFirst;
    }

    //�e�N���X�́A�����Ɏ����̊Ǘ����X�g��o�^����B
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ���[�h�����f�[�^�����񃍁[�h���ǂ����m�F���A����ł���Ώ���t���O������ύX���ۑ��B
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        if (!isLoadedAtFirst)
        {
            Debug.Log($"{this} �̓A�v�����N�����Ă���P��ڂ̃��[�h");
            isLoadedAtFirst = true;
            SaveSystem.Save(this);
        }
        else
        {
            Debug.Log($"{this} �̓A�v�����N����ɏ��Ȃ��Ƃ��P�񃍁[�h���ꂽ�悤�ł�");
        }

        Debug.Log($"{isLoadedAtFirst}");
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
            //�����f�B�N�V���i���𕪉����Ē��g�̃��X�g�̒��g�ɓ������C���X�^���X��1�̃��X�g�ɂ܂Ƃ߂Ȃ����B
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
        if (!isLoadedAtFirst)
        {
            //�Ǘ����X�g�Ɏ�����o�^�B
            Instances.Add(this);

            //�����̃C���X�^���X�ԍ��ƃp�X��ݒ肷��B
            instanceNumber = Instances.Count - 1;
            path = @$"{SaveSystem.SaveFolderPath}{GetType().Name}{instanceNumber}.json";
        }

        ////���[�h
        SaveSystem.Load(this);

        ////�Ǘ����X�g�X�V
        Instances[instanceNumber] = this;  //����Ȃ�����
        Debug.Log("------���[�h����------");
    }
}

#endregion 2�y�m�[�}���z ===========================================================






#region 2�y�R���|�[�l���g�p�z ==============================================================

//[Serializable]
//public abstract class SavableCompo : MonoBehaviourMyExtention,
//    SaveSystem.IFriendWith_SaveSystem,
//    ISave
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

#endregion 2�y�R���|�[�l���g�p�z ===========================================================






#region 2�y�V���O���g���p�z ==============================================================

[Serializable]
public abstract class SavableSingleton<SingletonType> : Singleton<SingletonType>,
    SaveSystem.IFriendWith_SaveSystem,
    ISave
     where SingletonType : Singleton<SingletonType>, new()
{
    #region �SSavable�n���N���X���ʂ̏���
    //���ʃf�[�^
    [SerializeField] protected string path = string.Empty;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B
    [SerializeField] protected int instanceNumber = -1;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B
    [SerializeField] protected bool isLoadedAtFirst = false;  //private �ɂ�����ۑ����ꂸ�G���[�ɂȂ�B


    //�����p�f�B�N�V���i���[
    public static Dictionary<string, List<SaveSystem.IFriendWith_SaveSystem>> InstancesListsManagementDictionaty = new();

    //�e�Z�[�u�\�N���X�ɗp�ӂ��� static�ȃC���X�^���X�Ǘ����X�g�̃Q�b�^�[
    public abstract List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; }

    //�����̃Z�[�u�t�@�C����ۑ�����ꏊ�̃p�X��Ԃ��B
    public string GetPath() { return path; }

    /// <summary>
    /// isFirstLoading�͊e�Z�[�u�\���N���X���Ƃɗp�ӂ��Ă��邽�ߒ��ۉ�������Ȃ��ϐ��B
    /// �Z�[�u�\�C���X�^���X�̌^����̓I�ɃL���X�Ƃ����ɊO������ύX����ꍇ�ɁA
    /// IFriendWith_SaveSystem ���p�ӂ����ύX�p�֐����g���̂��ǂ������������B
    /// </summary>
    void SaveSystem.IFriendWith_SaveSystem.SetFirstLoading(bool isLoadedAtFirst)
    {
        this.isLoadedAtFirst = isLoadedAtFirst;
    }

    //�e�N���X�́A�����Ɏ����̊Ǘ����X�g��o�^����B
    void SaveSystem.IFriendWith_SaveSystem.SetManagementDictionaty()
    {
        InstancesListsManagementDictionaty.Add($"{GetType().Name}s", Instances);
    }


    // ���[�h�����f�[�^�����񃍁[�h���ǂ����m�F���A����ł���Ώ���t���O������ύX���ۑ��B
    void SaveSystem.IFriendWith_SaveSystem.CheckFirstLoading()
    {
        if (!isLoadedAtFirst)
        {
            Debug.Log($"{this} �̓A�v�����N�����Ă���P��ڂ̃��[�h");
            isLoadedAtFirst = true;
            SaveSystem.Save(this);
        }
        else
        {
            Debug.Log($"{this} �̓A�v�����N����ɏ��Ȃ��Ƃ��P�񃍁[�h���ꂽ�悤�ł�");
        }

        Debug.Log($"{isLoadedAtFirst}");
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
            //�����f�B�N�V���i���𕪉����Ē��g�̃��X�g�̒��g�ɓ������C���X�^���X��1�̃��X�g�ɂ܂Ƃ߂Ȃ����B
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
        if (!isLoadedAtFirst)
        {
            //�Ǘ����X�g�Ɏ�����o�^�B
            Instances.Add(this);

            //�����̃C���X�^���X�ԍ��ƃp�X��ݒ肷��B
            instanceNumber = Instances.Count - 1;
            path = @$"{SaveSystem.SaveFolderPath}{GetType().Name}{instanceNumber}.json";
        }

        ////���[�h
        SaveSystem.Load(this);

        ////�Ǘ����X�g�X�V
        Instances[instanceNumber] = this;  //����Ȃ�����
        Debug.Log("------���[�h����------");
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