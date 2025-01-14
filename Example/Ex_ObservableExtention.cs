using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

// 使用例
public class Ex_ObservableExtention : MonoBehaviour
{
    public ReactiveProperty<bool> Flag = new ReactiveProperty<bool>(true);
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


    void Start()
    {
        Flag.UpdateWhileEqualTo(true, 0.1f)
        .Subscribe(_ =>
        {
            Debug.Log($"{stopwatch.Elapsed.TotalSeconds:F8}");
            stopwatch.Reset();
            stopwatch.Start();
        }).AddTo(this);
        stopwatch.Start();  // 呼び出し側で計測したいならここからStart
    }


    // フレームレート感確認用
    void Update() => transform.RotateAround(transform.position, Vector3.up, 1);
}
    