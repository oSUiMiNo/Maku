using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;


public class WindowSizer : MonoBehaviour
{
    TMP_Dropdown Dropdown => transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
    TextMeshProUGUI Txt => transform.Find("Text").GetComponent<TextMeshProUGUI>();

   
    void Start()
    {
        SetSize(Dropdown.value);
        Dropdown.onValueChanged.AsObservable().Subscribe(value =>
        {
            SetSize(value);
        });
    }


    void SetSize(int listNum)
    {
        Debug.Log($"{listNum}");
        if (listNum == 0)
            Screen.SetResolution(854, 480, FullScreenMode.Windowed);
        if (listNum == 1)
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        if (listNum == 2)
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        if (listNum == 3)
            Screen.SetResolution(3840, 2160, FullScreenMode.Windowed);
        
        if(Txt) Txt.text = $"{Screen.width} Å~ {Screen.height}";
    }
}
