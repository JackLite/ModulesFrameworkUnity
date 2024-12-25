using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using ModulesFrameworkUnity.DebugWindow.Entities.AddComponent;
using ModulesFrameworkUnity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Common
{
    /// <summary>
    ///     Base class for all windows that allows to find some type and choose it
    /// </summary>
    public abstract class CreateWindow : EditorWindow
    {
        protected virtual int MaxSearchResults { get; } = 16;
        protected virtual int RecentTypesCount { get; } = 5;
        protected abstract string RecentTypesKey { get; }

        protected List<Type> _allTypes = new List<Type>();
        protected List<string> _recentTypes = new();
        protected readonly List<CreateWindowRow> _rows = new List<CreateWindowRow>();
        protected readonly EditorDrawer _mainDrawer = new EditorDrawer();
        protected VisualElement _newComponentContainer;
        protected ScrollView _mainContainer;

        public virtual void ShowWindow()
        {
            var styles = Resources.Load<StyleSheet>("AddComponentWindow");
            _mainContainer = new ScrollView();
            _mainContainer.styleSheets.Add(styles);
            _mainContainer.AddToClassList("modules-debug--add-component");
            rootVisualElement.Add(_mainContainer);
            UpdateTypes();
            DrawSearch();

            LoadRecentTypes();
            DrawRecent();

            ShowAuxWindow();
        }

        protected virtual void UpdateTypes()
        {
            var filter = new AssemblyFilter();
            _allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(filter.Filter)
                .SelectMany(assembly => assembly.GetTypes().Where(t => t.IsValueType))
                .ToList();
        }

        protected virtual void DrawSearch()
        {
            var searchInput = new TextField("Search struct: ");
            searchInput.AddToClassList("modules-debug--add-component--search-input");
            searchInput.RegisterValueChangedCallback(ev => OnSearch(ev.newValue));
            _mainContainer.Add(searchInput);

            var rowsContainer = new VisualElement();
            _rows.Clear();
            for (var i = 0; i < MaxSearchResults; i++)
            {
                var row = new CreateWindowRow();
                row.style.display = DisplayStyle.None;
                row.OnChoose += OnChoose;
                _rows.Add(row);
                rowsContainer.Add(row);
            }
            _mainContainer.Add(rowsContainer);
        }

        protected virtual void LoadRecentTypes()
        {
            if (!EditorPrefs.HasKey(RecentTypesKey))
                return;

            var str = EditorPrefs.GetString(RecentTypesKey);
            _recentTypes = str.Split(';').ToList();
        }

        protected virtual void DrawRecent()
        {
            var rowIdx = 0;
            for (var i = _recentTypes.Count - 1; i >= 0; i--)
            {
                var typeName = _recentTypes[i];
                var typeIdx = _allTypes.FindIndex(t => t.FullName == typeName);
                if (typeIdx == -1)
                    continue;

                var type = _allTypes[typeIdx];
                var row = _rows[rowIdx++];
                InitRow(row, type);
            }
        }

        protected abstract void OnChoose(Type type);

        protected void AddToRecent(Type type)
        {
            var existedIdx = _recentTypes.FindIndex(t => t == type.FullName);
            if (existedIdx >= 0)
            {
                _recentTypes.RemoveAt(existedIdx);
                _recentTypes.Insert(0, type.FullName);
            }
            else
            {
                _recentTypes.Add(type.FullName);
                if (_recentTypes.Count > RecentTypesCount)
                    _recentTypes.RemoveAt(0);
            }

            var saveStr = string.Join(";", _recentTypes);
            EditorPrefs.SetString(RecentTypesKey, saveStr);
        }

        protected void DrawNewStruct(Type type, object newComponent, VisualElement parent)
        {
            var drawer = new StructsDrawer();
            drawer.Init(_mainDrawer, (_, newVal) => { newComponent = newVal; }, () => newComponent);
            drawer.SetVisible(true);
            drawer.Draw($"{type.Name} (new)", type, newComponent, parent);
            drawer.Foldout.AddToClassList("modules-debug--add-component--component-drawer");
            drawer.Foldout.Q(className: Foldout.inputUssClassName).pickingMode = PickingMode.Ignore;
            drawer.Foldout.Q(className: Foldout.toggleUssClassName).pickingMode = PickingMode.Ignore;
            drawer.Foldout.Q(className: Foldout.textUssClassName).pickingMode = PickingMode.Ignore;
            drawer.Foldout.Q(className: Foldout.checkmarkUssClassName).pickingMode = PickingMode.Ignore;
        }

        private void OnSearch(string newValue)
        {
            ResetRows();
            _newComponentContainer?.Clear();

            if (string.IsNullOrEmpty(newValue))
            {
                DrawRecent();
                return;
            }

            var filtered = new List<Type>(MaxSearchResults);
            foreach (var type in _allTypes)
            {
                if (type.Name.Contains(newValue, StringComparison.InvariantCultureIgnoreCase))
                    filtered.Add(type);
                if (filtered.Count == filtered.Capacity)
                    break;
            }

            filtered.Sort((s1, s2) => string.Compare(s1.FullName, s2.FullName, StringComparison.InvariantCultureIgnoreCase));
            for (var i = 0; i < filtered.Count; i++)
            {
                var type = filtered[i];
                var row = _rows[i];
                InitRow(row, type);
            }
        }

        private void InitRow(CreateWindowRow row, Type type)
        {
            row.Init(type);
            row.style.display = DisplayStyle.Flex;
        }

        private void ResetRows()
        {
            foreach (var row in _rows)
                row.style.display = DisplayStyle.None;
        }

        private class AssemblyFilter : UnityAssemblyFilter
        {
            public override bool Filter(Assembly assembly)
            {
                return base.Filter(assembly)
                    && !assembly.FullName.StartsWith("ModulesFramework")
                    && !assembly.FullName.StartsWith("Bee.")
                    && !assembly.FullName.StartsWith("Mono.")
                    && !assembly.FullName.StartsWith("System");
            }
        }
    }
}
