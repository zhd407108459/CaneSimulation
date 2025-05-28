using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : Interactable
{
    [SerializeField] private string nextScene;
    
    protected override void TriggerInteraction(PlayerInteract player)
    {
        SceneManager.LoadScene(nextScene);
    }
}
