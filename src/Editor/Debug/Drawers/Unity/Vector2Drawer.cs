using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class Vector2Drawer : FieldDrawer<Vector2>
    {
        private Vector2Field _field;

        protected override void Draw(string fieldName, Vector2 value, VisualElement parent, Action<Vector2, Vector2> onChanged)
        {
            _field = new Vector2Field(fieldName)
            {
                value = value
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<Vector2> getter)
        {
            _field.value = getter();
        }
    }
}