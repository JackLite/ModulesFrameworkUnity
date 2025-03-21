using System.Collections.Generic;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.EntitiesTags;
using ModulesFrameworkUnity.Settings;
using ModulesFrameworkUnity.Utils;
using System.Globalization;
using System.Threading.Tasks;
using ModulesFrameworkUnity.EmptyEntities;
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
        private int _frames;
#if MODULES_PERFORMANCE
        private readonly Stopwatch _stopwatch = new();
#endif
        private readonly ModulesSettings _settings;
        private readonly List<EmptyEntitiesService> _emptyEntitiesServices = new();

        public ModulesUnityAdapter(ModulesSettings settings)
        {
            _settings = settings;
            EntitiesTagStorage.Initialize();
            _modules = new MF(new UnityAssemblyFilter());
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
        }

        public void FixedUpdate()
        {
            _modules.RunPhysic();
        }

        public void LateUpdate()
        {
#if MODULES_PERFORMANCE
            _stopwatch.Start();
#endif

            _modules.PostRun();
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
