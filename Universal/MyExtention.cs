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


//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
// �I�I�I�@CompoUtil ���Ɉړ��ς݁@�I�I�I
//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I

//public interface IMyExtention
//{
//    T CheckAddComponent<T>(GameObject obj) where T : Component;
//    Component CheckAddComponent(Type type, GameObject gObj);
//    void CheckAddMultiComponent(List<Type> compos, GameObject gObj);
//    GameObject CreateChild(string name, GameObject parent, Type compo);
//    GameObject CreateChild(string name, GameObject parent, List<Type> compos);
//    List<GameObject> CreateChildren(string name, GameObject parent, Type compo, int quantity);
//    List<GameObject> CreateChildren(string name, GameObject parent, List<Type> compos, int quantity);
//}


////========================================
////  MonoBehaviour �̊g���N���X�Ȃ̂�  MonoBehaviour �̑���Ɏg��
////========================================
//public abstract class MonoBehaviourMyExtention : MonoBehaviour, IMyExtention
//{
//    //========================================
//    // �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾
//    // �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾
//    //========================================
//    public Compo CheckAddComponent<Compo>(GameObject gObj) where Compo : Component
//    {
//        //Debug.Log($"�R���| {typeof(Compo).Name}");
//        #region �Ăяo�����ʒm
//        var caller = new System.Diagnostics.StackFrame(1, false);
//        string callerMethodName = caller.GetMethod().Name;
//        string callerClassName = caller.GetMethod().DeclaringType.Name;
//        //Debug.Log("�N���X  " + callerClassName + " �́A     ���\�b�h  " + callerMethodName + "()  ����Ăяo����܂����B");
//        #endregion
//        //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
//        Compo targetCompo = gObj.GetComponent<Compo>();
//        if (targetCompo == null)
//        {
//            targetCompo = gObj.AddComponent<Compo>();
//        }
//        return targetCompo;
//    }


//    //========================================
//    // �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾
//    // �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B��L�� CheckComponent<T> �Ƃ͎኱�p�r���Ⴄ
//    // CheckComponent<T> �̏ꍇ�A�^������ <����.GetType()> �݂����ɁA�N���X���擾���Ȃ���ԐړI�ɓn�����Ƃ���ƁA
//    // <> �̕��������Z�q�Ɣ��f����Ă��܂��Ďg���Ȃ�
//    // �����������ꍇ�ɂ�������g��
//    // �������߂�l�� Component�^ �Ȃ̂ŁA�擾�����R���|�[�l���g���g�������ꍇ�͍X�ɕϊ����K�v
//    //========================================
//    public Component CheckAddComponent(Type Compo, GameObject gObj)
//    {
//        //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
//        Component targetCompo = gObj.GetComponent(Compo);
//        if (targetCompo == null)
//        {
//            targetCompo = gObj.AddComponent(Compo);
//        }
//        return targetCompo;
//    }


//    public void CheckAddMultiComponent(List<Type> compos, GameObject gObj)
//    {
//        foreach (Type a in compos)
//        {
//            CheckAddComponent(a, gObj);
//        }
//    }


//    public GameObject CreateChild(string name, GameObject parent, Type compo)
//    {
//        GameObject gObj = new GameObject();
//        CheckAddComponent(compo, gObj);
//        gObj.transform.parent = parent.transform;
//        gObj.name = name;

//        return gObj;
//    }

//    public GameObject CreateChild(string name, GameObject parent, List<Type> compos)
//    {
//        GameObject gObj = new GameObject();
//        CheckAddMultiComponent(compos, gObj);
//        gObj.transform.parent = parent.transform;
//        gObj.name = name;

//        return gObj;
//    }

//    public List<GameObject> CreateChildren(string name, GameObject parent, Type compo, int quantity)
//    {
//        List<GameObject> gObjs = new List<GameObject>();
//        for (int a = 0; a < quantity; a++)
//        {
//            GameObject gObj = new GameObject();
//            CheckAddComponent(compo, gObj);
//            gObj.transform.parent = parent.transform;
//            gObj.name = name;
//            gObjs.Add(gObj);
//        }
//        return gObjs;
//    }

