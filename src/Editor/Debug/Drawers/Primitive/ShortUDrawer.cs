using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class ShortUDrawer : FieldDrawer<ushort>
    {
        private IntDrawer _intDrawer;
        private IntegerField _field;

        protected override void Draw(string labelText, ushort value, VisualElement parent,
            Action<ushort, ushort> onChanged)
        {
            _field = new IntegerField(labelText)
            {
                value = value
            };
            DrawersUtil.InitNumberFieldStyle(_field.style);

            _field.RegisterValueChangedCallback(ev =>
            {
                _field.value = (ushort)ev.newValue;
                onChanged?.Invoke((ushort)ev.previousValue, (ushort)ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<ushort> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}