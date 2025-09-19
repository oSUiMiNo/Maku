using Cysharp.Threading.Tasks;
using System;
using System.Threading;


public class Delay
{
    public static UniTask Sec(float sec, CancellationToken ct = default)
        => UniTask.Delay(TimeSpan.FromSeconds(sec), cancellationToken: ct);

    public static UniTask Frame(int frame, CancellationToken ct = default)
        => UniTask.DelayFrame(frame, cancellationToken: ct);
}
