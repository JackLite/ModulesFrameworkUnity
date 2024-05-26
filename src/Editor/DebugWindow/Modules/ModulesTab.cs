using System;
using ModulesFrameworkUnity.DebugWindow.Modules.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    /// <summary>
    ///     Modules tab of a new debug window
    /// </summary>
    public class ModulesTab
    {
        private ModulesTabModeSwitcher _modeSwitcher;
        private ModulesGraphTab _graphTab;
        private ModulesListTab _listTab;
        private ModulesTabMode _currentMode;
        private VisualElement _root;

        public event Action<ModulesTabMode> OnSwitchMode;

        public ModulesTab(ModulesTabMode mode)
        {
            _currentMode = mode;

            _root = new VisualElement();
            _graphTab = new ModulesGraphTab();
            _root.Add(_graphTab.Root);
            _graphTab.Root.StretchToParentSize();
            _graphTab.Hide();

            _listTab = new ModulesListTab();
            _root.Add(_listTab.Root);
            _listTab.Root.StretchToParentSize();
            _listTab.Hide();
        }

        public void Show(VisualElement root)
        {
            _root ??= new VisualElement();
            root.Add(_root);
            _root.StretchToParentSize();
            _modeSwitcher ??= new ModulesTabModeSwitcher();
            _modeSwitcher.OnSwitchClick += SwitchMode;
            if (_currentMode == ModulesTabMode.Graph)
                _graphTab.Show();
            else
                _listTab.Show();
            _modeSwitcher.Draw(_root, _currentMode);
        }

        public void Hide()
        {
            _graphTab.Hide();
            _listTab.Hide();
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
    }
}