using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers
{
    public static class DrawersUtil
    {
        public static Button CreateRemoveBtn()
        {
            return new Button
            {
                text = "R",
                style =
                {
                    width = 20,
                    height = 20
                }
            };
        }
        
        public static void InitNumberFieldStyle(IStyle style)
        {
            style.minWidth = 300;
            style.alignContent = Align.Center;
        }
        
        /// <summary>
        ///     For struct and class drawers
        /// </summary>
        public static void InitObjectFieldStyle(Foldout foldout, int level, string labelText)
        {
            foldout.text = labelText;
            foldout.contentContainer.style.fontSize = 12;
            foldout.contentContainer.style.unityFontStyleAndWeight = FontStyle.Normal;
            foldout.style.fontSize = level > 1 ? 12 : 14;
            foldout.style.unityFontStyleAndWeight = FontStyle.Bold;

            foldout.value = level <= 1;
        }
    }
}