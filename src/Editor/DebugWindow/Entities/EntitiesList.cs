using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Controls list of entities in the left part of window
    /// </summary>
    public class EntitiesList
    {
        private ListView _listView;
        private readonly List<int> _entities = new();

        private int _currentSelected = -1;

        public event Action<int> OnEntitySelected;
        public event Action OnNoEntitySelected;

        public void Draw(VisualElement root)
        {
            if (_listView == null)
            {
                _listView = new ListView();
                _listView.AddToClassList("modules--entities-list");
            }

            _listView.Clear();
            _entities.Clear();
            _listView.makeItem = () => new EntityLabel();
            _listView.bindItem = (el, index) => ((EntityLabel)el).text = $"Entity {_entities[index]}";
            _listView.itemsSource = _entities;
            _listView.selectionType = SelectionType.Single;
            _listView.selectedIndicesChanged += (e) =>
            {
                UnityEngine.Debug.Log("Selected changes");
                var indices = e.ToArray();
                if (indices.Length == 0)
                    OnNoEntitySelected?.Invoke();

                _currentSelected = _entities[indices[0]];
                OnEntitySelected?.Invoke(_currentSelected);
            };
            root.Add(_listView);
        }

        public void AddEntity(int eid)
        {
            _entities.Add(eid);
            _listView.RefreshItems();
        }

        public void RemoveEntity(int eid)
        {
            if (!EditorApplication.isPlaying)
                return;

            if (_currentSelected == eid)
            {
                _currentSelected = -1;
                OnNoEntitySelected?.Invoke();
            }

            _entities.Remove(eid);
            _listView.RefreshItems();
        }

        private class EntityLabel : Label
        {
        }
    }
}