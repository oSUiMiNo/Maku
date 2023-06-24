using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public interface IMyExtention
{
    T CheckComponent<T>(GameObject obj) where T : Component;
    Component CheckComponent(Type type, GameObject gObj);
    void CheckMultiComponent(List<Type> compos, GameObject gObj);
    GameObject CreateChild(string name, GameObject parent, Type compo);
    GameObject CreateChild(string name, GameObject parent, List<Type> compos);
    List<GameObject> CreateChildren(string name, GameObject parent, Type compo, int quantity);
    List<GameObject> CreateChildren(string name, GameObject parent, List<Type> compos, int quantity);
    GameObject LoadPrefab(string name);
    void Pause();
    GameObject CreateCanvas(string name, RenderMode renderMode);
}



/// <summary>
///  MonoBehaviour の拡張クラスなので  MonoBehaviour の代わりに使う。
/// </summary>
public abstract class MonoBehaviourMyExtention : MonoBehaviour, IMyExtention
{
    /// <Summary>
    /// 型引数で指定されたコンポーネントへの参照を取得。
    /// コンポーネントがない場合はアタッチして参照を取得。
    /// </Summary>
    public Compo CheckComponent<Compo>(GameObject gObj) where Compo : Component
    {
        //Debug.Log($"コンポーネント   {typeof(Compo).Name}");
        #region 呼び出し元通知
            var caller = new System.Diagnostics.StackFrame(1, false);
        string callerMethodName = caller.GetMethod().Name;
        string callerClassName = caller.GetMethod().DeclaringType.Name;
        //Debug.Log("クラス  " + callerClassName + " の、     メソッド  " + callerMethodName + "()  から呼び出されました。");
        #endregion
        //ここでTryGetComponent 使うと、AddComponent の所で、MonoBehaviour 継承してないとか言われる。原因はそのうち調べる
        Compo targetCompo = gObj.GetComponent<Compo>();
        if(targetCompo == null)
        {
            targetCompo = gObj.AddComponent<Compo>();
        }
        return targetCompo;    
    }


    /// <Summary>
    /// 型引数で指定されたコンポーネントへの参照を取得。
    /// コンポーネントがない場合はアタッチして参照を取得。上記の CheckComponent<T> とは若干用途が違う。
    /// CheckComponent<T> の場合、型引数に <何か.GetType()> みたいに、クラスを取得しながら間接的に渡そうとすると、
    /// <> の部分が演算子と判断されてしまって使えない。
    /// そういった場合にこちらを使う。
    /// ただし戻り値が Component型 なので、取得したコンポーネントを使いたい場合は更に変換が必要。
    /// </Summary>
    public Component CheckComponent(Type Compo, GameObject gObj)
    {
        //ここでTryGetComponent 使うと、AddComponent の所で、MonoBehaviour 継承してないとか言われる。原因はそのうち調べる
        Component targetCompo = gObj.GetComponent(Compo);
        if(targetCompo == null)
        {
            targetCompo = gObj.AddComponent(Compo);
        }
        return targetCompo;
    }


    public void CheckMultiComponent(List<Type> compos, GameObject gObj)
    {
        foreach (Type a in compos)
        {
            CheckComponent(a, gObj);
        }
    }


    public GameObject CreateChild(string name, GameObject parent, Type compo)
    {
        GameObject gObj = new GameObject();
        CheckComponent(compo, gObj);
        gObj.transform.parent = parent.transform;
        gObj.name = name;
        
        return gObj;
    }

    public GameObject CreateChild(string name, GameObject parent, List<Type> compos)
    {
        GameObject gObj = new GameObject();
        CheckMultiComponent(compos, gObj);
        gObj.transform.parent = parent.transform;
        gObj.name = name;

        return gObj;
    }
    
    public List<GameObject> CreateChildren(string name, GameObject parent, Type compo, int quantity)
    {
        List<GameObject> gObjs = new List<GameObject>();
        for (int a = 0; a < quantity; a++)
        {
            GameObject gObj = new GameObject();
            CheckComponent(compo, gObj);
            gObj.transform.parent = parent.transform;
            gObj.name = name;
            gObjs.Add(gObj);
        }
        return gObjs;
    }

    public List<GameObject> CreateChildren(string name, GameObject parent , List<Type> compos, int quantity)
    {
        List<GameObject> gObjs = new List<GameObject>();
        for(int a = 0; a < quantity; a++)
        {
            GameObject gObj = new GameObject();
            CheckMultiComponent(compos, gObj);
            gObj.transform.parent = parent.transform;
            gObj.name = name;
            gObjs.Add(gObj);
        }
        return gObjs;
    }


    /// <summary>
    /// Resourcesフォルダ に入っているプレハブをロードして自分の子オブジェクトにする。
    /// </summary>
    /// <param name="name">ロードしたいプレハブの名前</param>
    /// <returns>ロードしたゲームオブジェクト</returns>
    public GameObject LoadPrefab(string name)
    {
        GameObject gObj = Instantiate((GameObject)Resources.Load(name));
        gObj.transform.parent = transform;
        gObj.name = gObj.name.Replace("(Clone)", "");
        return gObj;
    }

