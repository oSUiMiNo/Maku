using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;


public static class ObservableEx
{
    // ���b�Z�[�W�̒l���w�肵�����̂ƈ�v���Ă����������s�J�n�B�قȂ��Ă�����I��
    // ������s�̊Ԋu�͕b���w��(�ŒZ0.001s)�B�����n���Ȃ���΃t���[�����̎��s�ƂȂ�
    public static IObservable<long> UpdateWhileEqualTo<T>(
        this IObservable<T> source,
        T expectedValue,
        float sec = 0f)
    {
        // �C���^�[�o���̉����ݒ�
        float interval = sec;
        if (sec <= 0) interval = 0;
        else
        if (sec <= 0.02f) interval = 0.02f;

        // �t���[��������
        if (interval == 0)
            return source
                .Select(value =>
                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                        Observable.EveryUpdate() :
                        Observable.Empty<long>()
                )
                .Switch();
        // �b��������
        else
            return source
                .Select(value =>
                    EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                        Observable.Interval(TimeSpan.FromSeconds(sec)) :
                        Observable.Empty<long>()
                )
                .Switch();
    }


    // UpdateWhileEqualTo �̎��ԕ␳�ł܂��A���t�@
    public static IObservable<long> FixedUpdateWhileEqualTo<T>(
    this IObservable<T> source,
    T expectedValue,
    float sec = 0f)
    {
        // �C���^�[�o���̉����ݒ�
        float interval = sec <= 0 ? 0 : Mathf.Max(sec, 0.02f);

        return source
            .Select(value =>
            {
                if (!EqualityComparer<T>.Default.Equals(value, expectedValue))
                    return Observable.Empty<long>();

                if (interval == 0)
                    return Observable.EveryFixedUpdate();

                // �␳�@�\�t���C���^�[�o���ifixedTime�g�p�j
                return Observable.Create<long>(observer =>
                {
                    float startTime = Time.fixedTime;
                    float nextEmitTime = startTime + interval;
                    long count = 0;

                    return Observable.EveryFixedUpdate()
                        .Subscribe(_ =>
                        {
                            float currentTime = Time.fixedTime;

                            // ���s���ׂ��񐔂��v�Z
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


    // UpdateWhileEqualTo��1�t���[�����Z���b���𑪂�Ȃ�
    // ������͒Z�������(�f�t�H���g0.001, ����0.0001)���X���b�h�v�[���ł��̂�
    // �t���[���Ɉˑ�����Unity��API���g���Ȃ�(�� Time.time)
    public static IObservable<long> TimerWhileEqualTo<T>(
        this IObservable<T> source,
        T expectedValue,
        float sec = 0.001f)
    {
        // �C���^�[�o���̉����ݒ�
        float interval = sec;
        if (sec <= 0.0001f) interval = 0.0001f;

        // �ɏ��b��������
        return source
            .Select(value =>
                EqualityComparer<T>.Default.Equals(value, expectedValue) ?
                    ObservableStopwatch(interval) :
                    Observable.Empty<long>()
            )
            .Switch();
    }

    // Stopwatch���g���A�t���[�����[�g�Ɉˑ������u�w��b�v��OnNext���J��Ԃ��B
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
                            // �������[�v�̃X���b�h���Ɛ�΍��CPU�X�C�b�`
                            Thread.Sleep(0);
                            stopwatch.Reset();
                            stopwatch.Start();
                        }
                }
                catch (OperationCanceledException)
                {
                    // �L�����Z�����͉������Ȃ�
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
