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
        // �T�C�Y�̃f�[�^�Z�b�g��Ԃ�
        //========================================
        public static MeshSize MeshSize(this GameObject gO) => new MeshSize(gO);

        //========================================
        // ���b�V�����}�b�v�ɕϊ������f�[�^�Z�b�g��Ԃ�
        //========================================
        public static MeshMap Maps(this Mesh mesh, float pointCountPerArea) => new MeshMap(mesh, pointCountPerArea);

        //========================================
        // ���b�V������������
        // �e�I�u�W�F�N�g, �����������b�V���ɕt����}�e���A�� ���w��
        //========================================
        public static void CombineMesh(this GameObject parent, Material mt)
        {
            // �e�I�u�W�F�N�g��MeshFilter�����邩�ǂ����m�F
            MeshFilter parentMeshFilter = parent.CheckAddCompo<MeshFilter>();

            // �q�I�u�W�F�N�g��MeshFilter�ւ̎Q�Ƃ�z��Ƃ��ĕێ�
            // �������A�e�I�u�W�F�N�g�̃��b�V����GetComponentsInChildren�Ɋ܂܂��̂ŏ��O
            MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
            List<MeshFilter> meshFilterList = new List<MeshFilter>();
            for (int i = 1; i < meshFilters.Length; i++)
            {
                meshFilterList.Add(meshFilters[i]);
            }

            // �������郁�b�V���̔z����쐬
            CombineInstance[] combine = new CombineInstance[meshFilterList.Count];

            // �������郁�b�V���̏���CombineInstance�ɒǉ�
            for (int i = 0; i < meshFilterList.Count; i++)
            {
                combine[i].mesh = meshFilterList[i].sharedMesh;
                combine[i].transform = meshFilterList[i].transform.localToWorldMatrix;
                meshFilterList[i].gameObject.SetActive(false);
            }

            // �����������b�V�����Z�b�g
            parentMeshFilter.mesh = new Mesh();
            // �W���ŃT�|�[�g���Ă���C���f�b�N�X�o�b�t�@?���Ă�̒��_����65535�炵���A
            // mesh ����x�Ɏ����_��������𒴂��Ă��܂��ƃo�O��̂ŁA
            // �C���f�b�N�X�o�b�t�@�ɂ��܂��钸�_�����A16bit ���� 32bit �ɕύX���邱�Ƃ� �A
            // �����钸�_����65535����40���ɂȂ�B
            parentMeshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            parentMeshFilter.mesh.CombineMeshes(combine);

            Debug.Log($"{parentMeshFilter.mesh.vertexCount} ��");

            parent.AddComponent<MeshCollider>();

            // �����������b�V���Ƀ}�e���A�����Z�b�g
            parent.CheckAddCompo<MeshRenderer>().material = mt;

            // �e�I�u�W�F�N�g���A�N�e�B�u��
            parent.SetActive(true);
        }
    }


    //**********************************
    // ���b�V�����}�b�v���ɕϊ�
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


        #region �R���X�g���N�^�Ŏg���Ă���֐�����
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

                var area = Vector3.Cross(pos1 - pos0, pos2 - pos0).magnitude * 0.5f; // �O�p�`pos0,pos1,pos2�̖ʐ�
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
    // ���b�V���̃T�C�Y���v�Z
    //**********************************
    public class MeshSize
    {
        public Vector3 Orig { get; private set; }
        public Vector3 Real { get; private set; }
        public Mesh Mesh { get; private set; }

        public MeshSize(GameObject gO)
        {
            Mesh = gO.transform.GetComponent<MeshFilter>().mesh;

            // ���b�V���́i�o�E���Y�j�T�C�Y���擾
            Bounds bounds = Mesh.bounds;
            Orig = bounds.size;

            // �X�P�[�����|�����킹�����ۂ̃T�C�Y���擾
            Real = new Vector3(
                bounds.size.x * gO.transform.lossyScale.x,
                bounds.size.y * gO.transform.lossyScale.y,
                bounds.size.z * gO.transform.lossyScale.z
            );
            Debug.Log(Real);
        }
    }


}
