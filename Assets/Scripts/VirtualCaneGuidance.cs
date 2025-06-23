using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;

/// <summary>
/// Class used to provide feedback on cane position to help orient and guide the player to proper usage of the virtual cane.
/// </summary>
public class VirtualCaneGuidance : MonoBehaviour
{
    [Header("Guides")]
    [SerializeField] private Collider[] guideColliders;

    [SerializeField] private Transform hand;
    [SerializeField] private Transform detectorExtents;
    [SerializeField] private float maxYAngleRad = 0.05f;

    [SerializeField] private AudioClip warningSound;
    [SerializeField] private MeshRenderer caneGuideMesh;
    [SerializeField] private Transform caneGuideParent;
    
    [Header("Accessibility Handedness")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    
    // private
    private Camera _camera;
    private Vector3 _guideRelativePos;
    
    private AudioSource _audioSource;
    
    private bool _yAngleExceeded = false;
    private int _guideCollisions = 0;

    private Vector3 _startingHandLocalPos;
    private Quaternion _startingHandLocalRot;

    private void Awake()
    {
        _startingHandLocalPos = transform.localPosition;
        _startingHandLocalRot = transform.localRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _camera = Camera.main;
        _guideRelativePos = caneGuideParent.transform.position - _camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var y = (detectorExtents.position - hand.position).normalized.y;
        if (y > maxYAngleRad)
        {
            _yAngleExceeded = true;
            caneGuideMesh.enabled = true;
        }
        else
        {
            _yAngleExceeded = false;
            caneGuideMesh.enabled = false;
        }

        if (_yAngleExceeded || _guideCollisions > 0)
        {
            // Do something to indicate issue.
            _audioSource.clip = warningSound;
            if(!_audioSource.isPlaying)
                _audioSource.Play();
            EventBus<CaneTooHigh>.Raise(new CaneTooHigh());
        }
        else
        {
            _audioSource.Stop();
        }

        caneGuideParent.position = _camera.transform.position + _guideRelativePos;
        caneGuideParent.rotation = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0);

        // Switches the hand the cane is in by squeezing both triggers on the corresponding controller.
        // Right Hand
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) + 
            OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 1.8f)
        {
            transform.parent = rightHand;
            transform.localPosition = _startingHandLocalPos;
            transform.localRotation = _startingHandLocalRot;
        }
        
        // Left Hand
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) + 
            OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 1.8f)
        {
            transform.parent = leftHand;
            transform.localPosition = _startingHandLocalPos;
            transform.localRotation = _startingHandLocalRot;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (Collider collider in guideColliders)
        {
            if (other == collider)
            {
                _guideCollisions++;
                if(collider.TryGetComponent<MeshRenderer>(out var meshRenderer))
                {
                    meshRenderer.enabled = true;
                    meshRenderer.material.color = new Color(1, 0, 0, 0.4f);
                }
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        foreach (Collider collider in guideColliders)
        {
            if (other == collider)
            {
                _guideCollisions--;
                if(collider.TryGetComponent<MeshRenderer>(out var meshRenderer))
                {
                    meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                    meshRenderer.enabled = false;
                }
            }
        }
    }
}
