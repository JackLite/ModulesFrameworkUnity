using System;
using ModulesFramework.Systems.Events;

namespace ModulesFramework.Systems.Events
{
    [Obsolete("Will be deleted in 1.5.0. Use ModulesFrameworkUnity.Systems.IPostRunSystem")]
    public interface IPostRunEventSystem : IEventSystem
    {
        
    }

    [Obsolete("Will be deleted in 1.5.0. Use ModulesFrameworkUnity.Systems.IPostRunSystem")]
    public interface IPostRunEventSystem<T> : IPostRunEventSystem where T : struct
    {
        public void PostRunEvent(T ev);
    }
}

namespace ModulesFrameworkUnity.Systems
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