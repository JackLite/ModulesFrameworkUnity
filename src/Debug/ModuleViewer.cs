using System;
using ModulesFramework.Modules;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class ModuleViewer : MonoBehaviour
    {
        public EcsModule Module { get; private set; }

        public void Init(EcsModule module)
        {
            Module = module;
        }

        private void Update()
        {
            name = Module.GetType().Name;
            if (Module.IsGlobal)
                name += "|G";
            if (Module.IsInitialized)
                name += "|I";
            else
                transform.SetAsLastSibling();
            if (Module.IsActive)
                name += "|A";
        }
    }
}