//    public List<GameObject> CreateChildren(string name, GameObject parent, List<Type> compos, int quantity)
//    {
//        List<GameObject> gObjs = new List<GameObject>();
//        for (int a = 0; a < quantity; a++)
//        {
//            GameObject gObj = new GameObject();
//            CheckAddMultiComponent(compos, gObj);
//            gObj.transform.parent = parent.transform;
//            gObj.name = name;
//            gObjs.Add(gObj);
//        }
//        return gObjs;
//    }
//}


////========================================
////�y���Ӂz
//// MonoBehaviour ���p�����Ă���킯�ł͂Ȃ����A
//// MonoBehaviour ���p�����Ă���N���X�𗘗p���ăn���h�����O���Ă���̂ŁA
//// Awake()�����s�����ȑO�Ɏg���̂͗ǂ��Ȃ�
////========================================
//public class MyExtention : IMyExtention
//{
//    class MyExtentionHandler : SingletonCompo<MyExtentionHandler> { }

//    public T CheckAddComponent<T>(GameObject obj) where T : Component
//    {
//        return MyExtentionHandler.Compo.CheckAddComponent<T>(obj);
//    }

//    public Component CheckAddComponent(Type type, GameObject gObj)
//    {
//        return MyExtentionHandler.Compo.CheckAddComponent(type, gObj);
//    }

//    public void CheckAddMultiComponent(List<Type> compos, GameObject gObj)
//    {
//        MyExtentionHandler.Compo.CheckAddMultiComponent(compos, gObj);
//    }


//    public GameObject CreateChild(string name, GameObject parent, Type compo)
//    {
//        return MyExtentionHandler.Compo.CreateChild(name, parent, compo);
//    }

//    public GameObject CreateChild(string name, GameObject parent, List<Type> compos)
//    {
//        return MyExtentionHandler.Compo.CreateChild(name, parent, compos);
//    }

//    public List<GameObject> CreateChildren(string name, GameObject parent, Type compo, int quantity)
//    {
//        return MyExtentionHandler.Compo.CreateChildren(name, parent, compo, quantity);
//    }

//    public List<GameObject> CreateChildren(string name, GameObject parent, List<Type> compos, int quantity)
//    {
//        return MyExtentionHandler.Compo.CreateChildren(name, parent, compos, quantity);
//    }
//}


//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
// �I�I�I�@StringUtil �Ɉړ��ς݁@�I�I�I
//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I

//public static class StringExtentions
//{
//    // String�̃g���~���O
//    // �E���؂蔲��
//    public static string CropStr_R(this string str, string splitter, bool containSplitter)
//    {
//        int i = str.IndexOf(splitter);
//        if (i < 0) return str;

//        int a;
//        if (containSplitter) a = 0;
//        else a = splitter.Length;

//        return str.Substring(i + a);
//    }
//    // �E���؂藎�Ƃ�
//    public static string TrimStr_R(this string str, string splitter, bool containSplitter)
//    {
//        var i = str.IndexOf(splitter);
//        if (i < 0) return str;

//        int a;
//        if (containSplitter) a = splitter.Length;
//        else a = 0;

//        return str.Substring(0, i + a);
//    }

//    // --------------------------------
//    // �����̌�����
//    // --------------------------------
//    public static string HexColor(this string str, string hexCode) => string.Format("<color={0}>{1}</color>", hexCode, str);
//    public static string Red(this string str) => str.HexColor("red");
//    public static string Green(this string str) => str.HexColor("green");
//    public static string Blue(this string str) => str.HexColor("blue");
//    public static string Black(this string str) => str.HexColor("black");
//    public static string White(this string str) => str.HexColor("white");
//    public static string Gray(this string str) => str.HexColor("#808080");
//    public static string Yellow(this string str) => str.HexColor("yellow");
//    public static string Magenta(this string str) => str.HexColor("#FF00FF");
//    public static string Cyan(this string str) => str.HexColor("#00FFFF");
//    public static string Orange(this string str) => str.HexColor("orange");
//    public static string Purple(this string str) => str.HexColor("purple");
//    public static string Brown(this string str) => str.HexColor("#A52A2A");
//    public static string Size(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
//    public static string Large(this string str) => str.Size(16);
//    public static string Medium(this string str) => str.Size(11);
//    public static string Small(this string str) => str.Size(9);
//    public static string Bold(this string str) => string.Format("<b>{0}</b>", str);
//    public static string Italic(this string str) => string.Format("<i>{0}</i>", str);
//}


