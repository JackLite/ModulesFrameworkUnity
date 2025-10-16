using System;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug.Utils;
using ModulesFrameworkUnity.DebugWindow.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class DebugWindowTopBar : VisualElement
    {
        private DebugWindowWorldsWidget _worldsWidget;
        private DebugWindowTabs _tabs;
        private VisualElement _pauseWidgetContainer;
        private DebugWindowPauseWidget _pauseWidget;

        public event Action<string> OnWorldChanged;
        public event Action<DebugTabType> OnSwitchTab;

        public void Draw()
        {
            DrawWorldsWidget();
            _pauseWidgetContainer = new VisualElement();
            Add(_pauseWidgetContainer);
            if (MF.IsInitialized)
                DrawPauseWidget();
            DrawTabSwitcher();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        public void Refresh(DataWorld currentWorld)
        {
            _pauseWidget?.Refresh();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
                DrawPauseWidget();
            else if (change == PlayModeStateChange.ExitingPlayMode && _pauseWidget != null)
                _pauseWidget.RemoveFromHierarchy();
        }

        private void DrawPauseWidget()
        {
            _pauseWidget = new DebugWindowPauseWidget(DebugUtils.GetCurrentWorld());
            _pauseWidget.Draw();
            _pauseWidgetContainer.Add(_pauseWidget);
            if (EditorApplication.isPlaying)
                _pauseWidget.Show();
            else
                _pauseWidget.Hide();
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