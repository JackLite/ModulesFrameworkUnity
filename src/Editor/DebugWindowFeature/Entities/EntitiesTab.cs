using System;
using System.Collections.Generic;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Entities.EntityBlock;
using ModulesFrameworkUnity.DebugWindowFeature.Utils;
using ModulesFrameworkUnity.EntitiesTags;
using ModulesFrameworkUnity.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.Entities
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
        
        private DataWorld _currentWorld;
        
        public DataWorld CurrentWorld => _currentWorld ?? DebugUtils.GetCurrentWorld();

        public void Draw(VisualElement root, DebugSettings debugSettings)
        {
            _entityDrawer = new();
            _root = root;
            _root.AddToClassList("modules--entities-tab");
            var styles = Resources.Load<StyleSheet>("EntitiesTabUSS");
            _root.styleSheets.Add(styles);


            _dataContainer = new TwoPaneSplitView();
            _dataContainer.fixedPaneInitialDimension = 200;
            _root.Add(_dataContainer);
            _dataContainer.AddToClassList("modules--entities-tab--data");

            _entitiesList.Draw(_dataContainer, debugSettings.entitiesFullName);
            _entitiesList.OnEntitySelected += eid => OnEntitySelected(eid, CurrentWorld);
            DrawSearchField(_entitiesList.componentsFilter);

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

        private void DrawSearchField(string currentFilter)
        {
            _filter = new EntitiesNameFilter();
            _filter.Draw(currentFilter);
            _filter.OnInputChanged += OnFilterName;
            _root.Add(_filter);
            _filter.SendToBack();
        }

        private void OnFilterName(string val)
        {
            if (MF.IsInitialized)
                _entitiesList.FilterByComponent(val);
        }

        public void Show(DataWorld world)
        {
            _currentWorld = world;
            _root.style.display = DisplayStyle.Flex;
            EditorApplication.playModeStateChanged += OnPlayModeChanges;

            _entitiesList.Reset();
            if (!MF.IsInitialized)
                return;
            CreateViewersForExisted(CurrentWorld);

            if (EditorApplication.isPlaying)
                Subscribe(CurrentWorld);
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
            EditorApplication.playModeStateChanged -= OnPlayModeChanges;
            if (EditorApplication.isPlaying)
                Unsubscribe(CurrentWorld);
        }

        private void OnEntitySelected(int eid, DataWorld world)
        {
            var entity = world.GetEntity(eid);
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
                Subscribe(CurrentWorld);
                CreateViewersForExisted(CurrentWorld);
            }
        }

        private void Subscribe(DataWorld world)
        {
            world.OnEntityCreated += OnCreated;
            world.OnEntityChanged += OnChanged;
            world.OnCustomIdChanged += OnCustomIdChanged;
            world.OnEntityDestroyed += OnDestroyed;
            EntitiesTagStorage.Storage.OnTagChanged += _entitiesList.OnTagChanged;
        }

        private void Unsubscribe(DataWorld world)
        {
            world.OnEntityCreated -= OnCreated;
            world.OnEntityChanged -= OnChanged;
            world.OnCustomIdChanged -= OnCustomIdChanged;
            world.OnEntityDestroyed -= OnDestroyed;
            EntitiesTagStorage.Storage.OnTagChanged -= _entitiesList.OnTagChanged;
        }

        private void CreateViewersForExisted(DataWorld currentWorld)
        {
            foreach (var entity in currentWorld.GetAliveEntities())
            {
                CreateDrawer(entity);
            }
        }

        private void OnCreated(int eid)
        {
            CreateDrawer(DebugUtils.GetCurrentWorld().GetEntity(eid));
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
            if (entity.World != DebugUtils.GetCurrentWorld())
                return;

            _entitiesList.AddEntity(entity);
        }

        public void Refresh(DataWorld currentWorld)
        {
            if (!MF.IsInitialized)
                return;
            _entitiesList.Reset();
            _entityDrawer.Destroy();
            CreateViewersForExisted(currentWorld);
        }
    }
}
