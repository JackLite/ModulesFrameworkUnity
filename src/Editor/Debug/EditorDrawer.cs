using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Data;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModulesFrameworkUnity.Debug
{
    public class EditorDrawer
    {
        private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();
        
        public void DrawField(Type component, string fieldName, object fieldValue, ref int level)
        {
            if (fieldValue == null)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.LabelField($"{fieldName} is null");
                EditorGUILayout.EndVertical();
                return;
            }

            if (CustomDrawers.TryDraw(fieldName, fieldValue, ref level))
                return;

            if (TryDrawUnity(fieldName, fieldValue, level))
                return;

            if (fieldValue is string)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.TextField(fieldName, (string)fieldValue);
                EditorGUILayout.EndVertical();
                return;
            }

            if (TryDrawContainer(component, fieldName, fieldValue, ref level))
                return;

            if (fieldValue.GetType().IsEnum)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.EnumPopup(fieldName, (Enum)fieldValue);
                EditorGUILayout.EndVertical();
                return;
            }

            if (TryDrawStruct(component.FullName + fieldName, fieldName, fieldValue, ref level))
                return;
            
            DrawSimple(fieldName, fieldValue, level);
        }

        public bool Foldout(string key, string fieldName, GUIStyle style, int level)
        {
            EditorGUILayout.BeginVertical(style);
            if (!_foldouts.ContainsKey(key))
                _foldouts[key] = level == -1;
            EditorGUILayout.Space(10);
            _foldouts[key] = EditorGUILayout.Foldout(_foldouts[key], fieldName, true, style);
            EditorGUILayout.EndVertical();
            return _foldouts[key];
        }

        private bool TryDrawStruct(string key, string fieldName, object fieldValue, ref int level)
        {
            if (!fieldValue.GetType().IsValueType || fieldValue.GetType().IsPrimitive)
                return false;
            
            var style = DrawerUtility.ContainerStyle(level);
            if (!Foldout(key, fieldName, style, level))
                return true;
            if (fieldValue is Entity entity)
            {
                DrawSimple(fieldName, entity.Id, level);
                return true;
            }

            EditorGUILayout.BeginVertical(style);
            level++;
            foreach (var fieldInfo in fieldValue.GetType().GetFields())
            {
                var innerFieldValue = fieldInfo.GetValue(fieldValue);
                DrawField(fieldInfo.FieldType, fieldInfo.Name, innerFieldValue, ref level);
            }
            level--;
            EditorGUILayout.EndVertical();
            return true;
        }

        private bool TryDrawContainer(Type component, string fieldName, object fieldValue, ref int level)
        {
            if (fieldValue is IDictionary dictionary)
            {
                var style = DrawerUtility.ContainerStyle(level);
                if (!Foldout(component + fieldName, fieldName, style, level))
                    return true;
                EditorGUILayout.BeginVertical(style);
                var keysArr = dictionary.Keys.Cast<object>().ToArray();
                var valuesArr = dictionary.Values.Cast<object>().ToArray();
                level++;
                for (var i = 0; i < keysArr.Length; ++i)
                {
                    var key = keysArr[i];
                    var val = valuesArr[i];
                    var memberName = $"{fieldName} [{key}]";
                    DrawField(component, memberName, val, ref level);
                }
                level--;
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is IEnumerable enumerable)
            {
                var style = DrawerUtility.ContainerStyle(level);
                if (!Foldout(component + fieldName, fieldName, style, level))
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

        private bool TryDrawUnity(string fieldName, object fieldValue, int level)
        {
            if (fieldValue.GetType().IsSubclassOf(typeof(Component)))
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.ObjectField(fieldName, (Component)fieldValue, fieldValue.GetType(), true);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue.GetType().IsSubclassOf(typeof(Object)))
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.ObjectField(fieldName, (Object)fieldValue, fieldValue.GetType(), true);
                EditorGUILayout.EndVertical();
                return true;
            }

            if (fieldValue is Vector3 value)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.Vector3Field(fieldName, value);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Vector2 vector2)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.Vector2Field(fieldName, vector2);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Vector3Int vector3Int)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.Vector3IntField(fieldName, vector3Int);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Vector2Int vector2Int)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                EditorGUILayout.Vector2IntField(fieldName, vector2Int);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Quaternion quaternion)
            {
                EditorGUILayout.BeginVertical(DrawerUtility.OneFieldStyle(level));
                var euler = quaternion.eulerAngles;
                EditorGUILayout.Vector3Field(fieldName, euler);
                EditorGUILayout.EndVertical();
                return true;
            }

            return false;
        }
        
        private void DrawSimple(string fieldName, object fieldValue, int level)
        {
            if (!fieldValue.GetType().IsPrimitive)
            {
                UnityEngine.Debug.LogWarning($"Type {fieldValue.GetType()} is not supported yet");
                return;
            }

            var style = DrawerUtility.OneFieldStyle(level);
            EditorGUILayout.BeginVertical(style);
            switch (fieldValue)
            {
                case int value:
                    EditorGUILayout.IntField(fieldName, value);
                    break;
                case long value:
                    EditorGUILayout.LongField(fieldName, value);
                    break;
                case float value:
                    EditorGUILayout.FloatField(fieldName, value);
                    break;
                case double value:
                    EditorGUILayout.DoubleField(fieldName, value);
                    break;
                case bool value:
                    EditorGUILayout.Toggle(fieldName, value);
                    break;
            }
            EditorGUILayout.EndVertical();

        }

    }
}