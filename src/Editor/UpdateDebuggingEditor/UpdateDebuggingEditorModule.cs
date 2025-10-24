using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Debug.Attributes;

namespace ModulesFrameworkUnity.UpdateDebuggingEditor
{
    [Submodule(typeof(MFEditorModule))]
    [HideInDebug]
    public class UpdateDebuggingEditorModule : EcsModule
    {
        
    }
}