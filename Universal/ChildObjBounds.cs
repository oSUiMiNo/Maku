using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObjBounds : MonoBehaviour
{
    /// <summary>
    /// 子オブジェクトの統合Bounds
    /// </summary>
    public Bounds childObjBounds;

    void Start()
    {
        // 
        childObjBounds = CalcLocalObjBounds(this.gameObject);

        // Boundsの大きさと形状が見た目に分かるようコライダーを追加する
        BoxCollider collider = this.gameObject.AddComponent<BoxCollider>();
        // 計算されたバウンドボックスに合わせてコライダーの大きさと位置を変更する
        collider.center = childObjBounds.center;
        collider.size = childObjBounds.size;
    }

    /// <summary>
    /// 現在オブジェクトのローカル座標でのバウンド計算
    /// </summary>
    private Bounds CalcLocalObjBounds(GameObject obj)
    {
        // 指定オブジェクトのワールドバウンドを計算する
        Bounds totalBounds = CalcChildObjWorldBounds(obj, new Bounds());

        // ローカルオブジェクトの相対座標に合わせてバウンドを再計算する
        // オブジェクトのワールド座標とサイズを取得する
        Vector3 ObjWorldPosition = this.transform.position;
        Vector3 ObjWorldScale = this.transform.lossyScale;

        // バウンドのローカル座標とサイズを取得する
        Vector3 totalBoundsLocalCenter = new Vector3(
            (totalBounds.center.x - ObjWorldPosition.x) / ObjWorldScale.x,
            (totalBounds.center.y - ObjWorldPosition.y) / ObjWorldScale.y,
            (totalBounds.center.z - ObjWorldPosition.z) / ObjWorldScale.z);
        Vector3 meshBoundsLocalSize = new Vector3(
            totalBounds.size.x / ObjWorldScale.x,
            totalBounds.size.y / ObjWorldScale.y,
            totalBounds.size.z / ObjWorldScale.z);

        Bounds localBounds = new Bounds(totalBoundsLocalCenter, meshBoundsLocalSize);

        return localBounds;
    }

    /// <summary>
    /// 子オブジェクトのワールド座標でのバウンド計算（再帰処理）
    /// </summary>
    private Bounds CalcChildObjWorldBounds(GameObject obj, Bounds bounds)
    {
        // 指定オブジェクトの全ての子オブジェクトをチェックする
        foreach (Transform child in obj.transform)
        {
            // メッシュフィルターの存在確認
            MeshFilter filter = child.gameObject.GetComponent<MeshFilter>();

            if (filter != null)
            {
                // オブジェクトのワールド座標とサイズを取得する
                Vector3 ObjWorldPosition = child.position;
                Vector3 ObjWorldScale = child.lossyScale;

                // フィルターのメッシュ情報からバウンドボックスを取得する
                Bounds meshBounds = filter.mesh.bounds;

                // バウンドのワールド座標とサイズを取得する
                Vector3 meshBoundsWorldCenter = meshBounds.center + ObjWorldPosition;
                Vector3 meshBoundsWorldSize = Vector3.Scale(meshBounds.size, ObjWorldScale);

                // バウンドの最小座標と最大座標を取得する
                Vector3 meshBoundsWorldMin = meshBoundsWorldCenter - (meshBoundsWorldSize / 2);
                Vector3 meshBoundsWorldMax = meshBoundsWorldCenter + (meshBoundsWorldSize / 2);

                // 取得した最小座標と最大座標を含むように拡大/縮小を行う
                if (bounds.size == Vector3.zero)
                {
                    // 元バウンドのサイズがゼロの場合はバウンドを作り直す
                    bounds = new Bounds(meshBoundsWorldCenter, Vector3.zero);
                }
                bounds.Encapsulate(meshBoundsWorldMin);
                bounds.Encapsulate(meshBoundsWorldMax);
            }

            // 再帰処理
            bounds = CalcChildObjWorldBounds(child.gameObject, bounds);
        }
        return bounds;
    }
}