//public static class GameObjectExtentions
//{
//    //===================================
//    // �����Ǝq���I�u�W�F�N�g�̃��C���[��ύX
//    //===================================
//    public static void SetLayerRecursive(this GameObject parent, string layerName) { SetLayerRecursive(parent.transform, layerName); }
//    public static void SetLayerRecursive(this Transform parent, string layerName)
//    {
//        parent.gameObject.layer = LayerMask.NameToLayer(layerName);
//        foreach (Transform child in parent)
//        {
//            child.gameObject.layer = LayerMask.NameToLayer(layerName);
//            SetLayerRecursive(child, layerName);
//        }
//    }

//    //===================================
//    // �����Ǝq���I�u�W�F�N�g�̃A�N�e�B�u��Ԃ�ύX
//    //===================================
//    public static void SetActiveRecursive(this GameObject parent, bool activeState) { SetActiveRecursive(parent.transform, activeState); }
//    public static async void SetActiveRecursive(this Transform parent, bool activeState)
//    {
//        parent.gameObject.SetActive(activeState);
//        foreach (Transform child in parent)
//        {
//            SetActiveRecursive(child, activeState);
//        }
//    }
//}



//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
// �ړ��ς� 
// ���l�[���ς�
// SealableMonoBehaviourMyExtention -> SealableMonoBehaviour
//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
//public class SealableMonoBehaviour : MonoBehaviour//MonoBehaviourMyExtention
//{
//    protected virtual void Awake() { }
//    protected virtual void Start() { }
//    protected virtual void Update() { }
//    protected virtual void FixedUpdate() { }
//}




//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
// �I�I�I�@ObservableEx �Ɉړ��ς݁@�I�I�I
//�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I
//public static class ObservableExtensions
//{
//    // ���b�Z�[�W�̒l���w�肵�����̂ƈ�v���Ă����������s�J�n�B�قȂ��Ă�����I��
//    // ������s�̊Ԋu�͕b���w��(�ŒZ0.001s)�B�����n���Ȃ���΃t���[�����̎��s�ƂȂ�
//    public static IObservable<long> UpdateWhileEqualTo<T>(
//        this IObservable<T> source,
//        T expectedValue,
//        float sec = 0f )
//    {
//        // �C���^�[�o���̉����ݒ�
//        float interval = sec;
//        if (sec <= 0) interval = 0;
//        else
//        if (sec <= 0.02f) interval = 0.02f;

//        // �t���[��������
//        if (interval == 0)
//            return source
//                .Select(value =>
//                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
//                        Observable.EveryUpdate() :
//                        Observable.Empty<long>()
//                )
//                .Switch();
//        // �b��������
//        else
//            return source
//                .Select(value =>
//                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
//                        Observable.Interval(TimeSpan.FromSeconds(sec)) :
//                        Observable.Empty<long>()
//                )
//                .Switch();
//    }


//    // UpdateWhileEqualTo��1�t���[�����Z���b���𑪂�Ȃ�
//    // ������͒Z�������(�f�t�H���g0.001, ����0.0001)���X���b�h�v�[���ł��̂Ńt���[���Ɉˑ�����Unity��API���g���Ȃ�(�� Time.time)
//    public static IObservable<long> TimerWhileEqualTo<T>(
//        this IObservable<T> source,
//        T expectedValue,
//        float sec = 0.001f)
//    {
//        // �C���^�[�o���̉����ݒ�
//        float interval = sec;
//        if (sec <= 0.0001f) interval = 0.0001f;

