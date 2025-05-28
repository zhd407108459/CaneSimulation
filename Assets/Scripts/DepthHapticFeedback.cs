using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthHapticFeedback : MonoBehaviour
{
    // Controller to use, here the Oculus right-hand controller
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    // Penetration distance at which vibration reaches maximum intensity
    public float maxPenetrationDistance = 1.0f;
    // Vibration frequency (adjustable as needed)
    public float vibrationFrequency = 0.5f;
    // The cane itself is the depth reference (tip), assign in the Inspector
    public Transform depthReference;

    // All Colliders on the cane (and its child objects)
    private Collider[] caneColliders;

    void Start()
    {
        if (depthReference == null)
        {
            Debug.LogError("Please assign the depthReference object (the cane) in the Inspector.");
            return;
        }
        // Get all Colliders on depthReference and its children
        caneColliders = depthReference.GetComponentsInChildren<Collider>();
        if (caneColliders.Length == 0)
        {
            Debug.LogError("No Colliders found on the depthReference object.");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (depthReference == null)
            return;

        float maxPenetration = 0f;
        Vector3 direction;

        // Iterate through all cane colliders to calculate penetration with the other collider
        foreach (Collider col in caneColliders)
        {
            float penetrationDistance;
            // Physics.ComputePenetration calculates the minimum translation (penetrationDistance)
            // required to separate col and other; returns true if they overlap.
            bool penetrating = Physics.ComputePenetration(
                col, col.transform.position, col.transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out direction, out penetrationDistance);

            // Track the largest penetration distance as the "insertion depth"
            if (penetrating && penetrationDistance > maxPenetration)
            {
                maxPenetration = penetrationDistance;
            }
        }

        // Normalize penetration depth: when maxPenetration reaches maxPenetrationDistance, amplitude is 1
        float normalizedDepth = Mathf.Clamp01(maxPenetration / maxPenetrationDistance);
        float amplitude = normalizedDepth;

        // Trigger Oculus controller vibration with specified frequency and amplitude
        OVRInput.SetControllerVibration(vibrationFrequency, amplitude, controller);
    }

    void OnTriggerExit(Collider other)
    {
        // Stop vibration when exiting the trigger
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
