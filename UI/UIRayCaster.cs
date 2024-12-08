using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;


public class UIMsg
{
    public UIMsg(GameObject go, string msg = "")
    {
        GO = go;
        Msg = msg;
        TS = DateTime.UtcNow;
    }

    public GameObject GO;
    public string Msg;
    public DateTime TS;

    // �ł�����Msg�ƍł��x��Msg�̕b�������Z�o
    public static float SecDiff(List<UIMsg> uiMessages)
    {
        if (uiMessages == null || uiMessages.Count < 2) return 0;
        var earliest = uiMessages.Min(msg => msg.TS);
        var latest = uiMessages.Max(msg => msg.TS);
        return (float)Math.Abs((latest - earliest).TotalSeconds);
    }
}


// �J�����ɂ���
public class UIRayCaster : MonoBehaviour
{
    Camera cam;
    public bool Active;

    float doubleClickInterval = 0.25f;
    bool isHandlingClick = false;

    GameObject currentGO = null;
    GameObject previousGO = null;
    GameObject marginGO = null;

    bool isDragging = false;
    Vector3 initialMousePos = Vector3.zero;
    float dragThreshold = 5f;  // �h���b�O�J�n�Ɣ��肷��ړ�����
    GameObject draggedGO = null; // �h���b�O���̃Q�[���I�u�W�F�N�g

# if UNITY_EDITOR
    [SerializeField] bool debugVisualize = true;
#else
    bool debugVisualize = false;
#endif
    GameObject debugMarker;


    // �S�Ă̔\���I�ȑ���n�C�x���g�Ɠ��^�C�~���O�Ŕ��΂���C�x���g
    public IObservable<UIMsg> OnAction => Observable.Merge
    (
        OnNone,
        OnDrag_Start,
        OnDrag_Stop,
        OnDrag,
        OnClick_L,
        OnDoubleClick_L,
        OnClick_R,
        OnDoubleClick_R,
        OnClick_M
    //OnEnter, OnExit, OnOver �͖���
    ).Where(msg => enabled);

    public IObservable<UIMsg> OnNone => onNone.Where(msg => enabled);
    public IObservable<UIMsg> OnEnter => onEnter.Where(msg => enabled);
    public IObservable<UIMsg> OnExit => onExit.Where(msg => enabled);
    public IObservable<UIMsg> OnOver => onOver.Where(msg => enabled);
    public IObservable<UIMsg> OnDrag_Start => onDrag_Start.Where(msg => enabled);
    public IObservable<UIMsg> OnDrag_Stop => onDrag_Stop.Where(msg => enabled);
    public IObservable<UIMsg> OnDrag => onDrag.Where(msg => enabled);
    public IObservable<UIMsg> OnClick_L => onClick_L.Where(msg => enabled);
    public IObservable<UIMsg> OnDoubleClick_L => onDoubleClick_L.Where(msg => enabled);
    public IObservable<UIMsg> OnClick_R => onClick_R.Where(msg => enabled);
    public IObservable<UIMsg> OnDoubleClick_R => onDoubleClick_R.Where(msg => enabled);
    public IObservable<UIMsg> OnClick_M => onClick_M.Where(msg => enabled);

    public Subject<UIMsg> onNone = new Subject<UIMsg>();
    public Subject<UIMsg> onEnter = new Subject<UIMsg>();
    public Subject<UIMsg> onExit = new Subject<UIMsg>();
    public Subject<UIMsg> onOver = new Subject<UIMsg>();
    public Subject<UIMsg> onDrag_Start = new Subject<UIMsg>();
    public Subject<UIMsg> onDrag_Stop = new Subject<UIMsg>();
    public Subject<UIMsg> onDrag = new Subject<UIMsg>();
    public Subject<UIMsg> onClick_L = new Subject<UIMsg>();
    public Subject<UIMsg> onDoubleClick_L = new Subject<UIMsg>();
    public Subject<UIMsg> onClick_R = new Subject<UIMsg>();
    public Subject<UIMsg> onDoubleClick_R = new Subject<UIMsg>();
    public Subject<UIMsg> onClick_M = new Subject<UIMsg>();

    void Awake()
    {
        marginGO = new GameObject("marginGO");
        cam = GetComponent<Camera>();
        //#if UNITY_EDITOR
        if (debugVisualize)
        {
            debugMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(debugMarker.GetComponent<Collider>());
            debugMarker.transform.localScale = Vector3.one * 0.15f; // �����ȋ�
            debugMarker.GetComponent<Renderer>().material.color = Color.blue; // ���̐F��ύX
        }
        //#endif
    }

    void Start()
    {
        //OnAction.Subscribe(go => Debug.Log($"�A�N�V����: {go.name}"));

        //OnEnter.Subscribe(go => Debug.Log($"RayUI �}�E�X��������: {go.name}"));
        //OnExit.Subscribe(go => Debug.Log($"RayUI �}�E�X���o��: {go.name}"));
        //OnOver.Subscribe(go => Debug.Log($"RayUI �}�E�X�I�[�o�[��: {go.name}"));

        //OnNone.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �Ȃɂ��Ȃ�:  {msg.GO.name}"));

        //OnDrag_Start.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �h���b�O�J�n: {msg.GO.name}"));
        //OnDrag_Stop.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �h���b�O�I��: {msg.GO.name}"));
        //OnDrag.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �h���b�O��: {msg.GO.name}"));

        //OnClick_L.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �N���b�N��: {msg.GO.name}"));
        //OnDoubleClick_L.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �_�u���N���b�N��: {msg.GO.name}"));

        //OnClick_R.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �N���b�N�E: {msg.GO.name}"));
        //OnDoubleClick_R.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �_�u���N���b�N�E: {msg.GO.name}"));

        //OnClick_M.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} �N���b�N����: {msg.GO.name}"));
    }