//        // �ɏ��b��������
//        return source
//            .Select(value =>
//                EqualityComparer<T>.Default.Equals(value, expectedValue) ?
//                    ObservableStopwatch(interval) :
//                    Observable.Empty<long>()
//            )
//            .Switch();
//    }

//    // Stopwatch���g���A�t���[�����[�g�Ɉˑ������u�w��b�v��OnNext���J��Ԃ��B
//    private static IObservable<long> ObservableStopwatch(float sec)
//    {
//        return Observable.Create<long>(observer =>
//        {
//            var cts = new CancellationTokenSource();

//            UniTask.RunOnThreadPool(() =>
//            {
//                //var interval = TimeSpan.FromSeconds(sec);
//                long count = 0;
//                var stopwatch = new System.Diagnostics.Stopwatch();
//                try
//                {
//                    stopwatch.Start();
//                    //if (stopwatch.Elapsed.TotalSeconds >= interval.TotalSeconds)
//                    while (!cts.IsCancellationRequested)
//                    if (stopwatch.Elapsed.TotalSeconds >= sec)
//                    {
//                        observer.OnNext(count++);
//                        // �������[�v�̃X���b�h���Ɛ�΍��CPU�X�C�b�`
//                        Thread.Sleep(0);
//                        stopwatch.Reset();
//                        stopwatch.Start();
//                    }
//                }
//                catch (OperationCanceledException)
//                {
//                    // �L�����Z�����͉������Ȃ�
//                }
//                finally
//                {
//                    observer.OnCompleted();
//                }
//            }, cancellationToken: cts.Token).Forget();
//            return Disposable.Create(() => cts.Cancel());
//        });
//    }
//}



//public static class ListExtentions
//{
//    public static List<int> Mode(this List<int> numbers)
//    {
//        var groupedNumbers = numbers.GroupBy(n => n)
//                                    .Select(g => new { Number = g.Key, Count = g.Count() })
//                                    .OrderByDescending(g => g.Count);

//        int maxCount = groupedNumbers.First().Count;

//        List<int> modes = groupedNumbers.Where(g => g.Count == maxCount)
//                                  .Select(g => g.Number).ToList();

//        Debug.Log("�ŕp�l: " + string.Join(", ", modes));

//        return modes;
//    }
//}



//public static class RectTransformExtensions
//{
//    /// <summary>
//    /// ���W��ۂ����܂�Pivot��ύX����
//    /// </summary>
//    /// <param name="rectTransform">���g�̎Q��</param>
//    /// <param name="targetPivot">�ύX���Pivot���W</param>
//    public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, Vector2 targetPivot)
//    {
//        var diffPivot = targetPivot - rectTransform.pivot;
//        rectTransform.pivot = targetPivot;
//        var diffPos = new Vector2(rectTransform.sizeDelta.x * diffPivot.x, rectTransform.sizeDelta.y * diffPivot.y);
//        rectTransform.anchoredPosition += diffPos;
//    }

//    /// <summary>
//    /// ���W��ۂ����܂�Pivot��ύX����
//    /// </summary>
//    /// <param name="rectTransform">���g�̎Q��</param>
//    /// <param name="x">�ύX���Pivot��x���W</param>
//    /// <param name="y">�ύX���Pivot��y���W</param>
//    public static void SetPivotWithKeepingPosition(this RectTransform rectTransform, float x, float y)
//    {
//        rectTransform.SetPivotWithKeepingPosition(new Vector2(x, y));
//    }

//    /// <summary>
//    /// ���W��ۂ����܂�Anchor��ύX����
//    /// </summary>
//    /// <param name="rectTransform">���g�̎Q��</param>
//    /// <param name="targetAnchor">�ύX���Anchor���W (min,max�����ʂ̏ꍇ)</param>
//    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetAnchor)
//    {
//        rectTransform.SetAnchorWithKeepingPosition(targetAnchor, targetAnchor);
//    }

