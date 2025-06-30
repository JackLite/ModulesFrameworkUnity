using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModulesFramework.Utils;

namespace ModulesFrameworkUnity.Utils
{
    public static class AssemblyUtils
    {
        private static readonly AssemblyFilter _filter = new UnityAssemblyFilter();

        public static IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(_filter.Filter);
        }
    }
}