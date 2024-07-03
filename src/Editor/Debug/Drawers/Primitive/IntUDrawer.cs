#if UNITY_2022_1_OR_NEWER
using System;
using System.Globalization;
using UnityEngine;
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
            DrawersUtil.InitNumberFieldStyle(_field.style);

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
#endif