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
        // 子オブジェクトを作成
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
        // 子オブジェクトを複数作成
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
        // 自分と子孫オブジェクトのレイヤーを変更
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
        // 自分と子孫オブジェクトのアクティブ状態を変更
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
        // 2つのゲームオブジェクト間の距離取得
        //===========================================
        public static float Distance(this GameObject gO, GameObject targ)
        {
            return Vector3.Distance(gO.transform.position, targ.transform.position);
        }

        //===========================================
        // ゲームオブジェクトの全メッシュ取得
        //===========================================
        public static List<Mesh> GetMeshes(this GameObject gO)
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
        // ゲームオブジェクトの全メッシュについているテクスチャ取得
        //===========================================
        public static List<Texture> GetTexs(this GameObject gO)
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
    }
}
