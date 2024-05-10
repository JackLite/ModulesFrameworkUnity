using System.Runtime.InteropServices;
using ModulesFrameworkUnity.DebugWindow.Data;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.DebugWindow
{
    public class ModulesDebugWindowTabs
    {
        private DebugTabType _current;

        public DebugTabType CurrentTab => _current;

        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Modules"))
                _current = DebugTabType.Modules;
            
            GUILayout.Button("One Data");
            GUILayout.Button("Entities");
            EditorGUILayout.EndHorizontal();
        }
    }
}