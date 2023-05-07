using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class ModulesDebugParent : MonoBehaviour
    {
        private Transform _activeModulesHeader;
        private Transform _initializedModulesHeader;
        private Transform _nonInitializedModulesHeader;

        private readonly List<ModuleViewer> _children = new List<ModuleViewer>();

        private void Awake()
        {
            SpawnSeparators();
        }

        public void SpawnSeparators()
        {
            _activeModulesHeader = new GameObject("------ Active ------").transform;
            _activeModulesHeader.SetParent(transform);
            _initializedModulesHeader = new GameObject("------ Initialized ------").transform;
            _initializedModulesHeader.SetParent(transform);
            _nonInitializedModulesHeader = new GameObject("------ Non-initialized ------").transform;
            _nonInitializedModulesHeader.SetParent(transform);
        }

        public void AddChild(ModuleViewer viewer)
        {
            _children.Add(viewer);
        }

        public void UpdateHierarchy()
        {
            _activeModulesHeader.SetAsLastSibling();
            foreach (var activeViewer in _children.Where(c => c.Module.IsActive))
            {
                activeViewer.transform.SetAsLastSibling();
            }

            _initializedModulesHeader.SetAsLastSibling();
            foreach (var initViewer in _children.Where(c => !c.Module.IsActive && c.Module.IsInitialized))
            {
                initViewer.transform.SetAsLastSibling();
            }

            _nonInitializedModulesHeader.SetAsLastSibling();
            foreach (var nonInitViewer in _children.Where(c => !c.Module.IsInitialized))
            {
                nonInitViewer.transform.SetAsLastSibling();
            }
        }
    }
}