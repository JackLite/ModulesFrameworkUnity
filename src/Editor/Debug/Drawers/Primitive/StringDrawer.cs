using System;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class StringDrawer : FieldDrawer<string>
    {
        private TextField _field;

        protected override void Draw(string fieldName, string value, VisualElement parent, Action<string, string> onChanged)
        {
            _field = new TextField(fieldName)
            {
                value = value ?? string.Empty
            };
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