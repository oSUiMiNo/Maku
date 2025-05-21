using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyUtil;


public class CanvasUtil
{
    //========================================
    // ÉLÉÉÉìÉoÉXÇê∂ê¨
    //========================================
    public GameObject CreateCanvas(string name, RenderMode renderMode)
    {
        GameObject gObj = new GameObject();
        gObj.name = name;

        Canvas canvas = gObj.AddComponent<Canvas>();
        CanvasScaler canvasScaler = gObj.AddComponent<CanvasScaler>();
        GraphicRaycaster graphicRaycaster = gObj.AddComponent<GraphicRaycaster>();

        canvas.renderMode = renderMode;

        return gObj;
    }
}
