using System;
using ModulesFrameworkUnity.Debug.Entities;
using ModulesFrameworkUnity.DebugWindow.Data;
using ModulesFrameworkUnity.DebugWindow.Modules;
using ModulesFrameworkUnity.DebugWindow.Modules.Data;
using ModulesFrameworkUnity.DebugWindow.OneData;
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
        private OneDataTab _oneDataTab;

        [SerializeField]
        private EntitiesTab _entitiesTab;

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
            if (EditorPrefs.HasKey("MF.ModulesTabMode") && _modulesTabMode == ModulesTabMode.Undefined)
                _modulesTabMode = (ModulesTabMode)EditorPrefs.GetInt("MF.ModulesTabMode");
            _modulesTab ??= new ModulesTab(_modulesTabMode);
            _modulesTab.Show();
            _modulesTab.OnSwitchMode += OnSwitchMode;
            _modulesTab.Hide();
            rootVisualElement.Add(_modulesTab);

            var oneDataRoot = new VisualElement();
            _oneDataTab ??= new OneDataTab();
            rootVisualElement.Add(oneDataRoot);
            _oneDataTab.Draw(oneDataRoot);
            _oneDataTab.Hide();

            var entitiesRoot = new VisualElement();
            _entitiesTab ??= new EntitiesTab();
            rootVisualElement.Add(entitiesRoot);
            _entitiesTab.Draw(entitiesRoot);
            _entitiesTab.Show();

            _tabs ??= new DebugWindowTabs();
            _tabs.Draw(rootVisualElement);
            _tabs.SwitchTab += SwitchTab;
        }

        private void SwitchTab(DebugTabType type)
        {
            switch (type)
            {
                case DebugTabType.Modules:
                    _modulesTab.Show();
                    _oneDataTab.Hide();
                    _entitiesTab.Hide();
                    break;
                case DebugTabType.OneData:
                    _modulesTab.Hide();
                    _oneDataTab.Show();
                    _entitiesTab.Hide();
                    break;
                case DebugTabType.Entities:
                    _modulesTab.Hide();
                    _oneDataTab.Hide();
                    _entitiesTab.Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
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