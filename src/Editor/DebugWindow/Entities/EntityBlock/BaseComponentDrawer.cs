using ModulesFrameworkUnity.Debug.Utils;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Base class for component drawing
    /// </summary>
    public abstract class BaseComponentDrawer
    {
        protected readonly Type _componentType;
        protected int _eid;
        protected Button _pinButton;
        protected readonly VisualElement _componentContainer;
        protected readonly VisualElement _componentControlPanel;
        protected bool isAlwaysOpen;

        public event Action OnPin;

        public BaseComponentDrawer(Type componentType)
        {
            _componentType = componentType;
            isAlwaysOpen = EditorPrefs.GetBool(DebugUtils.GetComponentOpenKey(componentType));

            _componentContainer = new();
            _componentContainer.AddToClassList("modules--entities-tab--component-container");
            _componentControlPanel = new();
            _componentControlPanel.AddToClassList("modules--entities-tab--component-controls");
            AddKeepOpenToggle();
            AddPinBtn();
        }

        private void AddKeepOpenToggle()
        {
            var keepOpenToggle = new Toggle("Keep opened");
            keepOpenToggle.AddToClassList("modules--entities-tab--keep-open-toggle");
            keepOpenToggle.SetValueWithoutNotify(isAlwaysOpen);
            keepOpenToggle.RegisterValueChangedCallback(ev =>
            {
                if (ev.propagationPhase != PropagationPhase.AtTarget)
                    return;

                isAlwaysOpen = ev.newValue;
                EditorPrefs.SetBool(DebugUtils.GetComponentOpenKey(_componentType), isAlwaysOpen);
                OnAlwaysOpenChanged();
            });
            _componentControlPanel.Add(keepOpenToggle);
        }

        private void AddPinBtn()
        {
            _pinButton = new Button();
            _pinButton.AddToClassList("modules--entities-tab--pin-btn");
            _pinButton.clicked += () => OnPin?.Invoke();
            _pinButton.text = "Pin";
            _componentControlPanel.Add(_pinButton);
        }

        public void SetEntityId(int eid)
        {
            _eid = eid;
        }

        public void SetFirst()
        {
            _componentContainer.SendToBack();
        }

        public void SetPinned(bool isPinned)
        {
            _pinButton.text = isPinned ? "Unpin" : "Pin";
        }

        protected abstract void OnAlwaysOpenChanged();
    }
}