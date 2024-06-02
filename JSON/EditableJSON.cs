using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using UnityEngine;
using UniRx;


// まずCreateAssetMenu_JSONでもととなるJsonファイルを作成(Project View上で右クリック → Create/TextFile をクリック)
// コンストラクタに任意のJsonファイルのパスを渡すと値をいじれる形式であるJObjectとして返してくれる。
public class EditableJSON
{
    public ReactiveProperty<JObject> Obj
    {
        get
        {
            using (var sr = new StreamReader(Path, System.Text.Encoding.UTF8))
            {
                string s = sr.ReadToEnd();
                obj.Value = JObject.Parse(s);
                return obj;
            }
        }
    }
    private ReactiveProperty<JObject> obj = new ReactiveProperty<JObject>();

    public string Path { get; private set; }
    public JObject Obj0 // パースされた形式なので値をいじれる
    {
        get
        {
            using (var sr = new StreamReader(Path, System.Text.Encoding.UTF8))
            {
                string s = sr.ReadToEnd();
                obj0 = JObject.Parse(s);
                return obj0;
            }
        }
    }
    public string Json // 普通の文字列のJson
    {
        get
        {
            //Apply();
            //if (initialized) Apply();
            json = JsonConvert.SerializeObject(Obj, Formatting.Indented);
            return json;
        }
    }
    private JObject obj0 = null;
    private string json;
    private bool initialized = false;

    public EditableJSON(string jsonPath)
    {
        Path = jsonPath;
        obj0 = Obj0;
        initialized = true;
        obj = Obj;
        Obj.Subscribe(_ => Apply());
    }


    // 現状のJObjectの状態をもとのJsonファイルに上書きしたい場合に呼ぶ
    public void Apply()
    {
        DebugView.Log("あぷらい");
        using (var sw = new StreamWriter(Path, false, System.Text.Encoding.UTF8))
        {
            sw.Write(Json);
        }
    }

    // 扱っているJsonファイルそのものを削除する
    public void Delete()
    {
        File.Delete(Path);
    }
}