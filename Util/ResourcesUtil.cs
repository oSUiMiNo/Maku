using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maku
{
    // Resources �A�b�Z�b�g������
    public static class ResourcesUtil
    {
        //========================================
        // �v���n�u����� - �e�w���
        //========================================
        public static GameObject Instantiate(string name, GameObject parent = null)
        {
            GameObject gO = GameObject.Instantiate((GameObject)Resources.Load(name));
            gO.name = gO.name.Replace("(Clone)", "");
            if (parent != null) gO.transform.SetParent(parent.transform);
            return gO;
        }


        //========================================
        // �v���n�u����� -> �̃R���|�[�l���g���擾 - �e�w���
        //========================================
        public static Compo GetCompo<Compo>(string gOName, GameObject parent = null) where Compo : Component
        {
            return Instantiate(gOName, parent).GetComponent<Compo>();
        }
    }
}