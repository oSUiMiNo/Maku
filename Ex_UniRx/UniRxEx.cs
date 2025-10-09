using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;



public static class UniRxEx_Check
{
    sealed class Holder
    {
        public int Count;
        public object Tracked; // IObservable<T>
    }
    // �C�ӂ� IObservable �C���X�^���X�i�� or ���b�v��j�� ���L�J�E���^
    static readonly ConditionalWeakTable<object, Holder> Table = new();


    ///==============================================<summary>
    /// ��x�����w�ǃg���b�L���O��t�^�i���d�K�pOK�j
    ///</summary>=============================================
    public static IObservable<T> AsTracked<T>(this IObservable<T> source)
    {
        // ���Ƀ��b�v�ς݂Ȃ炻���Ԃ�
        if (Table.TryGetValue(source, out Holder existing))
            return (IObservable<T>)existing.Tracked;

        Holder holder = new();

        IObservable<T> tracked = source
            .DoOnSubscribe(() => Interlocked.Increment(ref holder.Count))
            // UniRx �ł� DoOnDispose �������̂� Finally ���g��
            .Finally(() => Interlocked.Decrement(ref holder.Count));
        // ����p�̓�d���s����������`�F�[���Ȃ火��L����
        //.Publish().RefCount();

        // ���ƃ��b�v��̗����𓯂� Holder �ɂЂ��t��
        Table.Add(source, holder);
        Table.Add(tracked, holder);
        holder.Tracked = tracked;

        return tracked;
    }


    ///==============================================<summary>
    /// �n���� IObservable �Ɍ��ݍw�ǎ҂����邩�H
    /// �I�y���[�^AsTracked�����܂������̂ɂ̂ݗL��
    ///</summary>=============================================
    public static bool HasSubscriber<T>(this IObservable<T> source)
    {
        if (Table.TryGetValue(source, out var h))
            return Volatile.Read(ref h.Count) > 0;

        throw new InvalidOperationException(
            "���� Observable �̍w�Ǐ�Ԃ𔻒肷��ɂ̓I�y���[�^ AsTracked() �����܂���");
    }


    ///==============================================<summary>
    /// �n���� IObservable �̌��݂̍w�ǎҐ�
    /// �I�y���[�^AsTracked�����܂������̂ɂ̂ݗL��
    ///</summary>=============================================
    public static int SubscriberCount<T>(this IObservable<T> source)
    {
        if (Table.TryGetValue(source, out var h))
            return Volatile.Read(ref h.Count);

        throw new InvalidOperationException(
               "���� Observable �̍w�Ǐ�Ԃ𔻒肷��ɂ̓I�y���[�^ AsTracked() �����܂���");
    }
}



public static class UniRxEx_Loop
{
    ///==============================================<summary>
    /// ���b�Z�[�W�̒l���w�肵�����̂ƈ�v���Ă����������s�J�n�B�قȂ��Ă�����I��
    /// ������s�̊Ԋu�͕b���w��(�ŒZ0.001s)�B�����n���Ȃ���΃t���[�����̎��s�ƂȂ�
    ///</summary>=============================================
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


    // �܂��A���t�@
    ///==============================================<summary>
    /// UpdateWhileEqualTo �̎��ԕ␳��
    ///</summary>=============================================
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


    ///==============================================<summary>
    /// UpdateWhileEqualTo��1�t���[�����Z���b���𑪂�Ȃ�
    /// ������͒Z�������(�f�t�H���g0.001, ����0.0001)���X���b�h�v�[���ł��̂�
    /// �t���[���Ɉˑ�����Unity��API���g���Ȃ�(�� Time.time)
    ///</summary>=============================================
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


    ///==============================================<summary>
    /// Stopwatch���g���A�t���[�����[�g�Ɉˑ������u�w��b�v��OnNext���J��Ԃ��B
    ///</summary>=============================================
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
