using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class IntDrawer : FieldDrawer<int>
    {
        private IntegerField _field;
        protected override void Draw(string labelText, int value, VisualElement parent, Action<int, int> onChanged)
        {
            _field = new IntegerField(labelText)
            {
                value = value
            };
            _field.style.minWidth = new StyleLength(300);
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<int> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
            _field.label = $"__{_field.label}";
        }
    }
}