using ModulesFramework;
using ModulesFrameworkUnity.Debug.Drawers.Complex;
using ModulesFrameworkUnity.EntitiesTags;
using ModulesFrameworkUnity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Entities.AddComponent
{
    public class AddComponentWindow : EditorWindow
    {
        private const int MaxSearchResults = 16;
        private const int RecentTypesCount = 5;
        private const string RecentTypesKey = "ModulesDebug.AddComponent.RecentTypes";

        private int _eid;
        private List<Type> _allTypes = new List<Type>();
        private List<string> _recentTypes = new List<string>(MaxSearchResults);
        private readonly List<AddComponentOneRow> _rows = new List<AddComponentOneRow>();
        private readonly EditorDrawer _mainDrawer = new EditorDrawer();
        private VisualElement _newComponentContainer;
        private ScrollView _mainContainer;

        public void Show(int eid)
        {
            _eid = eid;
            var entity = MF.World.GetEntity(eid);
            var tags = entity.GetEntityTagsAsString();
            var entityTitle = tags == string.Empty ? entity.GetCustomId() : tags;
            titleContent = new GUIContent($"Add component to {entityTitle}");

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

        private void UpdateTypes()
        {
            var filter = new AssemblyFilter();
            _allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(filter.Filter)
                .SelectMany(assembly => assembly.GetTypes().Where(t => t.IsValueType))
                .ToList();
        }

        private void DrawSearch()
        {
            var searchInput = new TextField("Search struct: ");
            searchInput.AddToClassList("modules-debug--add-component--search-input");
            searchInput.RegisterValueChangedCallback(ev => OnSearch(ev.newValue));
            _mainContainer.Add(searchInput);

            var rowsContainer = new VisualElement();
            _rows.Clear();
            for (var i = 0; i < MaxSearchResults; i++)
            {
                var row = new AddComponentOneRow();
                row.style.display = DisplayStyle.None;
                row.OnChoose += OnChoose;
                _rows.Add(row);
                rowsContainer.Add(row);
            }
            _mainContainer.Add(rowsContainer);
        }

        private void LoadRecentTypes()
        {
            if (!EditorPrefs.HasKey(RecentTypesKey))
                return;

            var str = EditorPrefs.GetString(RecentTypesKey);
            _recentTypes = str.Split(';').ToList();
        }

        private void DrawRecent()
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

        private void OnChoose(Type type)
        {
            if (_newComponentContainer == null)
            {
                _newComponentContainer = new VisualElement();
                _mainContainer.Add(_newComponentContainer);
                _newComponentContainer.AddToClassList("modules-debug--add-component--new-component");
            }
            else
            {
                _newComponentContainer.Clear();
            }
            var newComponent = Activator.CreateInstance(type);
            DrawNewComponent(type, newComponent, _newComponentContainer);

            var addButtonsContainer = new VisualElement();
            addButtonsContainer.AddToClassList("modules-debug--add-component--component-buttons");
            _newComponentContainer.Add(addButtonsContainer);

            var addBtn = new Button();
            addBtn.text = "Add";
            addBtn.clicked += () => AddComponent(type, newComponent);
            addBtn.AddToClassList("modules-debug--add-component--component-add");
            addButtonsContainer.Add(addBtn);

            var addMultipleBtn = new Button();
            addMultipleBtn.text = "Add as multiple";
            addMultipleBtn.clicked += () => AddMultipleComponent(type, newComponent);
            addMultipleBtn.AddToClassList("modules-debug--add-component--component-add");
            addButtonsContainer.Add(addMultipleBtn);
        }

        private void AddComponent(Type type, object component)
        {
            MF.World.AddComponent(_eid, type, component);
            AddToRecent(type);
        }

        private void AddMultipleComponent(Type type, object component)
        {
            MF.World.AddNewComponent(_eid, type, component);
            AddToRecent(type);
        }

        private void AddToRecent(Type type)
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

        private void DrawNewComponent(Type type, object newComponent, VisualElement parent)
        {
            var drawer = new StructsDrawer();
            drawer.Init(_mainDrawer, (_, newVal) => { newComponent = newVal; }, () => newComponent);
            drawer.SetVisible(true);
            drawer.Draw($"{type.Name} (new)", newComponent, parent);
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

        private void InitRow(AddComponentOneRow row, Type type)
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
