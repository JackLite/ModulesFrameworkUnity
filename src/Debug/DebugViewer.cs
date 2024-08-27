using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFramework.Modules;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class DebugViewer : MonoBehaviour
    {
        private DataWorld _world;
        private readonly Dictionary<int, EntityViewer> _viewers = new Dictionary<int, EntityViewer>();
        private Transform _entitiesParent;

        private SortedDictionary<Type, OneDataViewer> _oneDatas =
            new SortedDictionary<Type, OneDataViewer>(new TypeComparer());

        private Transform _oneDataParent;

        private ModulesDebugParent _modulesParent;

        private Dictionary<Type, ModuleViewer> _modules = new Dictionary<Type, ModuleViewer>();
        private readonly List<ModulesDebugParent> _modulesDebugParents = new List<ModulesDebugParent>();

        private void Awake()
        {
            if (FindObjectOfType<DebugViewer>() != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _entitiesParent = CreateRootParent("Entities - 0");
            _oneDataParent = CreateRootParent("One data");
            var modulesParent = CreateRootParent("Modules");
            _modulesParent = modulesParent.gameObject.AddComponent<ModulesDebugParent>();
            _modulesDebugParents.Add(_modulesParent);
        }

        private void Update()
        {
            foreach (var (type, viewer) in _modules)
            {
                viewer.UpdateGoName();
            }

            foreach (var parent in _modulesDebugParents)
            {
                parent.UpdateHierarchy();
            }

            UpdateChosenInspector();
        }

        private void OnDestroy()
        {
            _world.OnEntityCreated -= OnEntityCreated;
            _world.OnEntityChanged -= OnEntityChanged;
            _world.OnEntityDestroyed -= OnEntityDestroyed;

            _world.OnOneDataCreated -= OnOneDataCreated;
            _world.OnOneDataRemoved -= OnOneDataRemoved;
        }

        private Transform CreateRootParent(string parentName)
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
            world.OnOneDataRemoved += OnOneDataRemoved;

            CreateModulesViewers(world);
        }

        private void CreateModulesViewers(DataWorld world)
        {
            var allModules = world.GetAllModules();
            var modulesBag = allModules.ToHashSet();
            while (modulesBag.Count > 0)
            {
                var module = modulesBag.First();
                modulesBag.Remove(module);
                if (module.IsSubmodule)
                    CreateModuleParent(module, modulesBag);
                CreateModuleViewer(module);
            }

            _modules = _modules.OrderBy(kvp => kvp.Key.Name)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            CreateSubmodulesHierarchy();
        }

        private void CreateModuleParent(EcsModule submodule, HashSet<EcsModule> modules)
        {
            if (_modules.ContainsKey(submodule.Parent.GetType()))
                return;
            modules.TryGetValue(submodule.Parent, out var parent);
            modules.Remove(parent);
            if (parent.IsSubmodule)
                CreateModuleParent(parent, modules);
            CreateModuleViewer(parent);
        }

        private void CreateModuleViewer(EcsModule module)
        {
            var moduleView = new GameObject(module.GetType().Name);
            moduleView.transform.SetParent(_modulesParent.transform);
            var viewer = moduleView.AddComponent<ModuleViewer>();
            var parent = GetParentViewerForModule(module);
            viewer.Init(module, parent);
            _modules.Add(module.GetType(), viewer);
        }

        private ModuleViewer GetParentViewerForModule(EcsModule module)
        {
            if (!module.IsSubmodule)
                return null;
            return _modules[module.Parent.GetType()];
        }

        private void CreateSubmodulesHierarchy()
        {
            foreach (var (type, viewer) in _modules)
            {
                if (viewer.Module.IsSubmodule)
                {
                    var parentViewer = _modules[viewer.Module.Parent.GetType()];
                    viewer.transform.SetParent(parentViewer.transform);
                    if (!parentViewer.TryGetComponent<ModulesDebugParent>(out var modulesDebugParent))
                    {
                        modulesDebugParent = parentViewer.gameObject.AddComponent<ModulesDebugParent>();
                        _modulesDebugParents.Add(modulesDebugParent);
                    }

                    modulesDebugParent.AddChild(viewer);
                }
                else
                {
                    _modulesParent.AddChild(viewer);
                }
            }
        }

        private void OnOneDataCreated(Type type, OneData data)
        {
            if (_oneDatas.ContainsKey(type))
            {
                _oneDatas[type].Init(type, data);
                return;
            }

            var dataView = new GameObject();
            dataView.transform.SetParent(_oneDataParent);
            var viewer = dataView.AddComponent<OneDataViewer>();
            viewer.Init(type, data);
            _oneDatas.Add(type, viewer);
            foreach (var (_, dataViewer) in _oneDatas)
            {
                dataViewer.transform.SetAsLastSibling();
            }
        }

        private void OnOneDataRemoved(Type type)
        {
            if (!_oneDatas.ContainsKey(type))
                return;

            if (Application.isPlaying)
                Destroy(_oneDatas[type].gameObject);
            else
                DestroyImmediate(_oneDatas[type].gameObject);

            _oneDatas.Remove(type);
        }

        private void OnEntityCreated(int eid)
        {
            var entityView = new GameObject($"Entity_{eid}");
            entityView.transform.SetParent(_entitiesParent);
            var viewer = entityView.AddComponent<EntityViewer>();
            viewer.Init(eid, _world);
            if (_viewers.ContainsKey(eid))
            {
                UnityEngine.Debug.LogError($"Entity {eid} already added to viewer and will be replaced");
                if (_viewers[eid] != null)
                    DestroyImmediate(_viewers[eid]);
            }

            _viewers[eid] = viewer;
            _entitiesParent.name = $"Entities - {_viewers.Count.ToString()}";
        }

        private void OnEntityChanged(int eid)
        {
            if (!_viewers.TryGetValue(eid, out var viewer))
            {
                if (_world.IsEntityAlive(eid))
                    UnityEngine.Debug.LogError($"Entity {eid} was changed but there is no viewer for it");
                return;
            }

            viewer.UpdateComponents();
        }

        private void OnEntityDestroyed(int eid)
        {
            if (_viewers.ContainsKey(eid))
            {
                if (_viewers[eid] != null)
                {
                    if (Application.isPlaying)
                        Destroy(_viewers[eid].gameObject);
                    else
                        DestroyImmediate(_viewers[eid].gameObject);
                }

                _viewers.Remove(eid);
            }

            if (_entitiesParent != null)
                _entitiesParent.name = $"Entities - {_viewers.Count.ToString()}";
        }

        private void UpdateChosenInspector()
        {
            #if UNITY_EDITOR
            var objects = Selection.GetFiltered<GameObject>(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);
            foreach (var o in objects)
            {
                if (!IsParentOf(o))
                    continue;

                if (o.TryGetComponent(out OneDataViewer viewer))
                    viewer.RiseUpdate();
                if (o.TryGetComponent(out EntityViewer entityViewer))
                    entityViewer.RiseUpdate();
            }
            #endif
        }

        private bool IsParentOf(GameObject child)
        {
            if (child.transform.parent == null)
                return false;
            if (child.transform.parent == transform)
                return true;

            return IsParentOf(child.transform.parent.gameObject);
        }
    }
}