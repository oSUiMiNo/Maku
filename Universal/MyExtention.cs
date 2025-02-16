using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using System.Diagnostics.CodeAnalysis;



public interface IMyExtention
{
    T CheckAddComponent<T>(GameObject obj) where T : Component;
    Component CheckAddComponent(Type type, GameObject gObj);
    void CheckAddMultiComponent(List<Type> compos, GameObject gObj);
    GameObject CreateChild(string name, GameObject parent, Type compo);
    GameObject CreateChild(string name, GameObject parent, List<Type> compos);
    List<GameObject> CreateChildren(string name, GameObject parent, Type compo, int quantity);
    List<GameObject> CreateChildren(string name, GameObject parent, List<Type> compos, int quantity);
    GameObject LoadPrefab(string name);
    void Pause();
    GameObject CreateCanvas(string name, RenderMode renderMode);

    // Transform�̃��Z�b�g
    void TransReset_Local(Transform transform);
    void TransReset_World(Transform transform);

    //// String�̃g���~���O
    //// �E���؂蔲��
    //string CropStr_R(string str, string splitter, bool containSplitter);
    //// �E���؂藎�Ƃ�
    //string TrimStr_R(string str, string splitter, bool containSplitter);


    //void SetLayerRecursive(GameObject go, string layerName);
    //void SetLayerRecursive(Transform parent, string layerName);


    //void SetActiveRecursive(Transform parent, bool activeState);
    //void SetActiveRecursive(GameObject parent, bool activeState);
}



/// <summary>
///  MonoBehaviour �̊g���N���X�Ȃ̂�  MonoBehaviour �̑���Ɏg���B
/// </summary>
public abstract class MonoBehaviourMyExtention : MonoBehaviour, IMyExtention
{
    /// <Summary>
    /// �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾�B
    /// �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B
    /// </Summary>
    public Compo CheckAddComponent<Compo>(GameObject gObj) where Compo : Component
    {
        //Debug.Log($"�R���|�[�l���g   {typeof(Compo).Name}");
        #region �Ăяo�����ʒm
        var caller = new System.Diagnostics.StackFrame(1, false);
        string callerMethodName = caller.GetMethod().Name;
        string callerClassName = caller.GetMethod().DeclaringType.Name;
        //Debug.Log("�N���X  " + callerClassName + " �́A     ���\�b�h  " + callerMethodName + "()  ����Ăяo����܂����B");
        #endregion
        //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
        Compo targetCompo = gObj.GetComponent<Compo>();
        if (targetCompo == null)
        {
            targetCompo = gObj.AddComponent<Compo>();
        }
        return targetCompo;
    }


    /// <Summary>
    /// �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾�B
    /// �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B��L�� CheckComponent<T> �Ƃ͎኱�p�r���Ⴄ�B
    /// CheckComponent<T> �̏ꍇ�A�^������ <����.GetType()> �݂����ɁA�N���X���擾���Ȃ���ԐړI�ɓn�����Ƃ���ƁA
    /// <> �̕��������Z�q�Ɣ��f����Ă��܂��Ďg���Ȃ��B
    /// �����������ꍇ�ɂ�������g���B
    /// �������߂�l�� Component�^ �Ȃ̂ŁA�擾�����R���|�[�l���g���g�������ꍇ�͍X�ɕϊ����K�v�B
    /// </Summary>
    public Component CheckAddComponent(Type Compo, GameObject gObj)
    {
        //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
        Component targetCompo = gObj.GetComponent(Compo);
        if (targetCompo == null)
        {
            targetCompo = gObj.AddComponent(Compo);
        }
        return targetCompo;
    }


    public void CheckAddMultiComponent(List<Type> compos, GameObject gObj)
    {
        foreach (Type a in compos)
        {
            CheckAddComponent(a, gObj);
        }
    }


    public GameObject CreateChild(string name, GameObject parent, Type compo)
    {
        GameObject gObj = new GameObject();
        CheckAddComponent(compo, gObj);
        gObj.transform.parent = parent.transform;
        gObj.name = name;

        return gObj;
    }

