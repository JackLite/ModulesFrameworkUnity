﻿using System.Collections;
using ModulesFramework;
using ModulesFrameworkUnity.Settings;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class MFEntryPoint : MonoBehaviour
    {
        protected ModulesUnityAdapter _adapter;
        protected static bool _created;

        protected virtual void Awake()
        {
            if (_created)
            {
                DestroyImmediate(gameObject);
                return;
            }

            var settings = ModulesSettings.Load();
            if (settings.startMethod != StartMethod.Manual)
                return;

            DontDestroyOnLoad(gameObject);
            _adapter = new ModulesUnityAdapter(settings);
#if UNITY_EDITOR
            if (settings.useOldDebug)
                _adapter.StartDebug();
#endif
            _adapter.Start();
            _created = true;
        }

        protected virtual IEnumerator Start()
        {
            while (!MF.IsInitialized)
            {
                yield return new WaitForEndOfFrame();
            }
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