using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolDrawing : MonoBehaviour
{
    //public variables
    public RightHandControl toolController;
    public int resolution = 2048;
    public Texture2D toolTexture;
    public PaintCanvas canvas;

    //flags for the colored tool
    public bool colorEnabled = false;
    public Color inputColor = new Color(0, 0, 0, 1);

    //private variables
    private bool textureChanged = false;
    private const float MULTIPLIER = 0.001875f; // constant multiplier for stroke-texture size so it looks appropriate to the tool's model size
    private Vector2 prevCoord;                   // UV coordinates stored in the previous frame
    private float detectionDistance;     // the maximum distance between the tool tip and the canvas for drawing to occur
    private Color prevColor;
    private bool vibrateController = false;
    void Start()
    {
    }

    void Update()
    {
        if (colorEnabled) updateColor();
        updateToolSize();
        detectDraw();
    }

    void updateColor()
    {
        if (prevColor != inputColor || textureChanged)
        {
            var newPixels = toolTexture.GetPixels();
            for (int idx = 0; idx < newPixels.Length; idx++)
            {
                newPixels[idx] = new Color(inputColor.r, inputColor.g, inputColor.b, newPixels[idx].a);
            }
            toolTexture.SetPixels(newPixels);
            toolTexture.Apply();

            prevColor = inputColor;
            textureChanged = false;
        }
    }

    void updateToolSize()
    {
        detectionDistance = 0.0175f * transform.lossyScale.z;
    }

    void detectDraw()
    {
        Debug.DrawRay(transform.position, -transform.forward * 20f, Color.magenta);
        RaycastHit hitFront;
        if (Physics.Raycast(transform.position, -transform.forward, out hitFront))
        {
            // Debug.Log(hit.distance);
            if (hitFront.transform.CompareTag("Canvas")      // check if target is the canvas
                && hitFront.distance <= detectionDistance    // check if tool is close to canvas
                && prevCoord != hitFront.textureCoord2)      // check if hit-spot is different from previous frame 
            {
                float toolDepth = hitFront.distance > 2 * detectionDistance / 3 ?                   // ratio of current-tool-distance to max-tool-distance.
                                  3 * (hitFront.distance - detectionDistance) / (detectionDistance) : 1;  // creates a sense of pen-pressure (i.e. bigger strokes)
                                                                                                          // based on how close the tool-tip is to the canvas.

                prevCoord = hitFront.textureCoord2;             // update the previous frame's hit spot
                Vector2 pixelUV = hitFront.lightmapCoord;    // create a new UV coordinate
                pixelUV.y *= resolution;                // multiply UV by resolution to get pixel-based position
                pixelUV.x *= resolution;
                DrawTexture(canvas.getCurrentRT(), pixelUV.x, pixelUV.y, toolDepth); // draw the tool's texture onto the canvas

                if (!vibrateController)
                {
                    toolController.vibrate();
                    vibrateController = true;
                }
            }
            else
            {
                vibrateController = false;
            }
        }
    }

    void DrawTexture(RenderTexture rt, float posX, float posY, float toolDepth)
    {
        float textureMultX = transform.lossyScale.x * MULTIPLIER * toolDepth;
        float textureMultY = transform.lossyScale.y * MULTIPLIER * toolDepth;
        Debug.Log(textureMultX + "   " + textureMultY);
        RenderTexture.active = rt;                              // activate rendertexture for drawtexture();
        GL.PushMatrix();                                        // save matrixes
        GL.LoadPixelMatrix(0, resolution, resolution, 0);       // setup matrix for correct size


        Graphics.DrawTexture(new Rect(posX - toolTexture.width * textureMultX,              // draw brushtexture
                                     (rt.height - posY) - toolTexture.height * textureMultY,
                                     toolTexture.width * textureMultX / 0.5f,
                                     toolTexture.height * textureMultY / 0.5f),
                                     toolTexture);
        GL.PopMatrix();
        RenderTexture.active = null;                            // turn off rendertexture
    }
}