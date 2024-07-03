using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class ArrayDrawer : BaseCollectionDrawer<Array>
    {
        private VisualElement _elements;

        public override bool CanDraw(object value)
        {
            return value is Array;
        }

        public override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            _oldRef = (Array)fieldValue;
            _fieldName = labelText;
            _container = new VisualElement();
            _foldout = new Foldout
            {
                text = $"Array: {labelText} [{_oldRef.Length}]",
                value = false,
            };

            _elements = new VisualElement();
            _foldout.Add(_elements);
            DrawArray(labelText, _elements);

            _container.Add(_foldout);
            parent.Add(_container);
        }

        public override void Update()
        {
            ProceedNull();
            if (_isNull)
                return;
            var array = (Array)valueGetter();
            _foldout.text = $"Array: {_fieldName} [{array.Length}]";

            if (!ReferenceEquals(_oldRef, array))
            {
                _oldRef = array;
                _elements.Clear();
                DrawArray(_fieldName, _elements);
            }

            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }

        private void DrawArray(string fieldName, VisualElement container)
        {
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
                var drawer = mainDrawer.Draw(memberName, v, elementContainer, (prev, newVal) =>
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
    }
}