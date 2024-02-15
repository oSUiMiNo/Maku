using Cysharp.Threading.Tasks;
using System;

public class Delay
{
    public static async UniTask Second(double time)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));
    }
}