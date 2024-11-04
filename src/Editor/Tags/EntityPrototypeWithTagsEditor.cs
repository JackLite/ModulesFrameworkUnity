using Modules.Extensions.Prototypes.Editor;
using ModulesFrameworkUnity.EntitiesTags;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Tags
{
    [CustomPropertyDrawer(typeof(EntityPrototypeWithTag))]
    public class EntityPrototypeWithTagsEditor : EntityPrototypeEditor
    {
        protected override void DrawAdditional(SerializedProperty property, VisualElement root)
        {
            var tagsProperty = property.FindPropertyRelative(nameof(EntityPrototypeWithTag.tag));
            var tagField = new PropertyField(tagsProperty);
            tagField.AddToClassList("modules-proto--tag-field");
            root.Add(tagField);
            root.styleSheets.Add(Resources.Load<StyleSheet>("EntityPrototypeWithTag"));
        }
    }
}