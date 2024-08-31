using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


//[ExecuteInEditMode]
public class MTConverter
{
    //[SerializeField]
    //public List<GameObject> targets; // �ΏۂƂȂ�I�u�W�F�N�g�̔z��
    //[SerializeField]
    [SerializeField]
    protected List<PropNames_Orig_Dest> propNames_Orig_Dest = null; // ���̃V�F�[�_�[���獷���ւ����V�F�[�_�[�ɃR�s�[�������v���p�e�B��
    // �G�N�X�|�[�g����ۂ̕ۑ���
    public string exportDir = $"Assets/MTConverted/";

    // �^�[�Q�b�g
    GameObject target;
    // URP�֐؂�ւ��邽�߂̃V�F�[�_�[
    Shader shader;
    // �����ɃR�s�[�������v���p�e�B�̓񎟌��z���n�� ((�R�s�[���̃v���p�e�B��, �R�s�[��̃v���p�e�B��), (�R�s�[���̃v���p�e�B��, �R�s�[��̃v���p�e�B��), ...)
    public string[,] propNames_Orig_Dest_Array = null;
    // true�ɂ���ƁA�R�s�[�������v���p�e�B���蓮�Ŏw�肹���Ɏ����Ŏ擾���Ă���Ă����
    // ���ƂŁA��L�̃v���p�e�B���z�� null �Ȃ珟��Ɏ����ɂȂ�����ɕύX����B
    public bool setPropNamesAutomatic = false;
    // �e���I�u�W�F�N�g�̒��őΏۊO�ɂ��������
    public GameObject[] targetToExept = null;
    // true�ɂ���ƃR���o�[�g�����}�e���A���ƃv���n�u���G�N�X�|�[�g���Ă����
    // ���ƂŁA�t���O�ł͂Ȃ��o�͐�̃A�h���X���w�肷�邱�Ƃŏo�͂��������ɕύX����
    public bool toExport = false;

    public MTConverter(GameObject target, Shader shader)
    {
        this.target = target;
        this.shader = shader;
    }


    public void Execute()
    {
        if (!target.activeSelf) return;
        if (setPropNamesAutomatic) ConvertAutomatic();
        else
        if (propNames_Orig_Dest != null) Convert(propNames_Orig_Dest);
        else
        if (propNames_Orig_Dest_Array != null) Convert(propNames_Orig_Dest_Array);
    }


    /// <summary>
    /// ��̌݊����̂��肻���ȃV�F�[�_���m�̕ϊ��̍ۃR�s�[����v���p�e�B���̐ݒ�������ł���ė~������
    /// ���ƐV�̃v���p�e�B���������Ŏ擾���āA�o���̖��O����v�������R�s�[����
    /// �߂��᎞�Ԃ�����
    /// </summary>
    /// <param name="propNames_Orig_Dest_Array">���ƐV�Ŗ��O���Ⴄ�v���p�e�B�����蓮�Ŏw��</param>
    public void ConvertAutomatic()
    {
        List<string> PropNames_OrigMTs = new List<string>();
        List<string> PropNames_NewMTs = new List<string>();

        List<RendererMaterialsSet> sets = ChangeShader(target, shader);
        sets.ForEach(a =>
        {
            foreach (MaterialPropertyType c in Enum.GetValues(typeof(MaterialPropertyType)))
            {
                PropNames_NewMTs.AddRange(a.newMTs[0].GetPropertyNames(c));
            }
            foreach (var b in a.origMTs)
            {
                foreach (MaterialPropertyType c in Enum.GetValues(typeof(MaterialPropertyType)))
                {
                    PropNames_OrigMTs.AddRange(b.GetPropertyNames(c));
                }
            }
        });

        List<PropNames_Orig_Dest> propNames_Orig_Dest = new List<PropNames_Orig_Dest>();
        PropNames_OrigMTs.ForEach(a =>
        {
            PropNames_NewMTs.ForEach(b =>
            {
                if (a == b) propNames_Orig_Dest.Add(new PropNames_Orig_Dest(a, b));
            });
        });

        if (propNames_Orig_Dest_Array != null)
        {
            for (int i = 0; i < propNames_Orig_Dest_Array.GetLength(0); i++)
            {
                propNames_Orig_Dest.Add(new PropNames_Orig_Dest(propNames_Orig_Dest_Array[i, 0], propNames_Orig_Dest_Array[i, 1]));
            }
        }

        // �S�}�e���A���̃V�F�[�_�v���p�e�B�̃R�s�[
        CopyAllProps(sets, propNames_Orig_Dest);
        // �V�}�e���A���̓K�p
        Apply(sets);
        //�G�N�X�|�[�g
        if (toExport) Export(target, sets);
    }



