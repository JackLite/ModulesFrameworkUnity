using System;
using System.Linq;
using ModulesFramework;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Entities;
using ModulesFrameworkUnity.Debug.Utils;
using ModulesFrameworkUnity.DebugWindow.Data;
using ModulesFrameworkUnity.DebugWindow.Modules;
using ModulesFrameworkUnity.DebugWindow.Modules.Data;
using ModulesFrameworkUnity.DebugWindow.OneDataTab;
using ModulesFrameworkUnity.Settings;
using ModulesFrameworkUnity.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class DebugWindow : EditorWindow
    {
        private DebugWindowTabs _tabs;
        private DebugWindowWorldsWidget _worldsWidget;
        private ModulesTab _modulesTab;

        [SerializeField]
        private OneDataTabView _oneDataTab;

        [SerializeField]
        private EntitiesTab _entitiesTab;

        [SerializeField]
        private ModulesTabMode _modulesTabMode;

        [SerializeField]
        private DebugTabType _currentTab;

        [SerializeField]
        private string _currentWorldName;

        [MenuItem("Modules/Data Viewer")]
        private static void ShowWindow()
        {
            var window = CreateWindow<DebugWindow>();
            window.titleContent = new GUIContent("MF Data Viewer");
            window.Show();
        }

        private void OnEnable()
        {
            var styleSheet = Resources.Load<StyleSheet>("Modules.DebugWindow");
            rootVisualElement.styleSheets.Add(styleSheet);

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

            DrawWorldsWidget();

            ShowTab(_currentTab);
        }

        private void Update()
        {
            if (_currentWorldName != DebugUtils.GetCurrentWorldName())
                _worldsWidget.value = DebugUtils.GetCurrentWorldName();
        }

        private void DrawWorldsWidget()
        {
            _worldsWidget ??= new DebugWindowWorldsWidget();
            var allWorlds = DebugUtils.GetAllWorldNames();
            _worldsWidget.Init(allWorlds, DebugUtils.GetCurrentWorldName());
            _worldsWidget.RegisterValueChangedCallback(ev =>
            {
                DebugUtils.SetCurrentModule(ev.newValue);
                _currentWorldName = ev.newValue;
                _entitiesTab.Refresh();
                _oneDataTab.Refresh();
                _modulesTab.Refresh();
            });
            rootVisualElement.Add(_worldsWidget);
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
