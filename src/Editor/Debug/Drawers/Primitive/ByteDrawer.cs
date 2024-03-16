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
        private IntegerField _intField;

        protected override void Draw(string labelText, byte value, VisualElement parent, Action<byte, byte> onChanged)
        {
            _intField = new IntegerField(labelText)
            {
                value = value
            };
            _intField.RegisterValueChangedCallback(ev =>
            {
                _intField.value = (byte)ev.newValue;
                onChanged?.Invoke((byte)ev.previousValue, (byte)ev.newValue);
            });
            parent.Add(_intField);
        }

        protected override void Update(Func<byte> getter)
        {
            _intField.SetValueWithoutNotify(getter());
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _intField.isReadOnly = isReadOnly;
        }
    }
}