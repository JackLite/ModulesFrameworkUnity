using ModulesFrameworkUnity.Settings;
using UnityEditor;
using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    [CustomEditor(typeof(OneDataViewer))]
    public class OneDataViewerEditor : Editor
    {
        private OneDataViewer _viewer;
        private Vector2 _scrollPos;
        private EditorDrawer _drawer;
        private GUIStyle _dataNameStyle;

        void OnEnable()
        {
            _viewer = (OneDataViewer)serializedObject.targetObject;
            _drawer = new EditorDrawer();
            _dataNameStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.white
                },
                padding = new RectOffset(5, 0, 10, 10)
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            var type = _viewer.DataType;
            GUILayout.Label(type.Name, _dataNameStyle);
            var changed = _viewer.ChangedData.GetDataObject();
            var settings = EcsWorldContainer.Settings;
            var level = 0;
            foreach (var fieldInfo in type.GetFields())
            {
                var fieldValue = fieldInfo.GetValue(changed);
                var val = _drawer.DrawField(fieldInfo.FieldType, fieldInfo.Name, fieldValue, ref level);
                fieldInfo.SetValue(changed, val);
            }

            _viewer.ChangedData.SetDataObject(changed);

            if (settings.autoApplyChanges)
            {
                _viewer.UpdateData(changed);
            }
            else
            {
                var isApply = GUILayout.Button("Apply", EditorStyles.miniButtonMid);
                if (isApply)
                    _viewer.UpdateData(changed);
            }

            GUILayout.EndScrollView();
            Repaint();
        }
    }
}