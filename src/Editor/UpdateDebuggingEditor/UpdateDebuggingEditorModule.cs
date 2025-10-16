using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.DebugWindow.Attributes;

namespace ModulesFrameworkUnity.UpdateDebuggingEditor
{
    [Submodule(typeof(MFEditorModule))]
    [HideInDebug]
    public class UpdateDebuggingEditorModule : EcsModule
    {
        
    }
}