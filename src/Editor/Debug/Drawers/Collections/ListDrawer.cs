using System;
using System.Collections;
using System.Collections.Generic;
using ModulesFramework.Utils.Types;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class ListDrawer : BaseCollectionDrawer<IList>
    {
        private bool _wasDrawn;

        public override bool CanDraw(Type type, object value)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        protected override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            _container = new VisualElement();
            parent.Add(_container);
            _fieldName = labelText;

            _foldout = new Foldout
            {
                text = GetLabel(),
                value = false,
            };

            if (fieldValue == null)
                return;

            _oldRef = (IList)fieldValue;

            _container.Add(_foldout);

            if (_wasDrawn)
                DrawList(_fieldName, _foldout.contentContainer);

            _foldout.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue && !_wasDrawn)
                    DrawList(_fieldName, _foldout.contentContainer);
            });
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
                _oldRef.Add(CreateNewInsideCollection(innerType));
                _foldout.contentContainer.Clear();
                DrawList(labelText, _foldout.contentContainer);
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

            _foldout.text = GetLabel();

            if (_foldout.value == false)
                return;

            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }

        private void DrawList(string fieldName, VisualElement container)
        {
            var elementType = _oldRef.GetType().GetGenericArguments()[0];
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
                var drawer = mainDrawer.Draw(memberName, elementType, v, elementContainer,
                    (_, newVal) => { _oldRef[index] = newVal; }, () =>
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

        protected override string GetTypeLabel()
        {
            var innerTypeName = _type.GetGenericArguments()[0].Name;
            return $"{_fieldName} List<{innerTypeName}>";
        }

        private string GetLabel()
        {
            var value = valueGetter();
            var count = ((IList)value)?.Count ?? 0;
            return $"{GetTypeLabel()}[{count}]";
        }
    }
}