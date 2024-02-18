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
        [JsonIgnore] // Instances ���z�Q�ƂɂȂ��Ă���̂�[JsonIgnore]�����ăV���A���C�Y����Ȃ��悤�ɂ���
        public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
        public static List<SaveSystem.IFriendWith_SaveSystem> instances = new();


        #region �f�[�^
        public string UserName = "Default Name";
        public int UserRank = 0;
        public List<string> Items = new List<string>() { "��", "����" };
        public List<Friend> Friends = new List<Friend>();
        [JsonProperty] // �X�^�e�B�b�N�ȃf�[�^���V���A�A���C�Y�������ꍇ��[JsonProperty]������
        public static string TestStatic = "�X�^�e�B�b�N�f�[�^�̃e�X�g";
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
            Debug.Log("--------- �v���C���[0 �̏�� --------------------------------");
            Debug.Log($"���[�U�[�� : {p0.UserName}");
            Debug.Log($"���[�U�[�����N : {p0.UserRank}");
            string items0 = string.Empty;
            p0.Items.ForEach(a => items0 += $"{a} ");
            Debug.Log($"�A�C�e�� : {items0}");
            string friends0 = string.Empty;
            p0.Friends.ForEach(a => friends0 += $"{a.Name},{a.Rank} ");
            Debug.Log($"�t�����h : {friends0}");
            Debug.Log($"�X�^�e�B�b�N : {PlayerData.TestStatic}");
            Debug.Log("-----------------------------------------");

            Debug.Log("--------- �v���C���[1 �̏�� --------------------------------");
            Debug.Log($"���[�U�[�� : {p1.UserName}");
            Debug.Log($"���[�U�[�����N : {p1.UserRank}");
            string items1 = string.Empty;
            Debug.Log($"�A�C�e��0 : {items1}");
            p1.Items.ForEach(a => items1 += $"{a} ");
            Debug.Log($"�A�C�e�� : {items1}");
            string friends1 = string.Empty;
            p1.Friends.ForEach(a => friends1 += $"{a.Name},{a.Rank} ");
            Debug.Log($"�t�����h : {friends1}");
            Debug.Log($"�X�^�e�B�b�N : {PlayerData.TestStatic}");
            Debug.Log("-----------------------------------------");
        };
        InputEventHandler.OnDown_X += () =>
        {
            Debug.Log("�f�[�^�X�V�����B");
            p0.UserName = "������";
            p0.UserRank += 10;
            p0.Items.Add("�o�C�L���~��");
            p0.Friends.Add(new Friend("�܂�", 8));
            p0.Friends.Add(new Friend("���܂�", 15));
            PlayerData.TestStatic = "�X�^�e�B�b�N�f�[�^�ύX����";
        };
        InputEventHandler.OnDown_V += () =>
        {
            if (PlayerData.instances.Count == 0)
            {
                Debug.Log("�Z�[�u�f�[�^�̃C���X�^���X����");
                return;
            }
            foreach (var a in PlayerData.instances)
            {
                Debug.Log($"{a}");
            }
        };
    }
}
