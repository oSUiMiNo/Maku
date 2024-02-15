using UnityEngine;

public class MousePosFromGameObject
{
    private Vector3 mOffset;
    private float mZCoord;
    bool useOffset = false;


    //最初のワンクリック目で一度コンストラクタを呼ぶ
    public MousePosFromGameObject(GameObject target, bool useOffset)
    {
        this.useOffset = useOffset;

        ///<summary>
        ///ターゲットのワールド座標をスクリーン座標に変換
        /// zには、カメラからの距離が入るらしい。
        /// </summary>
        mZCoord = Camera.main.WorldToScreenPoint(target.transform.position).z;

        ///<summary>
        ///ターゲットのポジションからマウスのポジションを引くということは、
        ///マウスポジションを基準（0）としたときのターゲットのベクトル（方向と距離）が求まる。
        ///ちなみにGetMouseWorldPos()にてマウスとターゲットのZ値は合わせてあるのでx,yの距離だけが求まる。
        /// </summary>
        mOffset = target.transform.position - GetMouseWorldPos();
        this.useOffset = useOffset;
    }

    private Vector3 GetMouseWorldPos()
    {
        ///<summary>
        ///マウスのスクリーン座標を取得し
        ///zにはカメラ基準から見たターゲットのzにカメラ基準に見たターゲットのz値を入れる。
        /// </summary>
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        /// <summary>
        /// z値をカメラから見たターゲットのz値に変更したマウスのスクリーン座標を
        /// ワールド座標に変換
        /// /// </summary>
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    //マウスに追従させるタイミングで呼ぶ
    public Vector3 GameObjectPosFromMouse()
    {
        if (useOffset) return GetMouseWorldPos() + mOffset;
        else return GetMouseWorldPos();
    }
}
