using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModulesFramework;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Exceptions;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Utils;
using UnityEditor;

namespace ModulesFrameworkUnity.Debug.Utils
{
    internal class DebugUtils
    {
        private const string CurrentWorldKey = "Modules.Debug.CurrentWorld";

        public static string GetComponentOpenKey(Type componentType)
        {
            return $"modules-is-component-open-{componentType.Name}";
        }

        public static int GetModuleOrder(Type moduleType)
        {
            if (!MF.IsInitialized)
                return 0;

            var module = DebugUtils.GetCurrentWorld().GetModule(moduleType);
            if (!module.IsSubmodule)
                return 0;
            var parent = module.Parent;

            var submodulesOrder = parent!.GetSubmodulesOrder();
            return submodulesOrder.GetValueOrDefault(moduleType, 0);
        }

        public static HashSet<string> GetAllWorldNames()
        {
            if (MF.IsInitialized)
                return MF.GetAllWorlds().Select(w => w.WorldName).ToHashSet();

            var filter = new UnityAssemblyFilter();
            var worlds = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where filter.Filter(assembly)
                from type in assembly.GetTypes()
                where type.IsSubclassOf(typeof(EcsModule)) && type != typeof(EmbeddedGlobalModule)
                let worldAttribute = type.GetCustomAttribute<WorldBelongingAttribute>()
                where worldAttribute != null
                from world in worldAttribute.Worlds
                select world;

            var worldsSet = new HashSet<string>
            {
                "Default"
            };
            foreach (var world in worlds.OrderBy(w => w))
            {
                worldsSet.Add(world);
            }

            return worldsSet;
        }

        public static void SetCurrentModule(string name)
        {
            EditorPrefs.SetString(CurrentWorldKey, name);
        }

        public static DataWorld GetCurrentWorld()
        {
            var worldName = EditorPrefs.GetString(CurrentWorldKey, "Default");
            try
            {
                return MF.GetWorld(worldName);
            }
            catch (WorldNotFoundException)
            {
                UnityEngine.Debug.LogError($"[Modules.Debug] World {worldName} not found. " +
                                           "Set current world to default");

                EditorPrefs.SetString(CurrentWorldKey, "Default");
                return MF.GetWorld("Default");
            }
        }

        public static string GetCurrentWorldName()
        {
            return EditorPrefs.GetString(CurrentWorldKey, "Default");
        }
    }
}