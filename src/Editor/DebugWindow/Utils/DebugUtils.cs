using System;
using System.Collections.Generic;
using ModulesFramework;

namespace ModulesFrameworkUnity.Debug.Utils
{
    internal class DebugUtils
    {
        public static string GetComponentOpenKey(Type componentType)
        {
            return $"modules-is-component-open-{componentType.Name}";
        }

        public static int GetModuleOrder(Type moduleType)
        {
            if (!MF.IsInitialized)
                return 0;

            var module = MF.World.GetModule(moduleType);
            if (!module.IsSubmodule)
                return 0;
            var parent = module.Parent;

            var submodulesOrder = parent!.GetSubmodulesOrder();
            return submodulesOrder.GetValueOrDefault(moduleType, 0);
        }
    }
}