//    /// <summary>
//    /// ���W��ۂ����܂�Anchor��ύX����
//    /// </summary>
//    /// <param name="rectTransform">���g�̎Q��</param>
//    /// <param name="x">�ύX���Anchor��x���W (min,max�����ʂ̏ꍇ)</param>
//    /// <param name="y">�ύX���Anchor��y���W (min,max�����ʂ̏ꍇ)</param>
//    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float x, float y)
//    {
//        rectTransform.SetAnchorWithKeepingPosition(new Vector2(x, y));
//    }

//    /// <summary>
//    /// ���W��ۂ����܂�Anchor��ύX����
//    /// </summary>
//    /// <param name="rectTransform">���g�̎Q��</param>
//    /// <param name="targetMinAnchor">�ύX���AnchorMin���W</param>
//    /// <param name="targetMaxAnchor">�ύX���AnchorMax���W</param>
//    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, Vector2 targetMinAnchor, Vector2 targetMaxAnchor)
//    {
//        var parent = rectTransform.parent as RectTransform;
//        if (parent == null) { Debug.LogError("Parent cannot find."); }

//        var diffMin = targetMinAnchor - rectTransform.anchorMin;
//        var diffMax = targetMaxAnchor - rectTransform.anchorMax;
//        // anchor�̍X�V
//        rectTransform.anchorMin = targetMinAnchor;
//        rectTransform.anchorMax = targetMaxAnchor;
//        // �㉺���E�̋����̍������v�Z
//        var diffLeft = parent.rect.width * diffMin.x;
//        var diffRight = parent.rect.width * diffMax.x;
//        var diffBottom = parent.rect.height * diffMin.y;
//        var diffTop = parent.rect.height * diffMax.y;
//        // �T�C�Y�ƍ��W�̏C��
//        rectTransform.sizeDelta += new Vector2(diffLeft - diffRight, diffBottom - diffTop);
//        var pivot = rectTransform.pivot;
//        rectTransform.anchoredPosition -= new Vector2(
//             (diffLeft * (1 - pivot.x)) + (diffRight * pivot.x),
//             (diffBottom * (1 - pivot.y)) + (diffTop * pivot.y)
//        );
//    }

//    /// <summary>
//    /// ���W��ۂ����܂�Anchor��ύX����
//    /// </summary>
//    /// <param name="rectTransform">���g�̎Q��</param>
//    /// <param name="minX">�ύX���AnchorMin��x���W</param>
//    /// <param name="minY">�ύX���AnchorMin��y���W</param>
//    /// <param name="maxX">�ύX���AnchorMax��x���W</param>
//    /// <param name="maxY">�ύX���AnchorMax��y���W</param>
//    public static void SetAnchorWithKeepingPosition(this RectTransform rectTransform, float minX, float minY, float maxX, float maxY)
//    {
//        rectTransform.SetAnchorWithKeepingPosition(new Vector2(minX, minY), new Vector2(maxX, maxY));
//    }

//    /// <summary>
//    /// ����Ԃ��܂�
//    /// </summary>
//    public static float GetWidth(this RectTransform self)
//    {
//        return self.sizeDelta.x;
//    }

//    /// <summary>
//    /// ������Ԃ��܂�
//    /// </summary>
//    public static float GetHeight(this RectTransform self)
//    {
//        return self.sizeDelta.y;
//    }

//    /// <summary>
//    /// ����ݒ肵�܂�
//    /// </summary>
//    public static void SetWidth(this RectTransform self, float width)
//    {
//        var size = self.sizeDelta;
//        size.x = width;
//        self.sizeDelta = size;
//    }

//    /// <summary>
//    /// ������ݒ肵�܂�
//    /// </summary>
//    public static void SetHeight(this RectTransform self, float height)
//    {
//        var size = self.sizeDelta;
//        size.y = height;
//        self.sizeDelta = size;
//    }

//    /// <summary>
//    /// �T�C�Y��ݒ肵�܂�
//    /// </summary>
//    public static void SetSize(this RectTransform self, float width, float height)
//    {
//        self.sizeDelta = new Vector2(width, height);
//    }
//}




