using UnityEngine.VFX;

//VFXユニットにつけるコンポーネントの設計迷い中。

public abstract class VFXUnitComponent : MonoBehaviourMyExtention
{
    public VisualEffect vfx;

    void Start()
    {
        // 初期化時に VFXGraph の実体を自分にセットする。
        // 必ず子オブジェクトの1つ目に実体を置いておく。
        vfx = transform.GetChild(0).GetComponent<VisualEffect>();
        Init();
    }

    protected virtual void Init() { }
    public abstract void SetProperties();
}
