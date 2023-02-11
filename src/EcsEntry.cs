using System;
using ModulesFrameworkUnity.Settings;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class EcsEntry : MonoBehaviour
    {
        private ModulesUnityAdapter _adapter;
        private static bool _created;
        private void Awake()
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
            _adapter = new ModulesUnityAdapter(settings.logFilter);
            #if UNITY_EDITOR
            _adapter.StartDebug();
            #endif
            _adapter.Start();
        }

        private void Update()
        {
            _adapter.Update();
        }

        private void FixedUpdate()
        {
            _adapter.FixedUpdate();
        }

        private void LateUpdate()
        {
            _adapter.LateUpdate();
        }

        private void OnDestroy()
        {
            _adapter.OnDestroy();
        }
    }
}