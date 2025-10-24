using System;
using System.Linq;
using System.Reflection;
using ModulesFramework.Data;
using ModulesFramework.Modules;
using ModulesFrameworkUnity.Debug.Attributes;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Events;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Info;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging.Service
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
            var roots = _world.GetAllModules()
                .Where(m => m.IsRoot)
                .Where(m => m.GetType().GetCustomAttribute<HideInDebugAttribute>() == null)
                .ToList();
            
            var data = new ModulesPauseDebugData
            {
                currentModuleRunType = ModuleRunType.Run,
                currentRootIndex = 0,
                nextCallModule = roots.FirstOrDefault(),
                currentRoot = new ModuleDebugWrapper(roots.FirstOrDefault()),
                roots = roots,
            };

            _world.CreateOneData(data);
            _world.RiseEvent<UpdateDebuggingModuleChangedSignal>();
        }

        public void Reset()
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
                SetNextCallModule(GetNextModule().module);
                return;
            }

            data.currentRootIndex++;
            if (IsEmpty())
                return;

            data.currentRoot = new ModuleDebugWrapper(data.roots[data.currentRootIndex]);
            SetNextCallModule(GetNextModule().module);
        }

        public void SwitchToNextRunType()
        {
            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            data.currentRootIndex = 0;
            data.currentRoot = new ModuleDebugWrapper(data.roots[0]);
            SetNextCallModule(GetNextModule().module);
            data.currentModuleRunType = data.currentModuleRunType.Next();
            _world.RiseEvent<UpdateDebuggingModuleChangedSignal>();
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
            if (SkipStep(data.currentRoot))
            {
                SwitchToNextRoot();
                return;
            }

            SetNextCallModule(GetNextModule().module);
        }

        /// <summary>
        ///     Return true if it's last skip for wrapper
        /// </summary>
        private bool SkipStep(ModuleDebugWrapper wrapper)
        {
            if (wrapper.step == ModuleInternalStep.Composed)
            {
                if (wrapper.composed.Count > 0)
                {
                    var composedModule = wrapper.composed.Peek();
                    if (SkipStep(composedModule))
                        wrapper.composed.Dequeue();
                }

                if (wrapper.composed.Count == 0)
                {
                    wrapper.step = ModuleInternalStep.Self;
                }

                return false;
            }

            if (wrapper.step == ModuleInternalStep.Self)
            {
                wrapper.step = ModuleInternalStep.Submodules;
                return wrapper.submodules.Count == 0;
            }

            if (wrapper.step == ModuleInternalStep.Submodules)
            {
                if (wrapper.submodules.Count > 0)
                {
                    var submoduleWrapper = wrapper.submodules.Peek();
                    if (SkipStep(submoduleWrapper))
                        wrapper.submodules.Dequeue();
                    else
                        return false;
                }
            }

            return wrapper.submodules.Count == 0;
        }

        private void SwitchToNextRoot()
        {
            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            data.currentRootIndex++;
            if (data.currentRootIndex >= data.roots.Count)
            {
                data.currentRootIndex = 0;
                data.currentRoot = new ModuleDebugWrapper(data.roots[0]);
                data.currentModuleRunType = data.currentModuleRunType.Next();
                SetNextCallModule(data.currentRoot.module);
                return;
            }

            data.currentRoot = new ModuleDebugWrapper(data.roots[data.currentRootIndex]);
            SetNextCallModule(GetNextModule().module);
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

        private void SetNextCallModule(EcsModule module)
        {
            ref var data = ref _world.OneData<ModulesPauseDebugData>();
            data.nextCallModule = module;
            _world.RiseEvent<UpdateDebuggingModuleChangedSignal>();
        }
    }
}