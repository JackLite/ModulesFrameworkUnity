using System.Collections.Generic;
using ModulesFramework.Modules;
using ModulesFramework.Systems;
using ModulesFrameworkUnity.Debug.UpdateDebugging;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Info;

namespace ModulesFrameworkUnity.Debug
{
    internal struct ModulesPauseDebugData
    {
        public int currentRootIndex;
        public ModuleRunType currentModuleRunType;
        public EcsModule nextCallModule;
        public List<EcsModule> roots;
        public ModuleDebugWrapper currentRoot;
    }
}