using ModulesFramework;
using ModulesFramework.Data;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class EcsWorldContainer
    {
        private static EcsWorldContainer _instance;
        private Ecs _ecs;
        public static DataWorld World => _instance._ecs.World;
        
        [RuntimeInitializeOnLoadMethod]
        private static void Startup()
        {
            _instance = new EcsWorldContainer();
            var ecsMono = new GameObject("EcsWorld").AddComponent<EcsWorldMono>();
            ecsMono.OnUpdate = _instance.Update;
            ecsMono.OnFixedUpdate = _instance.FixedUpdate;
            ecsMono.OnLateUpdate = _instance.LateUpdate;
            ecsMono.OnDestroyed = _instance.OnDestroy;
            _instance._ecs = new Ecs();
            _instance._ecs.Start();
        }
        
        private void Update()
        {
            _ecs.Run();
        }

        private void FixedUpdate()
        {
            _ecs.RunPhysic();
        }

        private void LateUpdate()
        {
            _ecs.PostRun();
        }

        private void OnDestroy()
        {
            _ecs.Destroy();
        }
    }
}