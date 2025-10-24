using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems.Subscribes;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Events;
using ModulesFrameworkUnity.DebugWindowFeature.Common.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;

namespace ModulesFrameworkUnity.DebugWindowFeature.Modules.Systems
{
    [EcsSystem(typeof(MFEditorModule))]
    public class ModulesHighlightSystem :
        ISubscribeInitSystem<UpdateDebuggingModuleChangedSignal>,
        ISubscribeInitSystem<UpdateDebuggingSystemChangedSignal>,
        ISubscribeInitSystem<PauseChangedEvent>
    {
        private readonly DataWorld _world;

        public void HandleEvent(UpdateDebuggingModuleChangedSignal ev)
        {
            ref var modulesData = ref _world.OneData<ModulesPauseDebugData>();
            foreach (var window in DebugUtils.GetOpenedDebugWindow(_world))
            {
                window.ModulesTab.SetNextRunModule(modulesData.nextCallModule);
            }
        }

        public void HandleEvent(UpdateDebuggingSystemChangedSignal ev)
        {
            ref var systemsData = ref _world.OneData<SystemsPauseDebugData>();
            foreach (var window in DebugUtils.GetOpenedDebugWindow(_world))
            {
                if (systemsData.nextSystem != null)
                    window.ModulesTab.SetNextRunSystem(systemsData.nextSystem.GetType());
            }
        }

        public void HandleEvent(PauseChangedEvent ev)
        {
            foreach (var window in DebugUtils.GetOpenedDebugWindow(_world))
            {
                if (!ev.IsPaused)
                {
                    window.ModulesTab.ResetHighlighting();
                }
            }
        }
    }
}