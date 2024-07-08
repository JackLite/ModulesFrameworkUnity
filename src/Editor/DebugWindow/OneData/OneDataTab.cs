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
    public class OneDataTab : ISerializationCallbackReceiver
    {
        private Dictionary<Type, OneDataDrawer> _drawers = new();
        private TextField _searchField;

        [SerializeField]
        private string _searchStr;

        [SerializeField]
        private List<string> _pinnedSerializedData = new();

        private HashSet<string> _pinnedData = new();

        private VisualElement _root;
        private ScrollView _scrollView;
        private VisualElement _pinnedContainer;

        public void Draw(VisualElement root)
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanges;
            _root = root;
            _root.AddToClassList("modules--one-data-tab");
            var styles = Resources.Load<StyleSheet>("OneDataTabUSS");
            _root.styleSheets.Add(styles);

            CreateSearch();

            _pinnedContainer = new VisualElement();
            _pinnedContainer.AddToClassList("modules--one-data--pinned-data");
            _root.Add(_pinnedContainer);
            UpdatePinnedContainer();

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

            var clearBtn = new Button
            {
                text = "Clear"
            };
            clearBtn.clicked += () => _searchField.value = string.Empty;
            _searchField.Add(clearBtn);

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

            UpdatePinnedContainer();
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
            var isPinned = _pinnedData.Contains(dataType.Name);
            var parent = isPinned ? _pinnedContainer : _scrollView;
            var dataDrawer = new OneDataDrawer(data, parent);
            if (_drawers.TryGetValue(dataType, out var drawer))
                drawer.Destroy();

            _drawers[dataType] = dataDrawer;
            dataDrawer.SetPinned(isPinned);
            dataDrawer.OnPin += () => OnPin(dataDrawer);
            if (isPinned)
            {
                ResortPinnedDrawers();
                return;
            }

            ResortDrawers();
            FilterOneData(_searchStr);
        }

        private void OnPin(OneDataDrawer dataDrawer)
        {
            var isPinned = _pinnedData.Contains(dataDrawer.DataType.Name);
            if (isPinned)
            {
                _pinnedData.Remove(dataDrawer.DataType.Name);
                _scrollView.Add(dataDrawer.Element);
                ResortDrawers();
            }
            else
            {
                _pinnedData.Add(dataDrawer.DataType.Name);
                _pinnedContainer.Add(dataDrawer.Element);
                ResortPinnedDrawers();
            }

            UpdatePinnedContainer();
            dataDrawer.SetPinned(!isPinned);
        }

        private void OnRemoved(Type dataType)
        {
            if (_drawers.TryGetValue(dataType, out var drawer))
            {
                drawer.Destroy();
                _drawers.Remove(dataType);
            }
        }

        private void ResortDrawers()
        {
            var orderedDrawers = _drawers.Values
                .Where(d => !_pinnedData.Contains(d.DataType.Name))
                .OrderByDescending(d => d.DataType.Name);

            foreach (var dataDrawer in orderedDrawers)
            {
                dataDrawer.SetFirst();
            }
        }

        private void ResortPinnedDrawers()
        {
            var orderedDrawers = _drawers.Values
                .Where(d => _pinnedData.Contains(d.DataType.Name))
                .OrderByDescending(d => d.DataType.Name);
            foreach (var dataDrawer in orderedDrawers)
            {
                dataDrawer.SetFirst();
            }
        }

        private void UpdatePinnedContainer()
        {
            if (_pinnedData.Count > 0 && Application.isPlaying)
                _pinnedContainer.style.display = DisplayStyle.Flex;
            else
                _pinnedContainer.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        public void OnBeforeSerialize()
        {
            _pinnedSerializedData = _pinnedData.ToList();
        }

        public void OnAfterDeserialize()
        {
            _pinnedData = _pinnedSerializedData.ToHashSet();
        }
    }
}