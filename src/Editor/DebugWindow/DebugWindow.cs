using ModulesFrameworkUnity.DebugWindow.Modules;
using ModulesFrameworkUnity.DebugWindow.Modules.Data;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class DebugWindow : EditorWindow
    {
        private DebugWindowTabs _tabs;
        private ModulesTab _modulesTab;

        [SerializeField]
        private ModulesTabMode _modulesTabMode;

        [MenuItem("Modules/Data Viewer")]
        private static void ShowWindow()
        {
            var window = CreateWindow<DebugWindow>();
            window.titleContent = new GUIContent("MF Data Viewer");
            window.Show();
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
            _tabs ??= new DebugWindowTabs();
            if (EditorPrefs.HasKey("MF.ModulesTabMode") && _modulesTabMode == ModulesTabMode.Undefined)
                _modulesTabMode = (ModulesTabMode)EditorPrefs.GetInt("MF.ModulesTabMode");
            _modulesTab ??= new ModulesTab(_modulesTabMode);
            _modulesTab.Show(rootVisualElement);
            _modulesTab.OnSwitchMode += OnSwitchMode;
            _tabs.Draw(rootVisualElement);
        }

        private void OnSwitchMode(ModulesTabMode mode)
        {
            _modulesTabMode = mode;
            EditorPrefs.SetInt("MF.ModulesTabMode", (int) mode);
        }

        private void OnDisable()
        {
            _modulesTab.Hide();
            _modulesTab.OnSwitchMode -= OnSwitchMode;
            rootVisualElement.Clear();
        }
    }
}