    public GameObject CreateChild(string name, GameObject parent, List<Type> compos)
    {
        GameObject gObj = new GameObject();
        CheckAddMultiComponent(compos, gObj);
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
            CheckAddComponent(compo, gObj);
            gObj.transform.parent = parent.transform;
            gObj.name = name;
            gObjs.Add(gObj);
        }
        return gObjs;
    }

    public List<GameObject> CreateChildren(string name, GameObject parent, List<Type> compos, int quantity)
    {
        List<GameObject> gObjs = new List<GameObject>();
        for (int a = 0; a < quantity; a++)
        {
            GameObject gObj = new GameObject();
            CheckAddMultiComponent(compos, gObj);
            gObj.transform.parent = parent.transform;
            gObj.name = name;
            gObjs.Add(gObj);
        }
        return gObjs;
    }


    /// <summary>
    /// Resources�t�H���_ �ɓ����Ă���v���n�u�����[�h���Ď����̎q�I�u�W�F�N�g�ɂ���B
    /// </summary>
    /// <param name="name">���[�h�������v���n�u�̖��O</param>
    /// <returns>���[�h�����Q�[���I�u�W�F�N�g</returns>
    public GameObject LoadPrefab(string name)
    {
        GameObject gObj = Instantiate((GameObject)Resources.Load(name));
        gObj.transform.parent = transform;
        gObj.name = gObj.name.Replace("(Clone)", "");
        return gObj;
    }

    /// <summary>
    /// �G�f�B�^���ꎞ��~����
    /// </summary>
    public void Pause()
    {
        Debug.Log("------ �ꎞ��~ ------");
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

    // Transform�̃��Z�b�g
    public void TransReset_Local(Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public void TransReset_World(Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(
            transform.localScale.x / transform.lossyScale.x,
            transform.localScale.y / transform.lossyScale.y,
            transform.localScale.z / transform.lossyScale.z
        );
    }

    //// String�̃g���~���O
    //// �E���؂蔲��
    //public string CropStr_R(string str, string splitter, bool containSplitter)
    //{
    //    int i = str.IndexOf(splitter);
    //    if (i < 0) return str;

    //    int a;
    //    if (containSplitter) a = 0;
    //    else a = splitter.Length;

    //    return str.Substring(i + a);
    //}
    //// �E���؂藎�Ƃ�
    //public string TrimStr_R(string str, string splitter, bool containSplitter)
    //{
    //    var i = str.IndexOf(splitter);
    //    if (i < 0) return str;

    //    int a;
    //    if (containSplitter) a = splitter.Length;
    //    else a = 0;

    //    return str.Substring(0, i + a);
    //}


    //// �����Ǝq���I�u�W�F�N�g�̃��C���[��ύX
    //public void SetLayerRecursive(GameObject go, string layerName) { SetLayerRecursive(go.transform, layerName); }
    //public void SetLayerRecursive(Transform parent, string layerName)
    //{
    //    foreach (Transform child in parent)
    //    {
    //        child.gameObject.layer = LayerMask.NameToLayer(layerName);
    //        SetLayerRecursive(child, layerName);
    //    }
    //}


    //// �����Ǝq���I�u�W�F�N�g�̃A�N�e�B�u��Ԃ�ύX
    //public void SetActiveRecursive(Transform parent, bool activeState) { SetActiveRecursive(parent.gameObject, activeState); }
    //public async void SetActiveRecursive(GameObject parent, bool activeState)
    //{
    //    parent.gameObject.SetActive(true);
    //    foreach (Transform child in transform)
    //    {
    //        SetActiveRecursive(child.gameObject, activeState);
    //    }
    //}
}



/// <summary>
/// �y���Ӂz
/// MonoBehaviour ���p�����Ă���킯�ł͂Ȃ����AMonoBehaviour ���p�����Ă���N���X�𗘗p���ăn���h�����O���Ă���̂ŁA
/// Awake()�����s�����ȑO�Ɏg���̂͗ǂ��Ȃ��B
/// </summary>
public class MyExtention : IMyExtention
{
    class MyExtentionHandler : SingletonCompo<MyExtentionHandler> { }

    /// <Summary>
    /// �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾�B
    /// �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B
    /// </Summary>
    public T CheckAddComponent<T>(GameObject obj) where T : Component
    {
        return MyExtentionHandler.Compo.CheckAddComponent<T>(obj);
    }

    /// <Summary>
    /// �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾�B
    /// �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B��L�� CheckComponent<T> �Ƃ͎኱�p�r���Ⴄ�B
    /// CheckComponent<T> �̏ꍇ�A�^������ <����.GetType()> �݂����ɁA�N���X���擾���Ȃ���ԐړI�ɓn�����Ƃ���ƁA
    /// <> �̕��������Z�q�Ɣ��f����Ă��܂��Ďg���Ȃ��B
    /// �����������ꍇ�ɂ�������g���B
    /// �������߂�l�� Component�^ �Ȃ̂ŁA�擾�����R���|�[�l���g���g�������ꍇ�͍X�ɕϊ����K�v�B
    /// </Summary>
    public Component CheckAddComponent(Type type, GameObject gObj)
    {
        return MyExtentionHandler.Compo.CheckAddComponent(type, gObj);
    }

    public void CheckAddMultiComponent(List<Type> compos, GameObject gObj)
    {
        MyExtentionHandler.Compo.CheckAddMultiComponent(compos, gObj);
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
    /// Resources�t�H���_ �ɓ����Ă���v���n�u�����[�h���Ď����̎q�I�u�W�F�N�g�ɂ���B
    /// </summary>
    /// <param name="name">���[�h�������v���n�u�̖��O</param>
    /// <returns>���[�h�����Q�[���I�u�W�F�N�g</returns>
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

    // Transform�̃��Z�b�g
    public void TransReset_Local(Transform transform)
    {
        MyExtentionHandler.Compo.TransReset_World(transform);

    }
    public void TransReset_World(Transform transform)
    {
        MyExtentionHandler.Compo.TransReset_World(transform);
    }

    //// String�̃g���~���O
    //// �E���؂蔲��
    //public string CropStr_R(string str, string splitter, bool containSplitter)
    //{
    //    return MyExtentionHandler.Compo.CropStr_R(str, splitter, containSplitter);
    //}
    //// �E���؂藎�Ƃ�
    //public string TrimStr_R(string str, string splitter, bool containSplitter)
    //{
    //    return MyExtentionHandler.Compo.TrimStr_R(str, splitter, containSplitter);
    //}


    //public void SetLayerRecursive(GameObject go, string layerName) 
    //{
    //    MyExtentionHandler.Compo.SetLayerRecursive(go, layerName);
    //}
    //public void SetLayerRecursive(Transform parent, string layerName)
    //{
    //    MyExtentionHandler.Compo.SetLayerRecursive(parent, layerName);
    //}


    //public void SetActiveRecursive(Transform parent, bool active) 
    //{
    //    MyExtentionHandler.Compo.SetActiveRecursive(parent, active); 
    //}
    //public void SetActiveRecursive(GameObject parent, bool active)
    //{
    //    MyExtentionHandler.Compo.SetActiveRecursive(parent, active);
    //}
}





public static class StringExtentions
{
    // String�̃g���~���O
    // �E���؂蔲��
    public static string CropStr_R(this string str, string splitter, bool containSplitter)
    {
        int i = str.IndexOf(splitter);
        if (i < 0) return str;

        int a;
        if (containSplitter) a = 0;
        else a = splitter.Length;

        return str.Substring(i + a);
    }
    // �E���؂藎�Ƃ�
    public static string TrimStr_R(this string str, string splitter, bool containSplitter)
    {
        var i = str.IndexOf(splitter);
        if (i < 0) return str;

        int a;
        if (containSplitter) a = splitter.Length;
        else a = 0;

        return str.Substring(0, i + a);
    }

    // --------------------------------
    // �����̌�����
    // --------------------------------
    public static string HexColor(this string str, string hexCode) => string.Format("<color={0}>{1}</color>", hexCode, str);
    public static string Red(this string str) => str.HexColor("red");
    public static string Green(this string str) => str.HexColor("green");
    public static string Blue(this string str) => str.HexColor("blue");
    public static string Black(this string str) => str.HexColor("black");
    public static string White(this string str) => str.HexColor("white");
    public static string Gray(this string str) => str.HexColor("#808080");
    public static string Yellow(this string str) => str.HexColor("yellow");
    public static string Magenta(this string str) => str.HexColor("#FF00FF");
    public static string Cyan(this string str) => str.HexColor("#00FFFF");
    public static string Orange(this string str) => str.HexColor("orange");
    public static string Purple(this string str) => str.HexColor("purple");
    public static string Brown(this string str) => str.HexColor("#A52A2A");
    public static string Size(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
    public static string Large(this string str) => str.Size(16);
    public static string Medium(this string str) => str.Size(11);
    public static string Small(this string str) => str.Size(9);
    public static string Bold(this string str) => string.Format("<b>{0}</b>", str);
    public static string Italic(this string str) => string.Format("<i>{0}</i>", str);
}


public static class GameObjectExtentions
{
    // �����Ǝq���I�u�W�F�N�g�̃��C���[��ύX
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


    public static void SetActiveRecursive(this GameObject parent, bool activeState) { SetActiveRecursive(parent.transform, activeState); }
    public static async void SetActiveRecursive(this Transform parent, bool activeState)
    {
        parent.gameObject.SetActive(activeState);
        foreach (Transform child in parent)
        {
            SetActiveRecursive(child, activeState);
        }
    }
}




    /// <summary>
    /// �y�g�����z
    /// 1: �Ȃɂ��̊��N���X�ɂ������N���X�ɂ�����p�������Ă����B
    /// 
    /// 2: project��Assets�t�H���_�����ɁA�ucsc.rsp�v�Ƃ����t�@�C����ǉ��B�X�ɂ���̒��g�̃e�L�X�g���A
    /// �u-warnaserror+:0114�v�Ƃ��������Ă����B
    /// ����ɂ��A�h���N���X�� Start() �Ȃǂ��������Ă��܂����ۂɁA�G���[�ɂ��Ă����̂ŁA
    /// ����Ċ��N���X�ł��h���N���X�ł��ʂ̏�����������Start()�����������Ă��܂��āA
    /// ���N���X�� Start()�Ȃǂɏ������K�v�ȏ������㏑������Ă��܂����Ƃ�h����B
    /// 
    /// 3: �h����ł� Start()�����̊֐����g�������̂ł���΁A
    /// ���N���X�� SubStart() �݂����ȉ��z���\�b�h�����A���N���X�� Start()�̒��ŌĂ�ł����A
    /// �h���N���X�ŃI�[�o�[���C�h����B
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

public static class ObservableExtensions
{
    // ���b�Z�[�W�̒l���w�肵�����̂ƈ�v���Ă����������s�J�n�B�قȂ��Ă�����I��
    // ������s�̊Ԋu�͕b���w��(�ŒZ0.001s)�B�����n���Ȃ���΃t���[�����̎��s�ƂȂ�
    public static IObservable<long> UpdateWhileEqualTo<T>(
        this IObservable<T> source,
        T expectedValue,
        float sec = 0f )
    {
        // �C���^�[�o���̉����ݒ�
        float interval = sec;
        if (sec <= 0) interval = 0;
        else
        if (sec <= 0.02f) interval = 0.02f;

        // �t���[��������
        if (interval == 0)
            return source
                .Select(value =>
                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                        Observable.EveryUpdate() :
                        Observable.Empty<long>()
                )
                .Switch();
        // �b��������
        else
            return source
                .Select(value =>
                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                        Observable.Interval(TimeSpan.FromSeconds(sec)) :
                        Observable.Empty<long>()
                )
                .Switch();
    }


    // UpdateWhileEqualTo��1�t���[�����Z���b���𑪂�Ȃ�
    // ������͒Z�������(�f�t�H���g0.001, ����0.0001)���X���b�h�v�[���ł��̂Ńt���[���Ɉˑ�����Unity��API���g���Ȃ�(�� Time.time)
    public static IObservable<long> TimerWhileEqualTo<T>(
        this IObservable<T> source,
        T expectedValue,
        float sec = 0.001f)
    {
        // �C���^�[�o���̉����ݒ�
        float interval = sec;
        if (sec <= 0.0001f) interval = 0.0001f;

        // �ɏ��b��������
        return source
            .Select(value =>
                EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                    ObservableStopwatch(interval) :
                    Observable.Empty<long>()
            )
            .Switch();
    }

    // Stopwatch���g���A�t���[�����[�g�Ɉˑ������u�w��b�v��OnNext���J��Ԃ��B
    private static IObservable<long> ObservableStopwatch(float sec)
    {
        return Observable.Create<long>(observer =>
        {
            var cts = new CancellationTokenSource();

            UniTask.RunOnThreadPool(() =>
            {
                //var interval = TimeSpan.FromSeconds(sec);
                long count = 0;
                var stopwatch = new System.Diagnostics.Stopwatch();
                try
                {
                    stopwatch.Start();
                    //if (stopwatch.Elapsed.TotalSeconds >= interval.TotalSeconds)
                    while (!cts.IsCancellationRequested)
                    if (stopwatch.Elapsed.TotalSeconds >= sec)
                    {
                        observer.OnNext(count++);
                        // �������[�v�̃X���b�h���Ɛ�΍��CPU�X�C�b�`
                        Thread.Sleep(0);
                        stopwatch.Reset();
                        stopwatch.Start();
                    }
                }
                catch (OperationCanceledException)
                {
                    // �L�����Z�����͉������Ȃ�
                }
                finally
                {
                    observer.OnCompleted();
                }
            }, cancellationToken: cts.Token).Forget();
            return Disposable.Create(() => cts.Cancel());
        });
    }
}



public static class ListExtentions
{
    public static List<int> Mode(this List<int> numbers)
    {
        var groupedNumbers = numbers.GroupBy(n => n)
                                    .Select(g => new { Number = g.Key, Count = g.Count() })
                                    .OrderByDescending(g => g.Count);

        int maxCount = groupedNumbers.First().Count;

        List<int> modes = groupedNumbers.Where(g => g.Count == maxCount)
                                  .Select(g => g.Number).ToList();

        Debug.Log("�ŕp�l: " + string.Join(", ", modes));

        return modes;
    }
}



public static class RectTransformExtensions
{
    /// <summary>
    /// ���W��ۂ����܂�Pivot��ύX����
    /// </summary>
    /// <param name="rectTransform">���g�̎Q��</param>
    /// <param name="targetPivot">�ύX���Pivot���W</param>
    public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, Vector2 targetPivot)
    {
        var diffPivot = targetPivot - rectTransform.pivot;
        rectTransform.pivot = targetPivot;
        var diffPos = new Vector2(rectTransform.sizeDelta.x * diffPivot.x, rectTransform.sizeDelta.y * diffPivot.y);
        rectTransform.anchoredPosition += diffPos;
    }

    /// <summary>
    /// ���W��ۂ����܂�Pivot��ύX����
    /// </summary>
    /// <param name="rectTransform">���g�̎Q��</param>
    /// <param name="x">�ύX���Pivot��x���W</param>
    /// <param name="y">�ύX���Pivot��y���W</param>
    public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, float x, float y)
    {
        rectTransform.SetPivotWithKeepingPosition(new Vector2(x, y));
    }

    /// <summary>
    /// ���W��ۂ����܂�Anchor��ύX����
    /// </summary>
    /// <param name="rectTransform">���g�̎Q��</param>
    /// <param name="targetAnchor">�ύX���Anchor���W (min,max�����ʂ̏ꍇ)</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetAnchor)
    {
        rectTransform.SetAnchorWithKeepingPosition(targetAnchor, targetAnchor);
    }

    /// <summary>
    /// ���W��ۂ����܂�Anchor��ύX����
    /// </summary>
    /// <param name="rectTransform">���g�̎Q��</param>
    /// <param name="x">�ύX���Anchor��x���W (min,max�����ʂ̏ꍇ)</param>
    /// <param name="y">�ύX���Anchor��y���W (min,max�����ʂ̏ꍇ)</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float x, float y)
    {
        rectTransform.SetAnchorWithKeepingPosition(new Vector2(x, y));
    }

    /// <summary>
    /// ���W��ۂ����܂�Anchor��ύX����
    /// </summary>
    /// <param name="rectTransform">���g�̎Q��</param>
    /// <param name="targetMinAnchor">�ύX���AnchorMin���W</param>
    /// <param name="targetMaxAnchor">�ύX���AnchorMax���W</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetMinAnchor, Vector2 targetMaxAnchor)
    {
        var parent = rectTransform.parent as RectTransform;
        if (parent == null) { Debug.LogError("Parent cannot find."); }

        var diffMin = targetMinAnchor - rectTransform.anchorMin;
        var diffMax = targetMaxAnchor - rectTransform.anchorMax;
        // anchor�̍X�V
        rectTransform.anchorMin = targetMinAnchor;
        rectTransform.anchorMax = targetMaxAnchor;
        // �㉺���E�̋����̍������v�Z
        var diffLeft = parent.rect.width * diffMin.x;
        var diffRight = parent.rect.width * diffMax.x;
        var diffBottom = parent.rect.height * diffMin.y;
        var diffTop = parent.rect.height * diffMax.y;
        // �T�C�Y�ƍ��W�̏C��
        rectTransform.sizeDelta += new Vector2(diffLeft - diffRight, diffBottom - diffTop);
        var pivot = rectTransform.pivot;
        rectTransform.anchoredPosition -= new Vector2(
             (diffLeft * (1 - pivot.x)) + (diffRight * pivot.x),
             (diffBottom * (1 - pivot.y)) + (diffTop * pivot.y)
        );
    }

    /// <summary>
    /// ���W��ۂ����܂�Anchor��ύX����
    /// </summary>
    /// <param name="rectTransform">���g�̎Q��</param>
    /// <param name="minX">�ύX���AnchorMin��x���W</param>
    /// <param name="minY">�ύX���AnchorMin��y���W</param>
    /// <param name="maxX">�ύX���AnchorMax��x���W</param>
    /// <param name="maxY">�ύX���AnchorMax��y���W</param>
    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float minX, float minY, float maxX, float maxY)
    {
        rectTransform.SetAnchorWithKeepingPosition(new Vector2(minX, minY), new Vector2(maxX, maxY));
    }

    /// <summary>
    /// ����Ԃ��܂�
    /// </summary>
    public static float GetWidth(this RectTransform self)
    {
        return self.sizeDelta.x;
    }

    /// <summary>
    /// ������Ԃ��܂�
    /// </summary>
    public static float GetHeight(this RectTransform self)
    {
        return self.sizeDelta.y;
    }

    /// <summary>
    /// ����ݒ肵�܂�
    /// </summary>
    public static void SetWidth(this RectTransform self, float width)
    {
        var size = self.sizeDelta;
        size.x = width;
        self.sizeDelta = size;
    }

    /// <summary>
    /// ������ݒ肵�܂�
    /// </summary>
    public static void SetHeight(this RectTransform self, float height)
    {
        var size = self.sizeDelta;
        size.y = height;
        self.sizeDelta = size;
    }

    /// <summary>
    /// �T�C�Y��ݒ肵�܂�
    /// </summary>
    public static void SetSize(this RectTransform self, float width, float height)
    {
        self.sizeDelta = new Vector2(width, height);
    }
}




