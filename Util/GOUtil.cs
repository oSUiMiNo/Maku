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
        // �R���|������GO�̐e���擾
        //================================================
        public static GameObject Parent(this Component childCompo)
        {
            return childCompo.transform.parent.gameObject;
        }

        //================================================
        // �e���擾
        //================================================
        public static GameObject Parent(this GameObject child)
        {
            return child.transform.parent.gameObject;
        }

        //================================================
        // ���O�ŃR���|������GO�̎q���擾
        //================================================
        public static GameObject Child(this Component parentCompo, string name)
        {
            return parentCompo.transform.Find(name).gameObject;
        }

        //================================================
        // �R���|������GO�̑S�q���擾
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
        // ���O�Ŏq���擾
        //================================================
        public static GameObject Child(this GameObject parent, string name)
        {
            return parent.transform.Find(name).gameObject;
        }

        //================================================
        // �S�q���擾
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
        // �q���쐬
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
        // �q�𕡐��쐬
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
        // �����Ǝq���̃��C���[��ύX
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
        // �����Ǝq���̃A�N�e�B�u��Ԃ�ύX
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
        // 2��GO�Ԃ̋����擾
        //===========================================
        public static float Distance(this GameObject gO, GameObject targ)
        {
            return Vector3.Distance(gO.transform.position, targ.transform.position);
        }

        //===========================================
        // �S���b�V���擾
        //===========================================
        public static List<Mesh> Meshes(this GameObject gO)
        {
            List<Mesh> meshes = new();
            // SkinnedMeshRenderer �̏ꍇ
            var smr = gO.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                var mesh = new Mesh();
                smr.BakeMesh(mesh);
                meshes.Add(mesh);
            }
            // �ʏ�� MeshRenderer + MeshFilter �̏ꍇ
            else
            {
                var mesh = gO.GetComponentInChildren<MeshFilter>().sharedMesh;
                var meshCount = mesh.subMeshCount;
                meshes.Add(mesh);
            }
            return meshes;
        }

        //===========================================
        // �S���b�V���ɂ��Ă���e�N�X�`���擾
        //===========================================
        public static List<Texture> Texs(this GameObject gO)
        {
            List<Texture> texs = new();
            // SkinnedMeshRenderer �̏ꍇ
            var smr = gO.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                var tex = smr.sharedMaterial.mainTexture;
                texs.Add(tex);
            }
            // �ʏ�� MeshRenderer + MeshFilter �̏ꍇ
            else
            {
                var tex = gO.GetComponentInChildren<Renderer>().sharedMaterials.First().mainTexture;
                texs.Add(tex);
            }
            return texs;
        }

        //================================================
        // �T�C�Y�̃f�[�^�Z�b�g��Ԃ�
        //================================================
        public static MeshSize MeshSize(this GameObject gO) => new MeshSize(gO);

        //================================================
        // ���b�V������������
        // �e�I�u�W�F�N�g, �����������b�V���ɕt����}�e���A�����w��
        //================================================
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

        //================================================
        // �q���܂߂��S���b�V�����̒��S�ʒu���擾
        //================================================
        public static Vector3 CenterPos(GameObject gO)
        {
            Transform target = gO.transform;

            //��A�N�e�B�u���܂߂� target �� target �̎q�S�Ẵ����_���[�ƃR���C�_�[���擾
            var cols = target.GetComponentsInChildren<Collider>(true);
            var rens = target.GetComponentsInChildren<Renderer>(true);

            //�R���C�_�[�ƃ����_���[��1���Ȃ���� target.position �� center �ɂȂ�
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

                //�ŏ��̂P�x�����ʂ��� minPos��maxPos ��������
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

                //�R���C�_�[���P���Ȃ���΂P�x�����ʂ��� minPos �� maxPos ��������
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
    // ���b�V���̃T�C�Y���v�Z
    //*************************************
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
