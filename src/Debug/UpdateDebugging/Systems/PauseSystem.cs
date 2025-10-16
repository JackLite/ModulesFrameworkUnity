using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems.Subscribes;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.UpdateDebugging;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Service;
using ModulesFrameworkUnity.DebugWindow.Events;
using ModulesFrameworkUnity.DebugWindow.Service;

namespace ModulesFrameworkUnity.DebugWindow.Systems
{
    [EcsSystem(typeof(UpdateDebuggingModule))]
    public class PauseSystem : ISubscribeInitSystem<PauseChangedEvent>
    {
        private readonly DataWorld _world;
        private readonly UpdateDebuggingService _debuggingService;
        
        public void HandleEvent(PauseChangedEvent ev)
        {
            if (ev.IsPaused)
                PauseMF();
            else
                ResumeMF();
        }

        private void PauseMF()
        {
            ref var debugData = ref _world.OneData<DebugData>();
            debugData.isPause = true;
            _debuggingService.InitDebugging();
        }

        private void ResumeMF()
        {
            ref var debugData = ref _world.OneData<DebugData>();
            debugData.isPause = false;
            _debuggingService.Resume();
        }
    }
}