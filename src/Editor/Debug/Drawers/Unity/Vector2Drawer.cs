using System;
using UnityEngine;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class Vector2Drawer : FieldDrawer<Vector2>
    {
        private Vector2Field _field;

        protected override void Draw(string labelText, Vector2 value, VisualElement parent, Action<Vector2, Vector2> onChanged)
        {
            _field = new Vector2Field(labelText)
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