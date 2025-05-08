using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class CoroutineHaptics : MonoBehaviour
{
    private InputDevice rightDevice;
    public float amplitude = 0.5f;       // Vibration amplitude (0-1)
    public float impulseDuration = 0.1f; // Duration of each haptic impulse
    public float interval = 0.1f;        // Interval between impulses

    private bool hapticActive = false;

    void Start()
    {
        // Retrieve the right-hand controller
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
            rightDevice.StopHaptics(); // Stop any ongoing haptic feedback
        }
    }

    IEnumerator ContinuousHaptics()
    {
        while (hapticActive)
        {
            if (rightDevice.isValid)
            {
                // Send a haptic impulse on channel 0
                rightDevice.SendHapticImpulse(0, amplitude, impulseDuration);
            }
            yield return new WaitForSeconds(interval);
        }
    }
}
