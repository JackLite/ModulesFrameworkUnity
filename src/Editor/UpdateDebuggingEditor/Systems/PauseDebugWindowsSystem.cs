using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems.Subscribes;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Events;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;

namespace ModulesFrameworkUnity.UpdateDebuggingEditor.Systems
{
    /// <summary>
    ///     Update all opened debug windows when pause event risen
    /// </summary>
    [EcsSystem(typeof(UpdateDebuggingEditorModule))]
    public class PauseDebugWindowsSystem : 
        ISubscribeInitSystem<PauseChangedEvent>,
        ISubscribeInitSystem<UpdateDebuggingModuleChangedSignal>
    {
        private readonly DataWorld _world;

        public void HandleEvent(PauseChangedEvent ev)
        {
            foreach (var window in DebugUtils.GetOpenedDebugWindow(_world))
                window.SetPause(ev.IsPaused);
        }

        public void HandleEvent(UpdateDebuggingModuleChangedSignal ev)
        {
            var data = _world.OneData<ModulesPauseDebugData>();
            foreach (var window in DebugUtils.GetOpenedDebugWindow(_world))
                window.TopBar.PauseWidget.SetModuleRunType(data.currentModuleRunType);
        }
    }
}