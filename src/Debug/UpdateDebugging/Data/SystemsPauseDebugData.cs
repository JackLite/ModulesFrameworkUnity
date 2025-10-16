using System.Collections.Generic;
using ModulesFramework.Data.Events;
using ModulesFramework.Modules;
using ModulesFramework.Systems;
using ModulesFrameworkUnity.Debug.UpdateDebugging;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Info;

namespace ModulesFrameworkUnity.Debug
{
    internal struct SystemsPauseDebugData
    {
        public EcsModule currentModule;
        public Queue<IEventRunner> eventRunners;
        public ISystem nextSystem;
        public Queue<ISystem> updateSystems;
        public EventSystemsDebugWrapper eventSystemsWrapper;
    }
}