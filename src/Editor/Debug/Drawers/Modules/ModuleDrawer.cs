using System;
using ModulesFramework.Modules;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Modules
{
    public class ModuleDrawer : FieldRefDrawer
    {
        private TextField _text;
        private string _fieldName;

        public override bool CanDraw(Type type, object value)
        {
            return type == typeof(EcsModule);
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