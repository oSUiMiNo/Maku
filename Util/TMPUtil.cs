using TMPro;
using UnityEngine;


namespace Maku
{
    public static class TMPUtil
    {
        //=============================================
        // TMPのGO事態の隅を取得
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
        // 高速 場合によって不正確
        // TMP によって既に計算された値を使う
        //=============================================
        public static Vector3 PreferredTopLeft(this TMP_Text TMP)
        {
            Vector3[] corners = new Vector3[4];
            TMP.rectTransform.GetWorldCorners(corners);

            // RectTransformの幅（ワールド座標）
            float rectWorldWidth = Vector3.Distance(corners[0], corners[3]);
            // preferredWidthのワールド座標での幅
            float textWorldWidth = TMP.preferredWidth * TMP.transform.lossyScale.x;

            // アライメントに基づいてテキストの開始位置を計算
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

            // 左端から右方向へのベクトル（Y成分を0にして水平に）
            Vector3 leftToRight = corners[3] - corners[0];
            leftToRight.y = 0;  // 水平方向のみ

            // テキスト開始位置（左端）
            Vector3 leftX = corners[0] + leftToRight * startRatio;

            // Y座標の計算：preferredHeightを使用（ForceMeshUpdate不要で高速）
            float textWorldHeight = TMP.preferredHeight * TMP.transform.lossyScale.y;

            // 縦方向のアライメントを考慮して上端を計算
            float topY = corners[1].y;  // デフォルトは左上

            // 縦アライメントに応じて調整
            if (TMP.alignment == TextAlignmentOptions.Bottom ||
                TMP.alignment == TextAlignmentOptions.BottomLeft ||
                TMP.alignment == TextAlignmentOptions.BottomRight)
            {
                // 下揃えの場合：下端 + テキスト高さ = 上端
                topY = corners[0].y + textWorldHeight;
            }
            else if (TMP.alignment == TextAlignmentOptions.Center ||
                     TMP.alignment == TextAlignmentOptions.Left ||
                     TMP.alignment == TextAlignmentOptions.Right)
            {
                // 中央揃えの場合
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                float centerY = (corners[0].y + corners[1].y) / 2f;
                topY = centerY + textWorldHeight / 2f;
            }
            else
            {
                // 上揃えの場合
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                if (textWorldHeight < rectHeight)
                {
                    // テキストがRectより小さい場合の調整
                    float offset = (rectHeight - textWorldHeight);
                    topY = corners[1].y - offset;
                }
                else
                {
                    // テキストがはみ出す場合
                    topY = corners[1].y;
                }
            }

            return new Vector3(leftX.x, topY, TMP.transform.position.z);
        }
        public static Vector3 PreferredBottomRight(this TMP_Text TMP)
        {
            Vector3[] corners = new Vector3[4];
            TMP.rectTransform.GetWorldCorners(corners);
            // RectTransformの幅（ワールド座標）
            float rectWorldWidth = Vector3.Distance(corners[0], corners[3]);
            // preferredWidthのワールド座標での幅
            float textWorldWidth = TMP.preferredWidth * TMP.transform.lossyScale.x;
            // アライメントに基づいてテキストの開始位置を計算
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
            // 左端から右方向へのベクトル（Y成分を0にして水平に）
            Vector3 leftToRight = corners[3] - corners[0];
            leftToRight.y = 0;  // 水平方向のみ
                                // テキスト開始位置
            Vector3 textStart = corners[0] + leftToRight * startRatio;
            // テキスト終了位置（右端）
            Vector3 rightDirection = leftToRight.normalized;
            Vector3 result = textStart + rightDirection * textWorldWidth;
            // Y座標の計算：preferredHeightを使用（ForceMeshUpdate不要で高速）
            float textWorldHeight = TMP.preferredHeight * TMP.transform.lossyScale.y;
            // 縦方向のアライメントを考慮
            float bottomY = corners[0].y;  // デフォルトは左下
                                           // 縦アライメントに応じて調整
            if (TMP.alignment == TextAlignmentOptions.Top ||
                TMP.alignment == TextAlignmentOptions.TopLeft ||
                TMP.alignment == TextAlignmentOptions.TopRight)
            {
                // 上揃えの場合：上端 - テキスト高さ = 底
                bottomY = corners[1].y - textWorldHeight;
            }
            else
            if (TMP.alignment == TextAlignmentOptions.Center ||
                     TMP.alignment == TextAlignmentOptions.Left ||
                     TMP.alignment == TextAlignmentOptions.Right)
            {
                // 中央揃えの場合
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                float centerY = (corners[0].y + corners[1].y) / 2f;
                bottomY = centerY - textWorldHeight / 2f;
            }
            else
            {
                // 下揃えの場合：RectTransformの底 + (RectHeight - TextHeight) / 2
                float rectHeight = Vector3.Distance(corners[0], corners[1]);
                if (textWorldHeight < rectHeight)
                {
                    // テキストがRectより小さい場合の調整
                    float offset = (rectHeight - textWorldHeight);
                    bottomY = corners[0].y + offset;
                }
                else
                {
                    // テキストがはみ出す場合
                    bottomY = corners[0].y;
                }
            }
            result.y = bottomY;
            return result;
        }

