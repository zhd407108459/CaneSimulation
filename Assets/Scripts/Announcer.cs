using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class Announcer : MonoBehaviour
{
    public static Announcer instance;

    [SerializeField] private Transform follow;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip sampleGroundSound;
    [SerializeField] private AudioClip sampleObstacleSound;

    private TTSSpeaker _speaker;
    [SerializeField] private AudioSource audioSource;
    
    private EventBinding<CompleteLevel> _onCompleteLevel;
    private EventBinding<OutOfBounds> _onOutOfBounds;
    private EventBinding<CaneTooHigh> _onCaneTooHigh;
    
    private bool _hasStarted = false;

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

        // Start Coroutine
        StartCoroutine(SpeakIntro());
    }

    private void OnEnable()
    {
        _onCompleteLevel = new EventBinding<CompleteLevel>(OnGameEndReached);
        EventBus<CompleteLevel>.Register(_onCompleteLevel);
        _onOutOfBounds = new EventBinding<OutOfBounds>(OnOutOfBounds);
        EventBus<OutOfBounds>.Register(_onOutOfBounds);
        _onCaneTooHigh = new EventBinding<CaneTooHigh>(OnCaneTooHigh);
        EventBus<CaneTooHigh>.Register(_onCaneTooHigh);
    }

    private void OnDisable()
    {
        EventBus<CompleteLevel>.Deregister(_onCompleteLevel);
        EventBus<OutOfBounds>.Deregister(_onOutOfBounds);
        EventBus<CaneTooHigh>.Deregister(_onCaneTooHigh);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position;

        if (!_hasStarted && OVRInput.Get(OVRInput.Button.One))
        {
            _hasStarted = true;
            StartCoroutine(SpeakIntro());
        }
    }

    void OnGameEndReached()
    {
        _speaker.Speak("You have reached the end!");
        _speaker.SpeakQueued("You finished the game in " + Time.time + " seconds.");
    }

    void OnOutOfBounds()
    {
        if (!_speaker.IsActive)
        {
            _speaker.Speak("You are approaching an out of bounds area, please return.");
        }
    }

    void OnCaneTooHigh()
    {
        if (!_speaker.IsActive)
        {
            _speaker.Speak("Cane is too high! Please lower the cane.");
        }
    }

    IEnumerator SpeakIntro()
    {
        int i = 0;
        audioMixer.SetFloat("volumeSFX1", -80f);
        audioMixer.SetFloat("volumeSFX2", -80f);
        audioMixer.SetFloat("volumeSFX3", -80f);
        while (i < 100)
        {
            if (!_speaker.IsActive)
            {
                switch (i)
                {
                    case 0:
                        _speaker.Speak("Welcome to the VR Cane Simulation!");
                        break;
                    
                    case 1:
                        _speaker.Speak("Your goal is to reach the bus, indicated by this sound: ");
                        break;
                    
                    case 2:
                        audioMixer.SetFloat("volumeSFX3", 0.0f);
                        // Event to start playing bus sound.
                        EventBus<StartGoalSound>.Raise(new StartGoalSound());
                        // wait for sound to finish playing
                        yield return new WaitForSeconds(1.5f);
                        EventBus<StopGoalSound>.Raise(new StopGoalSound());
                        audioMixer.SetFloat("volumeSFX3", -80f);
                        _speaker.Speak("When your cane touches the ground it will vibrate and make a sound like this: ");
                        break;
                    case 3:
                        // Event to start playing ground scrape sound.
                        audioSource.PlayOneShot(sampleGroundSound);
                        // wait for sound to finish playing
                        yield return new WaitForSeconds(1.5f);
                        audioSource.Stop();
                        
                        _speaker.Speak(
                            "When you touch an obstacle, a different sound will start playing. This means you should try and avoid it.");
                        break;
                    case 4:
                        _speaker.Speak("One example of a sound an obstacle could make is this:");
                        break;
                    case 5:
                        // Event to start playing obstacle sound.
                        audioSource.PlayOneShot(sampleObstacleSound);
                        // wait for sound to finish playing
                        yield return new WaitForSeconds(1.5f);
                        audioSource.Stop();
                        
                        _speaker.Speak("With that, you are now ready to play! Good luck and have fun.");
                        // Unmute all sounds.
                        audioMixer.SetFloat("volumeSFX1", 0f);
                        audioMixer.SetFloat("volumeSFX2", 0f);
                        audioMixer.SetFloat("volumeSFX3", 0f);
                        EventBus<StartGoalSound>.Raise(new StartGoalSound());
                        GameDataCollector.instance.AddTaskStartRecord();
                        break;
                    default:
                        i = 100;
                        break;
                }
                i++;
            }
            else
            {
                yield return null;
            }
        }
    }

}
