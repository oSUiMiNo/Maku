using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using UnityEngine;
using UniRx;


// �܂�CreateAssetMenu_JSON�ł��ƂƂȂ�Json�t�@�C�����쐬(Project View��ŉE�N���b�N �� Create/TextFile ���N���b�N)
// �R���X�g���N�^�ɔC�ӂ�Json�t�@�C���̃p�X��n���ƒl���������`���ł���JObject�Ƃ��ĕԂ��Ă����B
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
    public JObject Obj0 // �p�[�X���ꂽ�`���Ȃ̂Œl���������
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
    public string Json // ���ʂ̕������Json
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


    // �����JObject�̏�Ԃ����Ƃ�Json�t�@�C���ɏ㏑���������ꍇ�ɌĂ�
    public void Apply()
    {
        DebugView.Log("���Ղ炢");
        using (var sw = new StreamWriter(Path, false, System.Text.Encoding.UTF8))
        {
            sw.Write(Json);
        }
    }

    // �����Ă���Json�t�@�C�����̂��̂��폜����
    public void Delete()
    {
        File.Delete(Path);
    }
}