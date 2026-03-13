using ModulesFramework.Systems.Events;

namespace ModulesFrameworkUnity.Systems
{
    public interface IPhysicRunEventSystem : IEventSystem
    {
    }

    public interface IPhysicRunEventSystem<T> : IEventSystem where T : struct
    {
        public void PhysicRunEvent(T ev);
    }

    public class PhysicRunEventInvoker : IRunEventSystemInvoker
    {
        public void Invoke<TEvent>(TEvent ev, IEventSystem system) where TEvent : struct
        {
            ((IPhysicRunEventSystem<TEvent>)system).PhysicRunEvent(ev);
        }
    }
}