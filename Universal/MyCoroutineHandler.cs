using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCoroutineHandler : SingletonCompo<MyCoroutineHandler>
{
    static public Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        return Compo.StartCoroutine(coroutine);
    }

    static public void SuspendStaticCoroutine(IEnumerator coroutine)
    {
        Compo.StopCoroutine(coroutine);
    }
}
