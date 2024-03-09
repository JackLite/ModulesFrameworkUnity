using System.Linq;
using ModulesFramework;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    [CustomEditor(typeof(ModuleViewer))]
    public class ModuleViewerEditor : Editor
    {
        private ModuleViewer _viewer;
        private static bool _systemsExpand;

        private void OnEnable()
        {
            _viewer = (ModuleViewer)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(_viewer.Module.GetType().Name, EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            if (_viewer.Module.IsInitialized && _viewer.Module.Systems.Any())
            {
                DrawSystems();
                EditorGUILayout.Space(20);
            }

            DrawInit();

            if (_viewer.Module.IsInitialized)
                DrawActivate();

            Repaint();
        }

        private void DrawSystems()
        {
            _systemsExpand = EditorGUILayout.BeginFoldoutHeaderGroup(
                _systemsExpand,
                "Systems",
                EditorStyles.foldoutHeader
            );
            if (_systemsExpand)
            {
                foreach (var systemType in _viewer.Module.Systems)
                {
                    var label = systemType.Name + " : ";
                    foreach (var i in systemType.GetInterfaces())
                    {
                        if (i == typeof(ISystem) || i == typeof(IEventSystem)
                            || i == typeof(IRunEventSystem)
                            || i == typeof(IPostRunEventSystem)
                            || i == typeof(IFrameEndEventSystem))
                            continue;

                        if (i.GenericTypeArguments.Length > 0)
                            label += $"{i.Name.Replace("`1", $"<{i.GetGenericArguments()[0].Name}>")}; ";
                        else
                            label += $"{i.Name}; ";
                    }

                    GUILayout.Label(label);
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawActivate()
        {
            GUILayout.Space(5);
            var activateBtnText = _viewer.Module.IsActive ? "Deactivate" : "Activate";
            var activate = GUILayout.Button(activateBtnText, EditorStyles.miniButtonMid);
            if (activate)
                _viewer.Module.SetActive(!_viewer.Module.IsActive);
        }

        private void DrawInit()
        {
            var initBtnText = _viewer.Module.IsInitialized ? "Destroy" : "Initialize";
            var initialize = GUILayout.Button(initBtnText, EditorStyles.miniButtonMid);
            if (initialize)
            {
                if (_viewer.Module.IsInitialized)
                    _viewer.Module.Destroy();
                else
                    _viewer.Module.Init().Forget();
            }
        }
    }
}