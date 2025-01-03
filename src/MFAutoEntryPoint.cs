using ModulesFrameworkUnity.Settings;
using UnityEngine;
using UnityEngine.Scripting;

namespace ModulesFrameworkUnity
{
    public class MFAutoEntryPoint
    {
        private static ModulesUnityAdapter _adapter;
        internal static ModulesSettings Settings { get; private set; }

        [Preserve]
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
            _adapter.Start();
        }
    }
}