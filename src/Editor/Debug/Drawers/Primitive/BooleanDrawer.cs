using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class BooleanDrawer : FieldDrawer<bool>
    {
        private Toggle _boolField;
        private bool _isReadOnly;

        protected override void Draw(string labelText, bool value, VisualElement parent, Action<bool, bool> onChanged)
        {
            _boolField = new Toggle(labelText)
            {
                value = value
            };
            _boolField.RegisterValueChangedCallback(ev =>
            {
                if (_isReadOnly)
                    return;
                _boolField.value = ev.newValue;
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_boolField);
        }

        protected override void Update(Func<bool> getter)
        {
            _boolField.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
        }
    }
}