using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.EntitiesTags;
using ModulesFrameworkUnity.Settings;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Tab with entities
    /// </summary>
    [Serializable]
    public class EntitiesTab
    {
        private VisualElement _root;
        private TwoPaneSplitView _dataContainer;

        private EntitiesList _entitiesList = new();
        private EntityDrawer _entityDrawer;
        private EntitiesNameFilter _filter;
        private EntityDrawerSettings _settings;

        [SerializeField]
        private List<string> _pinnedComponents = new();

        public void Draw(VisualElement root, DebugSettings debugSettings)
        {
            _entityDrawer = new();
            _root = root;
            _root.AddToClassList("modules--entities-tab");
            var styles = Resources.Load<StyleSheet>("EntitiesTabUSS");
            _root.styleSheets.Add(styles);

            _filter = new EntitiesNameFilter();
            _filter.Draw();
            _root.Add(_filter);
            _filter.OnInputChanged += OnFilterName;

            _dataContainer = new TwoPaneSplitView();
            _dataContainer.fixedPaneInitialDimension = 200;
            _root.Add(_dataContainer);
            _dataContainer.AddToClassList("modules--entities-tab--data");

            _entitiesList.Draw(_dataContainer, debugSettings.entitiesFullName);
            _entitiesList.OnEntitySelected += OnEntitySelected;

            _settings ??= new EntityDrawerSettings();
            _settings.Draw(_entityDrawer);

            _dataContainer.Add(_entityDrawer);
            _entityDrawer.style.flexBasis = new StyleLength(StyleKeyword.Null);
            _entityDrawer.OnPinComponent += components =>
            {
                _pinnedComponents.Clear();
                _pinnedComponents.AddRange(components);
            };
        }

        private void OnFilterName(string val)
        {
            if (MF.IsInitialized)
                _entitiesList.FilterByComponent(val);
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
            EditorApplication.playModeStateChanged += OnPlayModeChanges;

            _entitiesList.Reset();
            if (!MF.IsInitialized)
                return;
            CreateViewersForExisted();

            if (EditorApplication.isPlaying)
                Subscribe();
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
            EditorApplication.playModeStateChanged -= OnPlayModeChanges;
            if (EditorApplication.isPlaying)
                Unsubscribe();
        }

        private void OnEntitySelected(int eid)
        {
            var entity = MF.World.GetEntity(eid);
            // no support multiple worlds for now
            if (!entity.IsAlive())
                return;

            _entityDrawer.Destroy();
            _entityDrawer.SetEntity(entity);
            _entityDrawer.Draw(_settings.IsAllOpen, _pinnedComponents);
        }

        private void OnPlayModeChanges(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                Subscribe();
                CreateViewersForExisted();
            }
        }

        private void Subscribe()
        {
            MF.World.OnEntityCreated += OnCreated;
            MF.World.OnEntityChanged += OnChanged;
            MF.World.OnCustomIdChanged += OnCustomIdChanged;
            MF.World.OnEntityDestroyed += OnDestroyed;
            EntitiesTagStorage.Storage.OnTagChanged += _entitiesList.OnTagChanged;
        }

        private void Unsubscribe()
        {
            MF.World.OnEntityCreated -= OnCreated;
            MF.World.OnEntityChanged -= OnChanged;
            MF.World.OnCustomIdChanged -= OnCustomIdChanged;
            MF.World.OnEntityDestroyed -= OnDestroyed;
            EntitiesTagStorage.Storage.OnTagChanged -= _entitiesList.OnTagChanged;
        }

        private void CreateViewersForExisted()
        {
            foreach (var entity in MF.World.GetAliveEntities())
            {
                CreateDrawer(entity);
            }
        }

        private void OnCreated(int eid)
        {
            CreateDrawer(MF.World.GetEntity(eid));
        }

        private void OnChanged(int eid)
        {
            _entitiesList.OnEntityChanged(eid);
        }

        private void OnCustomIdChanged(int eid)
        {
            _entitiesList.OnCustomIdChanged(eid);
        }

        private void OnDestroyed(int eid)
        {
            _entitiesList.RemoveEntity(eid);
        }

        private void CreateDrawer(Entity entity)
        {
            // only main world for now
            if (entity.World != MF.World)
                return;

            _entitiesList.AddEntity(entity);
        }
    }
}