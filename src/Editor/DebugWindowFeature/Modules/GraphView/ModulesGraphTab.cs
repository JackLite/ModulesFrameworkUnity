using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Debug.Attributes;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;
using ModulesFrameworkUnity.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Modules.GraphView
{
    /// <summary>
    ///     View for modules visualized as graph
    /// </summary>
    public class ModulesGraphTab
    {
        private ModulesGraphView _graph;
        private readonly VisualElement _root = new();
        private readonly ModuleSystemsList _systemsList;

        private const string PositionSaveKey = "MF.Debug.ModulesTab.GraphPosition";
        private const string ScaleSaveKey = "MF.Debug.ModulesTab.GraphScale";

        public VisualElement Root => _root;

        public ModulesGraphTab()
        {
            _root.AddToClassList("modules-tab--graph-root");
            var styleSheet = Resources.Load<StyleSheet>("ModulesGraph");
            _root.styleSheets.Add(styleSheet);

            _systemsList = new();
            _root.Add(_systemsList);
        }

        public void Show()
        {
            DrawModules();
            EditorApplication.playModeStateChanged += OnPlayModeChanges;
            if (Application.isPlaying)
                UpdateForPlayMode();
        }

        public void Hide()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanges;
            _root.Clear();
        }

        private void OnPlayModeChanges(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                DrawModules();
                UpdateForPlayMode();
            }
            else if (change == PlayModeStateChange.ExitingPlayMode)
            {
                DrawModules();
            }
        }

        private void UpdateForPlayMode()
        {
            foreach (var node in _graph.Nodes)
            {
                node.SetPlayMode();
                var module = DebugUtils.GetCurrentWorld().GetModule(node.ModuleType);
                module.OnInitialized += node.OnModuleInit;
                module.OnActivated += node.OnModuleActivated;
                module.OnDeactivated += node.OnModuleDeactivated;
                module.OnDestroyed += node.OnModuleDestroyed;
                if (module.IsInitialized)
                    node.OnModuleInit();
                if (module.IsActive)
                    node.OnModuleActivated();
            }
        }

        private void DrawModules()
        {
            _root.Clear();
            var currentWorld = DebugUtils.GetCurrentWorldName();
            var modules = AssemblyUtils.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                    .Where(t => t != typeof(EmbeddedGlobalModule) && ModulesUtil.FilterModule(t, currentWorld))
                    .Where(t => t.IsSubclassOf(typeof(EcsModule)) && !t.IsAbstract)
                    .Where(t => t.GetCustomAttribute<HideInDebugAttribute>() == null)
                ).ToList();

            CreateGraph();

            var composedOfModules = new Dictionary<Type, List<Type>>();

            // collect composed modules
            foreach (var module in modules)
            {
                var tempInstance = (EcsModule)Activator.CreateInstance(module);
                var composedModules = tempInstance.ComposedOf.ToList();
                if (composedModules.Count > 0)
                    composedOfModules.Add(module, composedModules);
            }

            modules.RemoveAll(m => composedOfModules.Values.SelectMany(m => m).Contains(m));

            // first, create usual modules because they may not have submodules
            foreach (var module in modules)
            {
                if (module.GetCustomAttribute<SubmoduleAttribute>() != null)
                    continue;
                var node = _graph.AddModule(module, 0);
                if (composedOfModules.TryGetValue(module, out var composedModules))
                {
                    foreach (var composedModule in composedModules)
                        node.AddComposed(composedModule);
                }

                node.RefreshWidth();
            }

            // find all submodules
            // go through hierarchy
            // create nodes recursively and add point
            var orderedModules = modules.OrderBy(DebugUtils.GetModuleOrder).ThenBy(t => t.Name);
            foreach (var module in orderedModules)
            {
                var submoduleAttr = module.GetCustomAttribute<SubmoduleAttribute>();
                if (submoduleAttr == null)
                    continue;
                var parent = submoduleAttr.parent;
                var parentNode = _graph.AddModule(parent, CalculateLvl(parent));
                var parentPort = parentNode.InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Multi,
                    typeof(float));
                parentPort.portName = module.Name;
                parentNode.outputContainer.Add(parentPort);

                var childNode = _graph.AddModule(module, CalculateLvl(module));
                var childPort = childNode.InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Input,
                    Port.Capacity.Single,
                    typeof(float));
                childPort.portName = parent.Name;
                childNode.inputContainer.Add(childPort);

                parentNode.AddChild(childNode);

                var edge = childPort.ConnectTo(parentPort);
                childNode.Add(edge);
                parentNode.RefreshWidth();
                parentNode.RefreshPorts();
                childNode.RefreshPorts();
            }

            _graph.RefreshNodesPositions();
            _root.Add(_graph);
            _root.Add(_systemsList);
        }
        private void CreateGraph()
        {
            _graph = new ModulesGraphView();
            _graph.OnModuleSelected += OnModuleSelected;
            _graph.OnModuleUnselected += OnModuleUnselected;
            _graph.StretchToParentSize();
            _graph.viewTransformChanged += view =>
            {
                var position = view.viewTransform.matrix.GetPosition();
                var scale = view.viewTransform.matrix.lossyScale;
                var serializedPosition = EditorJsonUtility.ToJson(position);
                var serializedScale = EditorJsonUtility.ToJson(scale);
                EditorPrefs.SetString(PositionSaveKey, serializedPosition);
                EditorPrefs.SetString(ScaleSaveKey, serializedScale);
            };

            var position = RestoreSavedVector3(PositionSaveKey, Vector3.zero);
            var scale = RestoreSavedVector3(ScaleSaveKey, Vector3.one);
            _graph.UpdateViewTransform(position, scale);
        }


        private void OnModuleSelected(Type moduleType)
        {
            _systemsList.Init(moduleType);
        }

        private void OnModuleUnselected(Type moduleType)
        {
            _systemsList.Reset();
        }

        private int CalculateLvl(Type module, int startLevel = 0)
        {
            var infinitySafe = 1000;
            while (true)
            {
                infinitySafe--;
                if (infinitySafe <= 0)
                    throw new Exception("Recursive modules detected");

                var submoduleAttr = module.GetCustomAttribute<SubmoduleAttribute>();
                if (submoduleAttr == null)
                    return startLevel;

                var parent = submoduleAttr.parent;
                module = parent;
                startLevel += 1;
            }
        }

        public void HighlightModule(EcsModule module)
        {
            if (_graph == null)
                DrawModules();

            foreach (var moduleNode in _graph.Nodes)
            {
                var isHighlight = moduleNode.ModuleType == module.GetType();
                isHighlight |= moduleNode.ComposedModulesLabels.ContainsKey(module.GetType());
                moduleNode.SetHighlight(isHighlight);

                foreach (var (type, label) in moduleNode.ComposedModulesLabels)
                {
                    if (type == module.GetType())
                        label.AddToClassList("mf--module-node--highlight-composed-text");
                    else
                        label.RemoveFromClassList("mf--module-node--highlight-composed-text");
                }
            }

            _systemsList.Reset();
            _systemsList.Init(module.GetType());
        }

        public void ResetHighlighting()
        {
            foreach (var moduleNode in _graph.Nodes)
                moduleNode.SetHighlight(false);

            _systemsList.Reset();
        }

        public void HighlightSystem(Type systemType)
        {
            _systemsList.Highlight(systemType);
        }

        private static Vector3 RestoreSavedVector3(string key, Vector3 defaultValue)
        {
            if (!EditorPrefs.HasKey(key))
                return defaultValue;

            var result = defaultValue;
            var serializedPosition = EditorPrefs.GetString(key);
            try
            {
                result = JsonUtility.FromJson<Vector3>(serializedPosition);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }

            return result;
        }
    }
}
