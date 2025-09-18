using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class Delay
{
    // CT‚Å’†’f‰Â
    public static async UniTask Second(float time, CancellationToken ct = default)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: ct);
    }

    // CT‚Å’†’f‰Â
    public static async UniTask Frame(int frame, CancellationToken ct = default)
    {
        await UniTask.DelayFrame(frame, cancellationToken: ct);
    }
}
