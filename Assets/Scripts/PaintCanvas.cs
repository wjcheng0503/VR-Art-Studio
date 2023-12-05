using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanvas : MonoBehaviour
{
    Texture2D whiteMap;
    RenderTexture rt;

    // Start is called before the first frame update
    void Start()
    {
        resetMaterial();
    }

    public void resetMaterial()
    {
        CreateClearTexture();
        rt = getWhiteRT();
        renderMaterial();
    }

    public RenderTexture getCurrentRT()
    {
        return rt;
    }

    void renderMaterial()
    {
        Renderer rend = transform.GetComponent<Renderer>();
        rend.material.SetTexture("_PaintMap", rt);
    }

    RenderTexture getWhiteRT()
    {
        RenderTexture rt = new RenderTexture(2048, 2048, 32);
        Graphics.Blit(whiteMap, rt);
        return rt;
    }

    void CreateClearTexture()
    {
        whiteMap = new Texture2D(1, 1);
        whiteMap.SetPixel(0, 0, Color.white);
        whiteMap.Apply();
    }
}