    public void Convert(string[,] propNames_Orig_Dest_Array)
    {
        List<PropNames_Orig_Dest> propNames_Orig_Dest = new List<PropNames_Orig_Dest>();
        /* .Length���Ƒ������z��̑S�v�f�̐����擾���Ă��܂��̂ŁAGetLength(0) ��1�����ڂ����̗v�f�����擾����B
         * 2�����ڂ̒�����m�肽����� GetLength(1) */
        for (int i = 0; i < propNames_Orig_Dest_Array.GetLength(0); i++)
        {
            propNames_Orig_Dest.Add(new PropNames_Orig_Dest(propNames_Orig_Dest_Array[i, 0], propNames_Orig_Dest_Array[i, 1]));
        }

        Convert(propNames_Orig_Dest);
    }



    void Convert(List<PropNames_Orig_Dest> propNames_Orig_Dest)
    {
        // �^�[�Q�b�g�̑S�}�e���A���ɃV�F�[�_�K�p
        List<RendererMaterialsSet> sets = ChangeShader(target, shader);
        // �S�}�e���A���̃V�F�[�_�v���p�e�B�̃R�s�[
        CopyAllProps(sets, propNames_Orig_Dest);
        // �V�}�e���A���̓K�p
        Apply(sets);
        //�G�N�X�|�[�g
        if (toExport) Export(target, sets);
    }



    public List<RendererMaterialsSet> ChangeShader(GameObject target, Shader urpShader)
    {
        List<RendererMaterialsSet> rendererMaterialSets = new List<RendererMaterialsSet>();
        // �V�F�[�_�[���w�肳��Ă��Ȃ��ꍇ�̃G���[����
        if (urpShader == null)
        {
            Debug.LogError("URP shader is not specified.");
            return null;
        }

        // �^�[�Q�b�g�I�u�W�F�N�g���w�肳��Ă��Ȃ��ꍇ�̃G���[����
        if (target == null)
        {
            Debug.LogError("Target object is not specified.");
            return null;
        }

        // �^�[�Q�b�g�̎q���I�u�W�F�N�g�܂߂��ׂẴ����_�����擾
        Renderer[] childRenderers_All = target.GetComponentsInChildren<Renderer>();

        List<Renderer> childRenderers_Applicable = childRenderers_All.ToList();
        if (targetToExept != null)
        {
            // ���O���X�g�utargetToExept�v�Ɋ܂܂����̂����O�������X�g���쐬
            foreach (var a in childRenderers_All)
            {
                foreach (var b in targetToExept)
                {
                    //Debug.Log(b.name);
                    if (a.gameObject == b)
                    {
                        //Debug.Log(a.gameObject.name);
                        childRenderers_Applicable.Remove(a);
                    }
                }
            }
        }

        // �e�����_���[�ɑ΂��ă}�e���A����ύX
        foreach (Renderer renderer in childRenderers_Applicable)
        {
            /* VRM��1�̃����_���ɕ����}�e���A���̔z�񂪂��Ă����肷��B������L���b�V������B
             *�v���n�u�ɂ����}�e���A�����擾����ꍇ�� sharedMaterials ����Ȃ��Ƃł��Ȃ��炵���B*/
            Material[] origMTs = renderer.sharedMaterials;
            Material[] newMTs = renderer.sharedMaterials;
            // �e�}�e���A����V�������̂Ɍ���
            for (int i = 0; i < newMTs.Length; i++)
            {
                newMTs[i] = new Material(urpShader);
            }

            // �����_���̃}�e���A���z����A�S�ĐV�������̂ɒu����������z��ƌ�������
            rendererMaterialSets.Add(new RendererMaterialsSet(renderer, origMTs, newMTs));
        }

        return rendererMaterialSets;
    }



    void CopyAllProps(List<RendererMaterialsSet> sets, List<PropNames_Orig_Dest> propNames_Orig_Dest)
    {
        List<string> completedMaterialPathes = new List<string>();

        sets.ForEach(a =>
        {
            for (int i = 0; i < a.newMTs.Length; i++)
            {
                CopyProps(a.origMTs[i], a.newMTs[i], propNames_Orig_Dest);
            }
            a.renderer.materials = a.newMTs;
        });
    }



    void Apply(List<RendererMaterialsSet> sets)
    {
        sets.ForEach(a => a.renderer.materials = a.newMTs);
    }



