using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class UnityObjectDrawer : FieldDrawer
    {
        private ObjectField _field;
        public override bool CanDraw(object value)
        {
            return value is Object;
        }

        public override void Draw(string fieldName, object value, VisualElement parent)
        {
            _field = new ObjectField
            {
                objectType = value.GetType()
            };
            _field.SetValueWithoutNotify((Object)value);
            _field.RegisterValueChangedCallback(ev =>
            {
                valueChangedCb(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        public override void Update()
        {
            _field.SetValueWithoutNotify((Object)valueGetter());
        }
    }
}