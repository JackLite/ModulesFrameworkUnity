using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Utils;
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
        private ScrollView _scrollView;
        private readonly Dictionary<int, EntityLabel> _entityLabels = new();
        private readonly LinkedDictionary<int, int> _entities = new();
        private readonly HashSet<int> _filtered = new();
        private readonly Dictionary<int, string> _entityComponentsMap = new();
        private readonly StringBuilder _stringBuilder = new();
        private int _currentSelectedEid = -1;
        private EntityLabel _currentSelected;

        private string _currentFilter = string.Empty;

        private bool FilterActive => !string.IsNullOrWhiteSpace(_currentFilter);

        public event Action<int> OnEntitySelected;

        public void Draw(VisualElement root)
        {
            if (_scrollView == null)
            {
                _scrollView = new ScrollView();
                _scrollView.AddToClassList("modules--entities-list");
                _scrollView.focusable = true;
                _scrollView.RegisterCallback(
                    (NavigationMoveEvent ev, EntitiesList list) => list.OnNavigation(ev),
                    this
                );
            }

            root.Add(_scrollView);
        }

        public void AddEntity(Entity entity)
        {
            var eid = entity.Id;
            _entities.Add(eid, eid);

            var entityLabel = new EntityLabel(entity);
            entityLabel.UpdateName();
            _entityLabels[eid] = entityLabel;
            entityLabel.AddToClassList("modules--entities-tab--one-list-item");
            entityLabel.RegisterCallback((ClickEvent _, int eid) =>
            {
                UpdateSelectionIndex(eid);
            }, eid);

            _scrollView.Add(entityLabel);
            UpdateList();
            UpdateEntityComponents(eid);
        }

        public void OnEntityChanged(int eid)
        {
            UpdateList();
            UpdateEntityComponents(eid);
        }

        public void OnCustomIdChanged(int eid)
        {
            if (_entityLabels.TryGetValue(eid, out var label))
                label.UpdateName();
        }

        public void RemoveEntity(int eid)
        {
            if (!EditorApplication.isPlaying)
                return;

            var label = _entityLabels[eid];
            _entities.Remove(eid);

            if (label == _currentSelected)
            {
                _currentSelected = null;
                _currentSelectedEid = -1;
            }

            label.RemoveFromHierarchy();
            _entityLabels.Remove(eid);
            _entityComponentsMap.Remove(eid);
            UpdateList();
        }

        private void UpdateSelectionIndex(int eid)
        {
            if (_currentSelectedEid == eid)
                return;
            _currentSelectedEid = eid;
            var label = _entityLabels[eid];
            OnEntitySelected?.Invoke(eid);
            const string className = "modules--entities-tab--entity-selected";
            label.AddToClassList(className);
            _currentSelected?.RemoveFromClassList(className);
            _currentSelected = label;
            _scrollView.Focus();
        }

        private void UpdateEntityComponents(int eid)
        {
            _stringBuilder.Clear();
            foreach (var componentType in MF.World.GetEntitySingleComponentsType(eid))
                _stringBuilder.Append(componentType.Name);
            foreach (var componentType in MF.World.GetEntityMultipleComponentsType(eid))
                _stringBuilder.Append(componentType.Name);
            _entityComponentsMap[eid] = _stringBuilder.ToString();
        }

        private void UpdateList()
        {
            Filter();
            Sort();
        }

        public void FilterByComponent(string componentName)
        {
            _currentFilter = componentName;
            Filter();
            Sort();
        }

        private void Sort()
        {
            foreach (var (eid, label) in _entityLabels)
            {
                if (_filtered.Contains(eid) || string.IsNullOrWhiteSpace(_currentFilter))
                    label.style.display = DisplayStyle.Flex;
                else
                    label.style.display = DisplayStyle.None;
            }
        }

        private void Filter()
        {
            if (string.IsNullOrWhiteSpace(_currentFilter))
                return;

            _filtered.Clear();
            foreach (var (eid, fullName) in _entityComponentsMap)
            {
                if (fullName.Contains(_currentFilter, StringComparison.InvariantCultureIgnoreCase))
                    _filtered.Add(eid);
            }
        }

        private void OnNavigation(NavigationMoveEvent ev)
        {
            if (_currentSelectedEid == -1 || _entities.Count == 0 || (FilterActive && _filtered.Count == 0))
                return;

            var newEid = _currentSelectedEid;
            if (ev.direction == NavigationMoveEvent.Direction.Down)
            {
                do
                {
                    var nextNode = _entities[newEid].Next;
                    if (nextNode == null)
                        nextNode = _entities.FirstNode;

                    newEid = nextNode.Value;
                    if (!FilterActive || _filtered.Contains(newEid))
                        break;
                } while (newEid != _currentSelectedEid);
            }

            if (ev.direction == NavigationMoveEvent.Direction.Up)
            {
                do
                {
                    var prevNode = _entities[newEid].Previous;
                    if (prevNode == null)
                        prevNode = _entities.LastNode;

                    newEid = prevNode.Value;
                    if (!FilterActive || _filtered.Contains(newEid))
                        break;
                } while (newEid != _currentSelectedEid);
            }

            UpdateSelectionIndex(newEid);
        }

        private class EntityLabel : Label
        {
            public readonly int eid;

            public EntityLabel(Entity entity)
            {
                eid = entity.Id;
            }

            public void UpdateName()
            {
                var ent = MF.World.GetEntity(eid);
                if (ent.GetCustomId() == ent.Id.ToString(CultureInfo.InvariantCulture))
                    text = $"Entity ({ent.GetCustomId()})";
                else
                    text = $"{ent.GetCustomId()} ({ent.Id})";
            }
        }
    }
}