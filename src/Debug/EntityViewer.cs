using System;
using System.Collections.Generic;
using System.Text;
using ModulesFramework.Data;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class EntityViewer : MonoBehaviour
    {
        public readonly SortedDictionary<Type, Dictionary<int, object>> components
            = new SortedDictionary<Type, Dictionary<int, object>>(new TypeComparer());

        public readonly SortedDictionary<Type, Dictionary<int, object>> changedComponents
            = new SortedDictionary<Type, Dictionary<int, object>>(new TypeComparer());

        private readonly Dictionary<int, object> _cache = new Dictionary<int, object>();
        public int Eid { get; private set; }
        public DataWorld World { get; private set; }

        public void Init(int eid, DataWorld world)
        {
            Eid = eid;
            World = world;
        }

        public void AddComponent(int denseIndex, object component)
        {
            var type = component.GetType();
            if (!components.ContainsKey(type))
                components[type] = new Dictionary<int, object>();

            components[type].TryAdd(denseIndex, component);
        }

        public void AddComponents(EcsTable table, int eid)
        {
            if (table.IsMultiple)
            {
                AddMultipleComponents(table, eid);
                UpdateChanged(table);
                return;
            }

            AddComponent(-1, table.GetDataObject(eid));
            UpdateChanged(table);
        }

        private void UpdateChanged(EcsTable table)
        {
            if (!components.ContainsKey(table.Type) && changedComponents.ContainsKey(table.Type))
            {
                changedComponents.Remove(table.Type);
                return;
            }

            changedComponents.TryAdd(table.Type, new Dictionary<int, object>());

            // check if we need remove some from changes
            var removeIndexes = new HashSet<int>();
            foreach (var (index, _) in changedComponents[table.Type])
            {
                if (!components[table.Type].ContainsKey(index))
                    removeIndexes.Add(index);
            }

            foreach (var index in removeIndexes)
            {
                changedComponents[table.Type].Remove(index);
            }

            // check if we need add some to changes
            foreach (var (index, component) in components[table.Type])
            {
                if (!changedComponents[table.Type].ContainsKey(index))
                    changedComponents[table.Type][index] = component;
            }
        }

        private void AddMultipleComponents(EcsTable table, int eid)
        {
            _cache.Clear();
            table.GetDataObjects(eid, _cache);
            foreach (var (index, component) in _cache)
                AddComponent(index, component);
        }

        public void UpdateComponents()
        {
            components.Clear();
            World.MapTables((type, table) =>
            {
                if (World.HasComponent(Eid, type))
                    AddComponents(table, Eid);
            });
            UpdateName();
        }

        public void UpdateName()
        {
            var sb = new StringBuilder($"Entity_{Eid.ToString()}");
            foreach (var kvp in components)
            {
                var type = kvp.Key;
                sb.Append($"|{type.Name}");
            }

            name = sb.ToString();
        }
    }
}