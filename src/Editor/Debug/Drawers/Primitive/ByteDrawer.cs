using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class ByteDrawer : FieldDrawer<byte>
    {
        private IntDrawer _intDrawer;
        private IntegerField _field;

        protected override void Draw(string labelText, byte value, VisualElement parent, Action<byte, byte> onChanged)
        {
            _field = new IntegerField(labelText)
            {
                value = value
            };
            DrawersUtil.InitNumberFieldStyle(_field.style);
            _field.RegisterValueChangedCallback(ev =>
            {
                _field.value = (byte)ev.newValue;
                onChanged?.Invoke((byte)ev.previousValue, (byte)ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<byte> getter)
        {
            _field.SetValueWithoutNotify(getter());
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}