    void CopyProps(Material original, Material destination, List<PropNames_Orig_Dest> propNames_Orig_Dests)
    {
        for (int a = 0; a < propNames_Orig_Dests.Count; a++)
        {
            //Debug.Log($"{propNames_Orig_Dests[a].Original} --- {propNames_Orig_Dests[a].Destination}");
            if (original.HasProperty(propNames_Orig_Dests[a].Original) && destination.HasProperty(propNames_Orig_Dests[a].Destination))
            {
                // �e�N�X�`���n�̃v���p�e�B�Ȃ�A���}�e���A���̂��̃v���p�e�B�Ŏg���Ă���e�N�X�`�����R�s�[���ĐV�������ɃZ�b�g����
                if (original.HasTexture(propNames_Orig_Dests[a].Original))
                {
                    //Debug.Log($"�e�N�X�`���R�s�[");
                    destination.SetTexture(propNames_Orig_Dests[a].Destination, original.GetTexture(propNames_Orig_Dests[a].Original));
                }
                // �J���[�n�����l��
                else if (original.HasColor(propNames_Orig_Dests[a].Original))
                {
                    //Debug.Log($"�J���[�R�s�[");
                    destination.SetColor(propNames_Orig_Dests[a].Destination, original.GetColor(propNames_Orig_Dests[a].Original));
                }
                // ���̑����l�n�����l��
                else if (original.HasFloat(propNames_Orig_Dests[a].Original))
                {
                    //Debug.Log($"���l�R�s�[");
                    destination.SetFloat(propNames_Orig_Dests[a].Destination, original.GetFloat(propNames_Orig_Dests[a].Original));
                }
                else continue;
            }
        }
    }



    void Export(GameObject target, List<RendererMaterialsSet> sets, GameObject[] targetToExept = null)
    {
#if UNITY_EDITOR
        List<string> completedMTPathes = new List<string>();

        string directory = $"{exportDir}{target.name}_URP/";
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        sets.ForEach(a =>
        {
            for (int i = 0; i < a.newMTs.Length; i++)
            {
                a.newMTs[i].name = a.origMTs[i].name;

                string newMTPath = Path.Combine(directory, $"{a.newMTs[i].name}_URP.mat");

                //foreach (var b in completedMaterialPathes)
                //{
                //    Debug.Log($"������������ ����{b}");
                //}
                if (!completedMTPathes.Contains(newMTPath))
                {
                    //Debug.Log($"{a.newMTs[i].name}");
                    AssetDatabase.CreateAsset(a.newMTs[i], newMTPath);
                    EditorUtility.SetDirty(a.newMTs[i]);
                    AssetDatabase.SaveAssets();
                    completedMTPathes.Add(newMTPath);
                }
                Material exportedMT = (Material)AssetDatabase.LoadAssetAtPath(newMTPath, typeof(Material));
                a.newMTs[i] = exportedMT;

                //string newPrefabPath = Path.Combine(directory, $"{target.name}_URP.prefab");
                //PrefabUtility.SaveAsPrefabAsset(target, newPrefabPath);
                //EditorUtility.SetDirty(target);
                //AssetDatabase.SaveAssets();
            }
            a.renderer.materials = a.newMTs;
        });

        string newPrefabPath = Path.Combine(directory, $"{target.name}_URP.prefab");
        PrefabUtility.SaveAsPrefabAsset(target, newPrefabPath);
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();

        // ���M
        if (targetToExept == null) return;
        foreach (var a in targetToExept)
        {
            //string newPrefabPath = Path.Combine(directory, $"{target.name}_URP.prefab");
            PrefabUtility.SaveAsPrefabAsset(target, newPrefabPath);
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}



public class RendererMaterialsSet
{
    public Renderer renderer;
    public Material[] origMTs;
    public Material[] newMTs;

    public RendererMaterialsSet(Renderer renderer, Material[] origMTs, Material[] newMTs)
    {
        this.renderer = renderer;
        this.origMTs = origMTs;
        this.newMTs = newMTs;
    }
}



[Serializable]
public struct PropNames_Orig_Dest
{
    public string Original;
    public string Destination;

    public PropNames_Orig_Dest(string original, string destination)
    {
        this.Original = original;
        if (destination == "_") this.Destination = original;
        else this.Destination = destination;
    }

    public static PropNames_Orig_Dest Create(string original, string destination)
    {
        return new PropNames_Orig_Dest(original, destination);
    }
}

#if UNITY_EDITOR
//�G�f�B�^�g��(�C���X�y�N�^�\��)
[CustomPropertyDrawer(typeof(PropNames_Orig_Dest))]
public class Drawer_AxisLString2 : PropertyDrawer
{
    static readonly GUIContent LABEL_Orig = new GUIContent("��");
    static readonly GUIContent LABEL_Dest = new GUIContent("�V");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty property_A = property.FindPropertyRelative("Original");
        SerializedProperty property_B = property.FindPropertyRelative("Destination");

        //���O
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        //���x��
        contentPosition.width *= 1f / 2f; //2���ׂ�ꍇ (n �̂Ƃ��A1 / n)
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 15f;  //���x����(�K��)

        //�e�v�f
        EditorGUI.PropertyField(contentPosition, property_A, LABEL_Orig);
        contentPosition.x += contentPosition.width;

        EditorGUI.PropertyField(contentPosition, property_B, LABEL_Dest);

        EditorGUI.EndProperty();
    }
}
#endif