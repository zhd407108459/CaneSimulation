using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private GameObject collisionIndicator;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material collisionMaterial;
    [SerializeField] private AudioClip defaultCollisionSound;

    // List of tags, each corresponding to an entry in the collisionSounds list
    [SerializeField] private List<string> collisionTags = new List<string>();
    // List of audio clips for each tag; indices must match collisionTags
    [SerializeField] private List<AudioClip> collisionSounds = new List<AudioClip>();

    // Toggle whether to show the collision indicator
    [SerializeField] private bool showCollisionIndicator = true;

    // Maximum speed: when reached or exceeded, volume is 1
    [SerializeField] private float maxSpeed = 10f;
    // Minimum speed: below this, volume is 0
    [SerializeField] private float minSpeed = 0.1f;

    [SerializeField] private Transform hand;

    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    // Count of currently colliding colliders
    private int collisionCount = 0;

    // Last frame's position to calculate movement speed
    private Vector3 lastPosition;

    void Start()
    {
        // Get the MeshRenderer and apply the default material
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
        }

        // Get or add an AudioSource component and configure looping
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Show or hide the collision indicator based on the setting
        if (collisionIndicator != null && !showCollisionIndicator)
        {
            collisionIndicator.SetActive(false);
        }

        // Initialize lastPosition
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate speed (meters per second) based on positional change
        float distance = Vector3.Distance(transform.position, lastPosition);
        float speed = distance / Time.deltaTime;
        float normalizedSpeed = 0f;

        // Map [minSpeed, maxSpeed] to [0, 1] for volume control
        if (speed < minSpeed)
        {
            normalizedSpeed = 0f;
        }
        else
        {
            normalizedSpeed = Mathf.Clamp01((speed - minSpeed) / (maxSpeed - minSpeed));
        }

        // Adjust audio volume based on speed
        if (audioSource != null)
        {
            audioSource.volume = normalizedSpeed;
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionCount++;

        // On first collision, activate indicator and change material
        if (collisionCount == 1)
        {
            if (collisionIndicator != null && showCollisionIndicator)
            {
                collisionIndicator.SetActive(true);
            }
            if (meshRenderer != null && collisionMaterial != null)
            {
                meshRenderer.material = collisionMaterial;
            }
        }
        
        // TODO: Use CapsuleCastAll for points of contact.
        RaycastHit[] results = new RaycastHit[20];
        var size = Physics.CapsuleCastNonAlloc(hand.position, transform.position, 0.5f, transform.position - hand.position, results);
        //check hits against collider, and fire audio on contact points for each hit that matches the collider's.
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                if (results[i].collider == other)
                {
                    // Spawn audio source at point of contact
                    //results[i].point;
                }
            }
        }

        // Select the appropriate sound based on the collider's tag
        AudioClip selectedSound = defaultCollisionSound;
        for (int i = 0; i < collisionTags.Count; i++)
        {
            if (other.CompareTag(collisionTags[i]) && i < collisionSounds.Count && collisionSounds[i] != null)
            {
                selectedSound = collisionSounds[i];
                break;
            }
        }

        // Play the selected sound
        if (audioSource != null)
        {
            audioSource.clip = selectedSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount--;
        if (collisionCount <= 0)
        {
            collisionCount = 0;
            // Reset to default state when no collisions remain
            if (collisionIndicator != null && showCollisionIndicator)
            {
                collisionIndicator.SetActive(false);
            }
            if (meshRenderer != null && defaultMaterial != null)
            {
                meshRenderer.material = defaultMaterial;
            }
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
