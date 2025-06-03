using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MetaXRAudioSource))]
public class CollisionAudioSource : MonoBehaviour
{
    private AudioSource _audioSource;
    private bool _startPlay;

    public AudioSource audioSource => _audioSource;
    
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!_audioSource.isPlaying && _startPlay)
        {
            gameObject.SetActive(false);
        }
    }

    public void PlayInstancedAudio(AudioClip audio)
    {
        _audioSource.clip = audio;
        _audioSource.loop = true;
        _startPlay = true;
        _audioSource.Play();
    }

    public void StopInstancedAudio()
    {
        _audioSource.Stop();
        _startPlay = false;
        gameObject.SetActive(false);
    }
}
