using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug.Drawers
{
    internal class ListDrawer : IFieldDrawer
    {
        private readonly EditorDrawer _editorDrawer;

        public ListDrawer(EditorDrawer editorDrawer)
        {
            _editorDrawer = editorDrawer;
        }

        public bool TryDraw(
            Type component,
            string fieldName,
            object fieldValue,
            ref int level,
            out object newValue)
        {
            newValue = fieldValue;
            if (fieldValue.GetType().GetGenericTypeDefinition() != typeof(List<>))
                return false;

            var style = DrawerUtility.ContainerStyle(level);
            var value = (IList)fieldValue;
            var count = value.Count;
            if (!EditorDrawerUtility.Foldout(component + fieldName, $"{fieldName} ({count})", style, level))
                return true;

            EditorGUILayout.BeginVertical(style);
            level++;
            for (var i = 0; i < value.Count; i++)
            {
                var v = value[i];
                var memberName = $"{fieldName} [{i}]";
                var changedElement = _editorDrawer.DrawField(component, memberName, v, ref level);
                value[i] = changedElement;
            }

            level--;
            if (GUILayout.Button("Add", EditorStyles.miniButtonMid))
            {
                var innerType = fieldValue.GetType().GetGenericArguments()[0];
                if (!innerType.IsValueType && innerType.GetConstructor(Type.EmptyTypes) == null)
                {
                    UnityEngine.Debug.LogError($"There is no parameterless constructor for {innerType.Name}");
                }
                else
                {
                    value.Add(Activator.CreateInstance(innerType));
                }
            }


            EditorGUILayout.EndVertical();


            return true;
        }
    }
}