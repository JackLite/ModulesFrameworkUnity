using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class StringDrawer : FieldDrawer<string>
    {
        private TextField _field;

        public override bool CanDraw(object value)
        {
            return value == null || value is string;
        }

        protected override void Draw(string labelText, string value, VisualElement parent, Action<string, string> onChanged)
        {
            _field = new TextField(labelText);
            _field.SetValueWithoutNotify(value ?? string.Empty);
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<string> getter)
        {
            var value = getter() ?? string.Empty;
            _field.SetValueWithoutNotify(value);
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}