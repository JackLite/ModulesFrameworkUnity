using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug
{
    [CustomEditor(typeof(OneDataViewer))]
    public class OneDataViewerEditor : Editor
    {
        private OneDataViewer _viewer;
        private Vector2 _scrollPos;
        private EditorDrawer _drawer;

        private void OnEnable()
        {
            _viewer = (OneDataViewer)serializedObject.targetObject;
            _drawer = new EditorDrawer();
            _viewer.OnUpdate += _drawer.Update;
        }

        private void OnDisable()
        {
            _viewer.OnUpdate -= _drawer.Update;
        }

        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();
            var root = new VisualElement();
            root.Bind(serializedObject);

            var type = _viewer.DataType;
            _drawer.Draw(type.Name, type, _viewer.Data.GetDataObject(), root, (_, newVal) =>
            {
                _viewer.UpdateData(newVal);
            }, () => _viewer.Data.GetDataObject());

            return root;
        }
    }
}