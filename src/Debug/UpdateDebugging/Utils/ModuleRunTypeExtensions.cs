using System;

namespace ModulesFrameworkUnity.Debug
{
    internal static class ModuleRunTypeExtensions
    {
        public static ModuleRunType Next(this ModuleRunType type)
        {
            var nextIndex = (int)type + 1;
            if (nextIndex >= Enum.GetValues(typeof(ModuleRunType)).Length)
                nextIndex = 0;

            return (ModuleRunType)nextIndex;
        }
    }
}