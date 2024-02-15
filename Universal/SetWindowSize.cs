using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetWindowSize : MonoBehaviour
{
    enum Resolution
    {
        FullHD,
        HD,
        _4K
    }

    [SerializeField]
    Resolution resolution;

    TextMeshPro textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        switch (resolution)
        {
            case Resolution.HD:
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed, 60);
                break;
            case Resolution.FullHD:
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, 60);
                break;
            case Resolution._4K:
                Screen.SetResolution(3840, 2160, FullScreenMode.Windowed, 60);
                break;
        }
    }

    private void Update()
    {
        textMeshPro.text = $"Current Resolution: {Screen.width} Å~ {Screen.height}";
    }
}
