using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Common.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Entities;
using ModulesFrameworkUnity.DebugWindowFeature.Modules;
using ModulesFrameworkUnity.DebugWindowFeature.OneDataTab;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;
using ModulesFrameworkUnity.Settings;
using ModulesFrameworkUnity.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Common
{
    public class DebugWindow : EditorWindow
    {
        private DebugWindowTabs _tabs;
        private DebugWindowWorldsWidget _worldsWidget;

        [SerializeField] private OneDataTabView _oneDataTab;
        [SerializeField] private EntitiesTab _entitiesTab;
        [SerializeField] private DebugTabType _currentTab;
        [SerializeField] private string _currentWorldName;


        public DebugTabType CurrentTab { get; private set; }
        public DebugWindowTopBar TopBar { get; private set; }
        public ModulesTab ModulesTab { get; private set; }

        [MenuItem("Modules/Data Viewer")]
        private static void ShowWindow()
        {
            var window = CreateWindow<DebugWindow>();
            window.titleContent = new GUIContent("MF Data Viewer");
            window.Show();
            UnityEngine.Debug.Log("DebugWindow opened");
            
        }

        private void OnEnable()
        {
            var styleSheet = Resources.Load<StyleSheet>("Modules.DebugWindow");
            rootVisualElement.styleSheets.Add(styleSheet);
            _worldsWidget = new DebugWindowWorldsWidget();

            var debugSettings = ModulesSettings.Load().debugSettings;
            hideFlags = HideFlags.HideAndDontSave;
            ModulesTab ??= new ModulesTab();
            rootVisualElement.Add(ModulesTab);

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
            UnityEngine.Debug.Log("DebugWindow enabled");
            var test = new LinkedDictionary<string, int>();
            test.Add("1", 1);
            test.Add("3", 3);
            test.Add("2", 2);
            test.Add("0", 0);
            foreach (var item in test.Values)
            {
                UnityEngine.Debug.Log(item); 
            }
            test.Sort((n1, n2) => n1.CompareTo(n2));
            foreach (var item in test.Values)
            {
                UnityEngine.Debug.Log(item);
            }
        }

        private void DrawTopBar()
        {
            TopBar = new DebugWindowTopBar();
            rootVisualElement.Add(TopBar);
            TopBar.Draw();

            TopBar.OnWorldChanged += worldName =>
            {
                _currentWorldName = worldName;
                _entitiesTab.Refresh(GetWorld());
                _oneDataTab.Refresh();
                ModulesTab.Refresh();
            };
            TopBar.OnSwitchTab += SwitchTab;
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
                    ModulesTab.Hide();
                    _oneDataTab.Hide();
                    _entitiesTab.Show(GetWorld());
                    break;
                case DebugTabType.OneData:
                    ModulesTab.Hide();
                    _oneDataTab.Show();
                    _entitiesTab.Hide();
                    break;
                case DebugTabType.Modules:
                default:
                    ModulesTab.Show();
                    _oneDataTab.Hide();
                    _entitiesTab.Hide();
                    break;
            }
        }

        private void OnDisable()
        {
            ModulesTab.Hide();
            _oneDataTab.Hide();
            _entitiesTab.Hide();
            rootVisualElement.Clear();
        }

        public DataWorld GetWorld()
        {
            if (!MF.IsInitialized)
                return null;

            if (_currentWorldName == null)
                return MF.World;

            return MF.IsWorldExists(_currentWorldName) ? MF.GetWorld(_currentWorldName) : MF.World;
        }

        public void ChooseEntity(Entity entity)
        {
            if (_currentTab != DebugTabType.Entities)
                SwitchTab(DebugTabType.Entities);
            _entitiesTab.ChooseEntity(entity);
        }
    }
}