using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtil;
using System;
using UniRx;

// これをアタッチしたオブジェクトをクリックすると
// エフェクトのプレハブを生成
public class Test_MeshParticle : MonoBehaviour
{
    // 毎フレームクリック検知を監視
    void Start() => Observable.EveryUpdate()
    .Where(_ => Input.GetMouseButtonDown(0))
    .ThrottleFirst(TimeSpan.FromSeconds(1)) // 連打禁止
    .Subscribe(_ =>
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit)) return;
        if (hit.collider.gameObject == gameObject)
        {
            MeshParticle eff = ResourcesUtil.GetCompo<MeshParticle>("MeshParticleEffect");
            eff.Targ = hit.collider.gameObject;
        }
    })
    .AddTo(this);
}
