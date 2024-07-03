using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class DoubleDrawer : FieldDrawer<double>
    {
        private DoubleField _field;

        protected override void Draw(string labelText, double value, VisualElement parent, Action<double, double> onChanged)
        {
            _field = new DoubleField(labelText)
            {
                value = value
            };
            DrawersUtil.InitNumberFieldStyle(_field.style);

            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<double> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}