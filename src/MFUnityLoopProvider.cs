using System;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class MFUnityLoopProvider : MonoBehaviour
    {
        public Action OnUpdate;
        public Action OnFixedUpdate;
        public Action OnLateUpdate;
        public Action OnDestroyed;

        private void Awake()
        {
            #if UNITY_6000_5_OR_NEWER
            if (FindAnyObjectByType<MFUnityLoopProvider>() != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            #else 
            if (FindObjectOfType<MFUnityLoopProvider>() != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            #endif
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }
    }
}