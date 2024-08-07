using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;


// �d�v����
// �J�^���O�����烉���^�C���ɐ��������Q�[���I�u�W�F�N�g�̃C���f�b�N�X���Ǘ����ăZ�[�u����N���X����ō쐬


public class EditorbleInfo : MonoBehaviourMyExtention
{
    List<string> ExistingContentIDList = new List<string>();

    public string id;
    public string address;

    public void CreateID(string address)
    {
        if(!Editorbles.IDListDict.ContainsKey(address))
        {
            Debug.Log($"�A�h���X����{address}");
            Editorbles.IDListDict.Add(address, new List<string>());
        }
        this.address = address;
        id = $"{address}__{Editorbles.IDListDict[address].Count}";
        ExistingContentIDList.Add(id);
        Editorbles.IDListDict[address].Add(id);
    }

    public void SetID(string id, string address)
    {
        ExistingContentIDList.Add(id);
        int index = int.Parse(CropStr_R(id, "__", false));
        Debug.Log($"{address}_{index}, {id}, {Editorbles.IDListDict[address][index]}");
        this.address = address;
        this.id = id;
    }
}
