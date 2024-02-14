using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class EnumDrawer : FieldDrawer
    {
        private EnumField _field;

        public override bool CanDraw(object value)
        {
            return value is Enum;
        }

        public override void Draw(string fieldName, object value, VisualElement parent)
        {
            _field = new EnumField();
            var values = Enum.GetValues(value.GetType());
            if (values.Length == 0)
                return;

            _field.Init((Enum)values.GetValue(0));
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