using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Experimental.GlobalIllumination;
public class ColorUtil
{
    public static Color HEX(string hex)
    {
        if (!hex.Contains("#")) hex = "#" + hex;

        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);

        return color;
    }
}
