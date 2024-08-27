using System;
using ModulesFrameworkUnity.DebugWindow.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class DebugWindowTabs
    {
        public event Action<DebugTabType> SwitchTab;

        public void Draw(VisualElement root)
        {
            var tabs = CreatePanel();

            CreateModulesBtn(tabs);
            CreateOneDataBtn(tabs);
            CreateEntitiesBtn(tabs);
            root.Add(tabs);
        }

        private void CreateModulesBtn(Box tabs)
        {
            var modulesButton = new Button
            {
                text = "Modules"
            };
            modulesButton.clicked += () => SwitchTab?.Invoke(DebugTabType.Modules);
            tabs.Add(modulesButton);
        }

        private void CreateOneDataBtn(Box tabs)
        {
            var oneDataButton = new Button
            {
                text = "One Data"
            };
            oneDataButton.clicked += () => SwitchTab?.Invoke(DebugTabType.OneData);
            tabs.Add(oneDataButton);
        }

        private void CreateEntitiesBtn(Box tabs)
        {
            var entitiesButton = new Button
            {
                text = "Entities"
            };
            entitiesButton.clicked += () => SwitchTab?.Invoke(DebugTabType.Entities);
            tabs.Add(entitiesButton);
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
                    alignSelf = Align.FlexEnd,
                    position = Position.Absolute
                }
            };

            return tabs;
        }
    }
}