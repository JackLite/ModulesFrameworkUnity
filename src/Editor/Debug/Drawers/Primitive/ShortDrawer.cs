using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class ShortDrawer : FieldDrawer<short>
    {
        private IntDrawer _intDrawer;
        private IntegerField _field;

        protected override void Draw(string labelText, short value, VisualElement parent, Action<short, short> onChanged)
        {
            _field = new IntegerField(labelText)
            {
                value = value
            };
            DrawersUtil.InitNumberFieldStyle(_field.style);

            _field.RegisterValueChangedCallback(ev =>
            {
                _field.value = (short)ev.newValue;
                onChanged?.Invoke((short)ev.previousValue, (short)ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<short> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}