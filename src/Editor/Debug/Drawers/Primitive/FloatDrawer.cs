using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class FloatDrawer : FieldDrawer<float>
    {
        private FloatField _field;

        protected override void Draw(string labelText, float value, VisualElement parent, Action<float, float> onChanged)
        {
            _field = new FloatField(labelText)
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

        protected override void Update(Func<float> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}