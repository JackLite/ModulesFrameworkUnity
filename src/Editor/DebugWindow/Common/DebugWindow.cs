using System;
using System.Linq;
using ModulesFramework;
using ModulesFramework.Data;
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
        private DebugWindowTopBar _topBar;

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

        public DataWorld CurrentWorld { get; private set; }

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
            _worldsWidget = new DebugWindowWorldsWidget();

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

            DrawTopBar();
            ShowTab(_currentTab);
        }

        private void DrawTopBar()
        {
            _topBar = new DebugWindowTopBar();
            rootVisualElement.Add(_topBar);
            _topBar.Draw();

            _topBar.OnWorldChanged += worldName =>
            {
                _currentWorldName = worldName;
                if (EditorApplication.isPlaying)
                    CurrentWorld = DebugUtils.GetWorld(worldName);
                _topBar.Refresh(CurrentWorld);
                _entitiesTab.Refresh(CurrentWorld);
                _oneDataTab.Refresh();
                _modulesTab.Refresh();
            };
            _topBar.OnSwitchTab += SwitchTab;
        }

        private void Update()
        {
            if (_currentWorldName != DebugUtils.GetCurrentWorldName())
                _worldsWidget.value = DebugUtils.GetCurrentWorldName();
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
                    _entitiesTab.Show(CurrentWorld);
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