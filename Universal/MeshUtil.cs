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


    public class MeshMap
    {
        public int VertexCount;
        public Texture2D PosMap;
        public Texture2D UVMap;
        public Texture2D NormalMap;

        //================================
        // メッシュをマップに変換
        //================================
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

            VertexCount = count;
            PosMap = CreateMap(positions, width, height);
            UVMap = CreateMap(uvs, width, height);
            NormalMap = CreateMap(nrms, width, height);
        }


        #region Convert() で使っている関数たち
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




    public class MeshSize
    {
        public Vector3 Original { get; private set; }
        public Vector3 Real { get; private set; }
        public Mesh Mesh { get; private set; }

        public MeshSize(GameObject gO)
        {
            Mesh = gO.transform.GetComponent<MeshFilter>().mesh;

            // メッシュの（バウンズ）サイズを取得
            Bounds bounds = Mesh.bounds;
            Original = bounds.size;

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
