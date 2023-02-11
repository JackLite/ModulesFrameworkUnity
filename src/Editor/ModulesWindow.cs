using System;
using ModulesFramework;
using ModulesFrameworkUnity.Settings;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.ModulesUnity.Editor
{
    public class ModulesWindow : EditorWindow
    {
        private ModulesSettings _settings;

        [MenuItem("Modules/Unity Adapter Settings")]
        public new static void Show()
        {
            var window = GetWindow<ModulesWindow>();
            window.LoadSettings();
        }

        private void LoadSettings()
        {
            _settings = ModulesSettings.Load();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(20);
            DrawStartMethod();
            DrawLogType();
            DrawSaveButton();
        }

        private void DrawLogType()
        {
            var label = new GUIContent("Log filter");
            var chosen = EditorGUILayout.EnumFlagsField(label, _settings.logFilter);
            _settings.logFilter = (LogFilter)chosen;
        }

        private void DrawSaveButton()
        {
            var style = EditorStyles.miniButton;
            style.margin = new RectOffset(40, 40, 40, 0);
            style.fixedHeight = 30;
            style.alignment = TextAnchor.MiddleCenter;
            
            if (GUILayout.Button("Save", style))
            {
                _settings.Save();
                UnityEngine.Debug.Log("Saved!");
            }
        }

        private void DrawStartMethod()
        {
            var label = new GUIContent("Start method");
            var chosen = EditorGUILayout.EnumPopup(label, _settings.startMethod);
            _settings.startMethod = (StartMethod)chosen;
        }
    }
}