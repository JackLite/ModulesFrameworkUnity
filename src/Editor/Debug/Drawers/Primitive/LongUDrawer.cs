using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class LongUDrawer : FieldDrawer<ulong>
    {
        private UnsignedLongField _field;

        protected override void Draw(string fieldName, ulong value, VisualElement parent, Action<ulong, ulong> onChanged)
        {
            _field = new UnsignedLongField(fieldName)
            {
                value = value
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<ulong> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}