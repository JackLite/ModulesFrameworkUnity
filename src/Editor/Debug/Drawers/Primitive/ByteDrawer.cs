﻿using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class ByteDrawer : FieldDrawer<byte>
    {
        private IntDrawer _intDrawer;
        private IntegerField _intField;

        protected override void Draw(string fieldName, byte value, VisualElement parent, Action<byte, byte> onChanged)
        {
            _intField = new IntegerField(fieldName)
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