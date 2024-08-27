using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneDataTab
{
    /// <summary>
    ///     List of one data-s
    /// </summary>
    [Serializable]
    public class OneDataTabView : ISerializationCallbackReceiver
    {
        private Dictionary<Type, OneDataDrawer> _drawers = new();

        [SerializeField]
        private string _searchStr;

        [SerializeField]
        private List<string> _pinnedSerializedData = new();

        private HashSet<string> _pinnedData = new();

        private VisualElement _root;
        private OneDataList _list;
        private ScrollView _drawersRoot;

        public void Draw(VisualElement root)
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanges;
            _root = root;
            _root.AddToClassList("modules--one-data-tab");
            var styles = Resources.Load<StyleSheet>("OneDataTabUSS");
            _root.styleSheets.Add(styles);

            var dataContainer = new TwoPaneSplitView();
            dataContainer.fixedPaneInitialDimension = 200;
            dataContainer.style.minHeight = new StyleLength(new Length(100, LengthUnit.Percent));
            dataContainer.AddToClassList("modules--one-data-tab--data-container");

            _list = new OneDataList();
            _list.Draw(dataContainer, _searchStr, _pinnedData);
            _list.OnDataSelected += OnDataSelected;
            _list.OnSearch += OnSearch;
            _list.OnPinClicked += OnPinClicked;

            _drawersRoot = new ScrollView();
            dataContainer.Add(_drawersRoot);

            _root.Add(dataContainer);

            _drawers.Clear();
            if (!MF.IsInitialized)
                return;
            CreateViewersForExisted();
        }

        private void OnPinClicked(Type type)
        {
            if (!_pinnedData.Add(type.FullName))
                _pinnedData.Remove(type.FullName);
        }

        private void OnDataSelected(Type dataType)
        {
            foreach (var (_, d) in _drawers)
            {
                d.SetVisible(false);
            }

            _drawers[dataType].SetVisible(true);
        }

        private void OnSearch(string searchStr)
        {
            _searchStr = searchStr;
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

        private void OnCreated(Type dataType, OneData _)
        {
            if (!_drawers.TryGetValue(dataType, out var drawer))
            {
                drawer = new OneDataDrawer(dataType, _drawersRoot);
                _drawers.Add(dataType, drawer);
                drawer.SetVisible(false);
            }
            else
            {
                drawer.UpdateWrapper(dataType, _drawersRoot);
            }

            _list.AddData(dataType);
            _list.Search(_searchStr);
        }

        private void OnRemoved(Type dataType)
        {
            if (_drawers.TryGetValue(dataType, out var drawer))
            {
                drawer.Destroy();
                _list.OnRemoveOneData(dataType);
            }
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