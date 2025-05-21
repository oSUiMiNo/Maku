using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WindowSizeSetter : MonoBehaviour
{
    enum Resolution
    {
        HD,
        FullHD,
        _4K
    }

    [SerializeField]
    Resolution resolution;

    TextMeshPro tMPro;

    private void Awake()
    {
        tMPro = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        switch (resolution)
        {
            case Resolution.HD:
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;
            case Resolution.FullHD:
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
            case Resolution._4K:
                Screen.SetResolution(3840, 2160, FullScreenMode.Windowed);
                break;
        }
    }

    private void Update()
    {
        tMPro.text = $"Current Resolution: {Screen.width} Å~ {Screen.height}";
    }
}
