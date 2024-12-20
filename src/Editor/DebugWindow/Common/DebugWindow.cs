using ModulesFrameworkUnity.Debug.Entities;
using ModulesFrameworkUnity.DebugWindow.Data;
using ModulesFrameworkUnity.DebugWindow.Modules;
using ModulesFrameworkUnity.DebugWindow.Modules.Data;
using ModulesFrameworkUnity.DebugWindow.OneDataTab;
using ModulesFrameworkUnity.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class DebugWindow : EditorWindow
    {
        private DebugWindowTabs _tabs;
        private ModulesTab _modulesTab;

        [SerializeField]
        private OneDataTabView _oneDataTab;

        [SerializeField]
        private EntitiesTab _entitiesTab;

        [SerializeField]
        private ModulesTabMode _modulesTabMode;

        [SerializeField]
        private DebugTabType _currentTab;

        [MenuItem("Modules/Data Viewer")]
        private static void ShowWindow()
        {
            var window = CreateWindow<DebugWindow>();
            window.titleContent = new GUIContent("MF Data Viewer");
            window.Show();
        }

        private void OnEnable()
        {
            var debugSettings = ModulesSettings.Load().debugSettings;
            hideFlags = HideFlags.HideAndDontSave;
            if (EditorPrefs.HasKey("MF.ModulesTabMode") && _modulesTabMode == ModulesTabMode.Undefined)
                _modulesTabMode = (ModulesTabMode)EditorPrefs.GetInt("MF.ModulesTabMode");
            _modulesTab ??= new ModulesTab(_modulesTabMode);
            _modulesTab.OnSwitchMode += OnSwitchMode;
            rootVisualElement.Add(_modulesTab);

            var oneDataRoot = new VisualElement();
            _oneDataTab ??= new OneDataTabView();
            rootVisualElement.Add(oneDataRoot);
            _oneDataTab.Draw(oneDataRoot);
            _oneDataTab.Hide();

            var entitiesRoot = new VisualElement();
            _entitiesTab ??= new EntitiesTab();
            rootVisualElement.Add(entitiesRoot);
            _entitiesTab.Draw(entitiesRoot, debugSettings);

            _tabs ??= new DebugWindowTabs();
            _tabs.Draw(rootVisualElement);
            _tabs.SwitchTab += SwitchTab;

            ShowTab(_currentTab);
        }

        private void SwitchTab(DebugTabType type)
        {
            if (_currentTab == type)
                return;
            _currentTab = type;
            ShowTab(type);
        }

        private void ShowTab(DebugTabType type)
        {
            switch (type)
            {
                case DebugTabType.Entities:
                    _modulesTab.Hide();
                    _oneDataTab.Hide();
                    _entitiesTab.Show();
                    break;
                case DebugTabType.OneData:
                    _modulesTab.Hide();
                    _oneDataTab.Show();
                    _entitiesTab.Hide();
                    break;
                case DebugTabType.Modules:
                default:
                    _modulesTab.Show();
                    _oneDataTab.Hide();
                    _entitiesTab.Hide();
                    break;
            }
        }

        private void OnSwitchMode(ModulesTabMode mode)
        {
            _modulesTabMode = mode;
            EditorPrefs.SetInt("MF.ModulesTabMode", (int)mode);
        }

        private void OnDisable()
        {
            _modulesTab.Hide();
            _oneDataTab.Hide();
            _entitiesTab.Hide();
            _modulesTab.OnSwitchMode -= OnSwitchMode;
            rootVisualElement.Clear();
        }
    }
}