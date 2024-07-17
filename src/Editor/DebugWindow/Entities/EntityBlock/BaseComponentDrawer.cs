using System;
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

        public event Action OnPin;

        public BaseComponentDrawer(Type componentType)
        {
            _componentType = componentType;

            _componentContainer = new();
            _componentContainer.AddToClassList("modules--entities-tab--component-container");
            AddPinBtn();
        }

        private void AddPinBtn()
        {
            _pinButton = new Button();
            _pinButton.AddToClassList("modules--entities-tab--pin-btn");
            _pinButton.clicked += () => OnPin?.Invoke();
            _pinButton.text = "Pin";
            _componentContainer.Add(_pinButton);
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
    }
}