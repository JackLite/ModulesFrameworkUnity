using System;
using UnityEngine;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Unity
{
    public class Vector2IntDrawer : FieldDrawer<Vector2Int>
    {
        private Vector2IntField _field;

        protected override void Draw(string labelText, Vector2Int value, VisualElement parent, Action<Vector2Int, Vector2Int> onChanged)
        {
            _field = new Vector2IntField(labelText)
            {
                value = value
            };
            _field.RegisterValueChangedCallback(ev =>
            {
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        protected override void Update(Func<Vector2Int> getter)
        {
            _field.value = getter();
        }
    }
}