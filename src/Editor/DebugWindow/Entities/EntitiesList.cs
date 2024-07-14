﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ModulesFramework;
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
        private ScrollView _scrollView;
        private readonly Dictionary<int, EntityLabel> _entityLabels = new();
        private readonly List<int> _entities = new();
        private readonly HashSet<int> _filtered = new();
        private readonly Dictionary<int, string> _entityComponentsMap = new();
        private readonly StringBuilder _stringBuilder = new();
        private int _currentSelectedIdx = -1;
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
            _entities.Add(eid);
            var index = _entities.Count - 1;

            var entityLabel = new EntityLabel(entity, index);
            entityLabel.UpdateName();
            _entityLabels[eid] = entityLabel;
            entityLabel.AddToClassList("modules--entities-tab--one-list-item");
            entityLabel.RegisterCallback((ClickEvent _, int idx) =>
            {
                UpdateSelectionIndex(idx);
            }, index);

            _scrollView.Add(entityLabel);
            UpdateList();
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
            _entities.RemoveAt(label.listIndex);

            if (label == _currentSelected)
            {
                _currentSelected = null;
                _currentSelectedIdx = -1;
            }

            label.RemoveFromHierarchy();
            _entityLabels.Remove(eid);
            UpdateList();
        }

        private void UpdateSelectionIndex(int index)
        {
            if (_currentSelectedIdx == index)
                return;
            _currentSelectedIdx = index;
            var eid = _entities[index];
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
            if (_currentSelectedIdx == -1 || _entities.Count == 0 || (FilterActive && _filtered.Count == 0))
                return;

            var newIndex = _currentSelectedIdx;
            if (ev.direction == NavigationMoveEvent.Direction.Down)
            {
                do
                {
                    newIndex++;
                    if (newIndex >= _entities.Count)
                        newIndex = 0;

                    var eid = _entities[newIndex];
                    if (!FilterActive || _filtered.Contains(eid))
                        break;
                } while (newIndex != _currentSelectedIdx);
            }

            if (ev.direction == NavigationMoveEvent.Direction.Up)
            {
                do
                {
                    newIndex--;
                    if (newIndex < 0)
                        newIndex = _entities.Count - 1;

                    var eid = _entities[newIndex];
                    if (!FilterActive || _filtered.Contains(eid))
                        break;
                } while (newIndex != _currentSelectedIdx);
            }

            UpdateSelectionIndex(newIndex);
        }

        private class EntityLabel : Label
        {
            public readonly int eid;
            public readonly int listIndex;

            public EntityLabel(Entity entity, int listIndex)
            {
                eid = entity.Id;
                this.listIndex = listIndex;
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