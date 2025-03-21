using System;
using ModulesFrameworkUnity.DebugWindow.Modules.Data;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    /// <summary>
    ///     Visual of switcher for tab of modules
    /// </summary>
    public class ModulesTabModeSwitcher
    {
        private ModulesTabMode _currentMode;
        private Button _switchButton;

        public event Action OnSwitchClick;

        public void Draw(VisualElement root, ModulesTabMode mode)
        {
            _currentMode = mode;

            _switchButton = new Button
            {
                text = GetButtonMode(),
                style =
                {
                    alignSelf = Align.FlexEnd,
                    marginTop = 50,
                    marginLeft = 10
                }
            };
            _switchButton.AddToClassList("modules-tab--mode-switcher");
            _switchButton.clicked += () => OnSwitchClick?.Invoke();
            root.Add(_switchButton);
        }

        public void UpdateMode(ModulesTabMode mode)
        {
            _currentMode = mode;
            _switchButton.text = GetButtonMode();
        }

        private string GetButtonMode()
        {
            if (_currentMode == ModulesTabMode.Graph)
                return "Switch to List";
            return "Switch to Graph";
        }

        public void Show(ModulesTabMode currentMode)
        {
            UpdateMode(currentMode);
            _switchButton.visible = true;
        }

        public void Hide()
        {
            _switchButton.visible = false;
        }
    }
}
