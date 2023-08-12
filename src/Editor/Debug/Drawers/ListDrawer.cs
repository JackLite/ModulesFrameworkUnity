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

            var newList = (IList)Activator.CreateInstance(fieldValue.GetType());
            foreach (var v in value)
                newList.Add(v);

            EditorGUILayout.BeginVertical(style);
            level++;
            for (var i = 0; i < newList.Count; i++)
            {
                var v = newList[i];
                var memberName = $"{fieldName} [{i}]";
                var changedElement = _editorDrawer.DrawField(component, memberName, v, ref level);
                newList[i] = changedElement;
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
                    newList.Add(Activator.CreateInstance(innerType));
                }
            }


            EditorGUILayout.EndVertical();


            newValue = newList;
            return true;
        }
    }
}