using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ModulesFramework.UnityAdapter.Editor")]

// attribute for Unity stripping, otherwise the [Preserve] attribute doesn't work as expected
[assembly: UnityEngine.Scripting.AlwaysLinkAssembly]