using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;


// 重要メモ
// カタログ等からランタイムに生成したゲームオブジェクトのインデックスを管理してセーブするクラスを後で作成


public class EditorbleHeader : MonoBehaviourMyExtention
{
    //List<string> ExistingContentIDList = new List<string>();

    public string id;
    public string address;

    public void CreateID(string address)
    {
        if(!Editorbles.Ins.IDListDict.ContainsKey(address))
        {
            Debug.Log($"アドレス無し{address}");
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
        Debug.Log($"セットID　＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝");
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
