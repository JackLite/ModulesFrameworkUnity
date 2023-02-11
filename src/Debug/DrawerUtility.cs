using UnityEngine;

namespace ModulesFrameworkUnity.Debug
{
    public static class DrawerUtility
    {
        private const int LEFT_DEFAULT = 15;
        public static GUIStyle OneFieldStyle(int level)
        {
            return new GUIStyle
            {
                padding = new RectOffset(LEFT_DEFAULT + 5 * level, 0, 0, 0),
                normal =
                {
                    textColor = Color.white
                }
            };
        }
        
        public static GUIStyle ContainerStyle(int level)
        {
            return new GUIStyle
            {
                fontStyle = FontStyle.Italic,
                normal =
                {
                    textColor = Color.white
                },
                padding = new RectOffset(LEFT_DEFAULT + 5 * level, 0, 0, 0)
            };
        }
    }
}