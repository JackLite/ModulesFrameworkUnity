using ModulesFrameworkUnity.Debug.Drawers.Widgets;
using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class ArrayDrawer : BaseCollectionDrawer<Array>
    {
        private VisualElement _elements;

        public override bool CanDraw(Type type, object value)
        {
            return type.IsArray || value is Array;
        }

        protected override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            _fieldName = labelText;
            _container = new VisualElement();
            parent.Add(_container);
            _foldout = new Foldout
            {
                text = $"{labelText} {_type.GetElementType().Name}[0]",
                value = false,
            };

            _elements = new VisualElement();
            _foldout.Add(_elements);
            _container.Add(_foldout);

            if (fieldValue == null)
                return;

            _oldRef = (Array)fieldValue;

            _foldout.text = $"{labelText} {_type.GetElementType().Name}[{_oldRef.Length}]";

            DrawArray(labelText, _elements);
        }

        public override void Update()
        {
            ProceedNull();
            if (_isNull)
                return;
            var array = (Array)valueGetter();
            _foldout.text = $"{_fieldName} {_type.GetElementType().Name}[{_oldRef.Length}]";

            if (!ReferenceEquals(_oldRef, array))
            {
                _oldRef = array;
                _elements.Clear();
                DrawArray(_fieldName, _elements);
            }

            if (_foldout.value == false)
                return;

            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }

        private void DrawArray(string fieldName, VisualElement container)
        {
            var elementType = _type.GetElementType();
            for (var i = 0; i < _oldRef.Length; i++)
            {
                var elementContainer = new VisualElement
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    }
                };
                var v = _oldRef.GetValue(i);
                var memberName = $"{fieldName} [{i}]";
                var index = i;
                var drawer = mainDrawer.Draw(memberName, elementType, v, elementContainer, (prev, newVal) =>
                {
                    _oldRef.SetValue(newVal, index);
                }, () =>
                {
                    if (index < _oldRef.Length)
                        return _oldRef.GetValue(index);
                    return default;
                }, Level + 1, false);
                _drawers.Add(drawer);
                container.Add(elementContainer);
            }
        }

        protected override void OnNullChanged()
        {
            if (_isNull)
            {
                _foldout.Clear();
                _drawers.Clear();

                DrawCreateWidget();
            }
            else
            {
                _foldout.Clear();
                Draw(_fieldName, valueGetter(), _container);
            }
        }

        protected override void DrawCreateWidget()
        {
            var widget = new CreateArrayWidget(GetTypeLabel(), (size) =>
            {
                var newArr = Array.CreateInstance(_type.GetElementType(), size);
                valueChangedCb(_oldRef, newArr);
                _oldRef = newArr;
            });
            _container.Add(widget);
        }

        protected override string GetTypeLabel()
        {
            var elementType = _type.GetElementType();
            return $"{elementType.Name}[]";
        }
    }
}
