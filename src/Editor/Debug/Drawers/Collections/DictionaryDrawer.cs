using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Utils.Types;
using UnityEngine;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Collections
{
    public class DictionaryDrawer : BaseCollectionDrawer<IDictionary>
    {
        private VisualElement _elements;
        private VisualElement _addBlock;
        private readonly Dictionary<string, object> _newKeys = new();

        public override bool CanDraw(Type type, object value)
        {
            return type.IsGenericType && typeof(IDictionary).IsAssignableFrom(type);
        }

        protected override void Draw(string labelText, object fieldValue, VisualElement parent)
        {
            _fieldName = labelText;
            _container = new VisualElement();
            parent.Add(_container);

            _foldout = new Foldout
            {
                text = GetLabel(),
                value = false
            };
            _elements = new VisualElement();
            _foldout.Add(_elements);
            _container.Add(_foldout);

            if (fieldValue == null)
                return;

            var value = (IDictionary)fieldValue;
            _oldRef = value;
            _addBlock = CreateAddBlock();
            _foldout.Add(_addBlock);

            DrawDict(labelText);

            DrawAddBlock(labelText);
        }

        private void DrawAddBlock(string fieldName)
        {
            TryDrawValueOrSimpleAdd(fieldName);
            TryDrawStringAdd(fieldName);
        }

        private void TryDrawValueOrSimpleAdd(string fieldName)
        {
            var innerKeyType = _oldRef.GetType().GetGenericArguments()[0];
            if (!innerKeyType.IsValueType && innerKeyType.GetConstructor(Type.EmptyTypes) == null)
                return;

            var cacheKey = _oldRef.GetType().FullName + fieldName;
            if (!_newKeys.ContainsKey(cacheKey))
            {
                _newKeys[cacheKey] = Activator.CreateInstance(innerKeyType);
            }

            var keyType = _newKeys[cacheKey].GetType();
            mainDrawer.Draw($"New key [{keyType.GetTypeName()}]", keyType, _newKeys[cacheKey], _addBlock, (_, newVal) =>
            {
                _newKeys[cacheKey] = newVal;
            }, () => _newKeys[cacheKey], Level + 1, false);

            DrawAddBtn(() =>
            {
                var newKey = _newKeys[cacheKey];
                var valueType = _oldRef.GetType().GetGenericArguments()[1];
                if (!valueType.IsValueType && valueType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {valueType.GetTypeName()}");
                    return;
                }

                var newValue = Activator.CreateInstance(valueType);
                _oldRef[newKey] = newValue;
                _newKeys.Remove(cacheKey);
                _elements.Clear();
                DrawDict(fieldName);
                _addBlock.Clear();
                DrawAddBlock(fieldName);
            });
        }

        private void TryDrawStringAdd(string fieldName)
        {
            var innerKeyType = _oldRef.GetType().GetGenericArguments()[0];

            if (innerKeyType == typeof(string))
            {
                var cacheKey = _oldRef.GetType().FullName + fieldName;
                _newKeys.TryAdd(cacheKey, string.Empty);

                mainDrawer.Draw("New key [string]", typeof(string), _newKeys[cacheKey], _addBlock, (_, newVal) =>
                {
                    _newKeys[cacheKey] = newVal;
                }, () => _newKeys[cacheKey], Level + 1, false);

                DrawAddBtn(() =>
                {
                    var newKey = _newKeys[cacheKey];
                    var valueType = _oldRef.GetType().GetGenericArguments()[1];
                    if (!valueType.IsValueType && valueType.GetConstructor(Type.EmptyTypes) == null)
                    {
                        UnityEngine.Debug.LogError($"There is no parameterless constructor for {valueType.GetTypeName()}");
                        return;
                    }

                    var newValue = Activator.CreateInstance(valueType);
                    _oldRef[newKey] = newValue;
                    _newKeys.Remove(cacheKey);
                    _elements.Clear();
                    DrawDict(fieldName);
                    _addBlock.Clear();
                    DrawAddBlock(fieldName);
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

        private void DrawDict(string fieldName)
        {
            var keys = _oldRef.Keys.Cast<object>().ToArray();
            var valueType = _type.GetGenericArguments()[1];
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

                var val = _oldRef[key];
                var memberName = $"{fieldName} [{key}]";
                var valuesType = val.GetType();
                var drawer = mainDrawer.Draw(
                    memberName,
                    valueType,
                    val,
                    elementContainer,
                    OnChanged,
                    Getter,
                    Level + 1,
                    false);
                _drawers.Add(drawer);

                var removeBtn = DrawersUtil.CreateRemoveBtn();

                removeBtn.clicked += () =>
                {
                    _oldRef.Remove(key);
                    _elements.Clear();
                    _drawers.Clear();
                    _addBlock.Clear();
                    DrawDict(fieldName);
                    DrawAddBlock(fieldName);
                };

                elementContainer.Add(removeBtn);
                _elements.Add(elementContainer);
                continue;

                void OnChanged(object _, object newVal)
                {
                    _oldRef[key] = newVal;
                }

                object Getter()
                {
                    if (_oldRef.Contains(key))
                        return _oldRef[key];
                    if (valuesType.IsValueType)
                        return Activator.CreateInstance(valuesType);
                    return null;
                }
            }
        }

        public override void Update()
        {
            ProceedNull();
            if (_isNull)
                return;
            var dictionary = (IDictionary)valueGetter();
            _foldout.text = GetLabel();
            if (_drawers.Count != dictionary.Count || !ReferenceEquals(_oldRef, dictionary))
            {
                var wasOpen = _foldout.value;
                _oldRef = dictionary;
                _drawers.Clear();
                _elements.Clear();
                DrawDict(_fieldName);
                _addBlock.Clear();
                DrawAddBlock(_fieldName);
                _foldout.value = wasOpen;
                return;
            }

            if (_foldout.value == false)
                return;

            foreach (var drawer in _drawers)
            {
                drawer.Update();
            }
        }

        protected override string GetTypeLabel()
        {
            var genericArgs = _type.GetGenericArguments();
            var keyType = genericArgs[0].Name;
            var valueType = genericArgs[1].Name;
            return $"{_fieldName} Dict<{keyType},{valueType}>";
        }

        private string GetLabel()
        {
            var value = valueGetter();
            var count = ((IDictionary)value)?.Count ?? 0;
            return $"{GetTypeLabel()}[{count}]";
        }
    }
}
