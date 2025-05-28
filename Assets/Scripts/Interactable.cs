using UnityEngine;

public class Interactable : MonoBehaviour, IVisitor
{
    public void Visit<T>(T visitable) where T : IVisitable
    {
        if (visitable is PlayerInteract player)
        {
            TriggerInteraction(player);
        }
    }

    protected virtual void TriggerInteraction(PlayerInteract player)
    {
        throw new System.NotImplementedException();
    }
}