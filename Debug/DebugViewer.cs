using System;
using System.Collections.Generic;
using ModulesFramework;
using ModulesFramework.Data;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class DebugViewer : MonoBehaviour
    {
        private DataWorld _world;
        private readonly Dictionary<int, EntityViewer> _viewers = new Dictionary<int, EntityViewer>();
        private Transform _entitiesParent;
        
        private Dictionary<Type, OneDataViewer> _oneDatas = new Dictionary<Type, OneDataViewer>();
        private Transform _oneDataParent;
        
        private void Awake()
        {
            if (FindObjectOfType<DebugViewer>() != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            _entitiesParent = CreateParent("Entities - 0");
            _oneDataParent = CreateParent("One data");
        }

        private Transform CreateParent(string parentName)
        {
            var parent = new GameObject(parentName);
            parent.transform.SetParent(transform);
            return parent.transform;
        }

        public void Init(DataWorld world)
        {
            _world = world;
            world.OnEntityCreated += OnEntityCreated;
            world.OnEntityChanged += OnEntityChanged;
            world.OnEntityDestroyed += OnEntityDestroyed;

            world.OnOneDataCreated += OnOneDataCreated;
        }

        private void OnOneDataCreated(Type type, OneData data)
        {
            var dataView = new GameObject(type.Name);
            dataView.transform.SetParent(_oneDataParent);
            var viewer = dataView.AddComponent<OneDataViewer>();
            viewer.Init(type, data);
            _oneDatas.Add(type, viewer);
        }

        private void OnEntityCreated(int eid)
        {
            var entityView = new GameObject($"Entity_{eid}");
            entityView.transform.SetParent(_entitiesParent);
            var viewer = entityView.AddComponent<EntityViewer>();
            viewer.Init(eid, _world);
            _viewers.Add(eid, viewer);
            _entitiesParent.name = $"Entities - {_viewers.Count.ToString()}";
        }

        private void OnEntityChanged(int eid)
        {
            var viewer = _viewers[eid];
            viewer.components.Clear();
            _world.MapTables((type, table) =>
            {
                if(_world.HasComponent(eid, type))
                    viewer.AddComponent(table.GetDataObject(eid));
            });
            viewer.UpdateName();
        }

        private void OnEntityDestroyed(int eid)
        {
            if (_viewers.ContainsKey(eid))
            {
                Destroy(_viewers[eid].gameObject);
                _viewers.Remove(eid);
            }
            _entitiesParent.name = $"Entities - {_viewers.Count.ToString()}";
        }
    }
}