    /// <summary>
    /// エディタを一時停止する
    /// </summary>
    public void Pause()
    {
        Debug.Log("------ 一時停止 ------");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    public GameObject CreateCanvas(string name, RenderMode renderMode)
    {
        GameObject gObj = new GameObject();
        gObj.name = name;

        Canvas canvas = gObj.AddComponent<Canvas>();
        CanvasScaler canvasScaler = gObj.AddComponent<CanvasScaler>();
        GraphicRaycaster graphicRaycaster = gObj.AddComponent<GraphicRaycaster>();
            
        canvas.renderMode = renderMode;

        return gObj;
    }
}



/// <summary>
/// 【注意】
/// MonoBehaviour を継承しているわけではないが、MonoBehaviour を継承しているクラスを利用してハンドリングしているので、
/// Awake()が実行される以前に使うのは良くない。
/// </summary>
public class MyExtention : IMyExtention
{
    class MyExtentionHandler : SingletonCompo<MyExtentionHandler> { }

    /// <Summary>
    /// 型引数で指定されたコンポーネントへの参照を取得。
    /// コンポーネントがない場合はアタッチして参照を取得。
    /// </Summary>
    public T CheckComponent<T>(GameObject obj) where T : Component
    {
        return MyExtentionHandler.Compo.CheckComponent<T>(obj);
    }

    /// <Summary>
    /// 型引数で指定されたコンポーネントへの参照を取得。
    /// コンポーネントがない場合はアタッチして参照を取得。上記の CheckComponent<T> とは若干用途が違う。
    /// CheckComponent<T> の場合、型引数に <何か.GetType()> みたいに、クラスを取得しながら間接的に渡そうとすると、
    /// <> の部分が演算子と判断されてしまって使えない。
    /// そういった場合にこちらを使う。
    /// ただし戻り値が Component型 なので、取得したコンポーネントを使いたい場合は更に変換が必要。
    /// </Summary>
    public Component CheckComponent(Type type, GameObject gObj)
    {
        return MyExtentionHandler.Compo.CheckComponent(type, gObj);
    }

    public void CheckMultiComponent(List<Type> compos, GameObject gObj)
    {
        MyExtentionHandler.Compo.CheckMultiComponent(compos, gObj);
    }


    public GameObject CreateChild(string name, GameObject parent, Type compo)
    {
        return MyExtentionHandler.Compo.CreateChild(name, parent, compo);
    }

    public GameObject CreateChild(string name, GameObject parent, List<Type> compos)
    {
        return MyExtentionHandler.Compo.CreateChild(name, parent, compos);
    }

    public List<GameObject> CreateChildren(string name, GameObject parent, Type compo, int quantity)
    {
        return MyExtentionHandler.Compo.CreateChildren(name, parent, compo, quantity);
    }

    public List<GameObject> CreateChildren(string name, GameObject parent, List<Type> compos, int quantity)
    {
        return MyExtentionHandler.Compo.CreateChildren(name, parent, compos, quantity);
    }


    /// <summary>
    /// Resourcesフォルダ に入っているプレハブをロードして自分の子オブジェクトにする。
    /// </summary>
    /// <param name="name">ロードしたいプレハブの名前</param>
    /// <returns>ロードしたゲームオブジェクト</returns>
    public GameObject LoadPrefab(string name)
    {
        return MyExtentionHandler.Compo.LoadPrefab(name);
    }

    public void Pause()
    {
        MyExtentionHandler.Compo.Pause();
    }

    public GameObject CreateCanvas(string name, RenderMode renderMode)
    {
        return MyExtentionHandler.Compo.CreateCanvas(name, renderMode);
    }
}









/// <summary>
/// 【使い方】
/// 1: なにかの基底クラスにしたいクラスにこれを継承させておく。
/// 
/// 2: projectのAssetsフォルダ直下に、「csc.rsp」というファイルを追加。更にこれの中身のテキストを、
/// 「-warnaserror+:0114」とだけ書いておく。
/// これにより、派生クラスで Start() などを実装してしまった際に、エラーにしてくれるので、
/// 誤って基底クラスでも派生クラスでも別の処理を書いたStart()を実装っしてしまって、
/// 基底クラスの Start()などに書いた必要な処理が上書きされてしまうことを防げる。
/// 
/// 3: 派生先でも Start()相当の関数を使いたいのであれば、
/// 基底クラスで SubStart() みたいな仮想メソッドを作り、基底クラスの Start()の中で呼んでおき、
/// 派生クラスでオーバーライドする。
/// </summary>
public class SealableMonoBehaviourMyExtention : MonoBehaviourMyExtention
{
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
}





public class MyCoroutineHandler : SingletonCompo<MyCoroutineHandler>
{
    static public Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        return Compo.StartCoroutine(coroutine);
    }

