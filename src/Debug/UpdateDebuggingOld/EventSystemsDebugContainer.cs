using System.Collections.Generic;
using ModulesFramework.Data.Events;
using ModulesFramework.Modules;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity.Utils;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging
{
    /// <summary>
    ///     Container to run event systems is pause mode
    /// </summary>
    internal class EventSystemsDebugContainer<TSystem> : EventSystemsDebugContainer where TSystem : class, IEventSystem
    {
        public Queue<IEventRunner> eventRunners;
        public Queue<TSystem> systems;

        public EventSystemsDebugContainer(EcsModule module)
        {
            eventRunners = module.GetEventRunners(typeof(TSystem)).ToQueue();
            systems = new Queue<TSystem>();
            if (eventRunners.Count > 0)
            {
                systems = eventRunners.Peek().GetSystems<TSystem>().ToQueue();
            }
        }

        /// <summary>
        ///     Return true if all systems was finished
        /// </summary>
        public override bool RunNext()
        {
            if (eventRunners.Count == 0)
                return true;

            var runner = eventRunners.Peek();
            var system = systems.Dequeue();
            UnityEngine.Debug.Log($"[Modules.Adapter] Run event system {system.GetType().Name}");
            runner.RunSystem(system);

            while (systems.Count == 0)
            {
                if (eventRunners.Count == 0)
                    return true;

                eventRunners.Dequeue();
                systems = eventRunners.Peek().GetSystems<TSystem>().ToQueue();
            }

            return false;
        }

        public override ISystem GetNextSystem()
        {
            if (systems.Count == 0)
                return null;

            return systems.Peek();
        }
    }
    
    internal abstract class EventSystemsDebugContainer
    {
        public abstract bool RunNext();
        public abstract ISystem GetNextSystem();
    }
}