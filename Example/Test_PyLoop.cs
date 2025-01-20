using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UniRx;

public class Test_PyLoop : MonoBehaviour
{
    PyAPI py = new PyAPI(
        $"{Application.streamingAssetsPath}/PythonAssets",
        @"C:\Users\osuim\Documents\MyPJT\VEnvs\.venv\Scripts\python.exe"
    );

    PyFnc pyFnc;
    BoolReactiveProperty a = new BoolReactiveProperty( true );

    async void Start()
    {
        py.Exe("LogTest.py", 7).Forget();


        JObject input = new JObject();
        input["Data0"] = "‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ‚ ";
        input["Data1"] = "‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢‚¢";

        pyFnc = py.Idle("Test_Idle.py");
        a
        .TimerWhileEqualTo(true, 1)
        .Subscribe(_ =>
        {
            pyFnc.Exe(input);
        }).AddTo(this);


        pyFnc.OnOut.Subscribe(JO =>
        {
            Debug.Log($"{JO["Data1"]}");
        }).AddTo(this);
    }
}
    