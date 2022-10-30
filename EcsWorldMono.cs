using System;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class EcsWorldMono : MonoBehaviour
    {
        public Action OnUpdate;
        public Action OnFixedUpdate;
        public Action OnLateUpdate;
        public Action OnDestroyed;

        private void Awake()
        {
            if (FindObjectOfType<EcsWorldMono>() != this)
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