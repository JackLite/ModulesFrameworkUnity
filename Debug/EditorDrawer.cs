using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class EditorDrawer
    {
        private const int LEFT_DEFAULT = 15;
        private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();
        
        public void DrawField(Type component, string fieldName, object fieldValue, ref int level)
        {
            if (fieldValue == null)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.LabelField($"{fieldName} is null");
                EditorGUILayout.EndVertical();
                return;
            }

            if (TryDrawUnity(fieldName, fieldValue, level))
                return;

            if (TryDrawContainer(component, fieldName, fieldValue, ref level))
                return;

            if (fieldValue is string)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.TextField(fieldName, (string)fieldValue);
                EditorGUILayout.EndVertical();
                return;
            }

            if (fieldValue.GetType().IsEnum)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.EnumPopup(fieldName, (Enum)fieldValue);
                EditorGUILayout.EndVertical();
                return;
            }

            DrawSimple(fieldName, fieldValue, level);
        }

        public bool Foldout(string key, string fieldName, GUIStyle style)
        {
            EditorGUILayout.BeginVertical(style);
            if (!_foldouts.ContainsKey(key))
                _foldouts[key] = false;
            EditorGUILayout.Space(10);
            _foldouts[key] = EditorGUILayout.Foldout(_foldouts[key], fieldName, true, style);
            EditorGUILayout.EndVertical();
            return _foldouts[key];
        }

        private bool TryDrawContainer(Type component, string fieldName, object fieldValue, ref int level)
        {
            if (fieldValue is IDictionary dictionary)
            {
                var style = ContainerStyle(level);
                if (!Foldout(component + fieldName, fieldName, style))
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
                var style = ContainerStyle(level);
                if (!Foldout(component + fieldName, fieldName, style))
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
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.ObjectField(fieldName, (Component)fieldValue, fieldValue.GetType(), true);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Vector3)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.Vector3Field(fieldName, (Vector3)fieldValue);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Vector2)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.Vector2Field(fieldName, (Vector2)fieldValue);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Vector3Int)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.Vector3IntField(fieldName, (Vector3Int)fieldValue);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Vector2Int)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                EditorGUILayout.Vector2IntField(fieldName, (Vector2Int)fieldValue);
                EditorGUILayout.EndVertical();
                return true;
            }
            
            if (fieldValue is Quaternion)
            {
                EditorGUILayout.BeginVertical(OneFieldStyle(level));
                var euler = ((Quaternion)fieldValue).eulerAngles;
                EditorGUILayout.Vector3Field(fieldName, euler);
                EditorGUILayout.EndVertical();
                return true;
            }

            return false;
        }
        
        private void DrawSimple(string fieldName, object fieldValue, int level)
        {
            if (!fieldValue.GetType().IsPrimitive)
                throw new ArgumentException(
                    $"Field {fieldName} is not primitive, but {fieldValue.GetType()}");
            var style = OneFieldStyle(level);
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

        private GUIStyle OneFieldStyle(int level)
        {
            return new GUIStyle
            {
                padding = new RectOffset(LEFT_DEFAULT + 5 * level, 0, 0, 0),
                normal =
                {
                    textColor = Color.white
                }
            };
        }
        
        private GUIStyle ContainerStyle(int level)
        {
            return new GUIStyle
            {
                fontStyle = FontStyle.Italic,
                normal =
                {
                    textColor = Color.white
                },
                padding = new RectOffset(LEFT_DEFAULT + 5 * level, 0, 0, 0)
            };
        }
    }
}