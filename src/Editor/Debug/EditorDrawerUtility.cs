using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public static class EditorDrawerUtility
    {
        private static readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();
        public static bool Foldout(string key, string fieldName, GUIStyle style, int level)
        {
            EditorGUILayout.BeginVertical(style);
            if (!_foldouts.ContainsKey(key))
                _foldouts[key] = level == -1;
            EditorGUILayout.Space(10);
            _foldouts[key] = EditorGUILayout.Foldout(_foldouts[key], fieldName, true, style);
            EditorGUILayout.EndVertical();
            return _foldouts[key];
        }
    }
}