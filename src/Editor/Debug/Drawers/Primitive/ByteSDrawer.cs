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
        private IntegerField _intField;

        protected override void Draw(string labelText, sbyte value, VisualElement parent, Action<sbyte, sbyte> onChanged)
        {
            _intField = new IntegerField(labelText)
            {
                value = value
            };
            _intField.RegisterValueChangedCallback(ev =>
            {
                _intField.value = (sbyte)ev.newValue;
                onChanged?.Invoke((sbyte)ev.previousValue, (sbyte)ev.newValue);
            });
            parent.Add(_intField);
        }

        protected override void Update(Func<sbyte> getter)
        {
            _intField.SetValueWithoutNotify(getter());
        }
        
        public override void SetReadOnly(bool isReadOnly)
        {
            _intField.isReadOnly = isReadOnly;
        }
    }
}