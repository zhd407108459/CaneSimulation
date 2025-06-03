using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to provide feedback on cane position to help orient and guide the player to proper usage of the virtual cane.
/// </summary>
public class VirtualCaneGuidance : MonoBehaviour
{
    [SerializeField] private Collider[] guideColliders;

    [SerializeField] private Transform hand;
    [SerializeField] private Transform detectorExtents;
    [SerializeField] private float maxYAngleRad = 0.05f;

    [SerializeField] private AudioClip warningSound;
    
    private AudioSource _audioSource;
    
    private bool yAngleExceeded = false;
    private int guideCollisions = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        var y = (detectorExtents.position - hand.position).normalized.y;
        if (y > maxYAngleRad)
        {
            yAngleExceeded = true;
        }
        else
        {
            yAngleExceeded = false;
        }

        if (yAngleExceeded || guideCollisions > 0)
        {
            // Do something to indicate issue.
            _audioSource.clip = warningSound;
            if(!_audioSource.isPlaying)
                _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (Collider collider in guideColliders)
        {
            if (other == collider)
            {
                guideCollisions++;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        foreach (Collider collider in guideColliders)
        {
            if (other == collider)
            {
                guideCollisions--;
            }
        }
    }
}
