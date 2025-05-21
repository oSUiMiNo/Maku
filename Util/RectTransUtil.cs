using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyUtil
{
    public static class RectTransUtil
    {
        /// <summary>
        /// ���W��ۂ����܂�Pivot��ύX����
        /// </summary>
        /// <param name="rectTransform">���g�̎Q��</param>
        /// <param name="targetPivot">�ύX���Pivot���W</param>
        public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, Vector2 targetPivot)
        {
            var diffPivot = targetPivot - rectTransform.pivot;
            rectTransform.pivot = targetPivot;
            var diffPos = new Vector2(rectTransform.sizeDelta.x * diffPivot.x, rectTransform.sizeDelta.y * diffPivot.y);
            rectTransform.anchoredPosition += diffPos;
        }

        /// <summary>
        /// ���W��ۂ����܂�Pivot��ύX����
        /// </summary>
        /// <param name="rectTransform">���g�̎Q��</param>
        /// <param name="x">�ύX���Pivot��x���W</param>
        /// <param name="y">�ύX���Pivot��y���W</param>
        public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, float x, float y)
        {
            rectTransform.SetPivotWithKeepingPosition(new Vector2(x, y));
        }

        /// <summary>
        /// ���W��ۂ����܂�Anchor��ύX����
        /// </summary>
        /// <param name="rectTransform">���g�̎Q��</param>
        /// <param name="targetAnchor">�ύX���Anchor���W (min,max�����ʂ̏ꍇ)</param>
        public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetAnchor)
        {
            rectTransform.SetAnchorWithKeepingPosition(targetAnchor, targetAnchor);
        }

        /// <summary>
        /// ���W��ۂ����܂�Anchor��ύX����
        /// </summary>
        /// <param name="rectTransform">���g�̎Q��</param>
        /// <param name="x">�ύX���Anchor��x���W (min,max�����ʂ̏ꍇ)</param>
        /// <param name="y">�ύX���Anchor��y���W (min,max�����ʂ̏ꍇ)</param>
        public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float x, float y)
        {
            rectTransform.SetAnchorWithKeepingPosition(new Vector2(x, y));
        }

        /// <summary>
        /// ���W��ۂ����܂�Anchor��ύX����
        /// </summary>
        /// <param name="rectTransform">���g�̎Q��</param>
        /// <param name="targetMinAnchor">�ύX���AnchorMin���W</param>
        /// <param name="targetMaxAnchor">�ύX���AnchorMax���W</param>
        public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetMinAnchor, Vector2 targetMaxAnchor)
        {
            var parent = rectTransform.parent as RectTransform;
            if (parent == null) { Debug.LogError("Parent cannot find."); }

            var diffMin = targetMinAnchor - rectTransform.anchorMin;
            var diffMax = targetMaxAnchor - rectTransform.anchorMax;
            // anchor�̍X�V
            rectTransform.anchorMin = targetMinAnchor;
            rectTransform.anchorMax = targetMaxAnchor;
            // �㉺���E�̋����̍������v�Z
            var diffLeft = parent.rect.width * diffMin.x;
            var diffRight = parent.rect.width * diffMax.x;
            var diffBottom = parent.rect.height * diffMin.y;
            var diffTop = parent.rect.height * diffMax.y;
            // �T�C�Y�ƍ��W�̏C��
            rectTransform.sizeDelta += new Vector2(diffLeft - diffRight, diffBottom - diffTop);
            var pivot = rectTransform.pivot;
            rectTransform.anchoredPosition -= new Vector2(
                 (diffLeft * (1 - pivot.x)) + (diffRight * pivot.x),
                 (diffBottom * (1 - pivot.y)) + (diffTop * pivot.y)
            );
        }

        /// <summary>
        /// ���W��ۂ����܂�Anchor��ύX����
        /// </summary>
        /// <param name="rectTransform">���g�̎Q��</param>
        /// <param name="minX">�ύX���AnchorMin��x���W</param>
        /// <param name="minY">�ύX���AnchorMin��y���W</param>
        /// <param name="maxX">�ύX���AnchorMax��x���W</param>
        /// <param name="maxY">�ύX���AnchorMax��y���W</param>
        public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float minX, float minY, float maxX, float maxY)
        {
            rectTransform.SetAnchorWithKeepingPosition(new Vector2(minX, minY), new Vector2(maxX, maxY));
        }

        /// <summary>
        /// ����Ԃ��܂�
        /// </summary>
        public static float GetWidth(this RectTransform self)
        {
            return self.sizeDelta.x;
        }

        /// <summary>
        /// ������Ԃ��܂�
        /// </summary>
        public static float GetHeight(this RectTransform self)
        {
            return self.sizeDelta.y;
        }

        /// <summary>
        /// ����ݒ肵�܂�
        /// </summary>
        public static void SetWidth(this RectTransform self, float width)
        {
            var size = self.sizeDelta;
            size.x = width;
            self.sizeDelta = size;
        }

        /// <summary>
        /// ������ݒ肵�܂�
        /// </summary>
        public static void SetHeight(this RectTransform self, float height)
        {
            var size = self.sizeDelta;
            size.y = height;
            self.sizeDelta = size;
        }

        /// <summary>
        /// �T�C�Y��ݒ肵�܂�
        /// </summary>
        public static void SetSize(this RectTransform self, float width, float height)
        {
            self.sizeDelta = new Vector2(width, height);
        }
    }
}