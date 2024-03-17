using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class DictionaryDrawer : FieldDrawer
    {
        private Foldout _foldout;
        private VisualElement _elements;
        private VisualElement _addBlock;
        private string _fieldName;
        private readonly List<FieldDrawer> _drawers = new();
        private readonly Dictionary<string, object> _newKeys = new();

        public override bool CanDraw(object value)
        {
            var type = value.GetType();
            return type.IsGenericType && value is IDictionary;
        }

        public override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            _fieldName = labelText;
            _addBlock = CreateAddBlock();
            var value = (IDictionary)fieldValue;

            var container = new VisualElement();
            _foldout = new Foldout
            {
                text = $"Dictionary: {labelText} [{value.Count}]",
                value = false
            };
            _elements = new VisualElement();
            _foldout.Add(_elements);

            DrawDict(labelText, value);
            container.Add(_foldout);
            _foldout.Add(_addBlock);

            DrawAddBlock(labelText, value);


            parent.Add(container);
        }

        private void DrawAddBlock(string fieldName, IDictionary value)
        {
            TryDrawValueOrSimpleAdd(fieldName, value);
            TryDrawStringAdd(fieldName, value);
        }

        private void TryDrawValueOrSimpleAdd(string fieldName, IDictionary value)
        {
            var innerKeyType = value.GetType().GetGenericArguments()[0];
            if (!innerKeyType.IsValueType && innerKeyType.GetConstructor(Type.EmptyTypes) == null)
                return;

            var cacheKey = value.GetType().FullName + fieldName;
            if (!_newKeys.ContainsKey(cacheKey))
            {
                _newKeys[cacheKey] = Activator.CreateInstance(innerKeyType);
            }

            var keyType = _newKeys[cacheKey].GetType().Name;
            mainDrawer.Draw($"New key [{keyType}]", _newKeys[cacheKey], _addBlock, (_, newVal) =>
            {
                _newKeys[cacheKey] = newVal;
            }, () => _newKeys[cacheKey], Level + 1, false);

            DrawAddBtn(() =>
            {
                var newKey = _newKeys[cacheKey];
                var valueType = value.GetType().GetGenericArguments()[1];
                if (!valueType.IsValueType && valueType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {valueType.Name}");
                    return;
                }

                var newValue = Activator.CreateInstance(valueType);
                value[newKey] = newValue;
                _newKeys.Remove(cacheKey);
                _elements.Clear();
                DrawDict(fieldName, value);
                _addBlock.Clear();
                DrawAddBlock(fieldName, value);
            });
        }

        private void TryDrawStringAdd(string fieldName, IDictionary value)
        {
            var innerKeyType = value.GetType().GetGenericArguments()[0];

            if (innerKeyType == typeof(string))
            {
                var cacheKey = value.GetType().FullName + fieldName;
                _newKeys.TryAdd(cacheKey, string.Empty);

                mainDrawer.Draw("New key [string]", _newKeys[cacheKey], _addBlock, (_, newVal) =>
                {
                    _newKeys[cacheKey] = newVal;
                }, () => _newKeys[cacheKey], Level + 1, false);

                DrawAddBtn(() =>
                {
                    var newKey = _newKeys[cacheKey];
                    var valueType = value.GetType().GetGenericArguments()[1];
                    if (!valueType.IsValueType && valueType.GetConstructor(Type.EmptyTypes) == null)
                    {
                        UnityEngine.Debug.LogError($"There is no parameterless constructor for {valueType.Name}");
                        return;
                    }

                    var newValue = Activator.CreateInstance(valueType);
                    value[newKey] = newValue;
                    _newKeys.Remove(cacheKey);
                    _elements.Clear();
                    DrawDict(fieldName, value);
                    _addBlock.Clear();
                    DrawAddBlock(fieldName, value);
                });
            }
        }

        private static VisualElement CreateAddBlock()
        {
            var addBlock = new VisualElement
            {
                style =
                {
                    marginTop = 10,
                    paddingTop = 10,
                    marginBottom = 10,
                    paddingBottom = 10,
                    borderTopWidth = 1,
                    borderTopColor = new StyleColor(Color.black),
                    borderBottomWidth = 1,
                    borderBottomColor = new StyleColor(Color.black)
                }
            };

            return addBlock;
        }

        private void DrawAddBtn(Action onClick)
        {
            var addBtn = new Button
            {
                text = "Add"
            };
            addBtn.clicked += onClick;
            _addBlock.Add(addBtn);
        }

        private void DrawDict(string fieldName, IDictionary value)
        {
            var keys = value.Keys.Cast<object>().ToArray();
            foreach (var key in keys)
            {
                var elementContainer = new VisualElement
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                        alignItems = Align.FlexStart
                    }
                };
                
                var val = value[key];
                var memberName = $"{fieldName} [{key}]";
                var drawer = mainDrawer.Draw(memberName, val, elementContainer, (_, newVal) =>
                {
                    value[key] = newVal;
                }, () => val, Level + 1, false);
                _drawers.Add(drawer);
                
                var removeBtn = DrawersUtil.CreateRemoveBtn();
                
                removeBtn.clicked += () =>
                {
                    value.Remove(key);
                    _elements.Clear();
                    _drawers.Clear();
                    _addBlock.Clear();
                    DrawDict(fieldName, value);
                    DrawAddBlock(fieldName, value);
                };
                
                elementContainer.Add(removeBtn);
                _elements.Add(elementContainer);
            }
        }

        public override void Update()
        {
            _foldout.text = $"Dictionary: {_fieldName} [{((IDictionary)valueGetter()).Count}]";
            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }
    }
}