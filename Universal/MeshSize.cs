using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class MeshSize
//{
//    public Vector3 Original { get; private set; }
//    public Vector3 Real { get; private set; }
//    public Mesh Mesh { get; private set; }

//    public MeshSize(GameObject gObj)
//    {
//        Mesh = gObj.transform.GetComponent<MeshFilter>().mesh;

//        // ���b�V���́i�o�E���Y�j�T�C�Y���擾
//        Bounds bounds = Mesh.bounds;
//        Original = bounds.size;

//        // �X�P�[�����|�����킹�����ۂ̃T�C�Y���擾
//        Real = new Vector3(
//            bounds.size.x * gObj.transform.lossyScale.x,
//            bounds.size.y * gObj.transform.lossyScale.y,
//            bounds.size.z * gObj.transform.lossyScale.z
//        );
//        Debug.Log(Real);
//    }
//}