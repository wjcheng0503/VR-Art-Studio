using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyConControls : MonoBehaviour
{

    private List<Joycon> joycons;

    // Values made available via Unity
    public float[] stick;
    public Vector3 gyro;
    public Vector3 accel;
    public Vector3 origin;
    public int jc_ind = 0;
    public Quaternion orientation;
    public Transform pivot;
    public ToolSettings settings;

    float accelerometerUpdateInterval = 1.0f / 60.0f;
    float lowPassKernelWidthInSeconds = 1.0f;

    private float lowPassFilterFactor;
    private Vector3 lowPassValue = Vector3.zero;

    void Start()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        origin = gameObject.transform.position;
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }

        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        lowPassValue = accel;
    }

    // Update is called once per frame
    void Update()
    {
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            // get joystick position and set it to the controller object positions
            // Right controller sets the XY position,
            // Left controller sets the Z position
            stick = j.GetStick();
            gameObject.transform.localPosition += new Vector3(-stick[1], stick[0], 0) * 5 * Time.deltaTime;

            // Gyro values: x, y, z axis values (in radians per second)
            gyro = j.GetGyro();

            // Accel values:  x, y, z axis values (in Gs)
            accel = j.GetAccel();

            // get acceleration and try to set it to controller position
            // lowPassValue = LowPassFilterAccelerometer(lowPassValue, accel);
            // Vector3 dir = getDirection(lowPassValue);
            // gameObject.transform.position = origin + dir * 1000;
            // Debug.Log(dir * 1000);

            // get orientation and set it to the controller object
            orientation = j.GetVector();
            gameObject.transform.position = pivot.position;
            gameObject.transform.rotation = Quaternion.Inverse(orientation);
            gameObject.transform.Rotate(new Vector3(180, 180, 180), Space.Self);
            gameObject.transform.position = 2 * gameObject.transform.position - pivot.position;

            if (j.GetButtonUp(Joycon.Button.DPAD_UP))
            {
                settings.prevTool();
            }
            else if (j.GetButtonUp(Joycon.Button.DPAD_DOWN))
            {
                settings.nextTool();
            }
        }
    }

    Vector3 getDirection(Vector3 accValue)
    {
        Vector3 dir = Vector3.zero;
        dir.x = -accValue.y;
        dir.y = accValue.z;
        dir.z = -accValue.x;

        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        // Make it move 10 meters per second instead of 10 meters per frame...
        dir *= Time.deltaTime;
        return dir;
    }

    Vector3 LowPassFilterAccelerometer(Vector3 prevValue, Vector3 currentValue)
    {
        Vector3 newValue = Vector3.Lerp(prevValue, currentValue, lowPassFilterFactor);
        return newValue;
    }
}