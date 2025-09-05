using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Utils;
using ModulesFrameworkUnity.Debug.Utils;
using ModulesFrameworkUnity.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModuleSystemsList : ScrollView
    {
        private Dictionary<Type, List<Type>> _cacheSystems;

        public void Init(Type moduleType)
        {
            var systems = GetSystems(moduleType);
            DrawSystems(systems);
        }

        private ICollection<Type> GetSystems(Type moduleType)
        {
            if (Application.isPlaying)
            {
                var module = DebugUtils.GetCurrentWorld().GetModule(moduleType);
                return module.Systems.ToList();
            }

            _cacheSystems ??= EcsUtilities.FindSystems(new UnityAssemblyFilter().Filter);
            if (_cacheSystems.TryGetValue(moduleType, out var systems))
                return systems;
            return Array.Empty<Type>();
        }

        private void DrawSystems(ICollection<Type> systems)
        {
            if (systems.Count == 0)
            {
                var label = new Label("No systems");
                label.AddToClassList("modules-tab--systems-list--no-systems");
                Add(label);
                return;
            }

            foreach (var system in systems)
            {
                var label = new Label(system.Name);
                Add(label);
            }
        }
    }
}