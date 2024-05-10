using System;
using System.Linq;
using System.Reflection;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
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

        // private void OnGUI()
        // {
        //     if (!_initialized)
        //         Init();
        //
        //     _tabs.Draw();
        // }

        private void OnEnable()
        {
            var modules = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                    .Where(t => t != typeof(EmbeddedGlobalModule))
                    .Where(t => t.IsSubclassOf(typeof(EcsModule)) && !t.IsAbstract)
                ).ToArray();

            var graph = new ModulesGraphView();
            graph.StretchToParentSize();


            // first create usual modules cause they may not have submodules
            foreach (var module in modules)
            {
                if (module.GetCustomAttribute<SubmoduleAttribute>() != null)
                    continue;
                graph.AddModule(module, 0).RefreshWidth();
            }

            // find all submodules
            // go through hierarchy
            // create nodes recursively and add point
            foreach (var module in modules)
            {
                var submoduleAttr = module.GetCustomAttribute<SubmoduleAttribute>();
                if (submoduleAttr == null)
                    continue;
                var parent = submoduleAttr.parent;
                var parentNode = graph.AddModule(parent, CalculateLvl(module));
                var parentPort = parentNode.InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Output,
                    Port.Capacity.Multi,
                    typeof(float));
                parentPort.portName = module.Name;
                parentNode.outputContainer.Add(parentPort);

                var childNode = graph.AddModule(module, CalculateLvl(module));
                var childPort = childNode.InstantiatePort(
                    Orientation.Horizontal,
                    Direction.Input,
                    Port.Capacity.Single,
                    typeof(float));
                childPort.portName = parent.Name;
                childNode.inputContainer.Add(childPort);

                var edge = childPort.ConnectTo(parentPort);
                childNode.Add(edge);
                parentNode.RefreshWidth();
                parentNode.RefreshPorts();
                childNode.RefreshPorts();
            }
            graph.RefreshNodesPositions();
            rootVisualElement.Add(graph);
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