using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using MyUtil;


// dv
// J^O©ç^CÉ¶¬µ½Q[IuWFNgÌCfbNXðÇµÄZ[u·éNXðãÅì¬


public class EditorbleHeader : MonoBehaviour
{
    //List<string> ExistingContentIDList = new List<string>();

    public string id;
    public string address;

    public void CreateID(string address)
    {
        if(!Editorbles.Ins.IDListDict.ContainsKey(address))
        {
            Debug.Log($"AhX³µ{address}");
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
        //int index = int.Parse(CropStr_R(id, "__", false));
        int index = int.Parse(id.CropStr_R("__", false));
        Debug.Log($"ZbgID@");
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
