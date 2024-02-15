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

    #region �y�V���O���g�����z ===================================================================

    public static DebugView Compo = null;//static�������ϐ��͂P�������݂��Ȃ��Ȃ�B������Ăяo���Ƃ��ɃN���X����т����Ă��̃N���X�̊֐���.�ϐ����Ƃ��Ȃ��ĕϐ���������
    void Singletonize()  //�V���O���g���B�P�����C���X�^���X���Ȃ��B�܂��C���X�^���X���Ȃ�������C���X�^���X(���̃N���X�j���B
    {
        if (Compo == null)
        {
            Compo = this;
            DontDestroyOnLoad(gameObject);  //gameObject��this.gameobject�̗��B�܂肱��GameManager�N���X�B
        }
        else   //�����C���X�^���X���łɂ�������P�������߂����炱�킷�B
        {
            Destroy(gameObject);
        }
    }

    #endregion �y�V���O���g�����z ================================================================
    void Start()
    {
        Singletonize();
        text = transform.Find("Scroll View/Viewport/Content").GetComponent<TextMeshProUGUI>();
        Log("DebugView  ��������");
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
            Debug.LogError("�o�b�t�@�̃T�C�Y��1000���������̂Ńo�b�t�@���N���A���܂����B");
        }

        Debug.Log(message);
    }
}
