using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class ModulesUnityAdapter
    {
        public static DataWorld world;
        private readonly Ecs _ecs;

        public ModulesUnityAdapter(LogFilter logFilter = LogFilter.Full)
        {
            _ecs = new Ecs();
            world = _ecs.World;
            world.SetLogger(new UnityLogger());
            world.SetLogType(logFilter);
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
            _ecs.Run();
        }

        public void FixedUpdate()
        {
            _ecs.RunPhysic();
        }

        public void LateUpdate()
        {
            _ecs.PostRun();
        }

        public void OnDestroy()
        {
            _ecs.Destroy();
        }
    }
}