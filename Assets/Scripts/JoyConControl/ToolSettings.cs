using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ToolSettings : MonoBehaviour
{
    public List<GameObject> toolList;
    private int selectIdx;
    private int activeIdx;

    // Update is called once per frame
    void Start()
    {
        selectIdx = 0;
        activeIdx = 0;
        for (int idx = 0; idx < toolList.Count; idx++)
        {
            if (idx == activeIdx)
                toolList[idx].SetActive(true);
            else
                toolList[idx].SetActive(false);
        }
    }
    void Update()
    {
        if (selectIdx != activeIdx)
        {
            toolList[selectIdx].SetActive(true);
            toolList[activeIdx].SetActive(false);
            activeIdx = selectIdx;
        }
    }

    public void nextTool()
    {
        selectIdx = (selectIdx + 1) % toolList.Count;
        Debug.Log("next: " + selectIdx);
    }

    public void prevTool()
    {
        selectIdx = (selectIdx - 1) < 0 ? toolList.Count - 1 : selectIdx - 1;
        Debug.Log("prev: " + selectIdx);
    }
}
