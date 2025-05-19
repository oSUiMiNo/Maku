using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;


//// メッシュを結合するクラス
//public class MeshCombiner
//{
//    //========================================
//    // メッシュを結合する
//    // 親オブジェクト, 結合したメッシュに付けるマテリアル を指定
//    //========================================
//    public static void Combine(GameObject parent, Material mt)
//    {
//        // 親オブジェクトにMeshFilterがあるかどうか確認
//        MeshFilter parentMeshFilter = parent.CheckAddCompo<MeshFilter>();

//        // 子オブジェクトのMeshFilterへの参照を配列として保持
//        // ただし、親オブジェクトのメッシュもGetComponentsInChildrenに含まれるので除外
//        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
//        List<MeshFilter> meshFilterList = new List<MeshFilter>();
//        for (int i = 1; i < meshFilters.Length; i++)
//        {
//            meshFilterList.Add(meshFilters[i]);
//        }

//        // 結合するメッシュの配列を作成
//        CombineInstance[] combine = new CombineInstance[meshFilterList.Count];

//        // 結合するメッシュの情報をCombineInstanceに追加
//        for (int i = 0; i < meshFilterList.Count; i++)
//        {
//            combine[i].mesh = meshFilterList[i].sharedMesh;
//            combine[i].transform = meshFilterList[i].transform.localToWorldMatrix;
//            meshFilterList[i].gameObject.SetActive(false);
//        }

//        // 結合したメッシュをセット
//        parentMeshFilter.mesh = new Mesh();
//        /// <Summary>
//        /// 標準でサポートしているインデックスバッファ?ってやつの頂点数が65535個らしく、
//        /// mesh が一度に持つ頂点数がそれを超えてしまうとバグるので、
//        /// インデックスバッファにしまえる頂点数を、16bit から 32bit に変更することで 、
//        /// 扱える頂点数が65535個から40億個になる。
//        /// </Summary>        
//        parentMeshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
//        parentMeshFilter.mesh.CombineMeshes(combine);

//        Debug.Log($"{parentMeshFilter.mesh.vertexCount} 個");

//        parent.AddComponent<MeshCollider>();

//        // 結合したメッシュにマテリアルをセット
//        parent.CheckAddCompo<MeshRenderer>().material = mt;

//        // 親オブジェクトをアクティブに
//        parent.SetActive(true);
//    }
//}