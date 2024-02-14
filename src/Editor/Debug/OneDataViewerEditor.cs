using System;
using System.Linq.Expressions;
using System.Reflection;
using ModulesFramework;
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

        void OnEnable()
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
            DrawHeader(root);
            root.Bind(serializedObject);
            var type = _viewer.DataType;
            var method = typeof(OneData).GetMethod("GetDataObject", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var fieldInfo in type.GetFields())
            {
                var getter = CreateGetter(fieldInfo, method);
                var originValue = getter();
                _drawer.Draw(fieldInfo.Name, originValue, root, (_, val) =>
                    {
                        var currentVal = _viewer.Data.GetDataObject();
                        fieldInfo.SetValue(currentVal, val);
                        _viewer.UpdateData(currentVal);
                    },
                    () => getter.Invoke());
            }

            return root;
        }

        private Func<object> CreateGetter(FieldInfo field, MethodInfo methodInfo)
        {
            var getDataCall = Expression.Call(Expression.Constant(_viewer.Data), methodInfo);
            var convertData = Expression.Convert(getDataCall, _viewer.DataType);
            var fieldExp = Expression.Field(convertData, field.Name);
            var expr = Expression.Lambda<Func<object>>(Expression.Convert(fieldExp, typeof(object)));
            return expr.Compile();
        }

        private void DrawHeader(VisualElement root)
        {
            var header = new Label(_viewer.DataType.Name)
            {
                style =
                {
                    fontSize = new StyleLength(14),
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold),
                    marginTop = new StyleLength(5)
                }
            };
            root.Add(header);
        }
    }
}