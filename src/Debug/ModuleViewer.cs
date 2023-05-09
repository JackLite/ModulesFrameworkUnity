using System;
using ModulesFramework.Modules;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public class ModuleViewer : MonoBehaviour
    {
        public ModuleViewer Parent { get; private set; }
        public EcsModule Module { get; private set; }

        public void Init(EcsModule module, ModuleViewer parent)
        {
            Module = module;
            Parent = parent;
        }

        public void UpdateGoName()
        {
            name = Module.GetType().Name;
            if (Module.IsGlobal)
                name += "|G";
            
            if (Module.IsInitialized)
            {
                name += "|I";
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }

            if (Module.IsActive)
            {
                name += "|A";
            }
        }
    }
}