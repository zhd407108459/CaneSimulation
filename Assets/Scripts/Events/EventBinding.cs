using System;

namespace Event
{
    internal interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
    
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> onEvent = delegate(T _) {};
        private Action onEventNoArgs = delegate {};
        
        Action<T> IEventBinding<T>.OnEvent { get => onEvent; set => onEvent = value; }
        Action IEventBinding<T>.OnEventNoArgs { get => onEventNoArgs; set => onEventNoArgs = value; }

        public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;
        public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;

        /// <summary>
        /// Subscribe to an event with arguments.
        /// </summary>
        /// <param name="onEvent"></param>
        public void Add(Action<T> onEvent) => this.onEvent += onEvent;
        /// <summary>
        /// Unsubscribe from an event with arguments.
        /// </summary>
        /// <param name="onEvent"></param>
        public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
        
        /// <summary>
        /// Subscribe to an event with no arguments.
        /// </summary>
        /// <param name="onEventNoArgs"></param>
        public void Add(Action onEventNoArgs) => this.onEventNoArgs += onEventNoArgs;
        /// <summary>
        /// Unsubscribe from an event with no arguments.
        /// </summary>
        /// <param name="onEventNoArgs"></param>
        public void Remove(Action onEventNoArgs) => this.onEventNoArgs -= onEventNoArgs;
        
        public void Raise(T @event)
        {
            ((IEventBinding<T>)this).OnEvent.Invoke(@event);
            ((IEventBinding<T>)this).OnEventNoArgs.Invoke();
        }
    }
}