using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : Interactable
{
    [SerializeField] private string nextScene;

    private bool isUsed;

    private void Start()
    {
        isUsed = false;
    }

    protected override void TriggerInteraction(PlayerInteract player)
    {
        if (!isUsed)
        {
            EventBus<CompleteLevel>.Raise(new CompleteLevel());
            //SceneManager.LoadScene(nextScene);
            // prevent player from completing the level multiple times.
            isUsed = true;
        }
        
    }
}
