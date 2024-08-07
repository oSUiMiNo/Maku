using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;


// 重要メモ
// カタログ等からランタイムに生成したゲームオブジェクトのインデックスを管理してセーブするクラスを後で作成


public class EditorbleInfo : MonoBehaviourMyExtention
{
    List<string> ExistingContentIDList = new List<string>();

    public string id;
    public string address;

    public void CreateID(string address)
    {
        if(!Editorbles.IDListDict.ContainsKey(address))
        {
            Debug.Log($"アドレス無し{address}");
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
