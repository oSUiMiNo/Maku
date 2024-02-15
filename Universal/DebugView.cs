using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System;

public class DebugView : MonoBehaviour
{
    TextMeshProUGUI text;
    static List<string> Buffer = new List<string>();

    #region 【シングルトン化】 ===================================================================

    public static DebugView Compo = null;//staticをつけた変数は１つしか存在しなくなる。だから呼び出すときにクラスをよびだしてそのクラスの関数名.変数名としなくて変数名だけで
    void Singletonize()  //シングルトン。１つしかインスタンス作らない。まだインスタンスがなかったらインスタンス(このクラス）作る。
    {
        if (Compo == null)
        {
            Compo = this;
            DontDestroyOnLoad(gameObject);  //gameObjectはthis.gameobjectの略。つまりこのGameManagerクラス。
        }
        else   //もしインスタンスすでにあったら１つしかだめだからこわす。
        {
            Destroy(gameObject);
        }
    }

    #endregion 【シングルトン化】 ================================================================
    void Start()
    {
        Singletonize();
        text = transform.Find("Scroll View/Viewport/Content").GetComponent<TextMeshProUGUI>();
        Log("DebugView  準備完了");
    }


    public static void Log(object message)
    {
        Buffer.Add(message.ToString());
        if (Compo != null)
        {
            Buffer.ForEach( a => Compo.text.text += a.ToString() + "\n");
            Buffer.Clear();
        }    

        if(Buffer.Count > 1000)
        {
            Buffer.Clear();
            Debug.LogError("バッファのサイズが1000を上回ったのでバッファをクリアしました。");
        }

        Debug.Log(message);
    }
}
