using System;
using System.Linq;
using ModulesFramework.Data;
using ModulesFramework.Modules;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.UpdateDebugging;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Info;
using ModulesFrameworkUnity.Utils;

namespace ModulesFrameworkUnity.DebugWindow.Service
{
    internal class DebugModulesService
    {
        private readonly DataWorld _world;

        public DebugModulesService(DataWorld world)
        {
            _world = world;
        }

        public void CreateData()
        {
            var data = new ModulesPauseDebugData
            {
                currentModuleRunType = ModuleRunType.Run,
                currentRootIndex = -1,
                nextCallModule = _world.EmbeddedGlobalModule,
                currentRoot = new ModuleDebugWrapper(_world.EmbeddedGlobalModule),
                roots = _world.GetAllModules().Where(m => m.IsRoot).ToList(),
            };

            _world.CreateOneData(data);
        }

        public void Stop()
        {
            _world.RemoveOneData<ModulesPauseDebugData>();
        }

        public void RunNextModule()
        {
            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            if (IsEmpty())
            {
                _world.Logger.LogError(
                    $"[Modules.Adapter] There's no more modules to run for {data.currentModuleRunType}"
                );
                return;
            }

            var isLast = StepModule(data.currentModuleRunType, data.currentRoot);
            if (!isLast)
            {
                data.nextCallModule = GetNextModule().module;
                return;
            }

            data.currentRootIndex++;
            if (IsEmpty())
                return;

            data.currentRoot = new ModuleDebugWrapper(data.roots[data.currentRootIndex]);
            data.nextCallModule = GetNextModule().module;
        }

        public void SwitchToNextRunType()
        {
            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            data.currentRootIndex = -1;
            data.currentRoot = new ModuleDebugWrapper(_world.EmbeddedGlobalModule);
            data.nextCallModule = data.currentRoot.module;
            data.currentModuleRunType = data.currentModuleRunType.Next();
        }

        public bool IsEmpty()
        {
            var data = _world.OneData<ModulesPauseDebugData>();
            return data.currentRootIndex >= data.roots.Count;
        }

        /// <summary>
        ///     Returns true if step was last, i.e. there's nothing left to run
        /// </summary>
        /// <param name="runType"></param>
        /// <returns></returns>
        private bool StepModule(ModuleRunType runType, ModuleDebugWrapper wrapper)
        {
            var isLast = wrapper.step switch
            {
                ModuleInternalStep.Composed => StepComposed(runType, wrapper),
                ModuleInternalStep.Self => StepSelf(runType, wrapper),
                ModuleInternalStep.Submodules => StepSubmodules(runType, wrapper),
                _ => throw new ArgumentOutOfRangeException()
            };

            return isLast;
        }

        public ModuleDebugWrapper GetNextModule()
        {
            var data = _world.OneData<ModulesPauseDebugData>();
            return GetNextModule(data.currentRoot);
        }

        private ModuleDebugWrapper GetNextModule(ModuleDebugWrapper wrapper)
        {
            if (wrapper.step == ModuleInternalStep.Composed)
            {
                if (wrapper.composed.Count == 0)
                    return wrapper;

                var composedModule = wrapper.composed.Peek();
                return GetNextModule(composedModule);
            }

            if (wrapper.step == ModuleInternalStep.Self)
                return wrapper;

            if (wrapper.step == ModuleInternalStep.Submodules)
            {
                var submodule = wrapper.submodules.Peek();
                return GetNextModule(submodule);
            }

            throw new Exception("Invalid internal step");
        }

        private bool StepComposed(ModuleRunType runType, ModuleDebugWrapper wrapper)
        {
            if (wrapper.composed.Count == 0)
            {
                wrapper.step = ModuleInternalStep.Self;
                return StepModule(runType, wrapper);
            }

            var composedModule = wrapper.composed.Peek();
            var isLast = StepModule(runType, composedModule);
            if (isLast)
                wrapper.composed.Dequeue();

            return false;
        }

        private bool StepSelf(ModuleRunType runType, ModuleDebugWrapper wrapper)
        {
            UnityEngine.Debug.Log($"[Modules.Adapter] {runType} of {wrapper.module.GetType().Name}");
            switch (runType)
            {
                case ModuleRunType.Run:
                    wrapper.module.Run();
                    break;
                case ModuleRunType.PostRun:
                    wrapper.module.PostRun();
                    break;
                case ModuleRunType.FrameEnd:
                    wrapper.module.FrameEnd();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runType), runType, null);
            }

            if (wrapper.submodules.Count == 0)
                return true;

            wrapper.step = ModuleInternalStep.Submodules;
            return false;
        }

        private bool StepSubmodules(ModuleRunType runType, ModuleDebugWrapper wrapper)
        {
            var submodule = wrapper.submodules.Peek();
            var isLast = StepModule(runType, submodule);
            if (isLast)
                wrapper.submodules.Dequeue();

            return wrapper.submodules.Count == 0;
        }

        public void SwitchToNextModule()
        {
            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            var wrapper = data.currentRoot;
            if (wrapper.step == ModuleInternalStep.Composed)
            {
                if (wrapper.composed.Count > 0)
                {
                    wrapper.composed.Dequeue();
                    data.nextCallModule = GetNextModule().module;
                    return;
                }

                wrapper.step = ModuleInternalStep.Self;
            }

            if (wrapper.step == ModuleInternalStep.Self)
            {
                if (wrapper.submodules.Count == 0)
                    SwitchToNextRoot();
                else
                    wrapper.step = ModuleInternalStep.Submodules;
            }
            else if (wrapper.step == ModuleInternalStep.Submodules)
            {
                wrapper.submodules.Dequeue();
                if (wrapper.submodules.Count == 0)
                    SwitchToNextRoot();
            }

            data.nextCallModule = GetNextModule().module;
        }

        private void SwitchToNextRoot()
        {
            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            data.currentRootIndex++;
            if (data.currentRootIndex >= data.roots.Count)
            {
                data.currentRootIndex = -1;
                data.currentRoot = new ModuleDebugWrapper(_world.EmbeddedGlobalModule);
                data.nextCallModule = data.currentRoot.module;
                data.currentModuleRunType = data.currentModuleRunType.Next();
                return;
            }

            data.currentRoot = new ModuleDebugWrapper(data.roots[data.currentRootIndex]);
            data.nextCallModule = GetNextModule().module;
        }

        public void ProceedAllRemains()
        {
            while (!IsEmpty())
                RunNextModule();

            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            var runType = data.currentModuleRunType;
            while (runType != ModuleRunType.FrameEnd)
            {
                SwitchToNextRunType();
                runType = data.currentModuleRunType;
                while (!IsEmpty())
                    RunNextModule();
            }
        }
    }
}