    static public void SuspendStaticCoroutine(IEnumerator coroutine)
    {
        Compo.StopCoroutine(coroutine);
    }
}




public static class RectTransformExtensions
{
    /// <summary>
    /// 座標を保ったままPivotを変更する
    /// </summary>
    /// <param name="rectTransform">自身の参照</param>
    /// <param name="targetPivot">変更先のPivot座標</param>
    public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, Vector2 targetPivot)
    {
        var diffPivot = targetPivot - rectTransform.pivot;
        rectTransform.pivot = targetPivot;
        var diffPos = new Vector2(rectTransform.sizeDelta.x * diffPivot.x, rectTransform.sizeDelta.y * diffPivot.y);
        rectTransform.anchoredPosition += diffPos;
    }

    /// <summary>
    /// 座標を保ったままPivotを変更する
    /// </summary>
    /// <param name="rectTransform">自身の参照</param>
    /// <param name="x">変更先のPivotのx座標</param>
    /// <param name="y">変更先のPivotのy座標</param>
    public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, float x, float y)
    {
        rectTransform.SetPivotWithKeepingPosition(new Vector2(x, y));
    }

    /// <summary>
    /// 座標を保ったままAnchorを変更する
    /// </summary>
    /// <param name="rectTransform">自身の参照</param>
    /// <param name="targetAnchor">変更先のAnchor座標 (min,maxが共通の場合)</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetAnchor)
    {
        rectTransform.SetAnchorWithKeepingPosition(targetAnchor, targetAnchor);
    }

    /// <summary>
    /// 座標を保ったままAnchorを変更する
    /// </summary>
    /// <param name="rectTransform">自身の参照</param>
    /// <param name="x">変更先のAnchorのx座標 (min,maxが共通の場合)</param>
    /// <param name="y">変更先のAnchorのy座標 (min,maxが共通の場合)</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float x, float y)
    {
        rectTransform.SetAnchorWithKeepingPosition(new Vector2(x, y));
    }

    /// <summary>
    /// 座標を保ったままAnchorを変更する
    /// </summary>
    /// <param name="rectTransform">自身の参照</param>
    /// <param name="targetMinAnchor">変更先のAnchorMin座標</param>
    /// <param name="targetMaxAnchor">変更先のAnchorMax座標</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetMinAnchor, Vector2 targetMaxAnchor)
    {
        var parent = rectTransform.parent as RectTransform;
        if (parent == null) { Debug.LogError("Parent cannot find."); }

        var diffMin = targetMinAnchor - rectTransform.anchorMin;
        var diffMax = targetMaxAnchor - rectTransform.anchorMax;
        // anchorの更新
        rectTransform.anchorMin = targetMinAnchor;
        rectTransform.anchorMax = targetMaxAnchor;
        // 上下左右の距離の差分を計算
        var diffLeft = parent.rect.width * diffMin.x;
        var diffRight = parent.rect.width * diffMax.x;
        var diffBottom = parent.rect.height * diffMin.y;
        var diffTop = parent.rect.height * diffMax.y;
        // サイズと座標の修正
        rectTransform.sizeDelta += new Vector2(diffLeft - diffRight, diffBottom - diffTop);
        var pivot = rectTransform.pivot;
        rectTransform.anchoredPosition -= new Vector2(
             (diffLeft * (1 - pivot.x)) + (diffRight * pivot.x),
             (diffBottom * (1 - pivot.y)) + (diffTop * pivot.y)
        );
    }

    /// <summary>
    /// 座標を保ったままAnchorを変更する
    /// </summary>
    /// <param name="rectTransform">自身の参照</param>
    /// <param name="minX">変更先のAnchorMinのx座標</param>
    /// <param name="minY">変更先のAnchorMinのy座標</param>
    /// <param name="maxX">変更先のAnchorMaxのx座標</param>
    /// <param name="maxY">変更先のAnchorMaxのy座標</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float minX, float minY, float maxX, float maxY)
    {
        rectTransform.SetAnchorWithKeepingPosition(new Vector2(minX, minY), new Vector2(maxX, maxY));
    }

    /// <summary>
    /// 幅を返します
    /// </summary>
    public static float GetWidth(this RectTransform self)
    {
        return self.sizeDelta.x;
    }

    /// <summary>
    /// 高さを返します
    /// </summary>
    public static float GetHeight(this RectTransform self)
    {
        return self.sizeDelta.y;
    }

    /// <summary>
    /// 幅を設定します
    /// </summary>
    public static void SetWidth(this RectTransform self, float width)
    {
        var size = self.sizeDelta;
        size.x = width;
        self.sizeDelta = size;
    }

    /// <summary>
    /// 高さを設定します
    /// </summary>
    public static void SetHeight(this RectTransform self, float height)
    {
        var size = self.sizeDelta;
        size.y = height;
        self.sizeDelta = size;
    }

    /// <summary>
    /// サイズを設定します
    /// </summary>
    public static void SetSize(this RectTransform self, float width, float height)
    {
        self.sizeDelta = new Vector2(width, height);
    }
}




