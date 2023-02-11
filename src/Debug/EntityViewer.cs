using System;
using System.Collections.Generic;
using System.Text;
using ModulesFramework.Data;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class EntityViewer : MonoBehaviour
    {
        public readonly Dictionary<Type, object> components = new Dictionary<Type, object>();

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
            components[type] = component;
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