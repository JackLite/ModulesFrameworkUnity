using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Settings block for entity drawer
    /// </summary>
    public class EntityDrawerSettings : VisualElement
    {
        private Toggle _alwaysOpenToggle;

        public bool IsAllOpen { get; private set; }

        public void Draw()
        {
            AddToClassList("modules--entities-tab--entity-drawer-settings");
            _alwaysOpenToggle = new Toggle("Always open components");
            _alwaysOpenToggle.RegisterValueChangedCallback(ev => IsAllOpen = ev.newValue);
            Add(_alwaysOpenToggle);
        }
    }
}