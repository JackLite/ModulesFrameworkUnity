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
                padding = new RectOffset(30, 0, 0, 0)
            };
            _drawer = new EditorDrawer();
        }

        public override void OnInspectorGUI()
        {
            _viewer.components.Clear();
            _viewer.World.MapTables((type, table) =>
            {
                if (_viewer.World.HasComponent(_viewer.Eid, type))
                    _viewer.AddComponent(table.GetDataObject(_viewer.Eid));
            });
            _viewer.UpdateName();
            serializedObject.Update();
            
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            foreach (var kvp in _viewer.components)
            {
                var type = kvp.Key;
                if (!_drawer.Foldout(type.ToString(), type.ToString(), _componentNameStyle))
                    continue;
                
                var component = kvp.Value;
                var level = 0;
                foreach (var fieldInfo in type.GetFields())
                {
                    var fieldValue = fieldInfo.GetValue(component);
                    _drawer.DrawField(type, fieldInfo.Name, fieldValue, ref level);
                }
                GUILayout.Space(15);
            }
            GUILayout.EndScrollView();
        }

    }
}