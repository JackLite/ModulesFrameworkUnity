using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Utils;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging.Info
{
    public class ModuleDebugWrapper
    {
        public EcsModule module;
        public ModuleInternalStep step;
        public Queue<ModuleDebugWrapper> composed;
        public Queue<ModuleDebugWrapper> submodules;

        public ModuleDebugWrapper(EcsModule module)
        {
            this.module = module;
            composed = module.ComposedModules.Select(m => new ModuleDebugWrapper(m)).ToQueue();
            step = composed.Count > 0 ? ModuleInternalStep.Composed : ModuleInternalStep.Self;
            var submodulesOrder = module.GetSubmodulesOrder();
            submodules = module.Submodules
                .OrderBy(s => submodulesOrder.GetValueOrDefault(s.GetType(), 0))
                .Select(m => new ModuleDebugWrapper(m))
                .ToQueue();
        }

        public bool IsFinished()
        {
            return step is ModuleInternalStep.Submodules && submodules.Count == 0;
        }
    }
}