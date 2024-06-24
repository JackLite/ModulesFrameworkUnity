using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneData
{
    /// <summary>
    ///     List of one data-s
    /// </summary>
    [Serializable]
    public class OneDataTab
    {
        private Dictionary<Type, OneDataDrawer> _drawers = new();
        private TextField _searchField;

        [SerializeField]
        private string _searchStr;

        [SerializeField]
        private List<string> _pinnedData;

        private VisualElement _root;
        private ScrollView _scrollView;

        public void Draw(VisualElement root)
        {
            _root = root;
            _root.AddToClassList("modules-one-data-tab");
            var styles = Resources.Load<StyleSheet>("OneDataTabUSS");
            _root.styleSheets.Add(styles);

            CreateSearch();

            _scrollView = new ScrollView();
            _root.Add(_scrollView);
            _drawers.Clear();
            if (!MF.IsInitialized)
                return;
            CreateViewersForExisted();
        }

        private void CreateSearch()
        {
            _searchField = new TextField
            {
                label = "Filter",
                value = _searchStr
            };
            _searchField.AddToClassList("modules-search-field");
            _searchField.RegisterValueChangedCallback(ev => OnSearch(ev.newValue));
            _root.Add(_searchField);
        }

        private void OnSearch(string searchStr)
        {
            _searchStr = searchStr;
            FilterOneData(_searchStr);
        }

        private void FilterOneData(string searchStr)
        {
            if (string.IsNullOrWhiteSpace(searchStr))
            {
                foreach (var (_, dataDrawer) in _drawers)
                {
                    dataDrawer.SetVisible(true);
                }

                return;
            }

            foreach (var (_, dataDrawer) in _drawers)
            {
                var isMatch = dataDrawer.DataType.Name.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase);
                dataDrawer.SetVisible(isMatch);
            }
        }

        private void OnPlayModeChanges(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                CreateViewersForExisted();
            }
        }

        private void CreateViewersForExisted()
        {
            MF.World.OnOneDataCreated += OnCreated;
            MF.World.OnOneDataRemoved += OnRemoved;
            foreach (var data in MF.World.OneDataCollection)
            {
                OnCreated(data.GetDataObject().GetType(), data);
            }

            FilterOneData(_searchStr);
        }

        private void OnCreated(Type dataType, ModulesFramework.OneData data)
        {
            var dataDrawer = new OneDataDrawer(data, _scrollView);
            _drawers.Add(dataType, dataDrawer);
            ResortDrawers();
        }

        private void OnRemoved(Type dataType)
        {
            if (_drawers.TryGetValue(dataType, out var drawer))
            {
                drawer.Destroy();
                _drawers.Remove(dataType);
            }

            ResortDrawers();
        }

        private void ResortDrawers()
        {
            foreach (var dataDrawer in _drawers.Values.OrderByDescending(d => d.DataType.Name))
            {
                dataDrawer.SetFirst();
            }
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
    }
}