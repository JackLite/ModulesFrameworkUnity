using System.Reflection;
using ModulesFramework.Utils;
using ModulesFrameworkUnity.Settings;

namespace ModulesFrameworkUnity.Utils
{
    public class UnityAssemblyFilter : AssemblyFilter
    {
        private const char Wildcard = '*';
        private readonly ModulesSettings _settings = ModulesSettings.Load();

        public override bool Filter(Assembly assembly)
        {
            return base.Filter(assembly)
                   && FilterBySettings(assembly)
                   && !assembly.FullName.StartsWith("Unity")
                   && !assembly.FullName.StartsWith("UnityEngine");
        }

        private bool FilterBySettings(Assembly assembly)
        {
            foreach (var filterString in _settings.assemblyFilters)
            {
                var isStartsWithWildcard = filterString.StartsWith(Wildcard);
                var isEndsWithWildcard = filterString.EndsWith(Wildcard);
                if (isStartsWithWildcard && isEndsWithWildcard)
                {
                    if (assembly.FullName.Contains(filterString))
                        return false;
                }
                else if (isStartsWithWildcard)
                {
                    if (assembly.FullName.EndsWith(filterString))
                        return false;
                }

                if (isEndsWithWildcard)
                {
                    if (assembly.FullName.StartsWith(filterString))
                        return false;
                }

                if (assembly.FullName == filterString)
                    return false;
            }

            return true;
        }
    }
}