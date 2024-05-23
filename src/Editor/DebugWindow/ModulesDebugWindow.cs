using System;
using System.Linq;
using System.Reflection;
using ModulesFramework;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFramework.Utils;
using ModulesFrameworkUnity.DebugWindow.Modules;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class ModulesDebugWindow : EditorWindow
    {
        private ModulesDebugWindowTabs _tabs;
        private bool _initialized;
        private ModulesGraphView _graph;

        [MenuItem("Modules/Data Viewer")]
        private static void ShowWindow()
        {
            var window = GetWindow<ModulesDebugWindow>();
            window.titleContent = new GUIContent("MF Data Viewer");
            window.Init();
            window.Show();
        }

        private void Init()
        {
            _tabs = new ModulesDebugWindowTabs();
        }

        private void OnEnable()
        {
            DrawModules();
            EditorApplication.playModeStateChanged += OnPlayModeChanges;
            if (Application.isPlaying)
                UpdateForPlayMode();
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
                // add activate/deactivate btns to nodes
                node.SetPlayMode();
                var module = MF.Instance.MainWorld.GetModule(node.ModuleType);
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
            rootVisualElement.Clear();
            var modules = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                    .Where(t => t != typeof(EmbeddedGlobalModule) && ModuleUtil.GetWorldIndex(t).Contains(0))
                    .Where(t => t.IsSubclassOf(typeof(EcsModule)) && !t.IsAbstract)
                ).ToArray();

            _graph = new ModulesGraphView();
            _graph.StretchToParentSize();

            // first create usual modules cause they may not have submodules
            foreach (var module in modules)
            {
                if (module.GetCustomAttribute<SubmoduleAttribute>() != null)
                    continue;
                _graph.AddModule(module, 0).RefreshWidth();
            }

            // find all submodules
            // go through hierarchy
            // create nodes recursively and add point
            foreach (var module in modules.OrderBy(t => t.Name))
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
            rootVisualElement.Add(_graph);
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

        private void OnDisable()
        {
            rootVisualElement.Clear();
        }
    }
}