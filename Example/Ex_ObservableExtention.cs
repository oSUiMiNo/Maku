using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

// 使用例
public class Ex_ObservableExtention : MonoBehaviour
{
    public ReactiveProperty<bool> A = new ReactiveProperty<bool>(true);
    System.Diagnostics.Stopwatch stopwatch0 = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();



    void Start()
    {
        A.TimerWhileEqualTo(true, 0.01f)
        //.ObserveOnMainThread()
        .Subscribe(_ =>
        {
            B();
            Debug.Log($"A {stopwatch0.Elapsed.TotalSeconds:F8}");
            stopwatch0.Reset();
            stopwatch0.Start();
        }).AddTo(this);

        stopwatch0.Start();
        stopwatch1.Start();

    }

    void B()
    {
        Debug.Log($"B {stopwatch1.Elapsed.TotalSeconds:F8}");
        stopwatch1.Reset();
        stopwatch1.Start();
    }


    // フレームレート感確認用
    void Update() => transform.RotateAround(transform.position, Vector3.up, 1);
}
    