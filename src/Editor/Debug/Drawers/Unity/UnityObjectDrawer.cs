using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class UnityObjectDrawer : FieldDrawer
    {
        private ObjectField _field;
        public override int Order { get; } = 5;

        public override bool CanDraw(Type type, object value)
        {
            return typeof(Object).IsAssignableFrom(type);
        }

        protected override void Draw(string labelText, object value, VisualElement parent)
        {
            _field = new ObjectField
            {
                objectType = _type
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