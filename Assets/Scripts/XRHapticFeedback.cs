using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class XRHapticFeedback : MonoBehaviour
{
    private InputDevice rightHandDevice;

    void Start()
    {
        // Retrieve the right-hand controller
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
        if (devices.Count > 0)
        {
            rightHandDevice = devices[0];
        }
    }

    public void SendHapticImpulse(float amplitude, float duration)
    {
        if (rightHandDevice.isValid)
        {
            // amplitude ranges from 0.0 to 1.0
            rightHandDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }
}
