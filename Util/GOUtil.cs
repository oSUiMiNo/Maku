using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


namespace MyUtil
{
    public static class GOUtil
    {
        //================================================
        // コンポがついたGOの親を取得
        //================================================
        public static GameObject Parent(this Component childCompo)
        {
            return childCompo.transform.parent.gameObject;
        }

        //================================================
        // 親を取得
        //================================================
        public static GameObject Parent(this GameObject child)
        {
            return child.transform.parent.gameObject;
        }

        //================================================
        // 名前でコンポがついたGOの子を取得
        //================================================
        public static GameObject Child(this Component parentCompo, string name)
        {
            return parentCompo.transform.Find(name).gameObject;
        }

        //================================================
        // コンポがついたGOの全子を取得
        //================================================
        public static List<GameObject> Children(this Component parentCompo)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (var item in parentCompo.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject == parentCompo.gameObject) continue;
                children.Add(item.gameObject);
            }
            return children;
        }

        //================================================
        // 名前で子を取得
        //================================================
        public static GameObject Child(this GameObject parent, string name)
        {
            return parent.transform.Find(name).gameObject;
        }

        //================================================
        // 全子を取得
        //================================================
        public static List<GameObject> Children(this GameObject parent)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (var item in parent.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject == parent) continue;
                children.Add(item.gameObject);
            }
            return children;
        }

        //================================================
        // 子を作成
        //================================================
        public static GameObject CreateChild(this GameObject parent, string name, params Type[] compos)
        {
            GameObject GO = new GameObject();
            GO.CheckAddCompos(compos);
            GO.transform.parent = parent.transform;
            GO.name = name;

            return GO;
        }

        //================================================
        // 子を複数作成
        //================================================
        public static List<GameObject> CreateChildren(this GameObject parent, string name, int quantity, params Type[] compos)
        {
            List<GameObject> GOs = new List<GameObject>();
            for (int a = 0; a < quantity; a++)
            {
                GameObject GO = new GameObject();
                GO.CheckAddCompos(compos);
                GO.transform.parent = parent.transform;
                GO.name = name;
                GOs.Add(GO);
            }
            return GOs;
        }

        //===========================================
        // 自分と子孫のレイヤーを変更
        //===========================================
        public static void SetLayerRecursive(this GameObject parent, string layerName) { SetLayerRecursive(parent.transform, layerName); }
        public static void SetLayerRecursive(this Transform parent, string layerName)
        {
            parent.gameObject.layer = LayerMask.NameToLayer(layerName);
            foreach (Transform child in parent)
            {
                child.gameObject.layer = LayerMask.NameToLayer(layerName);
                SetLayerRecursive(child, layerName);
            }
        }

        //===========================================
        // 自分と子孫のアクティブ状態を変更
        //===========================================
        public static void SetActiveRecursive(this GameObject parent, bool activeState) { SetActiveRecursive(parent.transform, activeState); }
        public static async void SetActiveRecursive(this Transform parent, bool activeState)
        {
            parent.gameObject.SetActive(activeState);
            foreach (Transform child in parent)
            {
                SetActiveRecursive(child, activeState);
            }
        }

        //===========================================
        // 2つのGO間の距離取得
        //===========================================
        public static float Distance(this GameObject gO, GameObject targ)
        {
            return Vector3.Distance(gO.transform.position, targ.transform.position);
        }

        //===========================================
        // 全メッシュ取得
        //===========================================
        public static List<Mesh> Meshes(this GameObject gO)
        {
            List<Mesh> meshes = new();
            // SkinnedMeshRenderer の場合
            var smr = gO.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                var mesh = new Mesh();
                smr.BakeMesh(mesh);
                meshes.Add(mesh);
            }
            // 通常の MeshRenderer + MeshFilter の場合
            else
            {
                var mesh = gO.GetComponentInChildren<MeshFilter>().sharedMesh;
                var meshCount = mesh.subMeshCount;
                meshes.Add(mesh);
            }
            return meshes;
        }

        //===========================================
        // 全メッシュについているテクスチャ取得
        //===========================================
        public static List<Texture> Texs(this GameObject gO)
        {
            List<Texture> texs = new();
            // SkinnedMeshRenderer の場合
            var smr = gO.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                var tex = smr.sharedMaterial.mainTexture;
                texs.Add(tex);
            }
            // 通常の MeshRenderer + MeshFilter の場合
            else
            {
                var tex = gO.GetComponentInChildren<Renderer>().sharedMaterials.First().mainTexture;
                texs.Add(tex);
            }
            return texs;
        }

        //================================================
        // サイズのデータセットを返す
        //================================================
        public static MeshSize MeshSize(this GameObject gO) => new MeshSize(gO);

        //================================================
        // メッシュを結合する
        // 親オブジェクト, 結合したメッシュに付けるマテリアルを指定
        //================================================
        public static void CombineMesh(this GameObject parent, Material mt)
        {
            // 親オブジェクトにMeshFilterがあるかどうか確認
            MeshFilter parentMeshFilter = parent.CheckAddCompo<MeshFilter>();

            // 子オブジェクトのMeshFilterへの参照を配列として保持
            // ただし、親オブジェクトのメッシュもGetComponentsInChildrenに含まれるので除外
            MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
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
            // 標準でサポートしているインデックスバッファ?ってやつの頂点数が65535個らしく、
            // mesh が一度に持つ頂点数がそれを超えてしまうとバグるので、
            // インデックスバッファにしまえる頂点数を、16bit から 32bit に変更することで 、
            // 扱える頂点数が65535個から40億個になる。
            parentMeshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            parentMeshFilter.mesh.CombineMeshes(combine);

            Debug.Log($"{parentMeshFilter.mesh.vertexCount} 個");

            parent.AddComponent<MeshCollider>();

            // 結合したメッシュにマテリアルをセット
            parent.CheckAddCompo<MeshRenderer>().material = mt;

            // 親オブジェクトをアクティブに
            parent.SetActive(true);
        }

        //================================================
        // 子を含めた全メッシュ中の中心位置を取得
        //================================================
        public static Vector3 CenterPos(GameObject gO)
        {
            Transform target = gO.transform;

            //非アクティブも含めて target と target の子全てのレンダラーとコライダーを取得
            var cols = target.GetComponentsInChildren<Collider>(true);
            var rens = target.GetComponentsInChildren<Renderer>(true);

            //コライダーとレンダラーが1つもなければ target.position が center になる
            if (cols.Length == 0 && rens.Length == 0)
                return target.position;

            bool isInit = false;

            Vector3 minPos = Vector3.zero;
            Vector3 maxPos = Vector3.zero;

            for (int i = 0; i < cols.Length; i++)
            {
                var bounds = cols[i].bounds;
                var center = bounds.center;
                var size = bounds.size / 2;

                //最初の１度だけ通って minPosとmaxPos を初期化
                if (!isInit)
                {
                    minPos.x = center.x - size.x;
                    minPos.y = center.y - size.y;
                    minPos.z = center.z - size.z;
                    maxPos.x = center.x + size.x;
                    maxPos.y = center.y + size.y;
                    maxPos.z = center.z + size.z;

                    isInit = true;
                    continue;
                }

                if (minPos.x > center.x - size.x) minPos.x = center.x - size.x;
                if (minPos.y > center.y - size.y) minPos.y = center.y - size.y;
                if (minPos.z > center.z - size.z) minPos.z = center.z - size.z;
                if (maxPos.x < center.x + size.x) maxPos.x = center.x + size.x;
                if (maxPos.y < center.y + size.y) maxPos.y = center.y + size.y;
                if (maxPos.z < center.z + size.z) maxPos.z = center.z + size.z;
            }
            for (int i = 0; i < rens.Length; i++)
            {
                var bounds = rens[i].bounds;
                var center = bounds.center;
                var size = bounds.size / 2;

                //コライダーが１つもなければ１度だけ通って minPos と maxPos を初期化
                if (!isInit)
                {
                    minPos.x = center.x - size.x;
                    minPos.y = center.y - size.y;
                    minPos.z = center.z - size.z;
                    maxPos.x = center.x + size.x;
                    maxPos.y = center.y + size.y;
                    maxPos.z = center.z + size.z;

                    isInit = true;
                    continue;
                }

                if (minPos.x > center.x - size.x) minPos.x = center.x - size.x;
                if (minPos.y > center.y - size.y) minPos.y = center.y - size.y;
                if (minPos.z > center.z - size.z) minPos.z = center.z - size.z;
                if (maxPos.x < center.x + size.x) maxPos.x = center.x + size.x;
                if (maxPos.y < center.y + size.y) maxPos.y = center.y + size.y;
                if (maxPos.z < center.z + size.z) maxPos.z = center.z + size.z;
            }

            return (minPos + maxPos) / 2;
        }
    }


    //*************************************
    // メッシュのサイズを計算
    //*************************************
    public class MeshSize
    {
        public Vector3 Orig { get; private set; }
        public Vector3 Real { get; private set; }
        public Mesh Mesh { get; private set; }

        public MeshSize(GameObject gO)
        {
            Mesh = gO.transform.GetComponent<MeshFilter>().mesh;

            // メッシュの（バウンズ）サイズを取得
            Bounds bounds = Mesh.bounds;
            Orig = bounds.size;

            // スケールを掛け合わせた実際のサイズを取得
            Real = new Vector3(
                bounds.size.x * gO.transform.lossyScale.x,
                bounds.size.y * gO.transform.lossyScale.y,
                bounds.size.z * gO.transform.lossyScale.z
            );
            Debug.Log(Real);
        }
    }
}
