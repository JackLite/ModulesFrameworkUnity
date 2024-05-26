using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModulesNodeTitle
    {
        private readonly VisualElement _buttonsContainer;
        private readonly Button _initDestroyBtn = new Button();
        private readonly Button _activateBtn = new Button();
        private readonly VisualElement _buttonsPanel;

        public event Action OnInitDestroyClick;
        public event Action OnActivateClick;

        public ModulesNodeTitle(VisualElement buttonsContainer)
        {
            _buttonsContainer = buttonsContainer;
            _buttonsPanel = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceAround
                }
            };
            _initDestroyBtn.text = "Init";
            _activateBtn.text = "Activate";
            _buttonsPanel.Add(_initDestroyBtn);
            _buttonsPanel.Add(_activateBtn);
            _activateBtn.style.display = DisplayStyle.None;
            _initDestroyBtn.clicked += () => OnInitDestroyClick?.Invoke();
            _activateBtn.clicked += () => OnActivateClick?.Invoke();
        }

        public void ShowButtons()
        {
            _buttonsContainer.Add(_buttonsPanel);
        }

        public void UpdateInit(bool isInitialized)
        {
            if (isInitialized)
            {
                _initDestroyBtn.text = "Destroy";
                _activateBtn.style.display = DisplayStyle.Flex;
            }
            else
            {
                _initDestroyBtn.text = "Init";
                _activateBtn.style.display = DisplayStyle.None;
            }
        }

        public void UpdateActivate(bool isActive)
        {
            _activateBtn.text = isActive ? "Deactivate" : "Activate";
        }
    }
}