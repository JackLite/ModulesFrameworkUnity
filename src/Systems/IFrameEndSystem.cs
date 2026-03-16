namespace ModulesFramework.Systems.Events
{
    public interface IFrameEndEventSystem : IEventSystem
    {
        
    }

    public interface IFrameEndEventSystem<T> : IFrameEndEventSystem where T : struct
    {
        void FrameEndEvent(T ev);
    }
    
    public class FrameEndEventInvoker : IRunEventSystemInvoker
    {
        public void Invoke<TEvent>(TEvent ev, IEventSystem system) where TEvent : struct
        {
            ((IFrameEndEventSystem<TEvent>) system).FrameEndEvent(ev);
        }
    }
}