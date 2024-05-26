using ModulesFrameworkUnity.DebugWindow.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class DebugWindowTabs
    {
        private DebugTabType _current;

        public DebugTabType CurrentTab => _current;

        public void Draw(VisualElement root)
        {
            var tabs = CreatePanel();

            var modulesButton = new Button
            {
                text = "Modules"
            };
            var oneDataButton = new Button
            {
                text = "One Data"
            };
            var entitiesButton = new Button
            {
                text = "Entities"
            };
            tabs.Add(modulesButton);
            tabs.Add(oneDataButton);
            tabs.Add(entitiesButton);
            root.Add(tabs);
        }

        private static Box CreatePanel()
        {
            var tabs = new Box
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginTop = 5,
                    marginRight = 5,
                    paddingBottom = 5,
                    paddingTop = 5,
                    paddingLeft = 5,
                    paddingRight = 5,
                    backgroundColor = new StyleColor(new Color32(100, 100, 100, 255)),
                    width = 250,
                    justifyContent = Justify.SpaceAround,
                    alignSelf = Align.FlexEnd
                }
            };
            return tabs;
        }
    }
}