using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class ShortDrawer : FieldDrawer<short>
    {
        private IntDrawer _intDrawer;
        private IntegerField _intField;

        protected override void Draw(string fieldName, short value, VisualElement parent, Action<short, short> onChanged)
        {
            _intField = new IntegerField(fieldName)
            {
                value = value
            };
            _intField.RegisterValueChangedCallback(ev =>
            {
                _intField.value = (short)ev.newValue;
                onChanged?.Invoke((short)ev.previousValue, (short)ev.newValue);
            });
            parent.Add(_intField);
        }

        protected override void Update(Func<short> getter)
        {
            _intField.SetValueWithoutNotify(getter());
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _intField.isReadOnly = isReadOnly;
        }
    }
}