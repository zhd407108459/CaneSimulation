using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class CoroutineHaptics : MonoBehaviour
{
    private InputDevice rightDevice;
    public float amplitude = 0.5f;       // 震动幅度（0~1）
    public float impulseDuration = 0.1f; // 每次震动的持续时间
    public float interval = 0.1f;        // 两次震动之间的间隔

    private bool hapticActive = false;

    void Start()
    {
        // 获取右手控制器
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0)
        {
            rightDevice = devices[0];
        }
    }

    public void StartHaptics()
    {
        if (!hapticActive)
        {
            hapticActive = true;
            StartCoroutine(ContinuousHaptics());
        }
    }

    public void StopHaptics()
    {
        hapticActive = false;
        if (rightDevice.isValid)
        {
            rightDevice.StopHaptics();
        }
    }

    IEnumerator ContinuousHaptics()
    {
        while (hapticActive)
        {
            if (rightDevice.isValid)
            {
                rightDevice.SendHapticImpulse(0, amplitude, impulseDuration);
            }
            yield return new WaitForSeconds(interval);
        }
    }
}
