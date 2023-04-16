﻿using System;
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
        public const string FileName = "ModulesSettings.json";
        public StartMethod startMethod;
        public LogFilter logFilter = LogFilter.Full;
        public PerformanceSettings performanceSettings;

        public ModulesSettings()
        {
            performanceSettings = new PerformanceSettings
            {
                debugMode = false,
                warningAvgFrameMs = (1f / 60f) * 100f, // 10%
                panicAvgFrameMs = (1f / 60f) * 200f // 20%
            };
        }
        
        public static ModulesSettings Load()
        {
            var path = GetFullName();
            if (File.Exists(path))
                return JsonUtility.FromJson<ModulesSettings>(File.ReadAllText(path));
            return new ModulesSettings();
        }
        
        private static string GetFullName()
        {
            return Path.Combine(PathToSave, FileName);
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