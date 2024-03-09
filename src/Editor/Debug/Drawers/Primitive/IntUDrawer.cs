using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class IntUDrawer : FieldDrawer<uint>
    {
        private UnsignedIntegerField _field;

        protected override void Draw(string labelText, uint value, VisualElement parent, Action<uint, uint> onChanged)
        {
            _field = new UnsignedIntegerField(labelText)
            {
                value = value
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<uint> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}