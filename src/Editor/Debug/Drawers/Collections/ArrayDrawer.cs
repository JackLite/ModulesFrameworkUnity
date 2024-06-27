using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class ArrayDrawer : BaseCollectionDrawer
    {
        private VisualElement _elements;

        public override bool CanDraw(object value)
        {
            return value is Array;
        }

        public override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            var value = (Array)fieldValue;
            _container = new VisualElement();
            _foldout = new Foldout
            {
                text = $"Array: {labelText} [{value.Length}]",
                value = false,
            };

            _elements = new VisualElement();
            _foldout.Add(_elements);
            DrawArray(labelText, value, _elements);

            _container.Add(_foldout);
            parent.Add(_container);
        }

        public override void Update()
        {
            ProceedNull();
            if (_isNull)
                return;
            _foldout.text = $"Array: {_fieldName} [{((Array)valueGetter()).Length}]";
            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }

        private void DrawArray(string fieldName, Array value, VisualElement container)
        {
            for (var i = 0; i < value.Length; i++)
            {
                var elementContainer = new VisualElement
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                    }
                };
                var v = value.GetValue(i);
                var memberName = $"{fieldName} [{i}]";
                var index = i;
                var drawer = mainDrawer.Draw(memberName, v, elementContainer, (prev, newVal) =>
                {
                    value.SetValue(newVal, index);
                }, () =>
                {
                    if (index < value.Length)
                        return value.GetValue(index);
                    return default;
                }, Level + 1, false);
                _drawers.Add(drawer);
                container.Add(elementContainer);
            }
        }
    }
}