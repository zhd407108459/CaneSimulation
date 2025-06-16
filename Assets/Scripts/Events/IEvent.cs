namespace Event
{
    public interface IEvent{ }

    public struct CompleteLevel : IEvent
    {
        
    }

    public struct StartGoalSound : IEvent
    {
        
    }

    public struct StopGoalSound : IEvent
    {
        
    }

    public struct OutOfBounds : IEvent
    {
        
    }

    public struct CaneTooHigh : IEvent
    {
        
    }
}