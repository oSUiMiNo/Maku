using TMPro;
using UnityEngine;


namespace Maku
{
    public static class TMPUtil
    {
        //=============================================
        // TMP��GO���Ԃ̋����擾
        //=============================================
        public static Vector3 GOTopLeft(this TMP_Text TMP)
        {
            Vector3[] allCorners = new Vector3[4];
            TMP.rectTransform.GetWorldCorners(allCorners);
            return allCorners[1];
        }
        public static Vector3 GOBottomRight(this TMP_Text TMP)
        {
            Vector3[] allCorners = new Vector3[4];
            TMP.rectTransform.GetWorldCorners(allCorners);
            return allCorners[3];
        }

        //=============================================
        // ���� �ꍇ�ɂ���ĕs���m
        // TMP �ɂ���Ċ��Ɍv�Z���ꂽ�l���g��
        //=============================================
        public static Vector3 PreferredTopLeft(this TMP_Text TMP)
        {
            Vector3[] corners = new Vector3[4];
            TMP.rectTransform.GetWorldCorners(corners);

            // RectTransform�̕��i���[���h���W�j
            float rectWorldWidth = Vector3.Distance(corners[0], corners[3]);
            // preferredWidth�̃��[���h���W�ł̕�
            float textWorldWidth = TMP.preferredWidth * TMP.transform.lossyScale.x;

            // �A���C�����g�Ɋ�Â��ăe�L�X�g�̊J�n�ʒu���v�Z
            float startRatio = 0f;
            switch (TMP.alignment)
            {
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.MidlineLeft:
                case TextAlignmentOptions.CaplineLeft:
                    startRatio = 0f;
                    break;

                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Top:
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.Baseline:
                case TextAlignmentOptions.Midline:
                case TextAlignmentOptions.Capline:
                    startRatio = (rectWorldWidth - textWorldWidth) / rectWorldWidth * 0.5f;
                    break;

                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.TopRight:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BaselineRight:
                case TextAlignmentOptions.MidlineRight:
                case TextAlignmentOptions.CaplineRight:
                    startRatio = (rectWorldWidth - textWorldWidth) / rectWorldWidth;
                    break;
            }

            // ���[����E�����ւ̃x�N�g���iY������0�ɂ��Đ����Ɂj
            Vector3 leftToRight = corners[3] - corners[0];
            leftToRight.y = 0;  // ���������̂�

            // �e�L�X�g�J�n�ʒu�i���[�j
            Vector3 leftX = corners[0] + leftToRight * startRatio;

            // Y���W�̌v�Z�FpreferredHeight���g�p�iForceMeshUpdate�s�v�ō����j
            float textWorldHeight = TMP.preferredHeight * TMP.transform.lossyScale.y;

            // �c�����̃A���C�����g���l�����ď�[���v�Z
            float topY = corners[1].y;  // �f�t�H���g�͍���

            // �c�A���C�����g�ɉ����Ē���
            if (TMP.alignment == TextAlignmentOptions.Bottom ||
                TMP.alignment == TextAlignmentOptions.BottomLeft ||
                TMP.alignment == TextAlignmentOptions.BottomRight)
            {
                // �������̏ꍇ�F���[ + �e�L�X�g���� = ��[
                topY = corners[0].y + textWorldHeight;
            }
            else if (TMP.alignment == TextAlignmentOptions.Center ||
                     TMP.alignment == TextAlignmentOptions.Left ||
                     TMP.alignment == TextAlignmentOptions.Right)
            {
                // ���������̏ꍇ
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                float centerY = (corners[0].y + corners[1].y) / 2f;
                topY = centerY + textWorldHeight / 2f;
            }
            else
            {
                // �㑵���̏ꍇ
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                if (textWorldHeight < rectHeight)
                {
                    // �e�L�X�g��Rect��菬�����ꍇ�̒���
                    float offset = (rectHeight - textWorldHeight);
                    topY = corners[1].y - offset;
                }
                else
                {
                    // �e�L�X�g���͂ݏo���ꍇ
                    topY = corners[1].y;
                }
            }

            return new Vector3(leftX.x, topY, TMP.transform.position.z);
        }
        public static Vector3 PreferredBottomRight(this TMP_Text TMP)
        {
            Vector3[] corners = new Vector3[4];
            TMP.rectTransform.GetWorldCorners(corners);
            // RectTransform�̕��i���[���h���W�j
            float rectWorldWidth = Vector3.Distance(corners[0], corners[3]);
            // preferredWidth�̃��[���h���W�ł̕�
            float textWorldWidth = TMP.preferredWidth * TMP.transform.lossyScale.x;
            // �A���C�����g�Ɋ�Â��ăe�L�X�g�̊J�n�ʒu���v�Z
            float startRatio = 0f;
            switch (TMP.alignment)
            {
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.MidlineLeft:
                case TextAlignmentOptions.CaplineLeft:
                    startRatio = 0f;
                    break;
                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Top:
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.Baseline:
                case TextAlignmentOptions.Midline:
                case TextAlignmentOptions.Capline:
                    startRatio = (rectWorldWidth - textWorldWidth) / rectWorldWidth * 0.5f;
                    break;
                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.TopRight:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BaselineRight:
                case TextAlignmentOptions.MidlineRight:
                case TextAlignmentOptions.CaplineRight:
                    startRatio = (rectWorldWidth - textWorldWidth) / rectWorldWidth;
                    break;
            }
            // ���[����E�����ւ̃x�N�g���iY������0�ɂ��Đ����Ɂj
            Vector3 leftToRight = corners[3] - corners[0];
            leftToRight.y = 0;  // ���������̂�
                                // �e�L�X�g�J�n�ʒu
            Vector3 textStart = corners[0] + leftToRight * startRatio;
            // �e�L�X�g�I���ʒu�i�E�[�j
            Vector3 rightDirection = leftToRight.normalized;
            Vector3 result = textStart + rightDirection * textWorldWidth;
            // Y���W�̌v�Z�FpreferredHeight���g�p�iForceMeshUpdate�s�v�ō����j
            float textWorldHeight = TMP.preferredHeight * TMP.transform.lossyScale.y;
            // �c�����̃A���C�����g���l��
            float bottomY = corners[0].y;  // �f�t�H���g�͍���
                                           // �c�A���C�����g�ɉ����Ē���
            if (TMP.alignment == TextAlignmentOptions.Top ||
                TMP.alignment == TextAlignmentOptions.TopLeft ||
                TMP.alignment == TextAlignmentOptions.TopRight)
            {
                // �㑵���̏ꍇ�F��[ - �e�L�X�g���� = ��
                bottomY = corners[1].y - textWorldHeight;
            }
            else
            if (TMP.alignment == TextAlignmentOptions.Center ||
                     TMP.alignment == TextAlignmentOptions.Left ||
                     TMP.alignment == TextAlignmentOptions.Right)
            {
                // ���������̏ꍇ
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                float centerY = (corners[0].y + corners[1].y) / 2f;
                bottomY = centerY - textWorldHeight / 2f;
            }
            else
            {
                // �������̏ꍇ�FRectTransform�̒� + (RectHeight - TextHeight) / 2
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                if (textWorldHeight < rectHeight)
                {
                    // �e�L�X�g��Rect��菬�����ꍇ�̒���
                    float offset = (rectHeight - textWorldHeight);
                    bottomY = corners[0].y + offset;
                }
                else
                {
                    // �e�L�X�g���͂ݏo���ꍇ
                    bottomY = corners[0].y;
                }
            }
            result.y = bottomY;
            return result;
        }

