using Cysharp.Threading.Tasks;
//using UniGLTF;
using UnityEngine;
using UniRx;
using MyUtil;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MySlider : MyUI
{
    GameObject backGround;
    GameObject fill;
    GameObject handle;
    GameObject mouseTracker;
    GameObject point_Zero;

    Vector3 handleLange_High;
    Vector3 handleLange_Low;

    public FloatReactiveProperty Value_Float = new FloatReactiveProperty(0);
    public IntReactiveProperty Value_Int = new IntReactiveProperty(0);
 
    
    enum Type
    {
        Min_Max,
        Minus_Plus
    }
    [SerializeField]
    Type type;
    

    //MousePosFromGameObject mousePosFromGO;
    protected sealed override async void Awake0()
    {
        //presenter = UIPresenter.Childlen[gameObject]; // 親クラスでやってるから要らなくね？
        backGround = transform.Find("Body/BackGround").gameObject;
        fill = transform.Find("Body/Fill").gameObject;
        handle = transform.Find("Body/Handle").gameObject;
        mouseTracker = transform.Find("Body/MouseTracker").gameObject;
        point_Zero = transform.Find("Body/Point_Zero").gameObject;
        handleLange_High = transform.Find("Body/handleLange_High").localPosition;
        handleLange_Low = transform.Find("Body/handleLange_Low").localPosition;

        //if (type == Type.Minus_Plus) point_Zero.GetOrAddComponent<SpriteRenderer>().enabled = true;
        //else point_Zero.GetOrAddComponent<SpriteRenderer>().enabled = false;

        // 上記のGetOrAddComponentはUniGLTFを使うようなので、CheckAddComponentに置き換えてみたが、正常にうごくかはまだ分からない。
        //if (type == Type.Minus_Plus) CheckAddComponent<SpriteRenderer>(point_Zero).enabled = true;
        //else CheckAddComponent<SpriteRenderer>(point_Zero).enabled = false;
        if (type == Type.Minus_Plus) point_Zero.CheckAddCompo<SpriteRenderer>().enabled = true;
        else point_Zero.CheckAddCompo<SpriteRenderer>().enabled = false;

        //Vector3 filScale = fill.transform.localScale;
        //if (type == Type.Min_Max)
        //    filScale.x = handle.transform.localPosition.x * 5 / 11;
        //else
        //    filScale.x = backGround.transform.localScale.x / 2;

        //fill.transform.localScale = filScale;
        SetFill();　// 加筆

        //On_Down.Subscribe(_ =>
        //{
        //    //Debug.Log("ダウン");
        //    //mousePosFromGO = new MousePosFromGameObject(handle, false);
        //}).AddTo(gameObject);
        On_Drag.Subscribe(_ =>
        {
            //Debug.Log("スライド");
            //Vector3 MousePos_World = mousePosFromGO.GOPosFromMouse;
            Vector3 MousePos_World = handle.PosUnderMouse();
            MousePos_World.y = transform.position.y;
            SetHandle(MousePos_World);
            SetFill();
            SetValue();
        }).AddTo(gameObject);


        // == ここ動かすと起動時にクラッシュする。原因調べてリファクタリング ========= 
        //// 加筆
        //Value_Float.Subscribe(async value =>
        //{
        //    //handle.transform.localPosition = new Vector3(value, 0, 0);
        //    //await Delay.Frame(1);
        //    SetHandle(new Vector3(value, 0, 0));
        //    SetFill();
        //    SetValue();
        //}).AddTo(gameObject);
        //// 加筆
        //Value_Int.Subscribe(async value =>
        //{
        //    //handle.transform.localPosition = new Vector3(value, 0, 0);
        //    //await Delay.Frame(1);
        //    SetHandle(new Vector3(value, 0, 0));
        //    SetFill();
        //    SetValue();
        //}).AddTo(gameObject);
        // == ここ動かすと起動時にクラッシュする。原因調べてリファクタリング =========


        //Clicked.Subscribe(value => presenter.Clicked.Value = value);

        //Value_Float.Skip(1).Subscribe(async value =>
        //{
        //    presenter.value_Float.Value = value;
        //    await presenter.Execute();    
        //});

        //Value_Int.Skip(1).Subscribe(async value =>
        //{
        //    presenter.value_Int.Value = value;
        //    await presenter.Execute();
        //});

        SetHandle(handle.transform.position);
        SetFill();
        SetValue();
    }


    protected sealed override void Update()
    {
#if UNITY_EDITOR
        //エディタでハンドルのポジションを動かしたさいにちゃんとスライダーが更新される。
        if (!EditorApplication.isPlaying)
        {
            Awake0();
        }
#endif
    }


    void SetHandle(Vector3  pos)
    {
        pos.y = transform.position.y;


        if (mouseTracker.transform.localPosition.x > handleLange_High.x)
        {
            //Debug.Log("範囲より大きい");
            mouseTracker.transform.localPosition = handleLange_High;
        }
        else 
        if (mouseTracker.transform.localPosition.x < handleLange_Low.x)
        {
            //Debug.Log("範囲より小さい");
            mouseTracker.transform.localPosition = handleLange_Low;
        }
        else 
        {
            //Debug.Log("あいだ");
            mouseTracker.transform.position = pos;
        }

        if(mouseTracker.transform.localPosition.x >= handleLange_High.x)
        {
            handle.transform.localPosition = handleLange_High;
        }
        else
        if(mouseTracker.transform.localPosition.x <= handleLange_Low.x)
        {
            handle.transform.localPosition = handleLange_Low;
        }
        else
        {
            handle.transform.localPosition = mouseTracker.transform.localPosition;
        }
    }


    void SetFill()
    {
        Vector3 filScale = fill.transform.localScale;
        if (type == Type.Min_Max)
            filScale.x = handle.transform.localPosition.x * 5 / 11;
        else
            filScale.x = backGround.transform.localScale.x / 2;
        
        fill.transform.localScale = filScale;
    }


    void SetValue()
    {
        if(type == Type.Min_Max)
        {
            Value_Float.Value = handle.transform.localPosition.x * 5 / 11;
            Value_Int.Value = (int)Value_Float.Value;
        }
        else
        {
            Value_Float.Value = (handle.transform.localPosition.x * 5 / 11 - 5) * 2;
            Value_Int.Value = (int)Value_Float.Value;
        }
    }
}
