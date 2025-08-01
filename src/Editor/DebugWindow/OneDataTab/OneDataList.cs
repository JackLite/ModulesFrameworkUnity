using ModulesFramework;
using ModulesFrameworkUnity.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.OneDataTab
{
    public class OneDataList
    {
        private ScrollView _scrollView;
        private readonly LinkedDictionary<Type, OneDataLabel> _dataLabels = new();
        private readonly HashSet<Type> _filtered = new();
        private OneDataLabel _currentSelected;
        private TextField _searchField;
        private HashSet<string> _pinnedData = new();

        private string _currentFilter = string.Empty;

        private bool FilterActive => !string.IsNullOrWhiteSpace(_currentFilter);

        public event Action<Type> OnDataSelected;
        public event Action<string> OnSearch;
        public event Action OnCreateNewClicked;
        public event Action<Type> OnPinClicked;

        public void Draw(VisualElement root, string lastSearch, HashSet<string> pinTypes)
        {
            _pinnedData = new HashSet<string>(pinTypes);
            var container = new VisualElement();
            container.AddToClassList("modules--one-data-tab--left-block");
            _scrollView = new ScrollView();
            _scrollView.AddToClassList("modules--one-data-tab--list");
            _scrollView.focusable = true;
            _scrollView.mode = ScrollViewMode.VerticalAndHorizontal;
#if !UNITY_2022_1_OR_NEWER
            _scrollView.RegisterCallback((KeyDownEvent ev, OneDataList list) =>
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
                (NavigationMoveEvent ev, OneDataList list) => list.OnNavigation(ev.direction),
                this
            );
#endif

            CreateSearch(container, lastSearch);
            container.Add(_scrollView);
            CreateBottomContainer(container);
            root.Add(container);
        }

        private void CreateSearch(VisualElement root, string lastSearch)
        {
            _searchField = new TextField
            {
                value = lastSearch
            };
            _searchField.AddToClassList("modules--one-data-tab--search");
            _searchField.RegisterValueChangedCallback(ev => Search(ev.newValue));

            var clearBtn = new Button
            {
                text = "X"
            };
            clearBtn.clicked += () => _searchField.value = string.Empty;
            _searchField.Add(clearBtn);

            root.Add(_searchField);
        }

        private void CreateBottomContainer(VisualElement root)
        {
            var bottomContainer = new VisualElement();
            bottomContainer.AddToClassList("modules--one-data-tab--list-bottom-container");
            CreateAddButton(bottomContainer);
            root.Add(bottomContainer);
        }

        private void CreateAddButton(VisualElement root)
        {
            var addBtn = new Button
            {
                text = "Create One Data"
            };
            addBtn.AddToClassList("modules--one-data-tab--add-btn");
            addBtn.clicked += () => OnCreateNewClicked?.Invoke();
            root.Add(addBtn);
        }

        public void AddData(Type type)
        {
            if (_dataLabels.ContainsKey(type))
            {
                var label = _dataLabels[type].Value;
                label.UpdateText();
                UpdateList();
                return;
            }

            var dataLabel = new OneDataLabel(type, _pinnedData.Contains(type.FullName));
            _dataLabels.Add(type, dataLabel);
            dataLabel.AddToClassList("modules--one-data-tab--one-list-item");
            dataLabel.RegisterCallback((ClickEvent ev, Type selectedType) =>
            {
                if (ev.button != 0)
                    return;
                if (ev.target == dataLabel.pinBtn)
                    return;
                UpdateSelectionType(selectedType);
            }, type);
            dataLabel.OnPinClick += OnPin;
            dataLabel.OnRemoveClick += OnRemoveClick;

            _scrollView.Add(dataLabel);
            UpdateList();
        }


        private void OnPin(Type dataType)
        {
            if (!_pinnedData.Add(dataType.FullName))
                _pinnedData.Remove(dataType.FullName);
            _dataLabels[dataType].Value.SetPinned(_pinnedData.Contains(dataType.FullName));
            OnPinClicked?.Invoke(dataType);
            UpdateList();
        }

        private void OnRemoveClick(Type type)
        {
            if (!MF.IsInitialized)
                return;
            DebugUtils.GetCurrentWorld().RemoveOneData(type);
        }

        public void OnRemoveOneData(Type type)
        {
            if (!EditorApplication.isPlaying)
                return;

            var label = _dataLabels[type].Value;

            if (label == _currentSelected)
            {
                _currentSelected = null;
            }

            label.RemoveFromHierarchy();
            _dataLabels.Remove(type);
            UpdateList();
        }

        private void UpdateSelectionType(Type dataType)
        {
            var label = _dataLabels[dataType].Value;
            if (_currentSelected == label)
                return;
            OnDataSelected?.Invoke(dataType);
            const string className = "modules--one-data-tab--data-selected";
            label.AddToClassList(className);
            _currentSelected?.RemoveFromClassList(className);
            _currentSelected = label;
            _scrollView.Focus();
        }

        private void UpdateList()
        {
            Filter();
            UpdateVisibility();
            Sort();
        }

        private void Sort()
        {
            var sorted = _dataLabels.Values
                .OrderByDescending(label => _pinnedData.Contains(label.type.FullName))
                .ThenBy(label => label.type.GetTypeName());
            OneDataLabel lastPinned = null;
            foreach (var label in sorted)
            {
                label.RemoveFromClassList("modules--one-data-tab--last-pinned-item");
                if (_pinnedData.Contains(label.type.FullName))
                    lastPinned = label;
                label.BringToFront();
            }

            lastPinned?.AddToClassList("modules--one-data-tab--last-pinned-item");
        }

        public void Search(string searchStr)
        {
            _currentFilter = searchStr;
            Filter();
            UpdateVisibility();
            OnSearch?.Invoke(_currentFilter);
        }

        private void Filter()
        {
            if (string.IsNullOrWhiteSpace(_currentFilter))
                return;

            _filtered.Clear();
            foreach (var (type, _) in _dataLabels)
            {
                if (type.GetTypeName().Contains(_currentFilter, StringComparison.InvariantCultureIgnoreCase))
                    _filtered.Add(type);
            }
        }

        private void UpdateVisibility()
        {
            foreach (var (type, label) in _dataLabels)
            {
                if (_filtered.Contains(type) || string.IsNullOrWhiteSpace(_currentFilter))
                    label.style.display = DisplayStyle.Flex;
                else
                    label.style.display = DisplayStyle.None;
            }
        }

        private void OnNavigation(NavigationMoveEvent.Direction direction)
        {
            if (_dataLabels.Count == 0 || _currentSelected == null || (FilterActive && _filtered.Count == 0))
                return;

            var newType = _currentSelected.type;
            if (direction == NavigationMoveEvent.Direction.Down)
            {
                do
                {
                    var nextNode = _dataLabels[newType].Next ?? _dataLabels.FirstNode;

                    newType = nextNode.Value.type;
                    if (!FilterActive || _filtered.Contains(newType))
                        break;
                } while (newType != _currentSelected.type);
            }

            if (direction == NavigationMoveEvent.Direction.Up)
            {
                do
                {
                    var prevNode = _dataLabels[newType].Previous ?? _dataLabels.LastNode;

                    newType = prevNode.Value.type;
                    if (!FilterActive || _filtered.Contains(newType))
                        break;
                } while (newType != _currentSelected.type);
            }

            UpdateSelectionType(newType);
        }

        private class OneDataLabel : VisualElement
        {
            public readonly Type type;

            public Label label;
            public Button pinBtn;
            public Button removeBtn;

            public event Action<Type> OnPinClick;
            public event Action<Type> OnRemoveClick;

            public OneDataLabel(Type type, bool isPinned)
            {
                this.type = type;
                var generation = DebugUtils.GetCurrentWorld().GetOneDataWrapper(type)?.generation ?? 0;
                label = new Label($"{type.GetTypeName()} [Gen {generation.ToString(CultureInfo.InvariantCulture)}]");

                pinBtn = new Button();
                SetPinned(isPinned);
                pinBtn.clicked += () => OnPinClick?.Invoke(this.type);
                Add(pinBtn);
                Add(label);
                CreateRemoveBtn();
            }

            private void CreateRemoveBtn()
            {
                removeBtn = new Button();
                removeBtn.text = "R";
                removeBtn.clicked += () => OnRemoveClick?.Invoke(this.type);
                removeBtn.AddToClassList("modules--one-data-tab--remove-btn");
                Add(removeBtn);
            }

            public void UpdateText()
            {
                var generation = DebugUtils.GetCurrentWorld().GetOneDataWrapper(type)?.generation ?? 0;
                label.text = $"{type.GetTypeName()} [Gen {generation.ToString(CultureInfo.InvariantCulture)}]";
            }

            public void SetPinned(bool isPinned)
            {
                pinBtn.text = isPinned ? "Unpin" : "Pin";
            }
        }

        public void Reset()
        {
            _scrollView.Clear();
        }
    }
}