        //=============================================
        // �ᑬ
        // ���m�ȕ����̋�
        //=============================================
        public static Vector3 TxtTopLeft(this TMP_Text TMP)
        {
            TMP.ForceMeshUpdate();
            TMP_TextInfo textInfo = TMP.textInfo;
            if (textInfo.characterCount == 0) return TMP.transform.position;

            float minX = float.MaxValue;  // ��ԍ���X���W
            float maxY = float.MinValue;  // ��ԏ��Y���W

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.isVisible)
                {
                    Vector3 topLeft = TMP.transform.TransformPoint(charInfo.topLeft);
                    Vector3 topRight = TMP.transform.TransformPoint(charInfo.topRight);

                    // �ł�����X���W���X�V
                    if (topLeft.x < minX)
                    {
                        minX = topLeft.x;
                    }

                    // �ł����Y���W���X�V�i����ƉE��̗������`�F�b�N�j
                    maxY = Mathf.Max(maxY, topLeft.y, topRight.y);
                }
            }

            return new Vector3(minX, maxY, TMP.transform.position.z);
        }
        public static Vector3 TxtBottomRight(this TMP_Text TMP)
        {
            TMP.ForceMeshUpdate();
            TMP_TextInfo textInfo = TMP.textInfo;
            if (textInfo.characterCount == 0) return TMP.transform.position;
            float maxX = float.MinValue;
            float minY = float.MaxValue;  // ��Ԓ��Y���W
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.isVisible)
                {
                    Vector3 bottomRight = TMP.transform.TransformPoint(charInfo.bottomRight);
                    Vector3 bottomLeft = TMP.transform.TransformPoint(charInfo.bottomLeft);

                    // �ł��E��X���W���X�V
                    if (bottomRight.x > maxX)
                    {
                        maxX = bottomRight.x;
                    }

                    // �ł����Y���W���X�V�i�����ƉE���̗������`�F�b�N�j
                    minY = Mathf.Min(minY, bottomRight.y, bottomLeft.y);
                }
            }
            return new Vector3(maxX, minY, TMP.transform.position.z);
        }

        //=============================================
        // �ᑬ
        // �e�L�X�g���b�V�����Ԃ̋����擾����̂ŉe�⑕�����܂�
        //=============================================
        public static Vector3 MeshTopLeft(this TMP_Text TMP)
        {
            TMP.ForceMeshUpdate();
            Bounds bounds = TMP.textBounds;

            // bounds���獶�[��X���W�Ə�[��Y���W���擾
            Vector3 localLeftTop = new Vector3(bounds.min.x, bounds.max.y, 0);
            Vector3 worldLeftTop = TMP.transform.TransformPoint(localLeftTop);

            return worldLeftTop;
        }
        public static Vector3 MeshBottomRight(this TMP_Text TMP)
        {
            TMP.ForceMeshUpdate();
            Bounds bounds = TMP.textBounds;
            // bounds����E�[��X���W�ƒ��Y���W���擾
            Vector3 localRightBottom = new Vector3(bounds.max.x, bounds.min.y, 0);
            Vector3 worldRightBottom = TMP.transform.TransformPoint(localRightBottom);
            return worldRightBottom;
        }
    }

}