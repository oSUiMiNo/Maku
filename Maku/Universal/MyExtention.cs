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
///  MonoBehaviour �̊g���N���X�Ȃ̂�  MonoBehaviour �̑���Ɏg���B
/// </summary>
public abstract class MonoBehaviourMyExtention : MonoBehaviour, IMyExtention
{
    /// <Summary>
    /// �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾�B
    /// �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B
    /// </Summary>
    public Compo CheckComponent<Compo>(GameObject gObj) where Compo : Component
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
        if(targetCompo == null)
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
    public Component CheckComponent(Type Compo, GameObject gObj)
    {
        //������TryGetComponent �g���ƁAAddComponent �̏��ŁAMonoBehaviour �p�����ĂȂ��Ƃ�������B�����͂��̂������ׂ�
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
    public T CheckComponent<T>(GameObject obj) where T : Component
    {
        return MyExtentionHandler.Compo.CheckComponent<T>(obj);
    }

    /// <Summary>
    /// �^�����Ŏw�肳�ꂽ�R���|�[�l���g�ւ̎Q�Ƃ��擾�B
    /// �R���|�[�l���g���Ȃ��ꍇ�̓A�^�b�`���ĎQ�Ƃ��擾�B��L�� CheckComponent<T> �Ƃ͎኱�p�r���Ⴄ�B
    /// CheckComponent<T> �̏ꍇ�A�^������ <����.GetType()> �݂����ɁA�N���X���擾���Ȃ���ԐړI�ɓn�����Ƃ���ƁA
    /// <> �̕��������Z�q�Ɣ��f����Ă��܂��Ďg���Ȃ��B
    /// �����������ꍇ�ɂ�������g���B
    /// �������߂�l�� Component�^ �Ȃ̂ŁA�擾�����R���|�[�l���g���g�������ꍇ�͍X�ɕϊ����K�v�B
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




