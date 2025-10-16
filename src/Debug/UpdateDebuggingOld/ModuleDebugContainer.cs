using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework.Data;
using ModulesFramework.Data.Events;
using ModulesFramework.Modules;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity.Utils;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging
{
    internal class ModuleDebugContainer
    {
        public EcsModule module;
        public DataWorld world;
        public Queue<ModuleDebugContainer> composedModules;
        public Queue<ModuleDebugContainer> submodules;

        public ModuleInternalStep internalModuleStep;

        public Queue<ISystem> systems;
        public EventSystemsDebugContainer eventSystems;
        public SystemRunType systemRunType;

        public ModuleDebugContainer(EcsModule module, DataWorld world)
        {
            this.module = module;
            this.world = world;
            composedModules = module.ComposedModules.Select(cm => new ModuleDebugContainer(cm, world)).ToQueue();
            submodules = module.Submodules.Select(sm => new ModuleDebugContainer(sm, world)).ToQueue();
            internalModuleStep = ModuleInternalStep.Composed;
            UpdateSystems();
        }

        public bool StepSystem()
        {
            bool isLast;
            if (systemRunType == SystemRunType.RunEvents)
            {
                isLast = eventSystems.RunNext();
            }
            else if (systemRunType == SystemRunType.Run)
            {
                if (systems.Count > 0)
                {
                    var runSystem = (IRunSystem)systems.Dequeue();
                    UnityEngine.Debug.Log($"[Modules.Adapter] Run {runSystem.GetType().Name}");
                    runSystem.Run();
                }

                isLast = systems.Count == 0;
            }
            else if (systemRunType == SystemRunType.PostRunEvents)
            {
                isLast = eventSystems.RunNext();
            }
            else if (systemRunType == SystemRunType.PostRun)
            {
                if (systems.Count > 0)
                {
                    var postRunSystem = (IPostRunSystem)systems.Dequeue();
                    UnityEngine.Debug.Log($"[Modules.Adapter] Post run {postRunSystem.GetType().Name}");
                    postRunSystem.PostRun();
                }

                isLast = systems.Count == 0;
            }
            else if (systemRunType == SystemRunType.FrameEndEvents)
            {
                isLast = eventSystems.RunNext();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            if (!isLast)
                return false;

            systemRunType = systemRunType.Next();
            UpdateSystems();

            return true;
        }

        private ModuleDebugContainer GetNextModule()
        {
            if (internalModuleStep == ModuleInternalStep.Composed)
            {
                if (composedModules.Count == 0)
                    return this;

                return composedModules.Peek().GetNextModule();
            }

            if (internalModuleStep == ModuleInternalStep.Self)
                return this;

            if (internalModuleStep == ModuleInternalStep.Submodules)
                return submodules.Peek().GetNextModule();

            throw new Exception("Invalid internal step");
        }

        public ISystem GetNextSystem()
        {
            var nextModule = GetNextModule();
            if (nextModule == this)
            {
                if (systemRunType is SystemRunType.Run or SystemRunType.PostRun)
                    return systems.Count == 0 ? null : systems.Peek();

                return eventSystems.GetNextSystem();
            }

            return nextModule.GetNextSystem();
        }

        private void UpdateSystems()
        {
            switch (systemRunType)
            {
                case SystemRunType.RunEvents:
                    eventSystems = new EventSystemsDebugContainer<IRunEventSystem>(module);
                    break;
                case SystemRunType.Run:
                    systems = module.GetSystems(typeof(IRunSystem)).ToQueue();
                    break;
                case SystemRunType.PostRunEvents:
                    eventSystems = new EventSystemsDebugContainer<IPostRunEventSystem>(module);
                    break;
                case SystemRunType.PostRun:
                    systems = module.GetSystems(typeof(IPostRunSystem)).ToQueue();
                    break;
                case SystemRunType.FrameEndEvents:
                    eventSystems = new EventSystemsDebugContainer<IFrameEndEventSystem>(module);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}