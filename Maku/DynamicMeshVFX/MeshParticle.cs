using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 

// 今回の文字メッシュエフェクト本体
[ExecuteAlways]
public class MeshParticle : MonoBehaviourMyExtention
{
    public GameObject Model { get; set; }

    float modelDiableDelay = 2;
    float modelEnableDelay = 6.5f;
    float animEnableDelay = 1;
    float autoDestroyDelay = 22;

    class Eff : MyExtention
    {
        public GameObject unit;
        public float effectDiableDelay;
        public float pointCountPerArea;
        public bool startEffect;
         
        public Eff(
            string unitPrefabName,
            float effectDiableDelay,
            float pointCountPerArea,
            Transform transform,
            bool startEffect = true
            )
        {
            //this.unit = Instantiate((GameObject)Resources.Load(unitPrefabName));
            //this.unit.transform.SetParent(transform);
            this.unit = LoadPrefab(unitPrefabName);
            this.effectDiableDelay = effectDiableDelay;
            this.pointCountPerArea = pointCountPerArea;
            this.startEffect = startEffect;
            
            Debug.Log($"Effコンストラクタ {unit}");
        }
    }
    List<Eff> effs = new List<Eff>();
    
    Animator pausedAnim;


    void Start()
    {
        effs.Add(new Eff("MeshParticleVFXUnit1", modelEnableDelay + 5, 6000, transform));
        effs.Add(new Eff("MeshParticleVFXUnit2", modelDiableDelay, 3000, transform));
        effs.Add(new Eff("MeshParticleVFXUnit3", modelEnableDelay + 15, 3000, transform));
        Destroy(gameObject, autoDestroyDelay);
        StartCoroutine(StartTargetActiveSequence());
        effs.ForEach(eff => InitEffect(eff));
    }


    void InitEffect(Eff eff)
    {
        if (!eff.startEffect) return;

        var modelTrans = Model.transform;
        transform.SetPositionAndRotation(modelTrans.position, modelTrans.rotation);

        var meshAndTexs = GetMeshData();
        foreach (var (mesh, tex) in meshAndTexs)
        {
            eff.unit.SetActive(true);
            eff.unit.transform.SetParent(transform, false);

            VFXUnit_DynamicAttributeMap dynamicMap = eff.unit.GetComponent<VFXUnit_DynamicAttributeMap>();
            dynamicMap.mapSet = MeshToMap.Convert(mesh, eff.pointCountPerArea);
            dynamicMap.modelMainTex = tex;
        }

        StartCoroutine(StartEffectAtiveSequence(eff));
    }


    IEnumerator StartTargetActiveSequence()
    {
        yield return new WaitForSeconds(modelDiableDelay);
        Model.SetActive(false);

        yield return new WaitForSeconds(modelEnableDelay);
        //model.SetActive(true);

        if (pausedAnim != null)
        {
            yield return new WaitForSeconds(animEnableDelay);
            pausedAnim.enabled = true;
            pausedAnim = null;
        }
    }


    IEnumerator StartEffectAtiveSequence(Eff eff)
    {
        yield return new WaitForSeconds(eff.effectDiableDelay);
        Destroy(eff.unit.gameObject);
        effs.Remove(eff);        
    }



    IEnumerable<(Mesh, Texture)> GetMeshData()
    {
        var smr = Model.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null)
        {
            // stop animation
            pausedAnim = Model.GetComponentInParent<Animator>();
            pausedAnim.enabled = false;

            var material = smr.sharedMaterial;

            var mesh = new Mesh();
            smr.BakeMesh(mesh);

            Debug.Log($"上 {material.mainTexture}");

            return new[] { (mesh, material.mainTexture) };
        }
        else
        {
            var mf = Model.GetComponentInChildren<MeshFilter>();
            var renderer = Model.GetComponentInChildren<Renderer>();

            var mesh = mf.sharedMesh;
            var meshCount = mesh.subMeshCount;
            var materials = renderer.sharedMaterials;

            Debug.Log($"下 {materials}");
            Debug.Log(materials.First().mainTexture);

            return new[] { (mesh, materials.First().mainTexture) };
        }
    }
}