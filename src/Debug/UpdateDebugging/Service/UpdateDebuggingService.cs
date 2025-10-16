using ModulesFramework.Data;
using ModulesFrameworkUnity.DebugWindow.Service;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging.Service
{
    internal class UpdateDebuggingService
    {
        private readonly DataWorld _world;
        private readonly DebugModulesService _debugModulesService;
        private readonly DebugSystemsService _debugSystemsService;

        public UpdateDebuggingService(DataWorld world, DebugModulesService debugModulesService,
            DebugSystemsService debugSystemsService)
        {
            _world = world;
            _debugModulesService = debugModulesService;
            _debugSystemsService = debugSystemsService;
        }

        public void InitDebugging()
        {
            _debugModulesService.CreateData();
            _debugSystemsService.CreateDataFor(_world.EmbeddedGlobalModule, ModuleRunType.Run);
        }

        public void Resume()
        {
            // proceed all remain systems
            var moduleRunType = _world.OneData<ModulesPauseDebugData>().currentModuleRunType;
            _debugSystemsService.ProceedAllRemains(moduleRunType);
            // proceed all remain modules
            _debugModulesService.ProceedAllRemains();
        }

        public void RunNextModule()
        {
            _debugModulesService.RunNextModule();
            if (_debugModulesService.IsEmpty())
                _debugModulesService.SwitchToNextRunType();

            ReCreateSystemsData();
        }

        public void RunNextSystem()
        {
            if (_debugSystemsService.IsEmpty())
            {
                _debugModulesService.SwitchToNextModule();
                ReCreateSystemsData();
                return;
            }

            var moduleRunType = _world.OneData<ModulesPauseDebugData>().currentModuleRunType;
            _debugSystemsService.RunNextSystem(moduleRunType);
            if (_debugSystemsService.IsEmpty())
            {
                _debugModulesService.SwitchToNextModule();
                ReCreateSystemsData();
            }
        }

        private void ReCreateSystemsData()
        {
            var data = _world.OneData<ModulesPauseDebugData>();
            var nextModule = data.nextCallModule;
            _debugSystemsService.CreateDataFor(nextModule, data.currentModuleRunType);
        }
    }
}