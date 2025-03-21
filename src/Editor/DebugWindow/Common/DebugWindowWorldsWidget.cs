using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class DebugWindowWorldsWidget : DropdownField
    {
        public void Init(IEnumerable<string> worlds, string current)
        {
            choices = worlds.ToList();
            label = "Choose world: ";
            AddToClassList("modules--debug-window--worlds-switcher");
            SetValueWithoutNotify(current);
        }
    }
}
