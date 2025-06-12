using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;

public class GoalAudio : MonoBehaviour
{
    private AudioSource _audioSource;
    private EventBinding<StartGoalSound> _onGoalSound;
    private EventBinding<StopGoalSound> _onStopGoalSound;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _onGoalSound = new EventBinding<StartGoalSound>(OnGoalSound);
        EventBus<StartGoalSound>.Register(_onGoalSound);
        _onStopGoalSound = new EventBinding<StopGoalSound>(OnStopGoalSound);
        EventBus<StopGoalSound>.Register(_onStopGoalSound);
    }

    private void OnDisable()
    {
        EventBus<StartGoalSound>.Deregister(_onGoalSound);
        EventBus<StopGoalSound>.Deregister(_onStopGoalSound);
    }

    void OnGoalSound(StartGoalSound eventdata)
    {
        _audioSource.Play();
    }

    void OnStopGoalSound(StopGoalSound eventdata)
    {
        _audioSource.Stop();
    }
}
