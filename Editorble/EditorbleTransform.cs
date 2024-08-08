using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class EditorbleTransform : SavableCompo
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
    [SerializeField] float scale;
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 rotation;

    public Vector3 BaseScale
    {
        get { return baseScale; }
        set {
            baseScale = value;
            transform.localScale = baseScale;
        }
    }
    public float Scale 
    {
        get { return scale; }
        set {
            scale = value;
            transform.localScale = baseScale * scale;
        }
    }
    public Vector3 Position
    {
        get { return position; }
        set {
            position = value;
            transform.localPosition = position;
        }
    }
    public Vector3 Rotation
    {
        get { return rotation; }
        set {
            rotation  = value;
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }


    private void Start()
    {
        LoadEditorble();

        InputEventHandler.OnDown_L += () =>
        {
            LoadEditorble();
        };
        InputEventHandler.OnDown_S += () =>
        {
            Save();
        };

        InputEventHandler.OnDown_X += () =>
        {
            Scale += 0.5f;
        };
    }

    void OnApplicationQuit()
    {
        ResetFirstLoading();
    }


    void LoadEditorble()
    {
        Load();
        transform.localScale = baseScale * scale;
        transform.localPosition = position;
        transform.localRotation = Quaternion.Euler(rotation);
    }
}

