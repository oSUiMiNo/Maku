using Cysharp.Threading.Tasks;
using System;

public class Delay
{
    public static async UniTask Second(float time)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));
    }

    public static async UniTask Frame(int frame)
    {
        await UniTask.DelayFrame(frame);
    }
}