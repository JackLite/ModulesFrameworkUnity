using ModulesFrameworkUnity.DebugWindowFeature.Modules.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Modules
{
    /// <summary>
    ///     Modules tab of a new debug window
    /// </summary>
    public class ModulesTab : VisualElement
    {
        private readonly ModulesGraphTab _graphTab;

        public ModulesTab()
        {
            var styleSheet = Resources.Load<StyleSheet>("ModulesTab");
            styleSheets.Add(styleSheet);
            
            _graphTab = new ModulesGraphTab();
            Add(_graphTab.Root);
            _graphTab.Root.StretchToParentSize();
            _graphTab.Hide();

            var topBar = new VisualElement();
            topBar.AddToClassList("modules-tab--top-bar");
            Add(topBar);
        }

        public void Show()
        {
            this.StretchToParentSize();

            Refresh();
            style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _graphTab.Hide();
            style.display = DisplayStyle.None;
        }

        public void Refresh()
        {
            _graphTab.Show();
        }
    }
}
