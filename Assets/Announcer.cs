using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;

public class Announcer : MonoBehaviour
{
    public static Announcer instance;
    
    [SerializeField] private Transform follow;

    private TTSSpeaker _speaker;
    
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
        _speaker.Speak("Welcome to the VR Cane Simulation!");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position;
    }
}
