using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RightHandControl : MonoBehaviour
{
    public Transform tools;
    public ToolSettingsVR settings;
    public LineRenderer lineRend;
    private InputDevice rightHand;

    void Update()
    {
        if (!rightHand.isValid)
        {
            getRightHand();
        }
        else
        {
            controlRightHand();
        }
    }

    void getRightHand()
    {
        var gameControllers = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, gameControllers);

        foreach (var device in gameControllers)
        {
            Debug.Log(string.Format("Device name '{0}' has role '{1}'", device.name, device.role.ToString()));
        }

        if (gameControllers.Count > 0)
            rightHand = gameControllers[0];
    }

    void controlRightHand()
    {
        detectRayCast();
    }

    void detectRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(tools.position, -tools.transform.forward, out hit))
        {
            if (hit.transform.CompareTag("ToolOptions"))
            {
                lineRend.enabled = true;
                lineRend.SetPosition(0, tools.position);
                lineRend.SetPosition(1, hit.point);

                bool gripAction = rightHand.TryGetFeatureValue(CommonUsages.gripButton, out bool grip);
                bool triggerAction = rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);
                if ((gripAction && grip) || (triggerAction && trigger))
                {
                    settings.selectTool(hit.transform.gameObject);
                }
            }
            else if (hit.transform.CompareTag("ColorOptions"))
            {
                lineRend.enabled = true;
                lineRend.SetPosition(0, tools.position);
                lineRend.SetPosition(1, hit.point);

                bool gripAction = rightHand.TryGetFeatureValue(CommonUsages.gripButton, out bool grip);
                bool triggerAction = rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);
                if ((gripAction && grip) || (triggerAction && trigger))
                {
                    settings.selectColor(hit.transform.gameObject);
                }
            }
            else
                lineRend.enabled = false;
        }
    }

    public void vibrate()
    {
        rightHand.SendHapticImpulse(0u, 0.01f, 0.5f);
    }
}