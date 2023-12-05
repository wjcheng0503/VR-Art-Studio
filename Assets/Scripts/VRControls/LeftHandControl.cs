using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class LeftHandControl : MonoBehaviour
{
    public Transform player;
    public ToolSettingsVR settings;
    private InputDevice leftHand;
    private bool toolBtnFlag = false;

    // variables for graphite tool
    public ToolDrawing graphiteTool;
    public List<Texture2D> graphiteTx;
    public TextMeshProUGUI graphiteNameUI;
    public List<string> graphiteNames;
    private int graphiteIdx;
    private bool graphiteBtnFlag = false;

    // variables for displaying game information
    public List<GameObject> tasks;
    public PaintCanvas canvas;
    private int taskIdx;
    private bool taskFlag;

    public GameObject mainSceneProps;
    public GameObject tutorialBoard;
    public List<GameObject> tutorialImgs;
    private int imgIdx;

    private bool thumbstickClick = false;

    void Start()
    {
        graphiteIdx = 0;
        graphiteTool.toolTexture = graphiteTx[graphiteIdx];
        graphiteNameUI.text = graphiteNames[graphiteIdx];

        mainSceneProps.SetActive(false);

        taskFlag = false;
        taskIdx = 0;
        for (int idx = 0; idx < tasks.Count; idx++)
        {
            if (idx != taskIdx)
                tasks[idx].SetActive(false);
        }

        imgIdx = 0;
        for (int idx = 0; idx < tutorialImgs.Count; idx++)
        {
            if (idx != imgIdx)
                tutorialImgs[idx].SetActive(false);
        }
    }

    void Update()
    {
        if (!leftHand.isValid)
        {
            getLeftHand();
        }
        else
        {
            controlLeftHand();
        }
    }

    void getLeftHand()
    {
        var gameControllers = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, gameControllers);

        foreach (var device in gameControllers)
        {
            Debug.Log(string.Format("Device name '{0}' has role '{1}'", device.name, device.role.ToString()));
        }

        if (gameControllers.Count > 0)
            leftHand = gameControllers[0];
    }

    void controlLeftHand()
    {
        move();
        changeTool();
        changeGraphiteHardness();
        if (taskFlag)
            nextTask();
        else
            nextTutorialImg();
    }

    void move()
    {
        if (leftHand.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 movement) && movement != Vector2.zero)
            player.localPosition += new Vector3(movement.x, 0, movement.y) * Time.deltaTime;
    }

    void changeTool()
    {
        Debug.Log(toolBtnFlag);
        if (leftHand.TryGetFeatureValue(CommonUsages.gripButton, out bool next) && next)
        {
            if (!toolBtnFlag)
            {
                settings.nextTool();
                toolBtnFlag = true;
            }
        }
        else if (leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool prev) && prev)
        {
            if (!toolBtnFlag)
            {
                settings.prevTool();
                toolBtnFlag = true;
            }
        }
        else
            toolBtnFlag = false;
    }

    void changeGraphiteHardness()
    {
        if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool prev) && prev)
        {
            if (!graphiteBtnFlag)
            {
                prevHardness();
                graphiteBtnFlag = true;
            }
        }
        else if (leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool next) && next)
        {
            if (!graphiteBtnFlag)
            {
                nextHardness();
                graphiteBtnFlag = true;
            }
        }
        else
            graphiteBtnFlag = false;
    }

    void prevHardness()
    {
        graphiteIdx = (graphiteIdx - 1) < 0 ? graphiteTx.Count - 1 : graphiteIdx - 1;
        graphiteTool.toolTexture = graphiteTx[graphiteIdx];
        graphiteNameUI.text = graphiteNames[graphiteIdx];
    }

    void nextHardness()
    {
        graphiteIdx = (graphiteIdx + 1) % graphiteTx.Count;
        graphiteTool.toolTexture = graphiteTx[graphiteIdx];
        graphiteNameUI.text = graphiteNames[graphiteIdx];
    }

    void nextTask()
    {
        if (leftHand.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool click) && click)
        {
            if (!thumbstickClick)
            {
                if (taskIdx < tasks.Count - 1)
                {
                    tasks[taskIdx].SetActive(false);
                    taskIdx++;
                    tasks[taskIdx].SetActive(true);
                    canvas.resetMaterial();
                    thumbstickClick = true;
                }
            }
        }
        else
            thumbstickClick = false;
    }
    void nextTutorialImg()
    {
        if (leftHand.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool click) && click)
        {
            if (!thumbstickClick)
            {
                if (imgIdx < tutorialImgs.Count - 1)
                {
                    tutorialImgs[imgIdx].SetActive(false);
                    imgIdx++;
                    tutorialImgs[imgIdx].SetActive(true);
                    thumbstickClick = true;
                }

                else
                {
                    tutorialBoard.SetActive(false);
                    mainSceneProps.SetActive(true);
                    taskFlag = true;
                    thumbstickClick = true;
                }
            }
        }
        else
            thumbstickClick = false;
    }
}
