using System;
using ModulesFrameworkUnity.DebugWindow.Common;
using ModulesFrameworkUnity.DebugWindow.Modules.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    /// <summary>
    ///     Modules tab of a new debug window
    /// </summary>
    public class ModulesTab : VisualElement
    {
        private readonly ModulesTabModeSwitcher _modeSwitcher;
        private readonly ModulesGraphTab _graphTab;
        private readonly ModulesListTab _listTab;
        private ModulesTabMode _currentMode;

        public event Action<ModulesTabMode> OnSwitchMode;

        public ModulesTab(ModulesTabMode mode)
        {
            var styleSheet = Resources.Load<StyleSheet>("ModulesTabUSS");
            styleSheets.Add(styleSheet);
            _currentMode = mode;

            _graphTab = new ModulesGraphTab();
            Add(_graphTab.Root);
            _graphTab.Root.StretchToParentSize();
            _graphTab.Hide();

            _listTab = new ModulesListTab();
            Add(_listTab.Root);
            _listTab.Root.AddToClassList("modules-tab--list");
            _listTab.Hide();

            _modeSwitcher = new ModulesTabModeSwitcher();
            _modeSwitcher.OnSwitchClick += SwitchMode;
            _modeSwitcher.Draw(this, _currentMode);
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
            _listTab.Hide();
            _modeSwitcher.Hide();
            style.display = DisplayStyle.None;
        }

        private void SwitchMode()
        {
            if (_currentMode == ModulesTabMode.Graph)
            {
                _currentMode = ModulesTabMode.List;
                _graphTab.Hide();
                _listTab.Show();
            }
            else
            {
                _currentMode = ModulesTabMode.Graph;
                _listTab.Hide();
                _graphTab.Show();
            }

            _modeSwitcher.UpdateMode(_currentMode);
            OnSwitchMode?.Invoke(_currentMode);
        }

        public void Refresh()
        {
            if (_currentMode == ModulesTabMode.Graph)
                _graphTab.Show();
            else
                _listTab.Show();
            _modeSwitcher.Show(_currentMode);
        }
    }
}
