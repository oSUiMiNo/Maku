using UnityEngine;
using UnityEngine.VFX;

//VFXユニットにつけるコンポーネントの設計迷い中。

public abstract class VFXUnit : MonoBehaviour
{
    public VisualEffect VFX;

    void Start()
    {
        // 初期化時に VFXGraph の実体を自分にセットする。
        // 必ず子オブジェクトの1つ目に実体を置いておく。
        VFX = transform.GetChild(0).GetComponent<VisualEffect>();
        Init();
    }

    protected virtual void Init() { }
    public abstract void SetProperties();
}
