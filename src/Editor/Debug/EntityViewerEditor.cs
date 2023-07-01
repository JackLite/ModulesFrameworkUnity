using System.Globalization;
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
            _viewer.components.Clear();
            _viewer.World.MapTables((type, table) =>
            {
                if (_viewer.World.HasComponent(_viewer.Eid, type))
                    _viewer.AddComponents(table, _viewer.Eid);
            });
            _viewer.UpdateName();
            serializedObject.Update();

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

                if (!_drawer.Foldout(type.ToString(), fieldName, _componentNameStyle, -1))
                    continue;

                foreach (var component in kvp.Value)
                {
                    var level = 0;
                    foreach (var fieldInfo in type.GetFields())
                    {
                        var fieldValue = fieldInfo.GetValue(component);
                        _drawer.DrawField(type, fieldInfo.Name, fieldValue, ref level);
                    }

                    GUILayout.Space(5);
                }
                GUILayout.Space(15);
            }

            GUILayout.EndScrollView();
        }
    }
}