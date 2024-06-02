using System;
using System.Runtime.CompilerServices;
using UnityEngine;
[assembly: InternalsVisibleTo("JackLite.ModulesFrameworkUnityPackageEditor")]

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
            if (FindObjectOfType<MFUnityLoopProvider>() != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
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