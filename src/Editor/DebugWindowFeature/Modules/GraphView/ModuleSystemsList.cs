using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Systems;
using ModulesFramework.Utils;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;
using ModulesFrameworkUnity.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Modules.GraphView
{
    public class ModuleSystemsList : ScrollView
    {
        private Dictionary<Type, List<Type>> _cacheSystems;
        private readonly List<SystemLabel> _labels = new();

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
                return module.SystemTypes.ToList();
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

            foreach (var systemType in systems)
            {
                var label = new SystemLabel();
                label.SetSystem(systemType);
                _labels.Add(label);
                Add(label);
            }
        }

        public void Reset()
        {
            _labels.Clear();
            Clear();
        }

        public void Highlight(Type systemType)
        {
            foreach (var systemLabel in _labels)
                systemLabel.SetHighlight(systemLabel.SystemType == systemType);
        }
    }
}