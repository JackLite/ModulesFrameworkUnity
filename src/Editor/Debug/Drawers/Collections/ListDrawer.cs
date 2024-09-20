using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class ListDrawer : BaseCollectionDrawer<IList>
    {
        public override bool CanDraw(object value)
        {
            var type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            _fieldName = labelText;
            _oldRef = (IList)fieldValue;

            _foldout = new Foldout
            {
                text = $"List: {labelText} [{_oldRef.Count}]",
                value = false,
            };

            DrawList(labelText, _foldout.contentContainer);

            parent.Add(_foldout);
        }

        private void DrawAddBtn(string labelText)
        {
            var addBtn = new Button
            {
                text = "Add"
            };
            addBtn.clicked += () =>
            {
                var innerType = _oldRef.GetType().GetGenericArguments()[0];
                if(innerType == typeof(string))
                {
                    _oldRef.Add(string.Empty);
                    _foldout.contentContainer.Clear();
                    DrawList(labelText, _foldout.contentContainer);
                }
                else if (!innerType.IsValueType && innerType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {innerType.Name}");
                }
                else
                {
                    _oldRef.Add(Activator.CreateInstance(innerType));
                    _foldout.contentContainer.Clear();
                    DrawList(labelText, _foldout.contentContainer);
                }
            };
            _foldout.Add(addBtn);
        }

        public override void Update()
        {
            ProceedNull();
            if (_isNull)
                return;

            var list = (IList)valueGetter();
            var count = list.Count;
            if (count != _drawers.Count || !ReferenceEquals(_oldRef, list))
            {
                _oldRef = list;
                _drawers.Clear();
                _foldout.contentContainer.Clear();
                DrawList(_fieldName, _foldout.contentContainer);
                return;
            }

            _foldout.text = $"List: {_fieldName} [{count}]";
            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }

        private void DrawList(string fieldName, VisualElement container)
        {
            for (var i = 0; i < _oldRef.Count; i++)
            {
                var elementContainer = new VisualElement
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    }
                };
                var v = _oldRef[i];
                var memberName = $"{fieldName} [{i}]";
                var index = i;
                var drawer = mainDrawer.Draw(memberName, v, elementContainer, (_, newVal) =>
                {
                    _oldRef[index] = newVal;
                }, () =>
                {
                    if (index < _oldRef.Count)
                        return _oldRef[index];
                    return default;
                }, Level + 1, false);
                _drawers.Add(drawer);
                var removeBtn = DrawersUtil.CreateRemoveBtn();

                removeBtn.clicked += () =>
                {
                    _oldRef.RemoveAt(index);
                    _foldout.contentContainer.Clear();
                    _drawers.Clear();
                    DrawList(fieldName, _foldout.contentContainer);
                };
                elementContainer.Add(removeBtn);
                container.Add(elementContainer);
            }

            DrawAddBtn(fieldName);
        }
    }
}