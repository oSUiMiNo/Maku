using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���������X�N���v�^�u���I�u�W�F�N�g�̓t�H���_Resources�ɔz�u����
[CreateAssetMenu(fileName = "NotionAssetTable")]
public class NotionAssetTable : ScriptableObject
{
    #region == �C���X�^���X�̍쐬===============================================
    // ���̖��O�̃X�N���v�^�u���I�u�W�F�N�g��
    // �t�H���_Resources �ɔz�u����ĂȂ��Ƃ����Ȃ��B
    private static readonly string RESOURCE_PATH = "NotionAssetTable";

    private static NotionAssetTable ins = null;
    public static NotionAssetTable Ins
    {
        get
        {
            if (ins == null)
            {
                NotionAssetTable asset = Resources.Load(RESOURCE_PATH) as NotionAssetTable;
                if (asset == null)
                {
                    // �A�Z�b�g���w��̃p�X�ɖ����B
                    // �N��������Ɉړ����������A�����₪�����ȁI
                    Debug.AssertFormat(false, $"{RESOURCE_PATH} ���Ă�� Resources �ɖ�������");
                    asset = CreateInstance<NotionAssetTable>();
                }

                ins = asset;
            }

            return ins;
        }
    }
    #endregion ==============================

    // �����ɁANotion �����肽��Asset���̈ꗗ�������Ă���
    [SerializeField]
    public List<string> assetNames;
}
