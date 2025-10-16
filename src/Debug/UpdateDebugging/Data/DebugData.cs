using System.Collections.Generic;
using ModulesFramework.Modules;
using ModulesFramework.Systems;
using ModulesFrameworkUnity.Debug.UpdateDebugging;

namespace ModulesFrameworkUnity.Debug
{
    /// <summary>
    ///     One data for debug inside editor or MF debug mode
    /// </summary>
    internal struct DebugData
    {
        public bool isPause;
        public ModuleDebugContainer currentRoot;
    }
}