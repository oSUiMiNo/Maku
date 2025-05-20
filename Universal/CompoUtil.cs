using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;


namespace MyUtil
{
    public static class CompoUtil
    {
        //========================================
        // 型引数で指定されたコンポーネントへの参照を取得
        // コンポーネントがない場合はアタッチして参照を取得
        //========================================
        public static Compo CheckAddCompo<Compo>(this GameObject GO) where Compo : Component
        {
            //Debug.Log($"コンポ {typeof(Compo).Name}");
            #region 呼び出し元通知
            var caller = new System.Diagnostics.StackFrame(1, false);
            string callerMethodName = caller.GetMethod().Name;
            string callerClassName = caller.GetMethod().DeclaringType.Name;
            //Debug.Log("クラス  " + callerClassName + " の、     メソッド  " + callerMethodName + "()  から呼び出されました。");
            #endregion
            //ここでTryGetComponent 使うと、AddComponent の所で、MonoBehaviour 継承してないとか言われる。原因はそのうち調べる
            Compo targetCompo = GO.GetComponent<Compo>();
            if (targetCompo == null)
            {
                targetCompo = GO.AddComponent<Compo>();
            }
            return targetCompo;
        }



        //========================================
        // 型引数で指定されたコンポーネントへの参照を取得
        // コンポーネントがない場合はアタッチして参照を取得。上記の CheckComponent<T> とは若干用途が違う
        // CheckComponent<T> の場合、型引数に <何か.GetType()> みたいに、クラスを取得しながら間接的に渡そうとすると、
        // <> の部分が演算子と判断されてしまって使えない
        // そういった場合にこちらを使う
        // ただし戻り値が Component型 なので、取得したコンポーネントを使いたい場合は更に変換が必要
        // ※ 複数コンポを指定可能
        //========================================
        public static List<Component> CheckAddCompos(this GameObject GO, params Type[] compos)
        {
            List<Component> returnCompos = new List<Component>();
            foreach (Type compo in compos)
            {
                //CheckAddCompo(GO, compo);
                //ここでTryGetComponent 使うと、AddComponent の所で、MonoBehaviour 継承してないとか言われる。原因はそのうち調べる
                Component targetCompo = GO.GetComponent(compo);
                if (targetCompo == null)
                {
                    targetCompo = GO.AddComponent(compo);
                }
                returnCompos.Add(targetCompo);
            }
            return returnCompos;
        }

        //public static Component CheckAddCompo(this GameObject GO, Type Compo)
        //{
        //    //ここでTryGetComponent 使うと、AddComponent の所で、MonoBehaviour 継承してないとか言われる。原因はそのうち調べる
        //    Component targetCompo = GO.GetComponent(Compo);
        //    if (targetCompo == null)
        //    {
        //        targetCompo = GO.AddComponent(Compo);
        //    }
        //    return targetCompo;
        //}

        //public void CheckAddMultiCompo(List<Type> compos, GameObject gObj)
        //{
        //    foreach (Type a in compos)
        //    {
        //        CheckAddCompo(a, gObj);
        //    }
        //}
    }
}
