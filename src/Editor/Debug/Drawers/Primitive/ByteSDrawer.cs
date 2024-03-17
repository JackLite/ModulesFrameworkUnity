using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class ByteSDrawer : FieldDrawer<sbyte>
    {
        private IntDrawer _intDrawer;
        private IntegerField _field;

        protected override void Draw(string labelText, sbyte value, VisualElement parent,
            Action<sbyte, sbyte> onChanged)
        {
            _field = new IntegerField(labelText)
            {
                value = value
            };
            DrawersUtil.InitNumberFieldStyle(_field.style);

            _field.RegisterValueChangedCallback(ev =>
            {
                _field.value = (sbyte)ev.newValue;
                onChanged?.Invoke((sbyte)ev.previousValue, (sbyte)ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<sbyte> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}