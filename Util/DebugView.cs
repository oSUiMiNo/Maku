using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System;

public class DebugView : MonoBehaviour
{
    //TextMeshProUGUI text;
    static List<string> Buffer = new List<string>();
    static int BufferCount = 200;

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
        //text = transform.Find("Scroll View/Viewport/Content").GetComponent<TextMeshProUGUI>();
        //Log("DebugView  ��������");
    }

    int count = 0;
    private void Update()
    {
        Log($"�e�X�g���b�Z�[�W {count.ToString()}");
        count++;
    }


    public static void Log(object message)
    {
        Buffer.Add(message.ToString());
        if (Compo != null)
        {
            //Buffer.ForEach( a => Compo.text.text += a.ToString() + "\n");
            Buffer.ForEach(a => Compo.AddLabel(a.ToString()));
            Buffer.Clear(); 
        }    

        if(Buffer.Count > BufferCount)
        {
            Buffer.Clear();
            Debug.LogWarning("�o�b�t�@�̃T�C�Y��1000���������̂Ńo�b�t�@���N���A���܂����B");
        }

        Debug.Log(message);
    }


    // ���x���̃��X�g�i�ŐV200�s��ێ��j
    private List<string> labels = new List<string>();
    // �X�N���[���r���[�̃T�C�Y
    public float scrollViewWidth = 400f;
    public float scrollViewHeight = 300f;
    public int fontSize = 20;
    public int lineSpacingTop = -5;
    public int lineSpacingBottom = -5;
    // �����Ԋu�i�s�N�Z���P�ʁj
    [SerializeField] private float characterSpacing = -3f; // ���̒l�ŋ����A���̒l�ōL��
    // �w�i�F
    [SerializeField] private Color backgroundColor = new Color(1f, 0f, 0f, 1f); // �f�o�b�O�p�Ɋ��S�s�����ȐԂɐݒ�
    // �w�i�摜�i�I�v�V�����j
    [SerializeField] private Texture2D backgroundImage;
    // �J�X�^��GUIStyle
    private GUIStyle labelStyle;
    private GUIStyle verticalScrollbar;
    private bool stylesInitialized = false;
    private Vector2 scrollPosition = Vector2.zero;


    // ���x����ǉ����A�ő吔�𒴂����ꍇ�͌Â����x�����폜
    public void AddLabel(string text)
    {
        labels.Add(text);
        if (labels.Count > BufferCount)
            labels.RemoveAt(0); // �ŏ��̃��x���i�Â����́j���폜
    }


    void OnGUI()
    {
        // �X�^�C���̏���������x�����s��
        if (!stylesInitialized)
        {
            InitializeStyles();
            stylesInitialized = true;
        }

        // �X�N���[���r���[��z�u����ʒu�ƃT�C�Y���`
        float padding = 30f; // ��ʒ[����̗]��
        Rect scrollViewRect = new Rect(Screen.width - scrollViewWidth - padding, padding, scrollViewWidth, scrollViewHeight);

        // �X�N���[���r���[��z�u����G���A�̊J�n
        GUILayout.BeginArea(scrollViewRect);

        // �w�i��`��i�X�N���[���r���[�̑O�ɕ`�悷�邱�ƂŁA�X�N���[���r���[�̔w��ɕ\���j
        if (backgroundImage != null)
        {
            // �w�i�摜��`��
            GUI.DrawTexture(new Rect(0, 0, scrollViewWidth, scrollViewHeight), backgroundImage, ScaleMode.StretchToFill, true);
        }
        else
        {
            // �w�i�F��`��
            GUI.color = backgroundColor;
            GUI.Box(new Rect(0, 0, scrollViewWidth, scrollViewHeight), GUIContent.none);
            GUI.color = Color.white; // GUI.color�����ɖ߂�
        }

        // �X�N���[���r���[�̊J�n�i��{�I�ȃI�[�o�[���[�h���g�p�j
        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition,
            false, // alwaysShowHorizontal: �����X�N���[���o�[����ɕ\�����Ȃ�
            true,  // alwaysShowVertical: �����X�N���[���o�[����ɕ\��
            GUILayout.Width(scrollViewWidth),
            GUILayout.Height(scrollViewHeight)
        );

        // �X�N���[���r���[���ɕ\������R���e���c
        GUILayout.BeginVertical();
        foreach (var label in labels)
        {
            // �J�X�^�����x����`��
            GUILayout.Label(label, labelStyle);
        }
        GUILayout.EndVertical();

        // �X�N���[���r���[�̏I��
        GUILayout.EndScrollView();

        // �G���A�̏I��
        GUILayout.EndArea();
    }



    // �J�X�^���X�^�C����������
    private void InitializeStyles()
    {
        // ���x���p�̃J�X�^��GUIStyle��������
        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = fontSize; // �t�H���g�T�C�Y��ݒ�
        labelStyle.normal.textColor = Color.white; // �e�L�X�g�J���[�𔒂ɐݒ�i�f�o�b�O�p�j

        // �}�[�W���ƃp�f�B���O�𒲐����čs�Ԃ���������
        labelStyle.margin = new RectOffset(0, 0, lineSpacingTop, lineSpacingBottom); // �㉺�̃}�[�W����ݒ�
        labelStyle.padding = new RectOffset(0, 0, 0, 0); // �p�f�B���O���[���ɐݒ�

        // �X�N���[���o�[�̃X�^�C�����J�X�^�}�C�Y
        verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
        verticalScrollbar.fixedWidth = 20;
        verticalScrollbar.normal.background = MakeTex(2, 2, Color.gray); // �X�N���[���o�[�̔w�i�F���O���[�ɐݒ�
    }

    // �J�X�^�����x����`��B�e�������ʂɕ`�悵�A�w�肵���Ԋu��K�p�i����͎g�p���Ă��܂��񂪁A�K�v�ɉ����ė��p�\�ł��j
    /// <param name="text">�\������e�L�X�g</param>
    /// <param name="style">�g�p����GUIStyle</param>
    /// <param name="spacing">�����Ԋu�i�s�N�Z���P�ʁj</param>
    private void DrawCustomLabel(string text, GUIStyle style, float spacing)
    {
        // ���x���S�̂̃T�C�Y���v�Z
        GUIContent content = new GUIContent(text);
        Vector2 totalSize = style.CalcSize(content);

        // ���x����`�悷��̈���擾
        Rect rect = GUILayoutUtility.GetRect(totalSize.x, totalSize.y, style);

        float x = rect.x;
        float y = rect.y;

        foreach (char c in text)
        {
            string charStr = c.ToString();
            GUIContent charContent = new GUIContent(charStr);
            Vector2 charSize = style.CalcSize(charContent);

            // �e������`��
            GUI.Label(new Rect(x, y, charSize.x, charSize.y), charContent, style);

            // ���̕����̈ʒu���v�Z
            x += charSize.x + spacing;
        }
    }

    // �P�F�̃e�N�X�`���𐶐����܂��B
    /// <param name="width">�e�N�X�`���̕�</param>
    /// <param name="height">�e�N�X�`���̍���</param>
    /// <param name="col">�F</param>
    /// <returns>�������ꂽ�e�N�X�`��</returns>
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
