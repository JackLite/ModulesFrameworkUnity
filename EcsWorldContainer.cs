using System;
using Core;
using UnityEngine;

namespace TheTalesOfTwo
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
            ecsMono.onUpdate = _instance.Update;
            ecsMono.onFixedUpdate = _instance.FixedUpdate;
            ecsMono.onLateUpdate = _instance.LateUpdate;
            ecsMono.onDestroyed = _instance.OnDestroy;
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