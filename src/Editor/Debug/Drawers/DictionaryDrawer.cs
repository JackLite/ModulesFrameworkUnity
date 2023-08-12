using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug.Drawers
{
    public class DictionaryDrawer : IFieldDrawer
    {
        private EditorDrawer _editorDrawer;
        private Dictionary<string, object> _newKeys = new();

        public DictionaryDrawer(EditorDrawer editorDrawer)
        {
            _editorDrawer = editorDrawer;
        }

        public bool TryDraw(Type component, string fieldName, object fieldValue, ref int level,
            out object newValue)
        {
            newValue = fieldValue;
            var type = fieldValue.GetType();
            if (!type.IsGenericType || fieldValue is not IDictionary value)
                return false;

            var style = DrawerUtility.ContainerStyle(level);
            var count = value.Count;
            if (!EditorDrawerUtility.Foldout(component + fieldName, $"{fieldName} ({count})", style, level))
                return true;

            var newDict = (IDictionary)Activator.CreateInstance(type);

            foreach (var key in value.Keys)
            {
                newDict.Add(key, value[key]);
            }

            EditorGUILayout.BeginVertical(style);
            level++;

            var keys = value.Keys.Cast<object>().ToArray();
            foreach (var key in keys)
            {
                var val = newDict[key];
                var memberName = $"{fieldName} [{key}]";
                var changedVal = _editorDrawer.DrawField(component, memberName, val, ref level);
                newDict[key] = changedVal;
            }

            level--;

            var innerKeyType = type.GetGenericArguments()[0];
            if (innerKeyType.IsValueType || innerKeyType.GetConstructor(Type.EmptyTypes) != null)
            {
                var cacheKey = component.FullName + fieldName;
                if (!_newKeys.ContainsKey(cacheKey))
                {
                    _newKeys[cacheKey] = Activator.CreateInstance(innerKeyType);
                }

                _newKeys[cacheKey] = _editorDrawer.DrawField(
                    innerKeyType,
                    "New key",
                    _newKeys[cacheKey],
                    ref level);

                DrawAdd(type, newDict, _newKeys[cacheKey]);
            }

            if (innerKeyType == typeof(string))
            {
                var cacheKey = component.FullName + fieldName;
                _newKeys.TryAdd(cacheKey, string.Empty);
                _newKeys[cacheKey] =
                    (string)_editorDrawer.DrawField(innerKeyType, "New key", _newKeys[cacheKey], ref level);
                DrawAdd(type, newDict, _newKeys[cacheKey]);
            }

            EditorGUILayout.EndVertical();

            newValue = newDict;
            return true;
        }

        private static void DrawAdd(Type type, IDictionary newDict, object newKey)
        {
            if (newDict.Contains(newKey))
            {
                EditorGUILayout.LabelField("Key already exists!", EditorStyles.whiteBoldLabel);
            }
            else if (GUILayout.Button("Add", EditorStyles.miniButtonMid))
            {
                var innerValueType = type.GetGenericArguments()[1];
                if (!innerValueType.IsValueType && innerValueType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {innerValueType.Name}");
                }
                else
                {
                    newDict.Add(newKey, Activator.CreateInstance(innerValueType));
                }
            }
        }
    }
}