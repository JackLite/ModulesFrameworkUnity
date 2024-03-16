using System;
using UnityEngine;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class Vector3Drawer : FieldDrawer<Vector3>
    {
        private Vector3Field _field;

        protected override void Draw(string labelText, Vector3 value, VisualElement parent, Action<Vector3, Vector3> onChanged)
        {
            _field = new Vector3Field(labelText)
            {
                value = value
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<Vector3> getter)
        {
            _field.value = getter();
        }
    }
}