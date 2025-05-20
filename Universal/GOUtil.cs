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
        // �q�I�u�W�F�N�g���쐬
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
        // �q�I�u�W�F�N�g�𕡐��쐬
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
        // �����Ǝq���I�u�W�F�N�g�̃��C���[��ύX
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
        // �����Ǝq���I�u�W�F�N�g�̃A�N�e�B�u��Ԃ�ύX
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
        // 2�̃Q�[���I�u�W�F�N�g�Ԃ̋����擾
        //===========================================
        public static float Distance(this GameObject gO, GameObject targ)
        {
            return Vector3.Distance(gO.transform.position, targ.transform.position);
        }

        //===========================================
        // �Q�[���I�u�W�F�N�g�̑S���b�V���擾
        //===========================================
        public static List<Mesh> GetMeshes(this GameObject gO)
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
        // �Q�[���I�u�W�F�N�g�̑S���b�V���ɂ��Ă���e�N�X�`���擾
        //===========================================
        public static List<Texture> GetTexs(this GameObject gO)
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
    }
}
