using System.Reflection;
using ModulesFramework.Utils;

namespace ModulesFrameworkUnity.Utils
{
    public class UnityAssemblyFilter : AssemblyFilter
    {
        public override bool Filter(Assembly assembly)
        {
            return base.Filter(assembly)
                && !assembly.FullName.StartsWith("Unity")
                && !assembly.FullName.StartsWith("UnityEngine");
        }
    }
}