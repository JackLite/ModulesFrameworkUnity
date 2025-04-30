using System;
using ModulesFrameworkUnity.Debug.Events;
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
            CreateEventBtn(tabs);
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

        private void CreateEventBtn(Box tabs)
        {
            var eventButton = new Button
            {
                text = "RiseEvent"
            };
            eventButton.AddToClassList("modules--debug-window--rise-event-btn");
            eventButton.clicked += () =>
            {
                if (!Application.isPlaying)
                    return;
                var window = ScriptableObject.CreateInstance<RiseEventWindow>();
                window.ShowWindow();
            };
            tabs.Add(eventButton);
        }

        private static Box CreatePanel()
        {
            var tabs = new Box();
            tabs.AddToClassList("modules--debug-window--tabs-panel");

            return tabs;
        }
    }
}