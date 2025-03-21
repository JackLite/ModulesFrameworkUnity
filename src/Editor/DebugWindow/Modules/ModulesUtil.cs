using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Utils;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    internal class ModulesUtil
    {
        public static bool FilterModule(Type moduleType, string worldName)
        {
            var worldAttribute = moduleType.GetCustomAttribute<WorldBelongingAttribute>();
            return worldAttribute == null || worldAttribute.Worlds.Contains(worldName);
        }

        public static bool FilterModule(Type moduleType, params string[] worldNames)
        {
            var worldAttribute = moduleType.GetCustomAttribute<WorldBelongingAttribute>();
            if (worldAttribute == null)
                return true;

            foreach (var world in worldNames)
            {
                if (worldAttribute.Worlds.Contains(world))
                    return true;
            }

            return false;
        }
    }
}
