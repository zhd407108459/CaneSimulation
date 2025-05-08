using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadDetector : MonoBehaviour
{
    [Header("Tags of the Trigger to detect (multiple)")]
    [Tooltip("List of tags; if the other Collider matches any tag, audio will play.")]
    [SerializeField] private List<string> triggerTags = new List<string> { "Target" };

    private AudioSource audioSource;

    void Awake()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning($"[{nameof(HeadDetector)}] AudioSource component not found!");
        }
    }

    /// <summary>
    /// Called when another Collider enters the trigger zone.
    /// Ensure this GameObject or the other has a Collider with 'Is Trigger' enabled.
    /// </summary>
    /// <param name="other">The Collider of the incoming GameObject</param>
    void OnTriggerEnter(Collider other)
    {
        // Check if the other Collider's tag matches any of the specified tags
        foreach (var tag in triggerTags)
        {
            if (other.CompareTag(tag))
            {
                // Play the audio clip if not already playing
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                break;
            }
        }
    }

    /// <summary>
    /// Called when another Collider exits the trigger zone.
    /// Stops the audio if the Collider's tag matches.
    /// </summary>
    /// <param name="other">The Collider of the exiting GameObject</param>
    void OnTriggerExit(Collider other)
    {
        // Check if the other Collider's tag matches any of the specified tags
        foreach (var tag in triggerTags)
        {
            if (other.CompareTag(tag))
            {
                // Stop the audio clip
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                break;
            }
        }
    }
}
