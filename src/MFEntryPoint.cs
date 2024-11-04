using ModulesFrameworkUnity.Settings;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class MFEntryPoint : MonoBehaviour
    {
        protected ModulesUnityAdapter _adapter;
        protected static bool _created;

        protected virtual async void Awake()
        {
            if (_created)
            {
                DestroyImmediate(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            var settings = ModulesSettings.Load();
            if (settings.startMethod != StartMethod.Manual)
                return;
            _adapter = new ModulesUnityAdapter(settings);
            #if UNITY_EDITOR
            if (settings.useOldDebug)
                _adapter.StartDebug();
            #endif
            await _adapter.StartAsync();
            _created = true;
        }

        protected virtual void Update()
        {
            _adapter.Update();
        }

        protected virtual void FixedUpdate()
        {
            _adapter.FixedUpdate();
        }

        protected virtual void LateUpdate()
        {
            _adapter.LateUpdate();
        }

        protected virtual void OnDestroy()
        {
            _adapter.OnDestroy();
        }
    }
}