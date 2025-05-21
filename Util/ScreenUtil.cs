using UnityEngine;


namespace MyUtil
{
    public static class ScreenUtil
    {
        // GOの深度を変えずにマウスに追従させるためのワールドPosを返す
        public static Vector3 PosUnderMouse(this GameObject targ, bool UseOffset = false)
        {


            Vector3 targPos = targ.transform.position;
            Vector3 underMousePos = GetMousWorldPosOnTargetDepth(targ);

            if (UseOffset)
            {
                // オフセットについて
                // ターゲットのポジションからマウスのポジションを引くということは
                // マウスポジションを基準（0）としたときのターゲットのベクトル（方向と距離）が求まる
                // underMousePos とターゲットの深度は合わせてあるので(x,y)距離だけが求まる。
                Vector3 offset = targPos - underMousePos;
                return underMousePos + offset;
            }
            else
            {
                return underMousePos;
            }
        }


        // スクリーン平面ではなくターゲットと同じ深度平面上におけるマウスの座標を計算
        static Vector3 GetMousWorldPosOnTargetDepth(GameObject targ)
        {
            // カメラ基準から見たターゲットのz値が深度
            float targDepth = Camera.main.WorldToScreenPoint(targ.transform.position).z;
            // マウスのスクリーン座標を取得しzには深度を入れる
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = targDepth;
            // ワールド座標に変換して返す
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }
    }
}