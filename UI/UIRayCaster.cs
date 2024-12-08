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

    // 最も速いMsgと最も遅いMsgの秒数差を算出
    public static float SecDiff(List<UIMsg> uiMessages)
    {
        if (uiMessages == null || uiMessages.Count < 2) return 0;
        var earliest = uiMessages.Min(msg => msg.TS);
        var latest = uiMessages.Max(msg => msg.TS);
        return (float)Math.Abs((latest - earliest).TotalSeconds);
    }
}


// カメラにつける
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
    float dragThreshold = 5f;  // ドラッグ開始と判定する移動距離
    GameObject draggedGO = null; // ドラッグ中のゲームオブジェクト

# if UNITY_EDITOR
    [SerializeField] bool debugVisualize = true;
#else
    bool debugVisualize = false;
#endif
    GameObject debugMarker;


    // 全ての能動的な操作系イベントと同タイミングで発火するイベント
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
    //OnEnter, OnExit, OnOver は無し
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
            debugMarker.transform.localScale = Vector3.one * 0.15f; // 小さな球
            debugMarker.GetComponent<Renderer>().material.color = Color.blue; // 球の色を変更
        }
        //#endif
    }

    void Start()
    {
        //OnAction.Subscribe(go => Debug.Log($"アクション: {go.name}"));

        //OnEnter.Subscribe(go => Debug.Log($"RayUI マウスが入った: {go.name}"));
        //OnExit.Subscribe(go => Debug.Log($"RayUI マウスが出た: {go.name}"));
        //OnOver.Subscribe(go => Debug.Log($"RayUI マウスオーバー中: {go.name}"));

        //OnNone.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} なにもない:  {msg.GO.name}"));

        //OnDrag_Start.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} ドラッグ開始: {msg.GO.name}"));
        //OnDrag_Stop.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} ドラッグ終了: {msg.GO.name}"));
        //OnDrag.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} ドラッグ中: {msg.GO.name}"));

        //OnClick_L.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} クリック左: {msg.GO.name}"));
        //OnDoubleClick_L.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} ダブルクリック左: {msg.GO.name}"));

        //OnClick_R.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} クリック右: {msg.GO.name}"));
        //OnDoubleClick_R.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} ダブルクリック右: {msg.GO.name}"));

        //OnClick_M.Subscribe(msg => Debug.Log($"RayUI{gameObject.name} クリック中央: {msg.GO.name}"));
    }


    private void Update()
    {
        // 常にRayを飛ばす
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red); // Rayを可視化

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, cam.cullingMask))
        {
            currentGO = hit.collider.gameObject;
            //#if UNITY_EDITOR
            if (debugVisualize)
            {
                // 左クリックで球を生成
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log($"ヒット0{currentGO.name}");
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Destroy(sphere.GetComponent<Collider>());
                    sphere.transform.position = hit.point;
                    sphere.transform.localScale = Vector3.one * 0.15f; // 小さな球
                    sphere.GetComponent<Renderer>().material.color = Color.red; // 球の色を変更
                }
                debugMarker.SetActive(true);
                debugMarker.transform.position = hit.point;
            }
            //#endif

            // マウスが別のGOに入った時と出た時の判定
            if (currentGO != previousGO)
            {
                if (previousGO != null)
                {
                    // ◆マウスが出た◆
                    onExit.OnNext(new UIMsg(previousGO));
                }
                if (currentGO != null)
                {
                    // ◆マウスが入った◆
                    onEnter.OnNext(new UIMsg(currentGO));
                }
            }

            // ◆マウスオーバーされている間◆
            onOver.OnNext(new UIMsg(currentGO));

            // ヒットしている場合にクリック処理を一度だけ実行
            if (!isHandlingClick)
            {
                isHandlingClick = true;
                HandleClick().Forget();
            }

            // 左ボタン押下時に初期マウス位置を記録
            if (Input.GetMouseButtonDown(0))
            {
                initialMousePos = Input.mousePosition;
            }

            // ドラッグの開始判定
            if (Input.GetMouseButton(0) && !isDragging)
            {
                float distance = Vector3.Distance(Input.mousePosition, initialMousePos);
                if (distance > dragThreshold)
                {
                    isDragging = true;
                    draggedGO = currentGO; // ドラッグ対象を設定
                    // ◆ドラッグ開始◆
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
                // ◆マウスが出た◆
                onExit.OnNext(new UIMsg(previousGO));
            }

            // カレントGOリセット
            currentGO = null;
            // クリックハンドリングのフラグリセット
            isHandlingClick = false;

            if (Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.GetMouseButtonDown(2)) onNone.OnNext(new UIMsg(marginGO));
        }

        // ドラッグ中の処理
        if (isDragging)
        {
            // ドラッグの終了判定
            if (Input.GetMouseButtonUp(0) || currentGO != draggedGO)
            {
                isDragging = false;
                // ◆ドラッグ終了◆
                onDrag_Stop.OnNext(new UIMsg(draggedGO));
                draggedGO = null;
            }
            else
            {
                // ◆ドラッグ中◆
                onDrag.OnNext(new UIMsg(draggedGO));
            }
        }

        // 前回のオブジェクトを更新
        previousGO = currentGO;
    }

    private async UniTaskVoid HandleClick()
    {
        while (isHandlingClick)
        {
            GameObject targetObject = currentGO;
            // 左クリック
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log($"ヒット1{currentGO} {targetObject}");
                float firstClickTime = Time.time;
                bool doubleClickDetected = false;

                // ダブルクリック判定のためのループ
                while (Time.time - firstClickTime < doubleClickInterval)
                {
                    await UniTask.Yield(); // 次のフレームまで待機

                    if (Input.GetMouseButtonDown(0))
                    {
                        doubleClickDetected = true;
                        // ◆左ダブルクリック◆
                        onDoubleClick_L.OnNext(new UIMsg(targetObject));
                        isHandlingClick = false;
                        break;
                    }

                    // クリック対象から外れた場合、処理を終了
                    if (!isHandlingClick) return;
                }

                if (!doubleClickDetected)
                {
                    // ◆左クリック◆
                    //Debug.Log($"ヒット2{currentGO} {targetObject}");
                    onClick_L.OnNext(new UIMsg(targetObject));
                    isHandlingClick = false;
                }
            }
            // 右クリック
            else if (Input.GetMouseButtonDown(1))
            {
                float firstClickTime = Time.time;
                bool doubleClickDetected = false;

                // 右ダブルクリック判定のためのループ
                while (Time.time - firstClickTime < doubleClickInterval)
                {
                    await UniTask.Yield(); // 次のフレームまで待機

                    if (Input.GetMouseButtonDown(1))
                    {
                        doubleClickDetected = true;
                        // ◆右ダブルクリック◆
                        onDoubleClick_R.OnNext(new UIMsg(targetObject));
                        break;
                    }

                    // クリック対象から外れた場合、処理を終了
                    if (!isHandlingClick) return;
                }

                if (!doubleClickDetected)
                {
                    // ◆右クリック◆
                    onClick_R.OnNext(new UIMsg(targetObject));
                }
            }
            // ミドルクリック
            else if (Input.GetMouseButtonDown(2))
            {
                // ◆ミドルクリック◆
                onClick_M.OnNext(new UIMsg(targetObject));
            }

            await UniTask.Yield(); // 次のフレームまで待機
        }
    }
}
