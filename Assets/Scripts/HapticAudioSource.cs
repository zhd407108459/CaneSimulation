using UnityEngine;

public class HapticAudioSource : MonoBehaviour, IVisitable
{
    [SerializeField] public AudioClip collisionSound;

    public CollisionAudioSource collisionAudioSource;

    public void Accept(IVisitor visitor)
    {
        visitor.Visit(this);
    }
}