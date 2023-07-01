using System;
using System.Collections.Generic;
using System.Text;
using ModulesFramework.Data;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class EntityViewer : MonoBehaviour
    {
        public readonly Dictionary<Type, List<object>> components = new Dictionary<Type, List<object>>();
        private readonly List<object> _cache = new List<object>();
        public int Eid { get; private set; }
        public DataWorld World { get; private set; }

        public void Init(int eid, DataWorld world)
        {
            Eid = eid;
            World = world;
        }

        public void AddComponent(object component)
        {
            var type = component.GetType();
            if(!components.ContainsKey(type))
                components[type] = new List<object>();
            
            components[type].Add(component);
        }
        
        public void AddComponents(EcsTable table, int eid)
        {
            if(!table.IsMultiple)
            {
                AddComponent(table.GetDataObject(eid));
                return;
            }
            _cache.Clear();
            table.GetDataObjects(eid, _cache);
            foreach (var component in _cache)
            {
                AddComponent(component);
            }
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