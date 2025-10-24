using System;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Common.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;
using ModulesFrameworkUnity.UpdateDebuggingEditor.View;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Common
{
    public class DebugWindowTopBar : VisualElement
    {
        private DebugWindowWorldsWidget _worldsWidget;
        private DebugWindowTabs _tabs;
        private VisualElement _pauseWidgetContainer;
        public DebugWindowPauseWidget PauseWidget { get; private set; }

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
            PauseWidget?.SetWorld(currentWorld);
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
                DrawPauseWidget();
            else if (change == PlayModeStateChange.ExitingPlayMode && PauseWidget != null)
                PauseWidget.RemoveFromHierarchy();
        }

        private void DrawPauseWidget()
        {
            PauseWidget = new DebugWindowPauseWidget(DebugUtils.GetCurrentWorld());
            PauseWidget.Draw();
            _pauseWidgetContainer.Add(PauseWidget);
            if (EditorApplication.isPlaying)
                PauseWidget.Show();
            else
                PauseWidget.Hide();
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

        public void SetPause(bool isPause)
        {
            PauseWidget.SetPause(isPause);
        }
    }
}