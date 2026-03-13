using System;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;

namespace ModulesFramework.Systems.Events
{
    [Obsolete("Will be deleted in 1.5.0. Use ModulesFrameworkUnity.Systems.IPostRunSystem")]
    public interface IFrameEndEventSystem : IEventSystem
    {
        
    }

    [Obsolete("Will be deleted in 1.5.0. Use ModulesFrameworkUnity.Systems.IPostRunSystem")]
    public interface IFrameEndEventSystem<T> : IFrameEndEventSystem where T : struct
    {
        void FrameEndEvent(T ev);
    }
}

namespace ModulesFrameworkUnity.Systems
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