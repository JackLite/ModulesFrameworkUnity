using System;
using System.IO;
using ModulesFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ModulesFrameworkUnity.Settings
{
    [Serializable]
    public class ModulesSettings
    {
        public const string PathToSave = "Assets/ModulesSettings.json";
        public StartMethod startMethod;
        public LogFilter logFilter = LogFilter.Full;
        
        #if UNITY_EDITOR
        public void Save()
        {
            var serializedSettings = AssetDatabase.LoadAssetAtPath<TextAsset>(PathToSave);
            if (serializedSettings == null)
            {
                File.WriteAllText(PathToSave, string.Empty);
                AssetDatabase.Refresh();
                serializedSettings = AssetDatabase.LoadAssetAtPath<TextAsset>(PathToSave);
            }
            var contents = JsonUtility.ToJson(this);
            File.WriteAllText(PathToSave, contents);
            EditorUtility.SetDirty(serializedSettings);
            AssetDatabase.Refresh();
        }
        #endif

        public static ModulesSettings Load()
        {
            if (File.Exists(PathToSave))
                return JsonUtility.FromJson<ModulesSettings>(File.ReadAllText(PathToSave));
            return new ModulesSettings();
        }
    }
}