using ModulesFrameworkUnity.Settings;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class EcsWorldContainer
    {
        private static ModulesUnityAdapter _adapter;
        
        [RuntimeInitializeOnLoadMethod]
        private static void Startup()
        {
            var settings = ModulesSettings.Load();
            if (settings.startMethod != StartMethod.Auto)
                return;
            _adapter = new ModulesUnityAdapter(settings);
            var ecsMono = new GameObject("EcsWorld").AddComponent<EcsUnityLoopProvider>();
            ecsMono.OnUpdate = _adapter.Update;
            ecsMono.OnFixedUpdate = _adapter.FixedUpdate;
            ecsMono.OnLateUpdate = _adapter.LateUpdate;
            ecsMono.OnDestroyed = _adapter.OnDestroy;
            #if UNITY_EDITOR
            _adapter.StartDebug();
            #endif
            _adapter.Start();
        }
    }
}