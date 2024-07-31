using ModulesFramework;
using ModulesFrameworkUnity.Settings;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class ModulesWindow : EditorWindow
    {
        private ModulesSettings _settings;

        [MenuItem("Modules/Settings")]
        private static void ShowWindow()
        {
            var window = GetWindow<ModulesWindow>();
            window.titleContent = new GUIContent("MF Settings");
            window.LoadSettings();
        }

        private void LoadSettings()
        {
            _settings = ModulesSettings.Load();
        }

        private void OnGUI()
        {
            DrawTitle("Main");
            DrawStartMethod();
            DrawUseOldDebug();
            DrawWorldsCount();
            DrawLogType();
            DrawPerformanceSettings();
            DrawCheckEntity();
            DrawSaveButton();
        }

        private void DrawUseOldDebug()
        {
            var isUseOldDebug = _settings.useOldDebug;
            var debugContent = new GUIContent("Use old debug", "Turn on to use old debug based on GameObjects");
            _settings.useOldDebug = EditorGUILayout.Toggle(debugContent, isUseOldDebug);
        }

        private void DrawPerformanceSettings()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginVertical();
            DrawTitle("Performance Monitor");
            var isDebug = _settings.performanceSettings.debugMode;
            var debugContent = new GUIContent("Debug mode", "If turn off - only panic messages will be logging");
            _settings.performanceSettings.debugMode = EditorGUILayout.Toggle(debugContent, isDebug);

            var warn = _settings.performanceSettings.warningAvgFrameMs;
            var warnContent = new GUIContent("Warning Threshold",
                "First threshold. Use it to get warning when code too long but not critical");
            _settings.performanceSettings.warningAvgFrameMs = EditorGUILayout.FloatField(warnContent, warn);

            var panic = _settings.performanceSettings.panicAvgFrameMs;
            var panicContent = new GUIContent("Panic Threshold",
                "This is the critical threshold. " +
                "You should ignore it when scene loading or some expected going on. " +
                "It's good to set it to 10-20% from 1/targeted frame rate.");
            _settings.performanceSettings.panicAvgFrameMs = EditorGUILayout.FloatField(panicContent, panic);

            EditorGUILayout.EndVertical();
        }

        private void DrawCheckEntity()
        {
            EditorGUILayout.Space(20);
            var content = new GUIContent(
                "Delete empty entities",
                "If true - when you remove last component from entity it will be destroyed");
            _settings.deleteEmptyEntities = EditorGUILayout.Toggle(content, _settings.deleteEmptyEntities);
        }

        private void DrawWorldsCount()
        {
            var label = new GUIContent("Worlds count");
            var currentValue = Mathf.Clamp(_settings.worldsCount, 1, int.MaxValue);
            var chosen = EditorGUILayout.IntField(label, currentValue);
            _settings.worldsCount = chosen;
        }

        private void DrawTitle(string blockTitle)
        {
            var labelStyle = EditorStyles.boldLabel;
            EditorGUILayout.LabelField(blockTitle, labelStyle);
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