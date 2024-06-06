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
            Debug.Log("バックスペース！");
            if (_Text.Value.Length <= 0)
            {
                //SetPort();
                return;
            }

            string t = _Text.Value.Remove(_Text.Value.Length-1);
            _Text.Value = t;
            text_Input.text = t;
            //SetPort();
        };
    }


    protected sealed override void Update()
    {
        // 何かキーが押された場合
        if (UnityEngine.Input.anyKeyDown)
        {
            string keyStr = UnityEngine.Input.inputString; // 入力されたキーの名前を取得
            if (onlyNumber) InputNum(keyStr);
            else Input(keyStr);
        }
    }

    void InputNum(string c)
    {
        if (_Text.Value.Length >= limitOfLength) return;
        if (!int.TryParse(c, out int a)) return; //数字に変換できないつまり数字じゃないもじははじく。
        
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
