using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif

namespace ModulesFrameworkUnity.Debug.Drawers.Special
{
    public class UnsupportedDrawer : FieldDrawer
    {
        public override bool CanDraw(object value)
        {
            return false;
        }

        public override void Draw(string labelText, object value, VisualElement parent)
        {
            var label = new Label($"{value.GetType().Name} is not supported. You can create your own drawer");
            parent.Add(label);
        }

        public override void Update()
        {
        }
    }
}