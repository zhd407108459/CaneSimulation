using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : Interactable
{
    [SerializeField] private string nextScene;
    
    protected override void TriggerInteraction(PlayerInteract player)
    {
        EventBus<CompleteLevel>.Raise(new CompleteLevel());
        SceneManager.LoadScene(nextScene);
    }
}