    private void Update()
    {
        // ���Ray���΂�
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red); // Ray������

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cam.cullingMask))
        {
            currentGO = hit.collider.gameObject;
            //#if UNITY_EDITOR
            if (debugVisualize)
            {
                // ���N���b�N�ŋ��𐶐�
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log($"�q�b�g0{currentGO.name}");
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Destroy(sphere.GetComponent<Collider>());
                    sphere.transform.position = hit.point;
                    sphere.transform.localScale = Vector3.one * 0.15f; // �����ȋ�
                    sphere.GetComponent<Renderer>().material.color = Color.red; // ���̐F��ύX
                }
                debugMarker.SetActive(true);
                debugMarker.transform.position = hit.point;
            }
            //#endif

            // �}�E�X���ʂ�GO�ɓ��������Əo�����̔���
            if (currentGO != previousGO)
            {
                if (previousGO != null)
                {
                    // ���}�E�X���o����
                    onExit.OnNext(new UIMsg(previousGO));
                }
                if (currentGO != null)
                {
                    // ���}�E�X����������
                    onEnter.OnNext(new UIMsg(currentGO));
                }
            }

            // ���}�E�X�I�[�o�[����Ă���ԁ�
            onOver.OnNext(new UIMsg(currentGO));

            // �q�b�g���Ă���ꍇ�ɃN���b�N��������x�������s
            if (!isHandlingClick)
            {
                isHandlingClick = true;
                HandleClick().Forget();
            }

            // ���{�^���������ɏ����}�E�X�ʒu���L�^
            if (Input.GetMouseButtonDown(0))
            {
                initialMousePos = Input.mousePosition;
            }

            // �h���b�O�̊J�n����
            if (Input.GetMouseButton(0) && !isDragging)
            {
                float distance = Vector3.Distance(Input.mousePosition, initialMousePos);
                if (distance > dragThreshold)
                {
                    isDragging = true;
                    draggedGO = currentGO; // �h���b�O�Ώۂ�ݒ�
                    // ���h���b�O�J�n��
                    onDrag_Start.OnNext(new UIMsg(draggedGO));
                }
            }
        }
        else
        {
            //#if UNITY_EDITOR
            if (debugVisualize)
            {
                debugMarker.SetActive(false);
            }
            //#endif
            if (previousGO != null)
            {
                // ���}�E�X���o����
                onExit.OnNext(new UIMsg(previousGO));
            }

            // �J�����gGO���Z�b�g
            currentGO = null;
            // �N���b�N�n���h�����O�̃t���O���Z�b�g
            isHandlingClick = false;

            if (Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2)) onNone.OnNext(new UIMsg(marginGO));
        }

        // �h���b�O���̏���
        if (isDragging)
        {
            // �h���b�O�̏I������
            if (Input.GetMouseButtonUp(0) || currentGO != draggedGO)
            {
                isDragging = false;
                // ���h���b�O�I����
                onDrag_Stop.OnNext(new UIMsg(draggedGO));
                draggedGO = null;
            }
            else
            {
                // ���h���b�O����
                onDrag.OnNext(new UIMsg(draggedGO));
            }
        }

        // �O��̃I�u�W�F�N�g���X�V
        previousGO = currentGO;
    }

    private async UniTaskVoid HandleClick()
    {
        while (isHandlingClick)
        {
            GameObject targetObject = currentGO;
            // ���N���b�N
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log($"�q�b�g1{currentGO} {targetObject}");
                float firstClickTime = Time.time;
                bool doubleClickDetected = false;

                // �_�u���N���b�N����̂��߂̃��[�v
                while (Time.time - firstClickTime < doubleClickInterval)
                {
                    await UniTask.Yield(); // ���̃t���[���܂őҋ@

                    if (Input.GetMouseButtonDown(0))
                    {
                        doubleClickDetected = true;
                        // �����_�u���N���b�N��
                        onDoubleClick_L.OnNext(new UIMsg(targetObject));
                        isHandlingClick = false;
                        break;
                    }

                    // �N���b�N�Ώۂ���O�ꂽ�ꍇ�A�������I��
                    if (!isHandlingClick) return;
                }

                if (!doubleClickDetected)
                {
                    // �����N���b�N��
                    //Debug.Log($"�q�b�g2{currentGO} {targetObject}");
                    onClick_L.OnNext(new UIMsg(targetObject));
                    isHandlingClick = false;
                }
            }
            // �E�N���b�N
            else if (Input.GetMouseButtonDown(1))
            {
                float firstClickTime = Time.time;
                bool doubleClickDetected = false;

                // �E�_�u���N���b�N����̂��߂̃��[�v
                while (Time.time - firstClickTime < doubleClickInterval)
                {
                    await UniTask.Yield(); // ���̃t���[���܂őҋ@

                    if (Input.GetMouseButtonDown(1))
                    {
                        doubleClickDetected = true;
                        // ���E�_�u���N���b�N��
                        onDoubleClick_R.OnNext(new UIMsg(targetObject));
                        break;
                    }

                    // �N���b�N�Ώۂ���O�ꂽ�ꍇ�A�������I��
                    if (!isHandlingClick) return;
                }

                if (!doubleClickDetected)
                {
                    // ���E�N���b�N��
                    onClick_R.OnNext(new UIMsg(targetObject));
                }
            }
            // �~�h���N���b�N
            else if (Input.GetMouseButtonDown(2))
            {
                // ���~�h���N���b�N��
                onClick_M.OnNext(new UIMsg(targetObject));
            }

            await UniTask.Yield(); // ���̃t���[���܂őҋ@
        }
    }
}