        //=============================================
        // 低速
        // 正確な文字の隅
        //=============================================
        public static Vector3 TxtTopLeft(this TMP_Text TMP)
        {
            TMP.ForceMeshUpdate();
            TMP_TextInfo textInfo = TMP.textInfo;
            if (textInfo.characterCount == 0) return TMP.transform.position;

            float minX = float.MaxValue;  // 一番左のX座標
            float maxY = float.MinValue;  // 一番上のY座標

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.isVisible)
                {
                    Vector3 topLeft = TMP.transform.TransformPoint(charInfo.topLeft);
                    Vector3 topRight = TMP.transform.TransformPoint(charInfo.topRight);

                    // 最も左のX座標を更新
                    if (topLeft.x < minX)
                    {
                        minX = topLeft.x;
                    }

                    // 最も上のY座標を更新（左上と右上の両方をチェック）
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
            float minY = float.MaxValue;  // 一番底のY座標
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (charInfo.isVisible)
                {
                    Vector3 bottomRight = TMP.transform.TransformPoint(charInfo.bottomRight);
                    Vector3 bottomLeft = TMP.transform.TransformPoint(charInfo.bottomLeft);

                    // 最も右のX座標を更新
                    if (bottomRight.x > maxX)
                    {
                        maxX = bottomRight.x;
                    }

                    // 最も底のY座標を更新（左下と右下の両方をチェック）
                    minY = Mathf.Min(minY, bottomRight.y, bottomLeft.y);
                }
            }
            return new Vector3(maxX, minY, TMP.transform.position.z);
        }

        //=============================================
        // 低速
        // テキストメッシュ事態の隅を取得するので影や装飾も含む
        //=============================================
        public static Vector3 MeshTopLeft(this TMP_Text TMP)
        {
            TMP.ForceMeshUpdate();
            Bounds bounds = TMP.textBounds;

            // boundsから左端のX座標と上端のY座標を取得
            Vector3 localLeftTop = new Vector3(bounds.min.x, bounds.max.y, 0);
            Vector3 worldLeftTop = TMP.transform.TransformPoint(localLeftTop);

            return worldLeftTop;
        }
        public static Vector3 MeshBottomRight(this TMP_Text TMP)
        {
            TMP.ForceMeshUpdate();
            Bounds bounds = TMP.textBounds;
            // boundsから右端のX座標と底のY座標を取得
            Vector3 localRightBottom = new Vector3(bounds.max.x, bounds.min.y, 0);
            Vector3 worldRightBottom = TMP.transform.TransformPoint(localRightBottom);
            return worldRightBottom;
        }
    }

}