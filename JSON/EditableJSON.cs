using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

// �܂�CreateAssetMenu_JSON�ł��ƂƂȂ�Json�t�@�C�����쐬(Project View��ŉE�N���b�N �� Create/TextFile ���N���b�N)
// �R���X�g���N�^�ɔC�ӂ�Json�t�@�C���̃p�X��n���ƒl���������`���ł���JObject�Ƃ��ĕԂ��Ă����B
public class EditableJSON
{
    public string Path;
    public JObject Obj // �p�[�X���ꂽ�`���Ȃ̂Œl���������
    {
        get
        {
            Apply();
            using (var sr = new StreamReader(Path, System.Text.Encoding.UTF8))
                return JObject.Parse(sr.ReadToEnd());
        }
    }
    public string Json // ���ʂ̕������Json
    {
        get { return JsonConvert.SerializeObject(Obj); }
    }


    public EditableJSON(string jsonPath)
    {
        Path = jsonPath;
    }


    // �����JObject�̏�Ԃ����Ƃ�Json�t�@�C���ɏ㏑���������ꍇ�ɌĂ�
    public void Apply()
    {
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