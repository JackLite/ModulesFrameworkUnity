using System.Diagnostics;
using System.Globalization;
using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Settings;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class ModulesUnityAdapter
    {
        public static DataWorld world;
        private readonly Ecs _ecs;
        private double _elapsedTimeMs;
        private int _frames;
        private readonly Stopwatch _stopwatch = new();
        private readonly ModulesSettings _settings;

        public ModulesUnityAdapter(ModulesSettings settings)
        {
            _settings = settings;
            _ecs = new Ecs(settings.worldsCount);
            world = _ecs.MainWorld;
            world.SetLogger(new UnityLogger());
            world.SetLogType(_settings.logFilter);
        }

        public void StartDebug()
        {
            for (var i = 0; i < _settings.worldsCount; i++)
            {
                var debugViewer = new GameObject($"DebugViewer - World {i.ToString(CultureInfo.InvariantCulture)}");
                debugViewer.AddComponent<DebugViewer>().Init(_ecs.GetWorld(i));
            }
        }

        public void Start()
        {
            _ecs.Start();
        }

        public void Update()
        {
            #if MODULES_DEBUG
            _stopwatch.Start();
            #endif

            _ecs.Run();

            #if MODULES_DEBUG
            _stopwatch.Stop();
            _elapsedTimeMs += _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
            #endif
        }

        public void FixedUpdate()
        {
            _ecs.RunPhysic();
        }

        public void LateUpdate()
        {
            #if MODULES_DEBUG
            _stopwatch.Start();
            #endif

            _ecs.PostRun();

            #if MODULES_DEBUG
            _stopwatch.Stop();
            _elapsedTimeMs += _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
            _frames++;
            var targetFrameRate = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;
            if (_frames > targetFrameRate)
            {
                var avgFrameTimeMs = _elapsedTimeMs / _frames;
                if (avgFrameTimeMs > _settings.performanceSettings.warningAvgFrameMs && _settings.performanceSettings.debugMode)
                {
                    world.Logger.LogDebug(
                        $"[Performance] Avg frame time: {avgFrameTimeMs} ms. That is great than warning threshold",
                        LogFilter.Performance);
                }

                if (avgFrameTimeMs > _settings.performanceSettings.panicAvgFrameMs)
                {
                    world.Logger.LogWarning(
                        $"[Performance] Avg frame time: {avgFrameTimeMs} ms. That is great than panic threshold");
                }

                _frames = 0;
                _elapsedTimeMs = 0;
            }
            #endif
        }

        public void OnDestroy()
        {
            _ecs.Destroy();
        }
    }
}