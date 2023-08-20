using System;
using System.Globalization;
using System.Linq;
using ModulesFramework.Data;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    [CustomEditor(typeof(EntityViewer))]
    public class EntityViewerEditor : Editor
    {
        private EntityViewer _viewer;
        private Vector2 _scrollPos;
        private GUIStyle _componentNameStyle;
        private GUIStyle _tagNameStyle;
        private GUIStyle _fieldStyle;
        private EditorDrawer _drawer;

        void OnEnable()
        {
            _viewer = (EntityViewer)serializedObject.targetObject;
            _componentNameStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                margin = EditorStyles.foldout.margin,
                padding = EditorStyles.foldout.padding,
            };
            _tagNameStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                margin = EditorStyles.foldout.margin,
                padding = EditorStyles.foldout.padding,
            };

            _drawer = new EditorDrawer();
        }

        public override void OnInspectorGUI()
        {
            _viewer.UpdateComponents();
            serializedObject.Update();
            var settings = EcsWorldContainer.Settings;
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            foreach (var kvp in _viewer.components)
            {
                var type = kvp.Key;
                var fieldName = type.Name;
                if (kvp.Value.Count > 1)
                    fieldName += $"({kvp.Value.Count.ToString(CultureInfo.InvariantCulture)})";

                var fields = type.GetFields();

                if (fields.Length == 0)
                {
                    EditorGUILayout.LabelField(fieldName, _tagNameStyle);
                    continue;
                }

                if (!EditorDrawerUtility.Foldout(type.ToString(), fieldName, _componentNameStyle, -1))
                    continue;

                foreach (var (index, _) in _viewer.components[type])
                {
                    var level = 0;
                    var changed = _viewer.changedComponents[type][index];
                    foreach (var fieldInfo in type.GetFields())
                    {
                        var fieldValue = fieldInfo.GetValue(changed);
                        var newValue = _drawer.DrawField(type, fieldInfo.Name, fieldValue, ref level);
                        fieldInfo.SetValue(changed, newValue);
                    }

                    var ecsTable = _viewer.World.GetEcsTable(type);

                    _viewer.changedComponents[type][index] = changed;

                    if (settings.autoApplyChanges)
                    {
                        ApplyChanges(ecsTable, type);
                    }
                    else
                    {
                        var isApply = GUILayout.Button("Apply", EditorStyles.miniButtonMid);
                        if (isApply)
                            ApplyChanges(ecsTable, type);
                    }

                    GUILayout.Space(5);
                }

                GUILayout.Space(15);
            }

            GUILayout.EndScrollView();
        }

        private void ApplyChanges(EcsTable ecsTable, Type type)
        {
            if (ecsTable.IsMultiple)
                ecsTable.SetDataObjects(_viewer.Eid, _viewer.changedComponents[type].Values.ToList());
            else
                ecsTable.SetDataObject(_viewer.Eid, _viewer.changedComponents[type].Values.First());
        }
    }
}