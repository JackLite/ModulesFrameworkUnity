using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class ListDrawer : FieldDrawer
    {
        private Foldout _foldout;
        private string _fieldName;
        private readonly List<FieldDrawer> _drawers = new();

        public override bool CanDraw(object value)
        {
            var type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public override void Draw(string fieldName, object fieldValue, VisualElement parent)
        {
            _fieldName = fieldName;
            var value = (IList)fieldValue;

            var container = new VisualElement();
            _foldout = new Foldout
            {
                text = $"{fieldName} [{value.Count}]",
                value = false
            };
            _foldout.style.marginLeft = 10;

            DrawList(fieldName, value, _foldout.contentContainer);

            var addBtn = new Button
            {
                text = "Add"
            };
            addBtn.clicked += () =>
            {
                var innerType = fieldValue.GetType().GetGenericArguments()[0];
                if (!innerType.IsValueType && innerType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {innerType.Name}");
                }
                else
                {
                    value.Add(Activator.CreateInstance(innerType));
                    _foldout.contentContainer.Clear();
                    DrawList(fieldName, value, _foldout.contentContainer);
                }
            };
            container.Add(_foldout);
            container.Add(addBtn);
            parent.Add(container);
        }

        public override void Update()
        {
            _foldout.text = $"{_fieldName} [{((IList)valueGetter()).Count}]";
            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }

        private void DrawList(string fieldName, IList value, VisualElement container)
        {
            for (var i = 0; i < value.Count; i++)
            {
                var elementContainer = new VisualElement();
                elementContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                var v = value[i];
                var memberName = $"{fieldName} [{i}]";
                var index = i;
                var drawer = mainDrawer.Draw(memberName, v, elementContainer, (prev, newVal) =>
                {
                    value[index] = newVal;
                }, () =>
                {
                    if (index < value.Count)
                        return value[index];
                    return default;
                }, false);
                _drawers.Add(drawer);
                var removeBtn = new Button
                {
                    text = "R"
                };
                removeBtn.clicked += () =>
                {
                    value.RemoveAt(index);
                    _foldout.contentContainer.Clear();
                    _drawers.Clear();
                    DrawList(fieldName, value, _foldout.contentContainer);
                };
                elementContainer.Add(removeBtn);
                container.Add(elementContainer);
            }
        }
    }
}