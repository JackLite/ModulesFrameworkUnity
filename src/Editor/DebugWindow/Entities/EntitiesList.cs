using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities
{
    /// <summary>
    ///     Controls list of entities in the left part of window
    /// </summary>
    [Serializable]
    public class EntitiesList
    {
        private ScrollView _scrollView;
        private EntitiesListSearch _searchField;
        private Label _entitiesCount;
        private readonly Dictionary<int, EntityLabel> _entityLabels = new();
        private readonly LinkedDictionary<int, int> _entities = new();
        private readonly HashSet<int> _filtered = new();
        private readonly Dictionary<int, string> _entityComponentsMap = new();
        private readonly StringBuilder _stringBuilder = new();
        private int _currentSelectedEid = -1;
        private EntityLabel _currentSelected;

        private bool _isFullName;

        [SerializeField]
        public string componentsFilter = string.Empty;

        [SerializeField]
        private string _listFilter = string.Empty;

        private bool FilterActive =>
            !string.IsNullOrWhiteSpace(componentsFilter)
            || !string.IsNullOrWhiteSpace(_listFilter);

        public event Action<int> OnEntitySelected;

        public void Draw(VisualElement root, bool isFullName)
        {
            _isFullName = isFullName;
            if (_searchField == null)
            {
                _searchField = new EntitiesListSearch(_listFilter);
                _searchField.OnInputChanged += searchStr =>
                {
                    _listFilter = searchStr;
                    UpdateList();
                };
            }

            if (_scrollView == null)
            {
                _scrollView = new ScrollView();
                _scrollView.focusable = true;
                _scrollView.mode = ScrollViewMode.VerticalAndHorizontal;
                #if !UNITY_2022_1_OR_NEWER
                _scrollView.RegisterCallback((KeyDownEvent ev, EntitiesList list) =>
                {
                    if (ev.target != _scrollView)
                        return;
                    if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S)
                        list.OnNavigation(NavigationMoveEvent.Direction.Down);
                    else if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.W)
                        list.OnNavigation(NavigationMoveEvent.Direction.Up);
                }, this);
                #else
                _scrollView.RegisterCallback(
                    (NavigationMoveEvent ev, EntitiesList list) => list.OnNavigation(ev.direction),
                    this
                );
                #endif
            }

            if (_entitiesCount == null)
            {
                _entitiesCount = new Label();
                _entitiesCount.text = $"Entities: {_entities.Count}";
                _entitiesCount.AddToClassList("modules--entities-list--count");
            }

            var listRoot = new VisualElement();
            listRoot.AddToClassList("modules--entities-list");
            listRoot.Add(_searchField);
            listRoot.Add(_scrollView);
            listRoot.Add(_entitiesCount);
            root.Add(listRoot);
        }

        public void AddEntity(Entity entity)
        {
            var eid = entity.Id;
            _entities.Add(eid, eid);

            var entityLabel = new EntityLabel(entity);
            entityLabel.UpdateName(_stringBuilder, _isFullName);
            _entityLabels[eid] = entityLabel;
            entityLabel.AddToClassList("modules--entities-tab--one-list-item");
            entityLabel.RegisterCallback((ClickEvent _, int eid) =>
            {
                UpdateSelectionIndex(eid);
            }, eid);

            _scrollView.Add(entityLabel);
            UpdateEntityComponents(eid);
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
                label.UpdateName(_stringBuilder, _isFullName);
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
            label.RegisterCallback<NavigationMoveEvent>((ev) => UnityEngine.Debug.Log("l Move"));
            label.RegisterCallback<NavigationCancelEvent>((ev) => UnityEngine.Debug.Log("l cancel"));
            label.RegisterCallback<NavigationSubmitEvent>((ev) => UnityEngine.Debug.Log("l submit"));
        }

        private void UpdateEntityComponents(int eid)
        {
            _stringBuilder.Clear();
            foreach (var componentType in DebugUtils.GetCurrentWorld().GetEntitySingleComponentsType(eid))
            {
                _stringBuilder.Append("|");
                _stringBuilder.Append(componentType.GetTypeName());
            }

            foreach (var componentType in DebugUtils.GetCurrentWorld().GetEntityMultipleComponentsType(eid))
            {
                _stringBuilder.Append("|");
                _stringBuilder.Append(componentType.GetTypeName());
            }

            _entityComponentsMap[eid] = _stringBuilder.ToString();
            _entityLabels[eid].UpdateName(_stringBuilder, _isFullName);
        }

        private void UpdateList()
        {
            Filter();
            UpdateVisibility();
            _entitiesCount.text = $"Entities: {_entities.Count}";
        }

        public void FilterByComponent(string componentName)
        {
            componentsFilter = componentName;
            Filter();
            UpdateVisibility();
        }

        private void Filter()
        {
            if (!FilterActive)
                return;

            _filtered.Clear();
            foreach (var (eid, fullName) in _entityComponentsMap)
            {
                var label = _entityLabels[eid];
                var isInComponents = fullName.Contains(componentsFilter, StringComparison.InvariantCultureIgnoreCase);
                var isInList = label.displayName.Contains(_listFilter, StringComparison.InvariantCultureIgnoreCase);
                if (isInComponents && isInList)
                {
                    _filtered.Add(eid);
                }
            }
        }

        private void UpdateVisibility()
        {
            foreach (var (eid, label) in _entityLabels)
            {
                if (_filtered.Contains(eid) || !FilterActive)
                    label.style.display = DisplayStyle.Flex;
                else
                    label.style.display = DisplayStyle.None;
            }
        }

        private void OnNavigation(NavigationMoveEvent.Direction direction)
        {
            if (_currentSelectedEid == -1 || _entities.Count == 0 || (FilterActive && _filtered.Count == 0))
                return;

            var newEid = _currentSelectedEid;
            if (direction == NavigationMoveEvent.Direction.Down)
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

            if (direction == NavigationMoveEvent.Direction.Up)
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

        public void Reset()
        {
            _entities.Clear();
            _entityLabels.Clear();
            _entityComponentsMap.Clear();
            _scrollView.Clear();
            _filtered.Clear();
            _currentSelectedEid = -1;
        }

        public void OnTagChanged(int eid)
        {
            if (_entityLabels.TryGetValue(eid, out var label))
                label.UpdateName(_stringBuilder, _isFullName);
        }
    }
}
