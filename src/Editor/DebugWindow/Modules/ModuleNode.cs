using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModuleNode : Node
    {
        private float _width = 100;

        public float Width => _width;
        public int Level { get; private set; }

        public ModuleNode(int level)
        {
            Level = level;
        }

        public void RefreshWidth()
        {
            _width = 10 * title.Length;
            var inputPortMaxWidth = GetMaxWidth(inputContainer.Children().Where(v => v is Port).Cast<Port>());
            var outputPortMaxWidth = GetMaxWidth(outputContainer.Children().Where(v => v is Port).Cast<Port>());
            _width = Math.Max(_width, inputPortMaxWidth + outputPortMaxWidth);
            style.width = _width;
        }

        private float GetMaxWidth(IEnumerable<Port> ports)
        {
            return ports.Select(port => 10 * port.portName.Length).Prepend(0).Max();
        }
        
    }
}