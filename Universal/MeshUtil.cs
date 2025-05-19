using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//public class MeshMap
//{
//    public int vtxCount;
//    public Texture2D position;
//    public Texture2D uv;
//    public Texture2D normal;
//}


namespace MyUtil
{

    public static class MeshUtil
    {
        //========================================
        // サイズのデータセットを返す
        //========================================
        public static MeshSize MeshSize(this GameObject gO) => new MeshSize(gO);

        //========================================
        // メッシュをマップに変換したデータセットを返す
        //========================================
        public static MeshMap Maps(this Mesh mesh, float pointCountPerArea) => new MeshMap(mesh, pointCountPerArea);

        //========================================
        // メッシュを結合する
        // 親オブジェクト, 結合したメッシュに付けるマテリアル を指定
        //========================================
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
    }


    //**********************************
    // メッシュをマップ情報に変換
    //**********************************
    public class MeshMap
    {
        public int VertCount;
        public Texture2D PosMap;
        public Texture2D UVMap;
        public Texture2D NormMap;

        public MeshMap(Mesh mesh, float pointCountPerArea) 
        {
            IEnumerable<Vector3> vertices = mesh.vertices;
            IEnumerable<Vector2> uvList = mesh.uv;
            IEnumerable<Vector3> normals = mesh.normals;

            if (pointCountPerArea > 0f)
            {
                (vertices, uvList, normals) = DividePolygon(mesh, pointCountPerArea);
            }

            var count = vertices.Count();
            var (width, height) = CalcTexSize(count);

            var positions = vertices.Select(vtx => new Color(vtx.x, vtx.y, vtx.z));
            var uvs = uvList.Select(uv => new Color(uv.x, uv.y, 0f));
            var nrms = normals.Select(nrm => new Color(nrm.x, nrm.y, nrm.z));

            VertCount = count;
            PosMap = CreateMap(positions, width, height);
            UVMap = CreateMap(uvs, width, height);
            NormMap = CreateMap(nrms, width, height);
        }


        #region コンストラクタで使っている関数たち
        // Increase points to be evenly spaced
        private static (List<Vector3>, List<Vector2>, List<Vector3>) DividePolygon(Mesh mesh, float pointCountPerArea = 1f)
        {
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;
            var uvs = mesh.uv;
            var normals = mesh.normals;

            var posList = new List<Vector3>();
            var uvList = new List<Vector2>();
            var nrmList = new List<Vector3>();

            for (var i = 0; i < triangles.Length; i += 3)
            {
                var idx0 = triangles[i];
                var idx1 = triangles[i + 1];
                var idx2 = triangles[i + 2];

                var pos0 = vertices[idx0];
                var pos1 = vertices[idx1];
                var pos2 = vertices[idx2];

                var uv0 = uvs[idx0];
                var uv1 = uvs[idx1];
                var uv2 = uvs[idx2];

                var nrm0 = normals[idx0];
                var nrm1 = normals[idx1];
                var nrm2 = normals[idx2];

                var area = Vector3.Cross(pos1 - pos0, pos2 - pos0).magnitude * 0.5f; // 三角形pos0,pos1,pos2の面積
                var pointNum = Mathf.CeilToInt(area * pointCountPerArea);

                for (var pIdx = 0; pIdx < pointNum; ++pIdx)
                {
                    var wait0 = Random.value;
                    var wait1 = Random.value;
                    if (wait0 + wait1 > 1f)
                    {
                        wait0 = 1f - wait0;
                        wait1 = 1f - wait1;
                    }
                    var wait2 = 1f - wait0 - wait1;

                    var pos = pos0 * wait0 + pos1 * wait1 + pos2 * wait2;
                    var uv = uv0 * wait0 + uv1 * wait1 + uv2 * wait2;
                    var nrm = nrm0 * wait0 + nrm1 * wait1 + nrm2 * wait2;

                    posList.Add(pos);
                    uvList.Add(uv);
                    nrmList.Add(nrm);
                }
            }

            return (posList, uvList, nrmList);
        }

        private static Texture2D CreateMap(IEnumerable<Color> colors, int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            var buf = new Color[width * height];

            var idx = 0;
            foreach (var color in colors)
            {
                buf[idx] = color;
                idx++;
            }

            tex.SetPixels(buf);
            tex.Apply();

            return tex;
        }


        private static (int, int) CalcTexSize(int count)
        {
            float r = Mathf.Sqrt(count);
            var width = (int)Mathf.Ceil(r);
            var height = width;

            return (width, height);
        }
        #endregion
    }


    //**********************************
    // メッシュのサイズを計算
    //**********************************
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
