using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using MyUtil;


public class WindowSizer : MonoBehaviour
{
    TMP_Dropdown Dropdown => GetComponent<TMP_Dropdown>();
    TextMeshProUGUI Txt => gameObject.Child("SizeViewer").GetComponent<TextMeshProUGUI>();

   
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
        if (listNum == 0)
            Screen.SetResolution(854, 480, FullScreenMode.Windowed);
        if (listNum == 1)
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        if (listNum == 2)
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        if (listNum == 3)
            Screen.SetResolution(2560, 1440, FullScreenMode.Windowed);
        if (listNum == 4)
            Screen.SetResolution(3840, 2160, FullScreenMode.Windowed);
        
        if(Txt) Txt.text = $"{Screen.width} Å~ {Screen.height}";
    }
}
