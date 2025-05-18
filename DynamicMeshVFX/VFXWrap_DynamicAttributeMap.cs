using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using MyUtil;
using Cysharp.Threading.Tasks;


public abstract class VFXWrap : MonoBehaviour
{
    public VisualEffect VFX;

    async void Start()
    {
        VFX = GetComponent<VisualEffect>();
        Init();
        SetProps();
    }

    protected abstract void Init();
    protected abstract void SetProps();
}




public class VFXWrap_DynamicAttributeMap : VFXWrap
{
    public GameObject Targ;
    public MeshMap MeshMap;
    public Texture ModelMainTex;
    public float PointCountPerArea;

    protected override void Init()
    {
        MeshMap = new MeshMap(Targ.GetMeshes()[0], PointCountPerArea);
        ModelMainTex = Targ.GetTexs()[0];
    }

    protected override void SetProps()
    {
        VFX.SetTexture("PositionMap", MeshMap.PosMap);
        VFX.SetTexture("UVMap", MeshMap.UVMap);
        VFX.SetTexture("NormalMap", MeshMap.NormalMap);
        VFX.SetTexture("ModelMainTex", ModelMainTex);
        VFX.SetInt("ParticleCount", MeshMap.VertexCount);
    }
}
