using System;
using System.Diagnostics;
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
        private readonly double _nsPerTick;
        private readonly ModulesSettings _settings;

        public ModulesUnityAdapter(ModulesSettings settings)
        {
            _nsPerTick = Stopwatch.Frequency / Math.Pow(10, 9);
            _ecs = new Ecs();
            _settings = settings;
            world = _ecs.World;
            world.SetLogger(new UnityLogger());
            world.SetLogType(_settings.logFilter);
        }

        public void StartDebug()
        {
            var debugViewer = new GameObject("DebugViewer");
            debugViewer.AddComponent<DebugViewer>().Init(world);
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
            if (_frames > Application.targetFrameRate)
            {
                var avgFrameTimeMs = _elapsedTimeMs / _frames;
                if (avgFrameTimeMs > _settings.performanceSettings.warningAvgFrameMs)
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