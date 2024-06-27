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
        private List<string> _pinnedData = new();

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
                var isPinned = _pinnedData.Contains(dataDrawer.DataType.Name);
                dataDrawer.SetVisible(isMatch || isPinned);
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
        }

        private void OnCreated(Type dataType, ModulesFramework.OneData data)
        {
            var dataDrawer = new OneDataDrawer(data, _scrollView);
            if (_drawers.TryGetValue(dataType, out var drawer))
                drawer.Destroy();

            _drawers[dataType] = dataDrawer;
            dataDrawer.SetPinned(_pinnedData.Contains(dataType.Name));
            dataDrawer.OnPin += () => OnPin(dataDrawer);
            ResortDrawers();
            FilterOneData(_searchStr);
        }

        private void OnPin(OneDataDrawer dataDrawer)
        {
            var isPinned = _pinnedData.Contains(dataDrawer.DataType.Name);
            if (isPinned)
                _pinnedData.Remove(dataDrawer.DataType.Name);
            else
                _pinnedData.Add(dataDrawer.DataType.Name);
            dataDrawer.SetPinned(!isPinned);
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
            var orderedDrawers = _drawers.Values
                .OrderBy(d => _pinnedData.Contains(d.DataType.Name))
                .ThenByDescending(d => d.DataType.Name);
            foreach (var dataDrawer in orderedDrawers)
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