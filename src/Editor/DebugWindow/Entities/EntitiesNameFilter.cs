using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Filtering entities by name of component
    /// </summary>
    public class EntitiesNameFilter : VisualElement
    {
        private TextField _input;

        public event Action<string> OnInputChanged;

        public void Draw()
        {
            _input = new TextField();
            _input.label = "Component name";
            _input.AddToClassList("modules--entities-tab--name-filter");
            _input.RegisterValueChangedCallback(ev => OnInputChanged?.Invoke(ev.newValue));
            Add(_input);
        }
    }
}