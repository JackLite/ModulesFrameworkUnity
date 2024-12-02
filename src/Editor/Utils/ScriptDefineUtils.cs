using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;

namespace ModulesFrameworkUnity
{
    internal class ScriptDefineUtils
    {
        public static void Remove(string define)
        {
            var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget, out var defines);
            var hashSet = defines.ToHashSet();
            if (!hashSet.Contains(define))
                return;

            hashSet.Remove(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, hashSet.ToArray());
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        }

        public static void Add(string define)
        {
            var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget, out var defines);
            var hashSet = defines.ToHashSet();
            if (hashSet.Contains(define))
                return;

            hashSet.Add(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, hashSet.ToArray());
            CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
        }
    }
}
