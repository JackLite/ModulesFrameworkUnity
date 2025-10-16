using System;
using ModulesFramework.Systems;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Modules
{
    public class SystemDrawer : FieldRefDrawer
    {
        private TextField _text;
        private string _fieldName;

        public override bool CanDraw(Type type, object value)
        {
            return typeof(ISystem).IsAssignableFrom(type);
        }

        protected override void Draw(string labelText, object value, VisualElement parent)
        {
            _fieldName = labelText;
            _container = new();
            parent.Add(_container);
            _text ??= new TextField(labelText)
            {
                isReadOnly = true
            };
            _container.Add(_text);
            if (value == null)
            {
                nullDrawer.Draw(_fieldName, typeof(void), null, _container);
                return;
            }

            _text.SetValueWithoutNotify(value.GetType().Name);
        }

        public override void Update()
        {
            ProceedNull();
            if (!_isNull)
                _text.SetValueWithoutNotify(valueGetter().GetType().Name);
        }

        protected override void OnNullChanged()
        {
            if (_isNull)
            {
                _text?.RemoveFromHierarchy();
                nullDrawer.Draw(_fieldName, typeof(void), null, _container);
            }
            else
            {
                Draw(_fieldName, valueGetter(), _container);
            }
        }
    }
}