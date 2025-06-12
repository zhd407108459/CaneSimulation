using System.Collections;
using System.Collections.Generic;
using Event;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.Audio;

public class Announcer : MonoBehaviour
{
    public static Announcer instance;
    
    [SerializeField] private Transform follow;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip sampleGroundSound;
    [SerializeField] private AudioClip sampleObstacleSound;

    private TTSSpeaker _speaker;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _speaker = GetComponent<TTSSpeaker>();
        _audioSource = GetComponent<AudioSource>();
        
        // Start Coroutine
        audioMixer.SetFloat("Volume (of Sound Effects)", -80f);
        audioMixer.SetFloat("Volume (of Sound Effects 3)", -80f);
        // TODO: turn into coroutine
        _speaker.Speak("Welcome to the VR Cane Simulation!");
        _speaker.SpeakQueued("Your goal is to reach the bus, indicated by this sound: ");
        audioMixer.SetFloat("Volume (of Sound Effects 2)", 0.0f);
        audioMixer.SetFloat("Volume (of Sound Effects 3)", 0.0f);
        // Event to start playing bus sound.
        EventBus<StartGoalSound>.Raise(new StartGoalSound());
        // wait for sound to finish playing
        // yield return new WaitForSeconds(1.5f);
        EventBus<StopGoalSound>.Raise(new StopGoalSound());
        audioMixer.SetFloat("Volume (of Sound Effects 3)", -80f);
        _speaker.SpeakQueued("When your cane touches the ground it will vibrate and make a sound like this: ");
        // Event to start playing ground scrape sound.
        // wait for sound to finish playing
        audioMixer.SetFloat("Volume (of Sound Effects 2)", -80f);
        _speaker.SpeakQueued("When you touch an obstacle, a different sound will start playing. This means you should try and avoid it.");
        _speaker.SpeakQueued("One example of a sound an obstacle could make is this:");
        // Event to start playing obstacle sound.
        // wait for sound to finish playing
        _speaker.SpeakQueued("With that, you are now ready to play! Good luck and have fun.");
        // Unmute all sounds.
        audioMixer.SetFloat("Volume (of Sound Effects)", 0f);
        audioMixer.SetFloat("Volume (of Sound Effects 3)", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position;
    }

    void OnGameEndReached()
    {
        _speaker.Speak("You have reached the end!");
        _speaker.SpeakQueued("You finished the game in " + Time.time + " seconds.");
    }

    void OnOutOfBounds()
    {
        _speaker.Speak("You are approaching the out of bounds area, please return.");
    }
}
