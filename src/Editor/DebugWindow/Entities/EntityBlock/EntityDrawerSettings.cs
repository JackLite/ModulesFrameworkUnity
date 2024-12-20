using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Settings block for entity drawer
    /// </summary>
    [Serializable]
    public class EntityDrawerSettings
    {
        private const string AllOpenKey = "Modules.Debug.Entities.AlwaysOpenComponents";

        private Toggle _alwaysOpenToggle;
        private VisualElement _container;
        private bool _isAllOpen;

        public bool IsAllOpen => _isAllOpen;

        public void Draw(VisualElement root)
        {
            _isAllOpen = EditorPrefs.GetBool(AllOpenKey, false);
            _container = new();
            _container.AddToClassList("modules--entities-tab--entity-drawer-settings");
            _alwaysOpenToggle = new Toggle("Always open components");
            _alwaysOpenToggle.RegisterValueChangedCallback(ev =>
            {
                _isAllOpen = ev.newValue;
                EditorPrefs.SetBool(AllOpenKey, _isAllOpen);
            });
            _alwaysOpenToggle.SetValueWithoutNotify(_isAllOpen);
            _container.Add(_alwaysOpenToggle);
            root.Add(_container);
        }
    }
}