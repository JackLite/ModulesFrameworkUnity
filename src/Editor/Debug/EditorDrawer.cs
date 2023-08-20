using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug.Drawers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModulesFrameworkUnity.Debug
{
    public class EditorDrawer
    {
        private Dictionary<Type, IFieldDrawer> _drawers = new();

        public EditorDrawer()
        {
            _drawers.Add(typeof(IList), new ListDrawer(this));
            _drawers.Add(typeof(Array), new ArrayDrawer(this));
            _drawers.Add(typeof(IDictionary), new DictionaryDrawer(this));
        }
        
        public object DrawField(Type component, string fieldName, object fieldValue, ref int level)
        {
            object newValue = fieldValue;
            if (fieldValue == null)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.LabelField($"{fieldName} is null");
                EditorGUILayout.EndVertical();
                return newValue;
            }

            if (CustomDrawers.TryDraw(fieldName, fieldValue, ref level, out newValue))
                return newValue;

            if (TryDrawUnity(fieldName, fieldValue, level, out newValue))
                return newValue;

            if (fieldValue is string)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.TextField(fieldName, (string)fieldValue);
                EditorGUILayout.EndVertical();
                return newValue;
            }

            if (TryDrawContainer(component, fieldName, fieldValue, ref level, out newValue))
                return newValue;

            if (fieldValue.GetType().IsEnum)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.EnumPopup(fieldName, (Enum)fieldValue);
                EditorGUILayout.EndVertical();
                return newValue;
            }

            if (TryDrawEntity(fieldName, fieldValue, ref level))
                return newValue;

            if (TryDrawStruct(component.FullName + fieldName, fieldName, fieldValue, ref level, out newValue))
                return newValue;

            newValue = DrawSimple(fieldName, fieldValue, level);
            return newValue;
        }

        private bool TryDrawEntity(string fieldName, object fieldValue, ref int level)
        {
            if (fieldValue is not Entity e)
                return false;
            EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
            EditorGUILayout.LabelField($"{fieldName} [entity: {e.Id}]");
            EditorGUILayout.EndVertical();
            return true;
        }

        private bool TryDrawStruct(string key, string fieldName, object structValue, ref int level,
            out object newValue)
        {
            newValue = structValue;
            if (!structValue.GetType().IsValueType || structValue.GetType().IsPrimitive)
                return false;

            var style = DrawerUtility.ContainerStyle(level);
            if (!EditorDrawerUtility.Foldout(key, fieldName, style, level))
                return true;
            EditorGUILayout.BeginVertical(style);
            level++;
            foreach (var fieldInfo in structValue.GetType().GetFields())
            {
                var innerFieldValue = fieldInfo.GetValue(structValue);
                var newFieldValue = DrawField(fieldInfo.FieldType, fieldInfo.Name, innerFieldValue, ref level);
                fieldInfo.SetValue(structValue, newFieldValue);
            }

            level--;
            EditorGUILayout.EndVertical();
            return true;
        }

        private bool TryDrawContainer(
            Type component,
            string fieldName,
            object fieldValue,
            ref int level,
            out object newValue)
        {
            newValue = fieldValue;
            if (fieldValue is IDictionary)
            {
                return _drawers[typeof(IDictionary)].TryDraw(component, fieldName, fieldValue, ref level, out newValue);
            }

            if (fieldValue is Array)
            {
                return _drawers[typeof(Array)].TryDraw(component, fieldName, fieldValue, ref level, out newValue);
            }
            
            if (fieldValue is IList)
            {
                return _drawers[typeof(IList)].TryDraw(component, fieldName, fieldValue, ref level, out newValue);
            }

            if (fieldValue is IEnumerable enumerable)
            {
                var style = DrawerUtility.ContainerStyle(level);
                var count = enumerable.Cast<object>().Count();
                if (!EditorDrawerUtility.Foldout(component + fieldName, $"{fieldName} ({count})", style, level))
                    return true;
                EditorGUILayout.BeginVertical(style);
                var index = 0;
                level++;
                foreach (var v in enumerable)
                {
                    var memberName = $"{fieldName} [{index}]";
                    DrawField(component, memberName, v, ref level);
                    index++;
                }

                EditorGUILayout.EndVertical();
                level--;
                return true;
            }

            return false;
        }

        private bool TryDrawUnity(string fieldName, object fieldValue, int level, out object newValue)
        {
            newValue = fieldValue;
            if (fieldValue.GetType().IsSubclassOf(typeof(Component)))
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.ObjectField(fieldName, (Component)fieldValue, fieldValue.GetType(), true);
                EditorGUILayout.EndVertical();
                return true;
            }

            if (fieldValue.GetType().IsSubclassOf(typeof(Object)))
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.ObjectField(fieldName, (Object)fieldValue, fieldValue.GetType(), true);
                EditorGUILayout.EndVertical();
                return true;
            }

            if (fieldValue is Vector3 value)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.Vector3Field(fieldName, value);
                EditorGUILayout.EndVertical();
                return true;
            }

            if (fieldValue is Vector2 vector2)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.Vector2Field(fieldName, vector2);
                EditorGUILayout.EndVertical();
                return true;
            }

            if (fieldValue is Vector3Int vector3Int)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.Vector3IntField(fieldName, vector3Int);
                EditorGUILayout.EndVertical();
                return true;
            }

            if (fieldValue is Vector2Int vector2Int)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                newValue = EditorGUILayout.Vector2IntField(fieldName, vector2Int);
                EditorGUILayout.EndVertical();
                return true;
            }

            if (fieldValue is Quaternion quaternion)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                var euler = quaternion.eulerAngles;
                newValue = EditorGUILayout.Vector3Field(fieldName, euler);
                EditorGUILayout.EndVertical();
                return true;
            }

            return false;
        }

        public object DrawSimple(string fieldName, object fieldValue, int level)
        {
            if (!fieldValue.GetType().IsPrimitive)
            {
                var message = $"Type {fieldValue.GetType().Name} is not supported";
                EditorGUILayout.TextField(fieldName, message);
                return fieldValue;
            }

            var style = DrawerUtility.OneFieldStyle(level);
            object result = null;
            EditorGUILayout.BeginVertical(style);
            switch (fieldValue)
            {
                case int value:
                    result = EditorGUILayout.IntField(fieldName, value);
                    break;
                case long value:
                    result = EditorGUILayout.LongField(fieldName, value);
                    break;
                case float value:
                    result = EditorGUILayout.FloatField(fieldName, value);
                    break;
                case double value:
                    result = EditorGUILayout.DoubleField(fieldName, value);
                    break;
                case bool value:
                    result = EditorGUILayout.Toggle(fieldName, value);
                    break;
            }

            EditorGUILayout.EndVertical();

            return result;
        }
    }
}