using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModulesFramework;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFramework.Utils;
using ModulesFrameworkUnity.Debug.Utils;
using ModulesFrameworkUnity.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModulesListTab
    {
        private readonly List<ModulesListElement> _elements = new List<ModulesListElement>();
        public ScrollView Root { get; } = new();

        public void Show()
        {
            DrawList();
            EditorApplication.playModeStateChanged += OnPlayModeChanges;
            if (Application.isPlaying)
                UpdateForPlayMode();
            Root.visible = true;
        }

        private void DrawList()
        {
            Root.Clear();
            var filter = new UnityAssemblyFilter();
            var modules = AppDomain.CurrentDomain.GetAssemblies().Where(filter.Filter)
                .SelectMany(a => a.GetTypes()
                    .Where(t => t != typeof(EmbeddedGlobalModule) && ModuleUtil.GetWorldIndex(t).Contains(0))
                    .Where(t => t.IsSubclassOf(typeof(EcsModule)) && !t.IsAbstract)
                )
                .OrderBy(t => t.Name)
                .ToArray();

            var modulesRoot = new VisualElement
            {
                style =
                {
                    marginTop = 10
                }
            };

            var elements = new Dictionary<Type, ModulesListElement>();
            foreach (var moduleType in modules)
            {
                var element = CreateModuleElement();
                element.SetModule(moduleType);
                elements[moduleType] = element;

                if (moduleType.GetCustomAttribute<SubmoduleAttribute>() != null)
                    continue;

                modulesRoot.Add(element);
            }

            var orderedModules = modules.OrderBy(DebugUtils.GetModuleOrder).ThenBy(t => t.Name);
            foreach (var moduleType in orderedModules)
            {
                if (moduleType.GetCustomAttribute<SubmoduleAttribute>() == null)
                    continue;

                var parent = moduleType.GetCustomAttribute<SubmoduleAttribute>().parent;
                var parentElement = elements[parent];
                var childElement = elements[moduleType];
                parentElement.AddChild(childElement);
            }

            foreach (var element in elements.Values)
            {
                element.FinalizeView();
            }
            _elements.AddRange(elements.Values);

            Root.Add(modulesRoot);
        }

        private void OnPlayModeChanges(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                DrawList();
                UpdateForPlayMode();
            }
            else if (change == PlayModeStateChange.ExitingPlayMode)
            {
                DrawList();
            }
        }

        private void UpdateForPlayMode()
        {
            foreach (var element in _elements)
            {
                element.SetPlayMode();
                var module = MF.World.GetModule(element.ModuleType);
                module.OnInitialized += element.OnModuleInit;
                module.OnActivated += element.OnModuleActivated;
                module.OnDeactivated += element.OnModuleDeactivated;
                module.OnDestroyed += element.OnModuleDestroyed;
                if (module.IsInitialized)
                    element.OnModuleInit();
                if (module.IsActive)
                    element.OnModuleActivated();
            }
        }

        private ModulesListElement CreateModuleElement()
        {
            var element = new ModulesListElement();
            return element;
        }

        public void Hide()
        {
            Root.Clear();
            Root.visible = false;
        }
    }
}