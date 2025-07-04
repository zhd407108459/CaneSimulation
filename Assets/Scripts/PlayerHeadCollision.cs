using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class representing the player's head and records when the player's head crosses a set boundary.
/// </summary>
public class PlayerHeadCollision : MonoBehaviour
{
    [SerializeField] private AudioClip defaultCollisionSound;
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        GameDataCollector.instance.AddPlayerCollisionEnterRecord(transform.position, other.transform.position, other.gameObject.name);
        if (other.CompareTag("Extents"))
        {
            GameDataCollector.instance.AddPlayerCrossBoundaryRecord(transform.position);
            audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameDataCollector.instance.AddPlayerCollisionExitRecord(transform.position, other.transform.position, other.gameObject.name);
        if (other.CompareTag("Extents"))
        {
            audioSource.Stop();
        }
    }
}
