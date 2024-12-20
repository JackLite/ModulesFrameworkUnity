using System;
using System.IO;
using ModulesFramework;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModulesFrameworkUnity.Settings
{
    [Serializable]
    public class ModulesSettings
    {
        public const string PathToSave = "Assets/Resources/";
        public const string FileName = "ModulesSettings";
        public const string FileExt = "json";
        public const string FileNameWithExt = FileName + "." + FileExt;
        public StartMethod startMethod;
        public int worldsCount = 1;
        public LogFilter logFilter = LogFilter.Full;

        public DebugSettings debugSettings;
        public PerformanceSettings performanceSettings;

        public bool deleteEmptyEntities;

        public ModulesSettings()
        {
            performanceSettings = new PerformanceSettings
            {
                warningAvgFrameMs = (1f / 60f) * 100f, // 10%
                panicAvgFrameMs = (1f / 60f) * 200f // 20%
            };
            debugSettings = new DebugSettings();
        }

        public static ModulesSettings Load()
        {
            var serialized = Resources.Load<TextAsset>(FileName);
            if (serialized != null)
                return JsonUtility.FromJson<ModulesSettings>(serialized.text);
            return new ModulesSettings();
        }

        private static string GetFullName()
        {
            return Path.Combine(PathToSave, FileNameWithExt);
        }

#if UNITY_EDITOR
        public void Save()
        {
            CheckFolder();
            var path = GetFullName();
            var serializedSettings = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (serializedSettings == null)
            {
                File.WriteAllText(path, string.Empty);
                AssetDatabase.Refresh();
                serializedSettings = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            }

            var contents = JsonUtility.ToJson(this);
            File.WriteAllText(path, contents);
            EditorUtility.SetDirty(serializedSettings);
            AssetDatabase.Refresh();
        }

        private static void CheckFolder()
        {
            if (Directory.Exists(PathToSave))
                return;
            Directory.CreateDirectory(PathToSave);
        }
#endif
    }
}