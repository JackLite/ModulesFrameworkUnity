using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModulesFramework;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Debug.Attributes;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Service;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging
{
    [GlobalModule]
    [HideInDebug]
    internal class UpdateDebuggingModule : EcsModule
    {
        private Dictionary<Type, object> _dependencies;

        protected override Task Setup()
        {
            var debugModulesService = new DebugModulesService(world);
            var debugSystemsService = new DebugSystemsService(world);
            var debugService = new UpdateDebuggingService(world, debugModulesService, debugSystemsService);
            _dependencies = new Dictionary<Type, object>
            {
                { typeof(DebugModulesService), debugModulesService },
                { typeof(DebugSystemsService), debugSystemsService },
                { typeof(UpdateDebuggingService), debugService }
            };
            world.Logger.LogDebug("Test1", LogFilter.Full);
            return base.Setup();
        }

        public override Dictionary<Type, object> GetDependencies()
        {
            return _dependencies;
        }

        public void NextModule()
        {
            GetDependency<UpdateDebuggingService>()!.RunNextModule();
        }
        
        public void NextSystem()
        {
            GetDependency<UpdateDebuggingService>()!.RunNextSystem();
        }
    }
}