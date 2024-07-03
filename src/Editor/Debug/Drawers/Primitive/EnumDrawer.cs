using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class EnumDrawer : FieldDrawer
    {
        private BaseField<Enum> _field;

        public override bool CanDraw(object value)
        {
            if (value is not Enum)
                return false;
            var values = Enum.GetValues(value.GetType());
            return values.Length > 0;
        }

        public override void Draw(string labelText, object value, VisualElement parent)
        {
            var type = value.GetType();
            var values = Enum.GetValues(value.GetType());
            if (type.GetCustomAttribute<FlagsAttribute>() != null)
            {
                var flagsField = new EnumFlagsField(labelText);
                flagsField.Init((Enum)values.GetValue(0));
                _field = flagsField;
            }
            else
            {
                var simpleEnumField = new EnumField(labelText);
                simpleEnumField.Init((Enum)values.GetValue(0));
                _field = simpleEnumField;
            }

            _field.SetValueWithoutNotify((Enum)value);
            _field.RegisterValueChangedCallback(ev =>
            {
                valueChangedCb(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        public override void Update()
        {
            _field.SetValueWithoutNotify((Enum)valueGetter());
        }
    }
}