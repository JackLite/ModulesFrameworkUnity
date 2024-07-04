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
        private ScrollView _scrollView;

        public void Draw(VisualElement root)
        {
            _root = root;
            _root.AddToClassList("modules--entities-tab");
            var styles = Resources.Load<StyleSheet>("EntitiesTabUSS");
            _root.styleSheets.Add(styles);

            _scrollView = new ScrollView();
            _root.Add(_scrollView);

            if (!MF.IsInitialized)
                return;
            CreateViewersForExisted();
        }

        public void Show()
        {
            _root.visible = true;
            EditorApplication.playModeStateChanged += OnPlayModeChanges;
        }

        public void Hide()
        {
            _root.visible = false;
            EditorApplication.playModeStateChanged -= OnPlayModeChanges;
        }

        private void OnPlayModeChanges(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                MF.World.OnEntityCreated += OnCreated;
                // MF.World.OnEntityChanged += OnChanged;
                // MF.World.OnEntityDestroyed += OnDestroyed;
                CreateViewersForExisted();
            }
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

        private void CreateDrawer(Entity entity)
        {
            var drawer = new EntityDrawer();
            drawer.SetEntity(entity);
            drawer.Draw(_scrollView, true);
        }
    }
}