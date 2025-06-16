using Event;

public class BoundsAudioSource : HapticAudioSource
{
    public override void Accept(IVisitor visitor)
    {
        EventBus<OutOfBounds>.Raise(new OutOfBounds());
        visitor.Visit(this);
    }
}