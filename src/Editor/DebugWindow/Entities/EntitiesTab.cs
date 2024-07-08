using System;
using System.Collections.Generic;
using ModulesFramework;
using ModulesFramework.Data;
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
        private List<Type> _withFilter;

        private VisualElement _root;
        private VisualElement _dataContainer;
        private ScrollView _scrollView;

        private EntitiesList _entitiesList = new();
        private EntityDrawer _entityDrawer;

        public void Draw(VisualElement root)
        {
            _entityDrawer = new();
            _root = root;
            _root.AddToClassList("modules--entities-tab");
            var styles = Resources.Load<StyleSheet>("EntitiesTabUSS");
            _root.styleSheets.Add(styles);

            _dataContainer = new VisualElement();
            _root.Add(_dataContainer);
            _dataContainer.AddToClassList("modules--entities-tab--data");

            _entitiesList.Draw(_dataContainer);
            _entitiesList.OnEntitySelected += OnEntitySelected;

            _dataContainer.Add(_entityDrawer);

            if (!MF.IsInitialized)
                return;
            CreateViewersForExisted();
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
            EditorApplication.playModeStateChanged += OnPlayModeChanges;

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
            _entityDrawer.Draw();
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
            MF.World.OnEntityDestroyed += OnDestroyed;
        }

        private void Unsubscribe()
        {
            MF.World.OnEntityCreated -= OnCreated;
            MF.World.OnEntityDestroyed -= OnDestroyed;
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

        private void OnDestroyed(int eid)
        {
            _entitiesList.RemoveEntity(eid);
        }

        private void CreateDrawer(Entity entity)
        {
            // only main world for now
            if (entity.World != MF.World)
                return;

            _entitiesList.AddEntity(entity.Id);
        }
    }
}