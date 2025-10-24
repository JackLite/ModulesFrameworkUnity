using System.Collections.Generic;
using ModulesFramework.Data;
using ModulesFramework.Modules;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Events;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Info;
using ModulesFrameworkUnity.Utils;

namespace ModulesFrameworkUnity.Debug.UpdateDebugging.Service
{
    internal class DebugSystemsService
    {
        private readonly DataWorld _world;

        public DebugSystemsService(DataWorld world)
        {
            _world = world;
        }

        public void CreateDataFor(EcsModule module, ModuleRunType moduleRunType)
        {
            var data = new SystemsPauseDebugData
            {
                currentModule = module,
            };

            if (moduleRunType == ModuleRunType.Run)
                FillRunData(ref data);
            else if (moduleRunType == ModuleRunType.PostRun)
                FillPostRunData(ref data);
            else if (moduleRunType == ModuleRunType.FrameEnd)
                FillFrameEndData(ref data);

            _world.CreateOneData(data);

            UpdateNextRunSystem();
        }

        public void Reset()
        {
            _world.RemoveOneData<SystemsPauseDebugData>();
        }

        private void FillRunData(ref SystemsPauseDebugData data)
        {
            data.eventRunners = data.currentModule.GetEventRunners(typeof(IRunEventSystem)).ToQueue();
            data.updateSystems = data.currentModule.GetSystems(typeof(IRunSystem)).ToQueue();
            if (data.eventRunners.Count > 0)
            {
                data.eventSystemsWrapper =
                    new EventSystemsDebugWrapper<IRunEventSystem>(data.currentModule, data.eventRunners.Peek());
            }
        }

        private void FillPostRunData(ref SystemsPauseDebugData data)
        {
            data.eventRunners = data.currentModule.GetEventRunners(typeof(IPostRunEventSystem)).ToQueue();
            data.updateSystems = data.currentModule.GetSystems(typeof(IPostRunSystem)).ToQueue();
            if (data.eventRunners.Count > 0)
            {
                data.eventSystemsWrapper =
                    new EventSystemsDebugWrapper<IPostRunEventSystem>(data.currentModule, data.eventRunners.Peek());
            }
        }

        private void FillFrameEndData(ref SystemsPauseDebugData data)
        {
            data.eventRunners = data.currentModule.GetEventRunners(typeof(IPostRunEventSystem)).ToQueue();
            data.updateSystems = new Queue<ISystem>();
            if (data.eventRunners.Count > 0)
            {
                data.eventSystemsWrapper =
                    new EventSystemsDebugWrapper<IPostRunEventSystem>(data.currentModule, data.eventRunners.Peek());
            }
        }

        public void RunNextSystem(ModuleRunType moduleRunType)
        {
            if (moduleRunType == ModuleRunType.Run)
                NextRun();
            else if (moduleRunType == ModuleRunType.PostRun)
                NextPostRun();
            else if (moduleRunType == ModuleRunType.FrameEnd)
                NextFrameEnd();
        }

        private void NextFrameEnd()
        {
            ref var data = ref _world.OneData<SystemsPauseDebugData>();
            if (data.eventSystemsWrapper == null)
            {
                UpdateNextRunSystem();
                return;
            }

            var eventSystemsWrapper = data.eventSystemsWrapper;
            eventSystemsWrapper.RunNext(ModuleRunType.FrameEnd);
            if (eventSystemsWrapper.IsEmpty())
                RefreshEventSystems<IFrameEndEventSystem>(ref data);
        }

        private void NextPostRun()
        {
            ref var data = ref _world.OneData<SystemsPauseDebugData>();
            if (data.eventSystemsWrapper != null)
            {
                var eventSystemsWrapper = data.eventSystemsWrapper;
                eventSystemsWrapper.RunNext(ModuleRunType.PostRun);
                if (eventSystemsWrapper.IsEmpty())
                    RefreshEventSystems<IPostRunEventSystem>(ref data);

                return;
            }

            var system = (IPostRunSystem)data.updateSystems.Dequeue();
            UnityEngine.Debug.Log($"[Modules.Adapter] Post run system {system.GetType().GetTypeName()}");
            system.PostRun();

            if (data.updateSystems.Count > 0)
                UpdateNextRunSystem();

            RefreshEventSystems<IPostRunEventSystem>(ref data);
        }

        private void NextRun()
        {
            ref var data = ref _world.OneData<SystemsPauseDebugData>();
            if (data.eventSystemsWrapper != null)
            {
                var eventSystemsWrapper = data.eventSystemsWrapper;
                eventSystemsWrapper.RunNext(ModuleRunType.Run);
                if (eventSystemsWrapper.IsEmpty())
                    RefreshEventSystems<IRunEventSystem>(ref data);

                return;
            }

            var system = (IRunSystem)data.updateSystems.Dequeue();
            UnityEngine.Debug.Log($"[Modules.Adapter] Run system {system.GetType().GetTypeName()}");
            system.Run();

            RefreshEventSystems<IRunEventSystem>(ref data);
        }

        private void RefreshEventSystems<TEventSystem>(ref SystemsPauseDebugData data) where TEventSystem : IEventSystem
        {
            data.eventRunners = data.currentModule.GetEventRunners(typeof(TEventSystem)).ToQueue();
            data.eventSystemsWrapper = null;
            if (data.eventRunners.Count > 0)
            {
                data.eventSystemsWrapper =
                    new EventSystemsDebugWrapper<TEventSystem>(data.currentModule, data.eventRunners.Peek());
            }

            UpdateNextRunSystem();
        }

        public bool IsEmpty()
        {
            var data = _world.OneData<SystemsPauseDebugData>();
            return data.updateSystems.Count == 0 && data.eventSystemsWrapper == null;
        }

        public void ProceedAllRemains(ModuleRunType runType)
        {
            while (!IsEmpty())
            {
                RunNextSystem(runType);
            }
        }

        private void UpdateNextRunSystem()
        {
            ref var data = ref _world.OneData<SystemsPauseDebugData>();
            if (data.eventRunners.Count > 0)
                data.nextSystem = data.eventSystemsWrapper.GetNext();
            else if (data.updateSystems.Count > 0)
                data.nextSystem = data.updateSystems.Peek();
            else
                data.nextSystem = null;
            _world.RiseEvent<UpdateDebuggingSystemChangedSignal>();
        }
    }
}