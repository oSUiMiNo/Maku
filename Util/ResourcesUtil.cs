using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maku
{
    // Resources アッセットを扱う
    public static class ResourcesUtil
    {
        //========================================
        // プレハブを具現化 - 親指定可
        //========================================
        public static GameObject Instantiate(string name, GameObject parent = null)
        {
            GameObject gO = GameObject.Instantiate((GameObject)Resources.Load(name));
            gO.name = gO.name.Replace("(Clone)", "");
            if (parent != null) gO.transform.SetParent(parent.transform);
            return gO;
        }


        //========================================
        // プレハブを具現化 -> のコンポーネントを取得 - 親指定可
        //========================================
        public static Compo GetCompo<Compo>(string gOName, GameObject parent = null) where Compo : Component
        {
            return Instantiate(gOName, parent).GetComponent<Compo>();
        }
    }
}