using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// これをアタッチしたオブジェクトをクリックすると、
/// エフェクトのプレハブをジェネレートしてくれる。
/// </summary>
public class OnClick_GenarateMeshParticle : MonoBehaviourMyExtention
{
    /// <summary>
    /// MeshParticle をアタッチしたプレハブ。
    /// これをこのスクリプト内で Instantiate() している。
    /// Instantiate() すると、更にその(MeshParticleの)中で判定処理に入り、
    /// 通ったら、実際のエフェクトが Instantiate() される。
    /// </summary>
    MeshParticle meshParticlePrefab;

    [SerializeField] float interval = 1;


    float lastClickTime;

    private void Start()
    {
        LoadResources();
    }

    void LoadResources()
    {
        //生成の時にロードまでやるのは遅そうなので先にキャッシュしといて、
        //生成時はこれを Instantiate するだけ
        GameObject effectPrefab = (GameObject)Resources.Load("MeshParticleEffect");
        meshParticlePrefab = effectPrefab.GetComponent<MeshParticle>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastClickTime > interval)
        {
            lastClickTime = Time.time;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.gameObject != gameObject) return;

                MeshParticle mp = Instantiate(meshParticlePrefab);
                mp.Model = hit.collider.gameObject;
            }
        }
    }
}
