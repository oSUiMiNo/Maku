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

    [SerializeField]
    int limitOfLength = 500;

    [SerializeField]
    bool onlyNumber = false;

    


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
            if (!Active) return;
            if (_Text.Value.Length <= 0)
            {
                return;
            };
            string t = _Text.Value.Remove(_Text.Value.Length-2);
            _Text.Value = t;
        };
    }


    protected sealed override void Update()
    {
        if (!Active) return;
        // �����L�[�������ꂽ�ꍇ
        if (UnityEngine.Input.anyKeyDown)
        {
            string keyStr = UnityEngine.Input.inputString; // ���͂��ꂽ�L�[�̖��O���擾
            if (onlyNumber) InputNum(keyStr);
            else Input(keyStr);
        }
    }

    void InputNum(string c)
    {
        if (_Text.Value.Length >= limitOfLength) return;
        if (!int.TryParse(c, out int a)) return; //�����ɕϊ��ł��Ȃ��܂萔������Ȃ������͂͂����B
        
        _Text.Value += c;
        _Text.Value = _Text.Value;
    }

    void Input(string c)
    {
        if (_Text.Value.Length >= limitOfLength) return;
        _Text.Value += c;
        _Text.Value = _Text.Value;
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
