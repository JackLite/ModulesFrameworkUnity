using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Primitive
{
    public class StringDrawer : FieldDrawer<string>
    {
        private TextField _field;

        public override bool CanDraw(Type type, object value)
        {
            return type == typeof(string);
        }

        protected override void Draw(string labelText, string value, VisualElement parent, Action<string, string> onChanged)
        {
            _field = new TextField(labelText);
            var safeS = value ?? string.Empty;
            UpdateMultiline();
            _field.style.minWidth = 300;
            _field.style.whiteSpace = WhiteSpace.Normal;

            _field.SetValueWithoutNotify(safeS);
            _field.RegisterValueChangedCallback(ev =>
            {
                UpdateMultiline();
                onChanged?.Invoke(ev.previousValue, ev.newValue);
            });
            parent.Add(_field);
        }

        private void UpdateMultiline()
        {
            _field.multiline = _field.value.Length > 30;
            _field.style.minHeight = _field.value.Length > 30 ? 100 : 0;
        }

        protected override void Update(Func<string> getter)
        {
            var value = getter() ?? string.Empty;
            _field.SetValueWithoutNotify(value);
        }

        public override void SetReadOnly(bool isReadOnly)
        {
            _field.isReadOnly = isReadOnly;
        }
    }
}