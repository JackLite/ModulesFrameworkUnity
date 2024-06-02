using ModulesFrameworkUnity.Settings;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class MFAutoEntryPoint
    {
        private static ModulesUnityAdapter _adapter;
        internal static ModulesSettings Settings { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void Startup()
        {
            Settings = ModulesSettings.Load();
            if (Settings.startMethod != StartMethod.Auto)
                return;
            _adapter = new ModulesUnityAdapter(Settings);
            var ecsMono = new GameObject("EcsWorld").AddComponent<MFUnityLoopProvider>();
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