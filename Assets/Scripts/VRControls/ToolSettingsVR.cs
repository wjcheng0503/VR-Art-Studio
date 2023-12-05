using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ToolSettingsVR : MonoBehaviour
{
    public List<GameObject> toolList;
    public List<GameObject> toolModelList;
    private int selectIdx;
    private int activeIdx;

    public List<GameObject> colorList;
    public List<string> colorNameList;

    // Update is called once per frame
    void Start()
    {
        selectIdx = 0;
        activeIdx = 0;
        for (int idx = 0; idx < toolList.Count; idx++)
        {
            if (idx != activeIdx)
                toolList[idx].SetActive(false);
        }
    }
    void Update()
    {
        // updateTool();
    }

    public void nextTool()
    {
        selectIdx = (selectIdx + 1) % toolList.Count;
        // Debug.Log("next: " + selectIdx);
        updateTool();
    }

    public void prevTool()
    {
        selectIdx = (selectIdx - 1) < 0 ? toolList.Count - 1 : selectIdx - 1;
        // Debug.Log("prev: " + selectIdx);
        updateTool();
    }

    public void selectTool(GameObject option)
    {
        selectIdx = toolModelList.IndexOf(option);
        updateTool();
    }

    void updateTool()
    {
        if (selectIdx != activeIdx)
        {
            toolList[selectIdx].SetActive(true);
            toolList[activeIdx].SetActive(false);
            activeIdx = selectIdx;
        }
    }

    public void selectColor(GameObject colorObject)
    {
        ToolDrawing toolScript = toolList[activeIdx].GetComponentInChildren<ToolDrawing>();
        toolScript.inputColor = colorObject.GetComponent<ColorPaste>().color;

        if (toolScript.colorEnabled)
        {
            int idx = colorList.IndexOf(colorObject);
            toolList[activeIdx].GetComponentInChildren<TextMeshProUGUI>().text = colorNameList[idx];
        }
    }
}
