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
        
        // �ȉ��̏������ł̓_���H���ő�����Ă���instances�̓X�^�e�B�b�N����Ȃ��Ƃ����Ȃ��́H�Ǝv�������A
        // ���炭�ȉ��̂悤�ɏ����Ɩ{�N���X�̃C���X�^���X�������x�ɊǗ��p��Instances�������Ă��܂����A
        // ����͂P�N���X�P���X�g�ɂ�������������
        // public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = new();

        #region �f�[�^
        public string UserName = "Default Name";
        public int UserRank = 0;
        [JsonProperty] // �X�^�e�B�b�N�ȃf�[�^���V���A�A���C�Y�������ꍇ��[JsonProperty]������
        public static string Job = "�A���P�~�X�g";
        public List<string> Items = new List<string>() { "��", "����" };
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
            Debug.Log("--------- �v���C���[0 �̏�� --------------------------------");
            Debug.Log($"���[�U�[�� : {p0.UserName}");
            Debug.Log($"���[�U�[�����N : {p0.UserRank}");
            string items0 = string.Empty;
            p0.Items.ForEach(a => items0 += $"{a} ");
            Debug.Log($"�A�C�e�� : {items0}");
            string friends0 = string.Empty;
            foreach (var a in p0.Friends)
            {
                friends0 += $"{a.Key},{a.Value.Job},{a.Value.Rank} ";
            }
            Debug.Log($"�t�����h : {friends0}");
            Debug.Log($"�X�^�e�B�b�N : {PlayerData.Job}");
            Debug.Log("-----------------------------------------");

            Debug.Log("--------- �v���C���[1 �̏�� --------------------------------");
            Debug.Log($"���[�U�[�� : {p1.UserName}");
            Debug.Log($"���[�U�[�����N : {p1.UserRank}");
            string items1 = string.Empty;
            Debug.Log($"�A�C�e��0 : {items1}");
            p1.Items.ForEach(a => items1 += $"{a} ");
            Debug.Log($"�A�C�e�� : {items1}");
            string friends1 = string.Empty;
            foreach(var a in p1.Friends)
            {
                friends1 += $"{a.Key},{a.Value.Job},{a.Value.Rank} ";
            }
            Debug.Log($"�t�����h : {friends1}");
            Debug.Log($"�X�^�e�B�b�N : {PlayerData.Job}");
            Debug.Log("-----------------------------------------");
        };
        InputEventHandler.OnDown_X += () =>
        {
            Debug.Log("�f�[�^�X�V�����B");
            p0.UserName = "������";
            p0.UserRank += 10;
            p0.Items.Add("�o�C�L���~��");
            p0.Friends.Add("�܂�", new Friend("�G�������g�A�[�`���[", 8));
            p0.Friends.Add("���܂�", new Friend("�V�[���h�Z�[�W", 15));
            PlayerData.Job = "�n�C�Z�v�^�[";
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
