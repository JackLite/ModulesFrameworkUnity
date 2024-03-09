using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class ShortUDrawer : FieldDrawer<ushort>
    {
        private IntDrawer _intDrawer;
        private IntegerField _intField;

        protected override void Draw(string labelText, ushort value, VisualElement parent, Action<ushort, ushort> onChanged)
        {
            _intField = new IntegerField(labelText)
            {
                value = value
            };
            _intField.RegisterValueChangedCallback(ev =>
            {
                _intField.value = (ushort)ev.newValue;
                onChanged?.Invoke((ushort)ev.previousValue, (ushort)ev.newValue);
            });
            parent.Add(_intField);
        }

        protected override void Update(Func<ushort> getter)
        {
            _intField.SetValueWithoutNotify(getter());
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _intField.isReadOnly = isReadOnly;
        }
    }
}