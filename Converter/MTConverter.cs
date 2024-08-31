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
    //public List<GameObject> targets; // 対象となるオブジェクトの配列
    //[SerializeField]
    [SerializeField]
    protected List<PropNames_Orig_Dest> propNames_Orig_Dest = null; // 元のシェーダーから差し替えたシェーダーにコピーしたいプロパティ名
    // エクスポートする際の保存先
    public string exportDir = $"Assets/MTConverted/";

    // ターゲット
    GameObject target;
    // URPへ切り替えるためのシェーダー
    Shader shader;
    // ここにコピーしたいプロパティの二次元配列を渡す ((コピー元のプロパティ名, コピー先のプロパティ名), (コピー元のプロパティ名, コピー先のプロパティ名), ...)
    public string[,] propNames_Orig_Dest_Array = null;
    // trueにすると、コピーしたいプロパティを手動で指定せずに自動で取得してやってくれる
    // あとで、上記のプロパティ名配列が null なら勝手に自動になる方式に変更する。
    public bool setPropNamesAutomatic = false;
    // 親族オブジェクトの中で対象外にしたいやつ
    public GameObject[] targetToExept = null;
    // trueにするとコンバートしたマテリアルとプレハブをエクスポートしてくれる
    // あとで、フラグではなく出力先のアドレスを指定することで出力される方式に変更する
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
    /// 大体互換性のありそうなシェーダ同士の変換の際コピーするプロパティ名の設定を自動でやって欲しい時
    /// 元と新のプロパティ名を自動で取得して、双方の名前が一致するやつをコピーする
    /// めちゃ時間かかる
    /// </summary>
    /// <param name="propNames_Orig_Dest_Array">元と新で名前が違うプロパティだけ手動で指定</param>
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

        // 全マテリアルのシェーダプロパティのコピー
        CopyAllProps(sets, propNames_Orig_Dest);
        // 新マテリアルの適用
        Apply(sets);
        //エクスポート
        if (toExport) Export(target, sets);
    }



    public void Convert(string[,] propNames_Orig_Dest_Array)
    {
        List<PropNames_Orig_Dest> propNames_Orig_Dest = new List<PropNames_Orig_Dest>();
        /* .Lengthだと多次元配列の全要素の数を取得してしまうので、GetLength(0) で1次元目だけの要素数を取得する。
         * 2次元目の長さを知りたければ GetLength(1) */
        for (int i = 0; i < propNames_Orig_Dest_Array.GetLength(0); i++)
        {
            propNames_Orig_Dest.Add(new PropNames_Orig_Dest(propNames_Orig_Dest_Array[i, 0], propNames_Orig_Dest_Array[i, 1]));
        }

        Convert(propNames_Orig_Dest);
    }



    void Convert(List<PropNames_Orig_Dest> propNames_Orig_Dest)
    {
        // ターゲットの全マテリアルにシェーダ適用
        List<RendererMaterialsSet> sets = ChangeShader(target, shader);
        // 全マテリアルのシェーダプロパティのコピー
        CopyAllProps(sets, propNames_Orig_Dest);
        // 新マテリアルの適用
        Apply(sets);
        //エクスポート
        if (toExport) Export(target, sets);
    }



    public List<RendererMaterialsSet> ChangeShader(GameObject target, Shader urpShader)
    {
        List<RendererMaterialsSet> rendererMaterialSets = new List<RendererMaterialsSet>();
        // シェーダーが指定されていない場合のエラー処理
        if (urpShader == null)
        {
            Debug.LogError("URP shader is not specified.");
            return null;
        }

        // ターゲットオブジェクトが指定されていない場合のエラー処理
        if (target == null)
        {
            Debug.LogError("Target object is not specified.");
            return null;
        }

        // ターゲットの子孫オブジェクト含めすべてのレンダラを取得
        Renderer[] childRenderers_All = target.GetComponentsInChildren<Renderer>();

        List<Renderer> childRenderers_Applicable = childRenderers_All.ToList();
        if (targetToExept != null)
        {
            // 除外リスト「targetToExept」に含まれるものを除外したリストを作成
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

        // 各レンダラーに対してマテリアルを変更
        foreach (Renderer renderer in childRenderers_Applicable)
        {
            /* VRMは1つのレンダラに複数マテリアルの配列がついていたりする。それをキャッシュする。
             *プレハブについたマテリアルを取得する場合は sharedMaterials じゃないとできないらしい。*/
            Material[] origMTs = renderer.sharedMaterials;
            Material[] newMTs = renderer.sharedMaterials;
            // 各マテリアルを新しいものに交換
            for (int i = 0; i < newMTs.Length; i++)
            {
                newMTs[i] = new Material(urpShader);
            }

            // レンダラのマテリアル配列を、全て新しいものに置き換わった配列と交換する
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
                // テクスチャ系のプロパティなら、元マテリアルのそのプロパティで使われているテクスチャをコピーして新しい方にセットする
                if (original.HasTexture(propNames_Orig_Dests[a].Original))
                {
                    //Debug.Log($"テクスチャコピー");
                    destination.SetTexture(propNames_Orig_Dests[a].Destination, original.GetTexture(propNames_Orig_Dests[a].Original));
                }
                // カラー系も同様に
                else if (original.HasColor(propNames_Orig_Dests[a].Original))
                {
                    //Debug.Log($"カラーコピー");
                    destination.SetColor(propNames_Orig_Dests[a].Destination, original.GetColor(propNames_Orig_Dests[a].Original));
                }
                // その他数値系も同様に
                else if (original.HasFloat(propNames_Orig_Dests[a].Original))
                {
                    //Debug.Log($"数値コピー");
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
                //    Debug.Log($"＝＝＝＝＝＝ 完了{b}");
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

        // 加筆
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
//エディタ拡張(インスペクタ表示)
[CustomPropertyDrawer(typeof(PropNames_Orig_Dest))]
public class Drawer_AxisLString2 : PropertyDrawer
{
    static readonly GUIContent LABEL_Orig = new GUIContent("元");
    static readonly GUIContent LABEL_Dest = new GUIContent("新");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty property_A = property.FindPropertyRelative("Original");
        SerializedProperty property_B = property.FindPropertyRelative("Destination");

        //名前
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        //ラベル
        contentPosition.width *= 1f / 2f; //2つ並べる場合 (n 個のとき、1 / n)
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 15f;  //ラベル幅(適当)

        //各要素
        EditorGUI.PropertyField(contentPosition, property_A, LABEL_Orig);
        contentPosition.x += contentPosition.width;

        EditorGUI.PropertyField(contentPosition, property_B, LABEL_Dest);

        EditorGUI.EndProperty();
    }
}
#endif