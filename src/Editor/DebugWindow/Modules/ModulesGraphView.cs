using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModulesGraphView : GraphView
    {
        private readonly Dictionary<Type, ModuleNode> _nodes = new();
        private readonly SortedDictionary<int, float> _levelWidths = new();
        private readonly Dictionary<Type, float> _rowHeights = new();

        public ModulesGraphView()
        {
            var styleSheet = Resources.Load<StyleSheet>("ModulesGraphUSS");
            styleSheets.Add(styleSheet);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentZoomer());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        }

        public ModuleNode AddModule(Type module, int level)
        {
            if (_nodes.TryGetValue(module, out var createdNode))
                return createdNode;
            var node = new ModuleNode(level)
            {
                title = module.Name
            };
            node.RefreshExpandedState();
            node.RefreshPorts();
            AddElement(node);
            _nodes.Add(module, node);
            return node;
        }

        public ModuleNode GetNode(Type module)
        {
            return _nodes[module];
        }
        
        public void RefreshNodesPositions()
        {
            // first step - calculate max width at every level
            foreach (var node in _nodes.Values)
            {
                _levelWidths.TryAdd(node.Level, node.Width);
                _levelWidths[node.Level] = Math.Max(_levelWidths[node.Level], node.Width);
            }
            
            // second step - calculate max height for every parent module
            
            // third step - place parent modules based on max height starting from min
            // fourth step - place submodules at their level based on their count for parent
            foreach (var node in _nodes.Values)
            {
                var x = SumLevelsWidth(node.Level) + 30 * node.Level;
                node.SetPosition(new Rect(x, 0, 0, 0));
            }
        }
        
        private float SumLevelsWidth(int targetLevel)
        {
            var res = 0f;
            foreach (var (lvl, width) in _levelWidths)
            {
                if(lvl >= targetLevel)
                    break;
                
                res += width;
            }
            
            return res;
        }
    }
}