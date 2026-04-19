using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindowFeature.StructCreation
{
    /// <summary>
    ///     Widget that can search types and show them for choosing
    /// </summary>
    public class SearchStructWidget : VisualElement
    {
        protected virtual int MaxSearchResults { get; } = 10;
        protected virtual int RecentTypesCount { get; } = 5;

        protected readonly List<CreateWindowRow> rows = new List<CreateWindowRow>();
        protected List<Type> allTypes = new List<Type>();
        protected List<string> recentTypes = new();

        private Action<Type> _onChoose;
        private string _recentTypeKey;
        private TextField _searchInput;

        public event Action OnSearch;

        public void Init(string recentTypeKey, Action<Type> onChoose, bool isForGeneric = false)
        {
            _recentTypeKey = recentTypeKey;
            _onChoose = onChoose;
            UpdateTypes(isForGeneric);
            LoadRecentTypes();
        }

        protected virtual void LoadRecentTypes()
        {
            if (!EditorPrefs.HasKey(_recentTypeKey))
                return;

            var str = EditorPrefs.GetString(_recentTypeKey);
            recentTypes = str.Split(';').ToList();
        }

        public void AddToRecent(Type type)
        {
            var existedIdx = recentTypes.FindIndex(t => t == type.FullName);
            if (existedIdx >= 0)
            {
                recentTypes.RemoveAt(existedIdx);
                recentTypes.Insert(0, type.FullName);
            }
            else
            {
                recentTypes.Add(type.FullName);
                if (recentTypes.Count > RecentTypesCount)
                    recentTypes.RemoveAt(0);
            }

            var saveStr = string.Join(";", recentTypes);
            EditorPrefs.SetString(_recentTypeKey, saveStr);
        }

        public void Finish(Type genericType)
        {
            Clear();
            Add(new Label(genericType.GetTypeName()));
        }

        protected virtual void DrawRecent()
        {
            var rowIdx = 0;
            for (var i = recentTypes.Count - 1; i >= 0; i--)
            {
                var typeName = recentTypes[i];
                var typeIdx = allTypes.FindIndex(t => t.FullName == typeName);
                if (typeIdx == -1)
                    continue;

                var type = allTypes[typeIdx];
                var row = rows[rowIdx++];
                InitRow(row, type);
            }
        }

        public void DrawSearch()
        {
            _searchInput = new TextField("Search type: ");
            _searchInput.AddToClassList("modules-debug--create-struct--search-input");
            _searchInput.RegisterValueChangedCallback(ev => Search(ev.newValue));
            Add(_searchInput);

            var rowsContainer = new VisualElement();
            rowsContainer.AddToClassList("modules-debug--create-struct--rows-container");
            rows.Clear();
            for (var i = 0; i < MaxSearchResults; i++)
            {
                var row = new CreateWindowRow();
                row.style.display = DisplayStyle.None;
                row.OnChoose += _onChoose;
                rows.Add(row);
                rowsContainer.Add(row);
            }

            Add(rowsContainer);
            OnSearch?.Invoke();
        }

        private void Search(string newValue)
        {
            ResetRows();

            if (string.IsNullOrEmpty(newValue))
            {
                DrawRecent();
                return;
            }

            var filtered = new List<Type>(MaxSearchResults);
            foreach (var type in allTypes)
            {
                if (type.GetTypeName().Equals(newValue, StringComparison.InvariantCultureIgnoreCase))
                    filtered.Add(type);
                if (filtered.Count == filtered.Capacity)
                    break;
            }

            if (filtered.Count < filtered.Capacity)
            {
                foreach (var type in allTypes)
                {
                    if (filtered.Contains(type))
                        continue;
                    if (type.GetTypeName().StartsWith(newValue, StringComparison.InvariantCultureIgnoreCase))
                        filtered.Add(type);
                    if (filtered.Count == filtered.Capacity)
                        break;
                }
            }

            if (filtered.Count < filtered.Capacity)
            {
                foreach (var type in allTypes)
                {
                    if (filtered.Contains(type))
                        continue;
                    if (type.GetTypeName().Contains(newValue, StringComparison.InvariantCultureIgnoreCase))
                        filtered.Add(type);
                    if (filtered.Count == filtered.Capacity)
                        break;
                }
            }

            filtered.Sort((s1, s2) =>
                string.Compare(s1.FullName, s2.FullName, StringComparison.InvariantCultureIgnoreCase));
            for (var i = 0; i < filtered.Count; i++)
            {
                var type = filtered[i];
                var row = rows[i];
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
            foreach (var row in rows)
                row.style.display = DisplayStyle.None;
        }

        public void ResetSearch()
        {
            ResetRows();
            _searchInput.value = string.Empty;
        }

        protected virtual void UpdateTypes(bool isForGeneric)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (!isForGeneric)
            {
                var filter = new AssemblyFilter();
                assemblies = assemblies.Where(filter.Filter).ToArray();
                allTypes = assemblies
                    .Where(filter.Filter)
                    .SelectMany(assembly => assembly.GetTypes().Where(t => t.IsValueType))
                    .ToList();
            }
            else
            {
                allTypes = assemblies
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(t => !t.IsInterface && !t.IsAbstract)
                    .ToList();
            }
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