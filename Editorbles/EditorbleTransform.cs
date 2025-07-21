using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static InputEventHandler;
using MyUtil;


public interface IEditable { }


[System.Serializable]
public class EditorbleTransform : SavableCompo,
    IEditable
{
    public override List<SaveSystem.IFriendWith_SaveSystem> Instances { get; protected set; } = instances;
    private static List<SaveSystem.IFriendWith_SaveSystem> instances = new();
    public override string SaveFolderPath { get; set; }
    public override string FileName { get; set; }
    EditorbleHeader editorbleInfo => GetComponent<EditorbleHeader>();

    private void Awake()
    {
        SaveFolderPath = $"{Application.persistentDataPath}/Editorbles/{editorbleInfo.id}";
        FileName = "EditorbleTransform";
        id = editorbleInfo.id;
    }

    [SerializeField] string id;
    [SerializeField] Vector3 baseScale = Vector3.one;
    [SerializeField] float scale = 1;
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 rotation;
    //[SerializeField] Vector3Int angle90;

    public Vector3 BaseScale
    {
        get { return baseScale; }
        set
        {
            baseScale = value;
            transform.localScale = baseScale;
        }
    }
    public float Scale
    {
        get { return scale; }
        set
        {
            scale = value;
            if (Scale < 0.05f) scale = 0.05f;
            transform.localScale = baseScale * scale;
        }
    }
    public Vector3 Position
    {
        get { return position; }
        set
        {
            position = value;
            transform.localPosition = position;
        }
    }
    public Vector3 Rotation
    {
        get { return rotation; }
        set
        {
            Vector3 diff = rotation - value;
            //Debug.Log($"ディフ {-diff.x}");
            if (diff.x != 0) transform.RotateAround(transform.position, Vector3.right, -diff.x);
            else
            if (diff.y != 0) transform.RotateAround(transform.position, Vector3.up, -diff.y);
            else
            if (diff.z != 0) transform.RotateAround(transform.position, Vector3.forward, -diff.z);

            rotation = value;
            rotation.x = (int)Mathf.Repeat(rotation.x, 360);
            rotation.y = (int)Mathf.Repeat(rotation.y, 360);
            rotation.z = (int)Mathf.Repeat(rotation.z, 360);

            //if(diff.magnitude < 0.05f) transform.rotation  = Quaternion.identity;

            //rotation = transform.rotation.eulerAngles;

            //if(transform.rotation.eulerAngles.x > 180)
            //{
            //    rotation.x = transform.rotation.eulerAngles.x - 360;
            //}
            //if (transform.rotation.eulerAngles.y > 180)
            //{
            //    rotation.y = transform.rotation.eulerAngles.y - 360;
            //}
            //if (transform.rotation.eulerAngles.z > 180)
            //{
            //    rotation.z = transform.rotation.eulerAngles.z - 360;
            //}


            //rotation = new Vector3(
            //    Mathf.Repeat(transform.rotation.eulerAngles.x + 180, 360),
            //    Mathf.Repeat(transform.rotation.eulerAngles.y + 180, 360),
            //    Mathf.Repeat(transform.rotation.eulerAngles.z + 180, 360)
            //    );


            //Debug.Log($"ローテ {rotation}");
            //Debug.Log($"ローテオイラー {transform.rotation.eulerAngles}");
            //Debug.Log($"オイラー {transform.eulerAngles}");
        }
    }

    //public Vector3Int Angle90
    //{
    //    get { return angle90; }
    //    set
    //    {
    //        Debug.Log($"アングル0 {value}");
    //        angle90 = value;
    //        angle90.x = (int)Mathf.Repeat(angle90.x, 4);
    //        angle90.y = (int)Mathf.Repeat(angle90.y, 4);
    //        angle90.z = (int)Mathf.Repeat(angle90.z, 4);
    //        Debug.Log($"アングル1 {angle90}");
    //    }
    //}


    private void Start()
    {
        //Debug.Log($"スタートトランス");
        //LoadEditorble();

        OnDown_L += () =>
        {
            if (Flag_Shift || Flag_Ctrl || Flag_Alt) return;
            LoadEditorble();
        };
        OnDown_S += () =>
        {
            if (Flag_Shift || Flag_Ctrl || Flag_Alt) return;
            Save();
        };

        OnDown_X += () =>
        {
            if (Flag_Shift || Flag_Ctrl || Flag_Alt) return;
            Scale += 0.5f;
        };
    }

    //void OnApplicationQuit()
    //{
    //    //Debug.Log($"リセットファーストローディング");
    //    //ResetFirstLoading();
    //}
    public void Init()
    {
        LoadEditorble();
    }


    public void LoadEditorble()
    {
        Load();
        transform.localScale = baseScale * scale;
        transform.localPosition = position;
        transform.RotateAround(transform.position, Vector3.right, rotation.x);
        transform.RotateAround(transform.position, Vector3.up, rotation.y);
        transform.RotateAround(transform.position, Vector3.forward, rotation.z);
    }

    private void OnDestroy()
    {
        Debug.Log($"デストロイ");
        instances.Remove(this);
        //foreach(var ins in Instances)
        //{
        //}
        SaveSystem.UpdateAllIndex();
    }
}

