using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using UnityEngine.UI;

public class MyInputField : MyUI
{
    [SerializeField]
    public TextMeshPro text_Input => transform.Find("Text_Input").GetComponent<TextMeshPro>();

    [SerializeField]
    public StringReactiveProperty _Text = new StringReactiveProperty(string.Empty);


    protected sealed override void Awake0()
    {
        //Clicked.Subscribe(value => presenter.Clicked.Value = value);
        //Clicked.Subscribe(value => DebugView.Log($"{value}")); ;

        //_Text.Subscribe(async value =>
        //{
        //    presenter._Text.Value = value;
        //    await presenter.Execute();
        //});
    }


    protected sealed override void Start()
    {
        _Text.Subscribe(value => text_Input.text = value);


        InputEventHandler.OnDown_BackSpace += () =>
        {
            Debug.Log("�o�b�N�X�y�[�X�I");
            if (_Text.Value.Length <= 0)
            {
                //SetPort();
                return;
            }
            string t = _Text.Value.Remove(_Text.Value.Length-1);
            _Text.Value = t;
            //text_Input.text = text;
            //SetPort();
        };
    }


    protected sealed override void Update()
    {
        // �����L�[�������ꂽ�ꍇ
        if (Input.anyKeyDown)
        {
            string keyStr = Input.inputString; // ���͂��ꂽ�L�[�̖��O���擾
            InputString(keyStr);
        }
    }

    void InputString(string c)
    {
        if (_Text.Value.Length >= 5) return; 
        if (int.TryParse(c, out int a)) //�����ɕϊ��ł��Ȃ��܂萔������Ȃ������͂͂����B
        {
            _Text.Value += c;
            _Text.Value = _Text.Value;
            //text_Input.text = text;
            //SetPort();
        }
    }


    //void SetPort()
    //{
    //    if(text == string.Empty ||
    //       int.Parse(text) == 0)
    //    {
    //        OscServer.port = 39539;
    //    }
    //    else 
    //    {
    //        OscServer.port = int.Parse(text);
    //    }
    //}
}
