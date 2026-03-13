using System.Collections.Generic;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.EntitiesTags;
using ModulesFrameworkUnity.Settings;
using ModulesFrameworkUnity.Utils;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ModulesFrameworkUnity.EmptyEntities;
using ModulesFrameworkUnity.Systems;
using UnityEngine;
#if MODULES_PERFORMANCE
using System.Diagnostics;
#endif

namespace ModulesFrameworkUnity
{
    public class ModulesUnityAdapter
    {
        private readonly MF _modules;
        private double _elapsedTimeMs;
#if MODULES_PERFORMANCE
        private int _frames;
        private readonly Stopwatch _stopwatch = new();
#endif
        private readonly ModulesSettings _settings;
        private readonly List<EmptyEntitiesService> _emptyEntitiesServices = new();

        public ModulesUnityAdapter(ModulesSettings settings)
        {
            _settings = settings;
            EntitiesTagStorage.Initialize();
            _modules = new MF(new UnityAssemblyFilter());
            _modules.MainWorld.RegisterSystemType<IRunPhysicSystem>();
            _modules.MainWorld.RegisterSystemType<IPostRunSystem>();
            _modules.MainWorld.RegisterEventSystem<IPostRunEventSystem>(new PostRunEventInvoker());
            _modules.MainWorld.RegisterEventSystem<IFrameEndEventSystem>(new FrameEndEventInvoker());
            _modules.MainWorld.OnEntityDestroyed += EntitiesTagStorage.Storage.RemoveEntity;
            if (_settings.deleteEmptyEntities)
            {
                foreach (var world in _modules.Worlds)
                    _emptyEntitiesServices.Add(new EmptyEntitiesService(world));
            }

            _modules.MainWorld.SetLogger(new UnityLogger());
            _modules.MainWorld.SetLogType(_settings.logFilter);
        }

        public void Start()
        {
            _modules.Start().Forget();
        }

        public async Task StartAsync()
        {
            await _modules.Start();
        }

        public void Update()
        {
#if MODULES_PERFORMANCE
            _stopwatch.Start();
#endif
            _modules.Run();
#if MODULES_PERFORMANCE
            _stopwatch.Stop();
            _elapsedTimeMs += _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
#endif
#if UNITY_EDITOR
            DebugEventBus.RiseUpdate();
#endif
        }

        public void FixedUpdate()
        {
            foreach (var world in _modules.Worlds)
            {
                if (world.GetEventSystemTypes().ContainsKey(typeof(IPhysicRunEventSystem)))
                {
                    world.CallEventSystems<IPhysicRunEventSystem>();
                }
            }

            foreach (var world in _modules.Worlds)
            {
                if (world.GetSystemTypes().Contains(typeof(IRunPhysicSystem)))
                {
                    world.CallSystems<IRunPhysicSystem>(s => s.RunPhysic(), true);
                }
            }
        }

        public void LateUpdate()
        {
#if MODULES_PERFORMANCE
            _stopwatch.Start();
#endif
            foreach (var world in _modules.Worlds)
            {
                if (world.GetEventSystemTypes().ContainsKey(typeof(IPostRunEventSystem)))
                {
                    world.CallEventSystems<IPostRunEventSystem>();
                }
            }

            foreach (var world in _modules.Worlds)
            {
                if (world.GetSystemTypes().Contains(typeof(IPostRunSystem)))
                {
                    world.CallSystems<IPostRunSystem>(s => s.PostRun(), true);
                }
            }

            foreach (var world in _modules.Worlds)
            {
                if (world.GetEventSystemTypes().ContainsKey(typeof(IFrameEndEventSystem)))
                {
                    world.CallEventSystems<IFrameEndEventSystem>();
                }
            }

            foreach (var service in _emptyEntitiesServices)
                service.RemoveEmpty();

#if MODULES_PERFORMANCE
            _stopwatch.Stop();
            _elapsedTimeMs += _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
            _frames++;
            var targetFrameRate = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;
            if (_frames > targetFrameRate)
            {
                var avgFrameTimeMs = _elapsedTimeMs / _frames;
                if (avgFrameTimeMs > _settings.performanceSettings.warningAvgFrameMs)
                {
                    _modules.MainWorld.Logger.LogDebug(
                        $"[Performance] Avg frame time: {avgFrameTimeMs} ms. That is great than warning threshold",
                        LogFilter.Performance
                    );
                }

                if (avgFrameTimeMs > _settings.performanceSettings.panicAvgFrameMs)
                {
                    _modules.MainWorld.Logger.LogWarning(
                        $"[Performance] Avg frame time: {avgFrameTimeMs} ms. That is great than panic threshold");
                }

                _frames = 0;
                _elapsedTimeMs = 0;
            }
#endif
        }

        public void OnDestroy()
        {
            _modules.Destroy();
        }

        private static void CheckEmptiness(Entity entity)
        {
            if (!entity.IsAlive())
                return;

            if (entity.IsEmpty())
                entity.Destroy();
        }
    }
}