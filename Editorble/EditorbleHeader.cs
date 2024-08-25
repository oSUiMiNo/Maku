using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;


// �d�v����
// �J�^���O�����烉���^�C���ɐ��������Q�[���I�u�W�F�N�g�̃C���f�b�N�X���Ǘ����ăZ�[�u����N���X����ō쐬


public class EditorbleHeader : MonoBehaviourMyExtention
{
    //List<string> ExistingContentIDList = new List<string>();

    public string id;
    public string address;

    public void CreateID(string address)
    {
        if(!Editorbles.Ins.IDListDict.ContainsKey(address))
        {
            Debug.Log($"�A�h���X����{address}");
            Editorbles.Ins.IDListDict.Add(address, new List<string>());
        }
        this.address = address;
        id = $"{address}__{Editorbles.Ins.IDListDict[address].Count}";
        //ExistingContentIDList.Add(id);
        Editorbles.Ins.IDListDict[address].Add(id);
    }

    public void SetID(string id, string address)
    {
        //ExistingContentIDList.Add(id);
        //int index = int.Parse(CropStr_R(id, "__", false)) - 1;
        int index = int.Parse(CropStr_R(id, "__", false));
        Debug.Log($"�Z�b�gID�@��������������������������������������������������");
        Debug.Log($"{address}");
        Debug.Log($"{address}_{index}");
        Debug.Log($"{id}");
        Debug.Log($"{Editorbles.Ins.IDListDict[address][index]}");
        this.address = address;
        this.id = id;
    }

    public void Delete()
    {
        //Editorbles.IDListDict[address].Remove(id);
        EditorblesHandler.Compo.Delete(id);
        Destroy(gameObject);
    }
}
