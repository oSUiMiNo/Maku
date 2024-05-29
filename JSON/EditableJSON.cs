using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

// まずCreateAssetMenu_JSONでもととなるJsonファイルを作成(Project View上で右クリック → Create/TextFile をクリック)
// コンストラクタに任意のJsonファイルのパスを渡すと値をいじれる形式であるJObjectとして返してくれる。
public class EditableJSON
{
    public string Path;
    public JObject Obj // パースされた形式なので値をいじれる
    {
        get
        {
            Apply();
            using (var sr = new StreamReader(Path, System.Text.Encoding.UTF8))
                return JObject.Parse(sr.ReadToEnd());
        }
    }
    public string Json // 普通の文字列のJson
    {
        get { return JsonConvert.SerializeObject(Obj); }
    }


    public EditableJSON(string jsonPath)
    {
        Path = jsonPath;
    }


    // 現状のJObjectの状態をもとのJsonファイルに上書きしたい場合に呼ぶ
    public void Apply()
    {
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