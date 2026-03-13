using System;
using ModulesFrameworkUnity.DebugWindowFeature.Common.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Common
{
    public class DebugWindowTopBar : VisualElement
    {
        private DebugWindowWorldsWidget _worldsWidget;
        private DebugWindowTabs _tabs;
        private VisualElement _pauseWidgetContainer;

        public event Action<string> OnWorldChanged;
        public event Action<DebugTabType> OnSwitchTab;

        public void Draw()
        {
            DrawWorldsWidget();
            _pauseWidgetContainer = new VisualElement();
            Add(_pauseWidgetContainer);
            DrawTabSwitcher();
        }

        private void DrawWorldsWidget()
        {
            _worldsWidget ??= new DebugWindowWorldsWidget();
            var allWorlds = DebugUtils.GetAllWorldNames();
            _worldsWidget.Init(allWorlds, DebugUtils.GetCurrentWorldName());
            _worldsWidget.RegisterValueChangedCallback(ev =>
            {
                DebugUtils.SetCurrentModule(ev.newValue);
                OnWorldChanged?.Invoke(ev.newValue);
            });
            Add(_worldsWidget);
        }

        private void DrawTabSwitcher()
        {
            _tabs ??= new DebugWindowTabs();
            _tabs.Draw(this);
            _tabs.SwitchTab += t => OnSwitchTab?.Invoke(t);
        }
    }
}