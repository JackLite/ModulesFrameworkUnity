using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class LongDrawer : FieldDrawer<long>
    {
        private LongField _field;

        protected override void Draw(string fieldName, long value, VisualElement parent, Action<long, long> onChanged)
        {
            _field = new LongField(fieldName)
            {
                value = value
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<long> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}