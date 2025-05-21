using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyUtil
{
    public static class TransUtil
    {
        //========================================
        // ローカル Transform リセット
        //========================================
        public static void TransReset_Local(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }


        //========================================
        // ワールド Transform リセット
        //========================================
        public static void TransReset_World(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(
                transform.localScale.x / transform.lossyScale.x,
                transform.localScale.y / transform.lossyScale.y,
                transform.localScale.z / transform.lossyScale.z
            );
        }
    }
}