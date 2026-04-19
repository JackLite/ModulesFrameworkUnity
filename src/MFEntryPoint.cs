using ModulesFramework;
using ModulesFrameworkUnity.Settings;
using System.Collections;
using UnityEngine;

namespace ModulesFrameworkUnity
{
    public class MFEntryPoint : MonoBehaviour
    {
        protected ModulesUnityAdapter adapter;
        protected static bool created;

        protected virtual void Awake()
        {
            if (created)
            {
                DestroyImmediate(gameObject);
                return;
            }

            var settings = ModulesSettings.Load();
            if (settings.startMethod != StartMethod.Manual)
                return;

            DontDestroyOnLoad(gameObject);
            adapter = new ModulesUnityAdapter(settings);
            adapter.Start();
            created = true;
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
            adapter.Update();
        }

        protected virtual void FixedUpdate()
        {
            adapter.FixedUpdate();
        }

        protected virtual void LateUpdate()
        {
            adapter.LateUpdate();
        }

        protected virtual void OnDestroy()
        {
            if (adapter != null)
                adapter.OnDestroy();
        }
    }
}