using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Special
{
    public class NullDrawer : FieldDrawer
    {
        public override int Order => 10;

        public override bool CanDraw(object value)
        {
            return value == null;
        }

        public override void Draw(string fieldName, object value, VisualElement parent)
        {
            var label = new Label($"{fieldName} is null");
            parent.Add(label);
        }

        public override void Update()
        {
        }
    }
}