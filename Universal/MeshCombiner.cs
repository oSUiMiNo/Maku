using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;


// メッシュを結合するクラス
public class MeshCombiner : Singleton<MeshCombiner>
{
    //========================================
    // メッシュを結合する
    // フィールドパーツの親オブジェクト, 結合したメッシュに付けるマテリアル を指定
    //========================================
    public void Combine(GameObject fieldParent, Material combinedMat)
    {
        // 親オブジェクトにMeshFilterがあるかどうか確認
        MeshFilter parentMeshFilter = fieldParent.CheckAddCompo<MeshFilter>();


        // 子オブジェクトのMeshFilterへの参照を配列として保持
        // ただし、親オブジェクトのメッシュもGetComponentsInChildrenに含まれるので除外
        MeshFilter[] meshFilters = fieldParent.GetComponentsInChildren<MeshFilter>();
        List<MeshFilter> meshFilterList = new List<MeshFilter>();
        for (int i = 1; i < meshFilters.Length; i++)
        {
            meshFilterList.Add(meshFilters[i]);
        }

        // 結合するメッシュの配列を作成
        CombineInstance[] combine = new CombineInstance[meshFilterList.Count];

        // 結合するメッシュの情報をCombineInstanceに追加
        for (int i = 0; i < meshFilterList.Count; i++)
        {
            combine[i].mesh = meshFilterList[i].sharedMesh;
            combine[i].transform = meshFilterList[i].transform.localToWorldMatrix;
            meshFilterList[i].gameObject.SetActive(false);
        }

        // 結合したメッシュをセット
        parentMeshFilter.mesh = new Mesh();
        /// <Summary>
        /// 標準でサポートしているインデックスバッファ?ってやつの頂点数が65535個らしく、
        /// mesh が一度に持つ頂点数がそれを超えてしまうとバグるので、
        /// インデックスバッファにしまえる頂点数を、16bit から 32bit に変更することで 、
        /// 扱える頂点数が65535個から40億個になる。
        /// </Summary>        
        parentMeshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        parentMeshFilter.mesh.CombineMeshes(combine);

        Debug.Log($"{parentMeshFilter.mesh.vertexCount} 個");

        fieldParent.AddComponent<MeshCollider>();

        // 結合したメッシュにマテリアルをセット
        fieldParent.CheckAddCompo<MeshRenderer>().material = combinedMat;

        // 親オブジェクトをアクティブに
        fieldParent.SetActive(true);
    }



    //========================================
    // 引数のオブジェクトのコンポーネントをデタッチする
    //========================================
    public void RemoveMeshes(GameObject obj)
    {
        // 親オブジェクトのコンポーネントを取得し、Transform以外のコンポーネントをデタッチ
        foreach (Component a in obj.GetComponents<Component>())
        {
            if (a.GetType() != typeof(Transform))
            {
                GameObject.Destroy(a);
            }
        }
    }
}