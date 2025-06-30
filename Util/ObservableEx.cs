using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;


public static class ObservableEx
{
    // メッセージの値が指定したものと一致していたら定期実行開始。異なっていたら終了
    // 定期実行の間隔は秒数指定(最短0.001s)。何も渡さなければフレーム毎の実行となる
    public static IObservable<long> UpdateWhileEqualTo<T>(
        this IObservable<T> source,
        T expectedValue,
        float sec = 0f)
    {
        // インターバルの下限設定
        float interval = sec;
        if (sec <= 0) interval = 0;
        else
        if (sec <= 0.02f) interval = 0.02f;

        // フレーム毎発火
        if (interval == 0)
            return source
                .Select(value =>
                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                        Observable.EveryUpdate() :
                        Observable.Empty<long>()
                )
                .Switch();
        // 秒数毎発火
        else
            return source
                .Select(value =>
                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                        Observable.Interval(TimeSpan.FromSeconds(sec)) :
                        Observable.Empty<long>()
                )
                .Switch();
    }


    // UpdateWhileEqualTo の時間補正版まだアルファ
    public static IObservable<long> FixedUpdateWhileEqualTo<T>(
    this IObservable<T> source,
    T expectedValue,
    float sec = 0f)
    {
        // インターバルの下限設定
        float interval = sec <= 0 ? 0 : Mathf.Max(sec, 0.02f);

        return source
            .Select(value =>
            {
                if (!EqualityComparer<T>.Default.Equals(value, expectedValue))
                    return Observable.Empty<long>();

                if (interval == 0)
                    return Observable.EveryFixedUpdate();

                // 補正機能付きインターバル（fixedTime使用）
                return Observable.Create<long>(observer =>
                {
                    float startTime = Time.fixedTime;
                    float nextEmitTime = startTime + interval;
                    long count = 0;

                    return Observable.EveryFixedUpdate()
                        .Subscribe(_ =>
                        {
                            float currentTime = Time.fixedTime;

                            // 発行すべき回数を計算
                            while (currentTime >= nextEmitTime)
                            {
                                observer.OnNext(count++);
                                nextEmitTime += interval;
                            }
                        });
                });
            })
            .Switch();
    }


    // UpdateWhileEqualToは1フレームより短い秒数を測れない
    // こちらは短く測れる(デフォルト0.001, 下限0.0001)がスレッドプールでやるので
    // フレームに依存するUnityのAPIが使えない(例 Time.time)
    public static IObservable<long> TimerWhileEqualTo<T>(
        this IObservable<T> source,
        T expectedValue,
        float sec = 0.001f)
    {
        // インターバルの下限設定
        float interval = sec;
        if (sec <= 0.0001f) interval = 0.0001f;

        // 極小秒数毎発火
        return source
            .Select(value =>
                EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                    ObservableStopwatch(interval) :
                    Observable.Empty<long>()
            )
            .Switch();
    }

    // Stopwatchを使い、フレームレートに依存せず「指定秒」でOnNextを繰り返す。
    private static IObservable<long> ObservableStopwatch(float sec)
    {
        return Observable.Create<long>(observer =>
        {
            var cts = new CancellationTokenSource();

            UniTask.RunOnThreadPool(() =>
            {
                //var interval = TimeSpan.FromSeconds(sec);
                long count = 0;
                var stopwatch = new System.Diagnostics.Stopwatch();
                try
                {
                    stopwatch.Start();
                    //if (stopwatch.Elapsed.TotalSeconds >= interval.TotalSeconds)
                    while (!cts.IsCancellationRequested)
                        if (stopwatch.Elapsed.TotalSeconds >= sec)
                        {
                            observer.OnNext(count++);
                            // 無限ループのスレッドが独占対策でCPUスイッチ
                            Thread.Sleep(0);
                            stopwatch.Reset();
                            stopwatch.Start();
                        }
                }
                catch (OperationCanceledException)
                {
                    // キャンセル時は何もしない
                }
                finally
                {
                    observer.OnCompleted();
                }
            }, cancellationToken: cts.Token).Forget();
            return Disposable.Create(() => cts.Cancel());
        });
    }
}
