using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyUtil;
using Cysharp.Threading.Tasks;
using System;
using System.Xml.Linq;


// 


// 今回の文字メッシュエフェクト本体
[ExecuteAlways]
public class MeshParticle : MonoBehaviour
{
    // ターゲットゲームオブジェクト 
    public GameObject Targ;
    // 遅延パラメータ
    public float Delay_TargDiable = 2;
    public float Delay_TargEnable = 6.5f;
    public float Delay_AnimEnable = 1;
    public float Delay_Destroy = 22;
    // 動的メッシュVXFの管理辞書
    Dictionary<string, VFXWrap_DynamicAttributeMap> vfxs = new();
    // アニメーション
    Animator pausedAnim;


    void Start()
    {
        transform.SetPositionAndRotation(Targ.transform.position, Targ.transform.rotation);

        Create_DynamicAttributeMap("MeshParticleVFX1", 6000);
        Create_DynamicAttributeMap("MeshParticleVFX2", 3000);
        Create_DynamicAttributeMap("MeshParticleVFX3", 3000);

        LifeCycle();
    }


    //===========================================
    // VFXを生成しパラメータを代入
    //===========================================
    void Create_DynamicAttributeMap(
        string name,
        float pointCountPerArea
    )
    {
        vfxs.Add(name, ResourcesUtil.GetCompo<VFXWrap_DynamicAttributeMap>(name));
        vfxs[name].transform.SetParent(transform, false);
        vfxs[name].Targ = Targ;
        vfxs[name].PointCountPerArea = pointCountPerArea;
    }


    //===========================================
    // ライフサイクル
    //===========================================
    async void LifeCycle()
    {
        async void Cycle_Targ()
        {
            // アニメーションを停止
            pausedAnim = Targ.GetComponentInParent<Animator>();
            if(pausedAnim != null)
            {
                pausedAnim.enabled = false;
            }

            await Delay.Second(Delay_TargDiable);
            Targ.SetActive(false);

            await Delay.Second(Delay_TargEnable);
            //TargetGO.SetActive(true);

            if (pausedAnim != null)
            {
                await Delay.Second(Delay_AnimEnable);
                pausedAnim.enabled = true;
                pausedAnim = null;
            }
        }

        async void Cycle_VFX(string name, float diableDelay)
        {
            vfxs[name].gameObject.SetActive(true);
            await Delay.Second(diableDelay);
            Destroy(vfxs[name].gameObject);
            vfxs.Remove(name);
        }

        Cycle_Targ();
        Cycle_VFX("MeshParticleVFX1", Delay_TargEnable + 5);
        Cycle_VFX("MeshParticleVFX2", Delay_TargDiable);
        Cycle_VFX("MeshParticleVFX3", Delay_TargEnable + 12);

        await Delay.Second(Delay_Destroy);
        Destroy(gameObject);
    }
}