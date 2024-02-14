using System;
using System.Globalization;
using System.Linq;
using ModulesFramework.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();
            var root = new VisualElement();
            _viewer.changedComponents.Clear();
            _viewer.UpdateComponents();
            foreach (var kvp in _viewer.components)
            {
                var componentFoldout = new Foldout();
                root.Add(componentFoldout);

                var type = kvp.Key;
                var fieldName = type.Name;
                if (kvp.Value.Count > 1)
                    fieldName += $"({kvp.Value.Count.ToString(CultureInfo.InvariantCulture)})";

                componentFoldout.text = fieldName;
                componentFoldout.value = false;
                var fields = type.GetFields();

                if (fields.Length == 0)
                    continue;

                foreach (var (index, _) in _viewer.components[type])
                {
                    var origin = _viewer.components[type][index];
                    var componentName = type.Name;
                    var ecsTable = _viewer.World.GetEcsTable(type);
                    if (ecsTable.IsMultiple)
                        componentName = $"{componentName}[{index}]";

                    _drawer.Draw(componentName, origin, componentFoldout, (_, newVal) =>
                    {
                        _viewer.changedComponents[type][index] = newVal;
                        ApplyChanges(ecsTable, type);
                    }, () => _viewer.changedComponents[type][index]);
                }
            }

            return root;
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