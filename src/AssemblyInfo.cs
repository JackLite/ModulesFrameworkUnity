using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ModulesFramework.UnityAdapter.Editor")]
[assembly: InternalsVisibleTo("MF.UnityAdapter.Tests")]

// attribute for Unity stripping, otherwise the [Preserve] attribute doesn't work as expected
[assembly: UnityEngine.Scripting.AlwaysLinkAssembly]