namespace ModulesFramework.Systems.Events
{
    public interface IPostRunEventSystem : IEventSystem
    {
        
    }

    public interface IPostRunEventSystem<T> : IPostRunEventSystem where T : struct
    {
        public void PostRunEvent(T ev);
    }
    
    public class PostRunEventInvoker : IRunEventSystemInvoker
    {
        public void Invoke<TEvent>(TEvent ev, IEventSystem system) where TEvent : struct
        {
            ((IPostRunEventSystem<TEvent>) system).PostRunEvent(ev);
        }
    }
}