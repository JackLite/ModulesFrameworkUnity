using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

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

        public override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            _fieldName = labelText;
            var value = (IList)fieldValue;

            var container = new VisualElement();
            _foldout = new Foldout
            {
                text = $"{labelText} [{value.Count}]",
                value = false,
                style =
                {
                    marginLeft = 10
                }
            };

            DrawList(labelText, value, _foldout.contentContainer);

            container.Add(_foldout);
            parent.Add(container);
        }

        private void DrawAddBtn(string labelText, IList value)
        {
            var addBtn = new Button
            {
                text = "Add"
            };
            addBtn.clicked += () =>
            {
                var innerType = value.GetType().GetGenericArguments()[0];
                if (!innerType.IsValueType && innerType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {innerType.Name}");
                }
                else
                {
                    value.Add(Activator.CreateInstance(innerType));
                    _foldout.contentContainer.Clear();
                    DrawList(labelText, value, _foldout.contentContainer);
                }
            };
            _foldout.Add(addBtn);
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
                var drawer = mainDrawer.Draw(memberName, v, elementContainer, (_, newVal) =>
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

            DrawAddBtn(fieldName, value);
        }
    }
}