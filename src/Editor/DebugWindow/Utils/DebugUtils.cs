using System;

namespace ModulesFrameworkUnity.Debug.Utils
{
    internal class DebugUtils
    {
        public static string GetComponentOpenKey(Type componentType)
        {
            return $"modules-is-component-open-{componentType.Name}";
        }
    }
}