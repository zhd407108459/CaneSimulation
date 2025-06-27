using System.Collections;
using System.Collections.Generic;
using Event;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetector : MonoBehaviour, IVisitor
{
    [SerializeField] private GameObject collisionIndicator;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material collisionMaterial;
    [SerializeField] private AudioClip defaultCollisionSound;

    // Toggle whether to show the collision indicator
    [SerializeField] private bool showCollisionIndicator = true;

    // Maximum speed: when reached or exceeded, volume is 1
    [SerializeField] private float maxSpeed = 10f;
    // Minimum speed: below this, volume is 0
    [SerializeField] private float minSpeed = 0.1f;

    [SerializeField] private float collisionOverlapRadius = 0.2f;

    [SerializeField] private Transform hand;
    [SerializeField] private CollisionAudioSource collisionAudioSourcePrefab;
    [SerializeField] private LayerMask collisionLayerMask;

    [SerializeField] private int objectsTouched = 0;

    private MeshRenderer meshRenderer;

    // Count of currently colliding colliders
    private int collisionCount = 0;

    // Last frame's position to calculate movement speed
    private Vector3 lastPosition;

    private Vector3 lastContactPoint;
    private bool isExit;

    private float volume;
    
    private EventBinding<CompleteLevel> onCompleteLevel;
    
    private CollisionRecorder collisionRecorder;

    void OnEnable()
    {
        onCompleteLevel = new EventBinding<CompleteLevel>(OnCompleteLevel);
        EventBus<CompleteLevel>.Register(onCompleteLevel);
    }

    void OnDisable()
    {
        EventBus<CompleteLevel>.Deregister(onCompleteLevel);
    }

    void Start()
    {
        // Get the MeshRenderer and apply the default material
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
        }

        // Show or hide the collision indicator based on the setting
        if (collisionIndicator != null && !showCollisionIndicator)
        {
            collisionIndicator.SetActive(false);
        }

        // Initialize lastPosition
        lastPosition = transform.position;
        
        collisionRecorder = new CollisionRecorder();
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

        volume = normalizedSpeed;

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionCount++;
        collisionRecorder.numCollisions++;

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
        
        RaycastHit[] results = new RaycastHit[20];
        var size = Physics.CapsuleCastNonAlloc(hand.position, transform.position, collisionOverlapRadius, (transform.position - hand.position).normalized, results, 4, collisionLayerMask);
        //check hits against collider, and fire audio on contact points for each hit that matches the collider's.
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                if (results[i].collider == other)
                {
                    isExit = false;
                    // Record the collision enter.
                    GameDataCollector.instance.AddCaneCollisionEnterRecord(transform.position, results[i].point, results[i].collider.gameObject.name);
                    // Spawn audio source at point of contact
                    //results[i].point;
                    var newVisitables = other.GetComponents<IVisitable>();
                    if (newVisitables.Length > 0)
                    {
                        foreach (var visitable in newVisitables)
                        {
                            lastContactPoint = results[i].point;
                            visitable.Accept(this);
                        }
                    }
                    else
                    {
                        var haptic = other.AddComponent<HapticAudioSource>();
                        haptic.collisionSound = defaultCollisionSound;
                        haptic.Accept(this);
                    }
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        RaycastHit[] results = new RaycastHit[20];
        var size = Physics.CapsuleCastNonAlloc(hand.position, transform.position, 0.5f, (transform.position - hand.position).normalized, results, 4, collisionLayerMask);
        //check hits against collider, and fire audio on contact points for each hit that matches the collider's.
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                if (results[i].collider == other)
                {
                    isExit = false;
                    // Spawn audio source at point of contact
                    //results[i].point;
                    var newVisitables = other.GetComponents<IVisitable>();
                    if (newVisitables.Length > 0)
                    {
                        foreach (var visitable in newVisitables)
                        {
                            lastContactPoint = results[i].point;
                            visitable.Accept(this);
                        }
                    }
                    else
                    {
                        var haptic = other.AddComponent<HapticAudioSource>();
                        haptic.collisionSound = defaultCollisionSound;
                        haptic.Accept(this);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Record the collision exit.
        GameDataCollector.instance.AddCaneCollisionExitRecord(transform.position, other.transform.position, other.gameObject.name);

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
        }
        
        isExit = true;
        var newVisitables = other.GetComponents<IVisitable>();
        foreach (var visitable in newVisitables)
        {
            visitable.Accept(this);
        }
    }

    public void CreateCollisionAudioSourceFromHaptic(HapticAudioSource hapticAudioSource)
    {
        if (hapticAudioSource.collisionAudioSource == null)
        {
            hapticAudioSource.collisionAudioSource = Instantiate(collisionAudioSourcePrefab, lastContactPoint, Quaternion.identity);
        }

        hapticAudioSource.collisionAudioSource.transform.position = lastContactPoint;
    }

    public void Visit<T>(T visitable) where T : IVisitable
    {
        if (visitable is HapticAudioSource hapticAudioSource)
        {
            if (isExit)
            {
                hapticAudioSource.collisionAudioSource.StopInstancedAudio();
            }
            else
            {
                CreateCollisionAudioSourceFromHaptic(hapticAudioSource);
                hapticAudioSource.collisionAudioSource.audioSource.volume = volume;
                hapticAudioSource.collisionAudioSource.gameObject.SetActive(true);
                hapticAudioSource.collisionAudioSource.PlayInstancedAudio(hapticAudioSource.collisionSound);
            }
        }
    }

    private void OnCompleteLevel(CompleteLevel level)
    {
        GameDataCollector.instance.AddTaskCompletionRecord(Time.fixedTime);
    }

    public class CollisionRecorder
    {
        public int numCollisions = 0;
    }
}
