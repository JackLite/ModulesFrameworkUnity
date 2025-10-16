using System;
using System.Collections.Generic;
using ModulesFramework.Data.Events;
using ModulesFramework.Modules;
using ModulesFramework.Systems.Events;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Utils;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging.Info
{
    /// <summary>
    ///     Wraps up event runner for concrete event for concrete system types
    /// </summary>
    internal class EventSystemsDebugWrapper<TEventSystem> : EventSystemsDebugWrapper where TEventSystem : IEventSystem
    {
        private readonly EcsModule _module;
        private readonly IEventRunner _eventRunner;
        private readonly Queue<TEventSystem> _eventSystems;

        public EventSystemsDebugWrapper(EcsModule module, IEventRunner eventRunner)
        {
            _module = module;
            _eventRunner = eventRunner;
            _eventSystems = eventRunner.GetSystems<TEventSystem>().ToQueue();
        }

        public override IEventSystem GetNext()
        {
            if (_eventSystems.Count == 0)
                return null;
            return _eventSystems.Peek();
        }

        public override void RunNext(ModuleRunType runType)
        {
            UnityEngine.Debug.Log($"[Modules.Adapter] Run system {_eventSystems.Peek().GetType().GetTypeName()}");
            _eventRunner.RunSystem(_eventSystems.Dequeue());
            switch (runType)
            {
                case ModuleRunType.Run:
                    _module.DequeueRunEventRunner(_eventRunner.EventType);
                    break;
                case ModuleRunType.PostRun:
                    _module.DequeuePostRunEventRunner(_eventRunner.EventType);
                    break;
                case ModuleRunType.FrameEnd:
                    _module.DequeueFrameEndEventRunner(_eventRunner.EventType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runType), runType, null);
            }
        }

        public override bool IsEmpty()
        {
            return _eventSystems.Count == 0;
        }
    }

    internal abstract class EventSystemsDebugWrapper
    {
        public abstract IEventSystem GetNext();
        public abstract void RunNext(ModuleRunType runType);
        public abstract bool IsEmpty();
    }
}