using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Special
{
    public class NullDrawer : FieldDrawer
    {
        private VisualElement _container;
        private string _fieldName;
        private bool _isNotNull;
        private FieldDrawer _innerDrawer;
        public override int Order => -10;

        public override bool CanDraw(object value)
        {
            return value == null;
        }

        public override void Draw(string labelText, object value, VisualElement parent)
        {
            _container = new VisualElement();
            _fieldName = labelText;
            parent.Add(_container);
            DrawLabel();
        }

        private void DrawLabel()
        {
            var label = new Label(_fieldName + " is null");
            _container.Add(label);
        }

        public override void Update()
        {
            var fieldValue = valueGetter();
            if (fieldValue != null && !_isNotNull)
            {
                _container.Clear();
                _innerDrawer = mainDrawer.Draw(_fieldName, fieldValue, _container, valueChangedCb, valueGetter, Level);
                _isNotNull = true;
            }
            else if (fieldValue == null && _isNotNull)
            {
                _container.Clear();
                DrawLabel();
                _innerDrawer = null;
                _isNotNull = false;
            }

            _innerDrawer?.Update();
        }
    }
}