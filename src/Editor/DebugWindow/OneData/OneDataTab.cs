using System;
using System.Collections.Generic;
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
        private List<OneDataDrawer> _viewers;
        private TextField _searchField;

        [SerializeField]
        private string _searchStr;

        private ScrollView _root;

        public void Draw(ScrollView root)
        {
            _root = root;
            _viewers ??= new List<OneDataDrawer>();
            _viewers.Clear();
            CreateSearch();
            if (!MF.IsInitialized)
                return;
            CreateViewersForExisted();
        }

        private void CreateSearch()
        {
            _searchField = new TextField
            {
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 300,
                    marginTop = 10
                },
                label = "Filter",
                value = _searchStr
            };
            var inputText = _searchField.Query().Class("unity-text-element--inner-input-field-component").First();
            inputText.style.fontSize = 14;

            var label = _searchField.Query().Class("unity-text-field__label").First();
            label.style.fontSize = 16;
            label.style.minWidth = 0;
            label.style.marginRight = 15;
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
                foreach (var dataDrawer in _viewers)
                {
                    dataDrawer.SetVisible(true);
                }

                return;
            }

            foreach (var dataDrawer in _viewers)
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
            foreach (var data in MF.World.OneDataCollection)
            {
                OnCreated(data.GetDataObject().GetType(), data);
            }

            FilterOneData(_searchStr);
        }

        private void OnCreated(Type dataType, ModulesFramework.OneData data)
        {
            var dataDrawer = new OneDataDrawer(data, _root);
            _viewers.Add(dataDrawer);
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