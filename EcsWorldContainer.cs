using Core;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class EcsWorldContainer
    {
        private static EcsWorldContainer instance;
        private Ecs _ecs;
        public static DataWorld World => instance._ecs.World;
        
        [RuntimeInitializeOnLoadMethod]
        private static void Startup()
        {
            instance = new EcsWorldContainer();
            var ecsMono = new GameObject("EcsWorld").AddComponent<EcsWorldMono>();
            ecsMono.onUpdate = instance.Update;
            ecsMono.onFixedUpdate = instance.FixedUpdate;
            ecsMono.onLateUpdate = instance.LateUpdate;
            ecsMono.onDestroyed = instance.OnDestroy;
            instance._ecs = new Ecs();
            instance._ecs.Start();
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