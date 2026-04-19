using System;
using ModulesFramework.Utils.Types;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Modules.GraphView
{
    public class SystemLabel : Label
    {
        public Type SystemType { get; private set; }

        public void SetSystem(Type systemType)
        {
            SystemType = systemType;
            text = SystemType.GetTypeName();
        }

        public void SetHighlight(bool isHighlighted)
        {
            if (isHighlighted)
                AddToClassList("mf--system-label--highlight");
            else
                RemoveFromClassList("mf--system-label--highlight");
        }
    }
}