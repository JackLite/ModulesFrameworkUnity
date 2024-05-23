using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModulesGraphView : GraphView
    {
        private readonly Dictionary<Type, ModuleNode> _nodes = new();
        private readonly SortedDictionary<int, float> _levelWidths = new();
        private Dictionary<int, float> _rowY = new();
        
        public IReadOnlyCollection<ModuleNode> Nodes => _nodes.Values;

        public ModulesGraphView()
        {
            var styleSheet = Resources.Load<StyleSheet>("ModulesGraphUSS");
            styleSheets.Add(styleSheet);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        }

        public ModuleNode AddModule(Type module, int level)
        {
            if (_nodes.TryGetValue(module, out var createdNode))
                return createdNode;
            var node = new ModuleNode(level, module)
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
            _rowY = _nodes.Values.Select(n => n.Level).Distinct().ToDictionary(l => l, _ => 0f);
            foreach (var node in _nodes.Values.OrderByDescending(n => n.Level))
            {
                node.RefreshAdjustedHeight();
            }

            AlignNodesFirstStep();
            AlignNodesSecondStep();

            foreach (var node in _nodes.Values.OrderByDescending(n => n.Level))
            {
                _levelWidths.TryAdd(node.Level, node.Width);
                _levelWidths[node.Level] = Math.Max(_levelWidths[node.Level], node.Width);
            }

            foreach (var node in _nodes.Values)
            {
                var x = SumLevelsWidth(node.Level) + 100 * node.Level;
                node.SetX(x);
            }
        }

        private void AlignNodesFirstStep()
        {
            var roots = _nodes.Values.Where(n => n.Level == 0)
                .OrderBy(n => n.SubmodulesCount)
                .ThenBy(n => n.ModuleType.Name);
            foreach (var node in roots)
            {
                node.SetY(_rowY[0]);
                node.AlignChildren(_rowY[0], ref _rowY);
                _rowY[0] += node.SelfHeight;
                _rowY[0] = _rowY.Values.Max();
            }
        }

        private void AlignNodesSecondStep()
        {
            foreach (var node in _nodes.Values.OrderByDescending(n => n.Level).ThenBy(n => n.ModuleType.Name))
            {
                var diff = node.CalculateChildrenOffset();
                if (node.ParentModule == null)
                    node.AddY(diff);
                else
                    node.ParentModule.OffsetChildrenY(diff, node);
            }
        }

        private float SumLevelsWidth(int targetLevel)
        {
            var res = 0f;
            foreach (var (lvl, width) in _levelWidths)
            {
                if (lvl >= targetLevel)
                    break;

                res += width;
            }

            return res;
        }
    }
}