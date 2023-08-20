using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug.Drawers
{
    public class ArrayDrawer : IFieldDrawer
    {
        private readonly EditorDrawer _editorDrawer;

        public ArrayDrawer(EditorDrawer editorDrawer)
        {
            _editorDrawer = editorDrawer;
        }

        public bool TryDraw(Type component, string fieldName, object fieldValue, ref int level,
            out object newValue)
        {
            newValue = fieldValue;
            if (fieldValue is not Array value)
                return false;

            var style = DrawerUtility.ContainerStyle(level);
            var count = value.Length;
            if (!EditorDrawerUtility.Foldout(component + fieldName, $"{fieldName} ({count})", style, level))
                return true;

            var innerType = value.GetType().GetElementType();
            var newArr = Array.CreateInstance(innerType, value.Length);
            Array.Copy(value, newArr, value.Length);

            EditorGUILayout.BeginVertical(style);
            level++;
            for (var i = 0; i < newArr.Length; i++)
            {
                var v = newArr.GetValue(i);
                var memberName = $"{fieldName} [{i}]";
                var changedElement = _editorDrawer.DrawField(component, memberName, v, ref level);
                newArr.SetValue(changedElement, i);
            }

            level--;
            if (GUILayout.Button("Add", EditorStyles.miniButtonMid))
            {
                if (!innerType.IsValueType && innerType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {innerType.Name}");
                }
                else
                {
                    var tempArr = Array.CreateInstance(innerType, newArr.Length + 1);
                    Array.Copy(newArr, tempArr, newArr.Length);
                    newArr = tempArr;
                    newArr.SetValue(Activator.CreateInstance(innerType), newArr.Length - 1);
                }
            }

            EditorGUILayout.EndVertical();

            newValue = newArr;
            return true;
        }
    }
}